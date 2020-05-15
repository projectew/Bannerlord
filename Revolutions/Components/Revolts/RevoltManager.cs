using Revolutions.Components.Base.Factions;
using Revolutions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby;

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
            InformationManager.AddQuickInformation(information);

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionFailed(revolt.Settlement);

            if (revolt.IsMinorFaction)
            {
                Managers.Kingdom.DestroyKingdom(revolt.Party.Owner.Clan.Kingdom);
            }
            else
            {
                DestroyPartyAction.Apply(revolt.SettlementInfo.Garrision, revolt.Party.MobileParty);
            }

            this.Revolts.Remove(revolt);
        }

        internal void EndSucceededRevolut(Revolt revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolt.Settlement.Name.ToString());
            InformationManager.AddQuickInformation(information);

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionSucceeded(revolt.Settlement);

            if (RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic && revolt.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolt.SettlementInfo.IsLoyalFactionOfImperialCulture)
            {
                revolt.Settlement.OwnerClan.AddRenown(-RevolutionsSettings.Instance.RevoltsImperialRenownLoss);
            }

            if (RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyByRevolt(revolt.Party.LeaderHero, revolt.Settlement);
                revolt.Party.LeaderHero.Clan.AddRenown(RevolutionsSettings.Instance.RevoltsMinorFactionsRenownGainOnWin);

                var amountOTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseSize + (int)(revolt.Settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralProsperityMulitplier)) / 3;
                var eliteUnits = new TroopRoster();
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.RangedEliteMilitiaTroop, amountOTroops);
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.MeleeEliteMilitiaTroop, amountOTroops * 2);
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
            try
            {
                InformationManager.AddNotice(new SettlementRebellionMapNotification(settlement, information));
                InformationManager.AddQuickInformation(information);
            }
            catch (Exception)
            {
                InformationManager.AddQuickInformation(information);
            }

            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero leader;

            if (atWarWithLoyalFaction)
            {
                leader = Managers.Faction.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                leader = Managers.Character.CreateRandomLeader(settlementInfo.LoyalFaction?.Leader?.Clan ?? settlement.OwnerClan, settlementInfo);
                Managers.Character.GetInfo(leader.CharacterObject).IsRevoltKingdomLeader = true;

                var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);
                var banner = bannerInfo != null ? new Banner(bannerInfo.BannerId) : null;

                var clan = Managers.Clan.CreateClan(leader, null, null, banner);
                Managers.Clan.GetInfo(leader.Clan).IsRevoltClan = true;

                Managers.Kingdom.CreateKingdom(leader, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"), banner ?? clan.Banner, false);
                Managers.Kingdom.GetInfo(leader.Clan.Kingdom).IsRevoltKingdom = true;
            }

            var mobileParty = Managers.Party.CreateMobileParty(leader, null, settlement.GatePosition, settlement, !atWarWithLoyalFaction, true);

            var amountOfTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseSize + (int)(settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralProsperityMulitplier)) / 3;
            var basicUnits = new TroopRoster();
            basicUnits.AddToCounts(leader.Culture.RangedMilitiaTroop, amountOfTroops);
            basicUnits.AddToCounts(leader.Culture.MeleeMilitiaTroop, amountOfTroops * 2);
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

            DeclareWarAction.Apply(leader.MapFaction, settlement.MapFaction);

            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            SetPartyAiAction.GetActionForBesiegingSettlement(mobileParty, settlement);

            this.Revolts.Add(new Revolt(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction));
            settlementInfo.HasRebellionEvent = true;
        }
    }
}