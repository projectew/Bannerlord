using Helpers;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Factions;
using Revolutions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Components.Revolts
{
    internal class RevoltManager
    {
        #region Singleton

        private RevoltManager() { }

        static RevoltManager()
        {
            Instance = new RevoltManager();
        }

        internal static RevoltManager Instance { get; private set; }

        #endregion

        internal HashSet<Revolt> Revolts = new HashSet<Revolt>();

        internal Revolt GetRevoltByPartyId(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.PartyId == id);
        }

        internal Revolt GetRevoltByParty(PartyBase party)
        {
            return this.GetRevoltByPartyId(party.Id);
        }

        internal Revolt GetRevoltBySettlementId(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.SettlementId == id);
        }

        internal Revolt GetRevoltBySettlement(Settlement settlement)
        {
            return this.GetRevoltBySettlementId(settlement.StringId);
        }

        internal List<Settlement> GetSettlements()
        {
            return this.Revolts.Select(r => r.Settlement).ToList();
        }

        internal List<PartyBase> GetParties()
        {
            return this.Revolts.Select(r => r.Party).ToList();
        }

        internal void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var info in Managers.Settlement.Infos)
            {
                foreach (var party in info.Settlement.Parties)
                {
                    if (party.IsLordParty && party.Party.Owner.Clan == info.Settlement.OwnerClan)
                    {
                        info.Settlement.Town.Loyalty += RevolutionsSettings.Instance.GeneralPlayerInTownLoyaltyIncrease;

                        if (info.Settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                        {
                            var textObject = new TextObject("{=PqkwszGz}Seeing you spend time at {SETTLEMENT}, your subjects feel more loyal to you.");
                            textObject.SetTextVariable("SETTLEMENT", info.Settlement.Name.ToString());
                            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                        }

                        break;
                    }
                }
            }
        }

        internal void CheckRevoltProgress()
        {
            foreach (var settlementInfo in Managers.Settlement.Infos)
            {
                var settlement = settlementInfo.Settlement;

                if (!settlement.IsTown)
                {
                    continue;
                }

                if (settlementInfo.LoyalFactionId == settlementInfo.CurrentFactionId)
                {
                    continue;
                }

                if (settlementInfo.CurrentFactionInfo?.CanRevolt == false || settlementInfo.HasRebellionEvent)
                {
                    settlementInfo.RevoltProgress = 0;
                    continue;
                }

                settlementInfo.RevoltProgress += RevolutionsSettings.Instance.GeneralMinimumObedienceLoyalty - settlement.Town.Loyalty;

                if (settlementInfo.RevoltProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRebellionEvent(settlement);
                    continue;
                }

                if (settlementInfo.RevoltProgress < 0)
                {
                    settlementInfo.RevoltProgress = 0;
                }
            }
        }

        internal void EndFailedRevolt(Revolt revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolt.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionFailed(revolt.Settlement);

            if (revolt.IsMinorFaction)
            {
                Managers.Kingdom.DestroyKingdom(revolt.Party.Owner.Clan.Kingdom);
                KillCharacterAction.ApplyByExecution(revolt.Party.Owner, revolt.Settlement.OwnerClan?.Kingdom.Leader ?? revolt.Settlement.OwnerClan.Leader);
            }

            if (revolt.Party?.MobileParty != null)
            {
                DestroyPartyAction.Apply(revolt.SettlementInfo.Garrision, revolt.Party.MobileParty);
            }

            this.Revolts.Remove(revolt);
        }

        internal void EndSucceededRevolut(Revolt revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolt.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionSucceeded(revolt.Settlement);

            if (RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic && revolt.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolt.SettlementInfo.IsLoyalFactionOfImperialCulture)
            {
                revolt.Settlement.OwnerClan.AddRenown(-RevolutionsSettings.Instance.RevoltsImperialRenownLoss);
            }

            if (RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyBySiege(revolt.Party.LeaderHero, revolt.Party.LeaderHero, revolt.Settlement);
                revolt.Party.LeaderHero.Clan.AddRenown(RevolutionsSettings.Instance.RevoltsMinorFactionsRenownGainOnWin);

                var amountOfBasicTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseArmy + (int)(revolt.Settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralArmyProsperityMulitplier)) / 3;
                var eliteUnits = new TroopRoster();
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.RangedEliteMilitiaTroop, amountOfBasicTroops);
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.MeleeEliteMilitiaTroop, amountOfBasicTroops * 2);
                revolt.Party.MobileParty.MemberRoster.Add(eliteUnits);

                revolt.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);

                SetPartyAiAction.GetActionForPatrollingAroundSettlement(revolt.Party.MobileParty, revolt.Settlement);

                this.Revolts.Remove(revolt);
            }
        }

        internal void StartRebellionEvent(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero leader;

            if (atWarWithLoyalFaction)
            {
                leader = Managers.Faction.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                leader = Managers.Character.CreateRandomLeader(settlement.OwnerClan, settlementInfo);
                Managers.Character.GetInfo(leader.CharacterObject).IsRevoltKingdomLeader = true;

                Managers.Clan.CreateClan(leader, leader.Name, leader.Name);
                Managers.Clan.GetInfo(leader.Clan).IsRevoltClan = true;

                var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);
                if (bannerInfo != null)
                {
                    bannerInfo.Used = true;
                }

                var revoltKingdom = Managers.Kingdom.CreateKingdom(leader, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"), bannerInfo != null ? new Banner(bannerInfo.BannerId) : null, false);
                Managers.Kingdom.GetInfo(revoltKingdom).IsRevoltKingdom = true;
            }

            var mobileParty = Managers.Party.CreateMobileParty(leader, settlement.GatePosition, settlement, !atWarWithLoyalFaction, true);

            var amountOfBasicTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseArmy + (int)(settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralArmyProsperityMulitplier)) / 3;
            var basicUnits = new TroopRoster();
            basicUnits.AddToCounts(leader.Culture.RangedMilitiaTroop, amountOfBasicTroops);
            basicUnits.AddToCounts(leader.Culture.MeleeMilitiaTroop, amountOfBasicTroops * 2);
            mobileParty.MemberRoster.Add(basicUnits);

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (var troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number, false);
                }

                settlement.MilitaParty.RemoveParty();
            }

            if (!atWarWithLoyalFaction)
            {
                mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);
            }

            DeclareWarAction.Apply(leader.Clan.Kingdom, settlement.MapFaction);

            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            SetPartyAiAction.GetActionForBesiegingSettlement(mobileParty, settlement);

            this.Revolts.Add(new Revolt(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction));
            settlementInfo.HasRebellionEvent = true;
        }
    }
}