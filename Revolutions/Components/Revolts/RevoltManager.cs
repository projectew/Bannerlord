using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using Helpers;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Factions;
using Revolts;

namespace Revolutions.Components.Revolts
{
    public class RevoltManager
    {
        #region Singleton

        private RevoltManager() { }

        static RevoltManager()
        {
            Instance = new RevoltManager();
        }

        public static RevoltManager Instance { get; private set; }

        #endregion

        public HashSet<Revolt> Revolts = new HashSet<Revolt>();

        public Revolt GetRevoltByPartyId(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.PartyId == id);
        }

        public Revolt GetRevoltByParty(PartyBase party)
        {
            return this.GetRevoltByPartyId(party.Id);
        }

        public Revolt GetRevoltBySettlementId(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.SettlementId == id);
        }

        public Revolt GetRevoltBySettlement(Settlement settlement)
        {
            return this.GetRevoltBySettlementId(settlement.StringId);
        }

        public List<Settlement> GetSettlements()
        {
            return this.Revolts.Select(r => r.Settlement).ToList();
        }

        public List<PartyBase> GetParties()
        {
            return this.Revolts.Select(r => r.Party).ToList();
        }

        public void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var info in RevolutionsManagers.Settlement.Infos)
            {
                foreach (var party in info.Settlement.Parties)
                {
                    if (party.IsLordParty && party.Party.Owner.Clan == info.Settlement.OwnerClan)
                    {
                        info.Settlement.Town.Loyalty += Settings.Instance.GeneralPlayerInTownLoyaltyIncrease;

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

        public void CheckRevoltProgress()
        {
            foreach (var settlementInfo in RevolutionsManagers.Settlement.Infos)
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

                if (!settlementInfo.CurrentFactionInfoRevolts.CanRevolt || settlementInfo.HasRebellionEvent)
                {
                    settlementInfo.RevoltProgress = 0;
                    continue;
                }

                settlementInfo.RevoltProgress += Settings.Instance.GeneralMinimumObedienceLoyalty - settlement.Town.Loyalty;

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

        public void EndFailedRevolt(Revolt Revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", Revolt.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            Revolt.SettlementInfo.CurrentFactionInfoRevolts.CityRevoltionFailed(Revolt.Settlement);

            if (Revolt.IsMinorFaction)
            {
                var kingdom = Revolt.Party.Owner.Clan.Kingdom;
                var mapFaction = kingdom.MapFaction;
                foreach (var faction in Campaign.Current.Factions.Where(go => go.IsAtWarWith(mapFaction)))
                {
                    if (kingdom.MapFaction.IsAtWarWith(faction))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(faction, mapFaction);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(mapFaction, faction);
                    }
                }

                KillCharacterAction.ApplyByExecution(Revolt.Party.Owner, Revolt.Settlement.OwnerClan?.Kingdom.Leader ?? Revolt.Settlement.OwnerClan.Leader);
                RevolutionsManagers.Kingdom.RemoveKingdom(kingdom);
            }

            if (Revolt.Party?.MobileParty != null)
            {
                DestroyPartyAction.Apply(Revolt.SettlementInfo.Garrision, Revolt.Party.MobileParty);
            }

            this.Revolts.Remove(Revolt);
        }

        public void EndSucceededRevoluton(Revolt Revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", Revolt.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            Revolt.SettlementInfo.CurrentFactionInfoRevolts.CityRevoltionSucceeded(Revolt.Settlement);

            if (Settings.Instance.RevoltsImperialLoyaltyMechanic && Revolt.SettlementInfo.IsCurrentFactionOfImperialCulture && !Revolt.SettlementInfo.IsLoyalFactionOfImperialCulture)
            {
                Revolt.Settlement.OwnerClan.AddRenown(-Settings.Instance.RevoltsImperialRenownLoss);
            }

            if (Settings.Instance.RevoltsMinorFactionsMechanic && Revolt.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyBySiege(Revolt.Party.LeaderHero, Revolt.Party.LeaderHero, Revolt.Settlement);
                Revolt.Party.LeaderHero.Clan.AddRenown(Settings.Instance.RevoltsMinorFactionsRenownGainOnWin);

                var companion = RevolutionsManagers.Character.CreateRandomLeader(Revolt.Party.LeaderHero.Clan, Revolt.SettlementInfo);
                RevolutionsManagers.Character.GetInfo(companion.CharacterObject);
                RevolutionsManagers.Clan.CreateClan(companion, companion.Name, companion.Name);
                var mobileParty = RevolutionsManagers.Party.CreateMobileParty(companion, Revolt.Settlement.GatePosition, Revolt.Settlement, true, true);
                ChangeKingdomAction.ApplyByJoinToKingdom(companion.Clan, Revolt.Party.LeaderHero.Clan.Kingdom, true);

                RevolutionsManagers.Clan.GetInfo(companion.Clan).CanJoinOtherKingdoms = false;

                var amountOfEliteTroops = (Settings.Instance.RevoltsGeneralBaseArmy + (int)(Revolt.Settlement.Prosperity * Settings.Instance.RevoltsGeneralArmyProsperityMulitplier)) / 2;
                mobileParty.MemberRoster.Add(RevolutionsManagers.Party.GenerateEliteTroopRoster(mobileParty.LeaderHero, amountOfEliteTroops));

                Revolt.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                mobileParty.Ai.SetDoNotMakeNewDecisions(false);

                SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, Revolt.Settlement);
            }

            this.Revolts.Remove(Revolt);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorHelper.Yellow));

            var settlementInfo = RevolutionsManagers.Settlement.GetInfo(settlement);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero hero;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.Faction.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = RevolutionsManagers.Character.CreateRandomLeader(settlement.OwnerClan, settlementInfo);
                RevolutionsManagers.Character.GetInfo(hero.CharacterObject).IsRevoltKingdomLeader = true;
                RevolutionsManagers.Clan.CreateClan(hero, hero.Name, hero.Name);
                RevolutionsManagers.Kingdom.CreateKingdom(hero, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"));

                RevolutionsManagers.Clan.GetInfo(hero.Clan).CanJoinOtherKingdoms = false;
            }

            var mobileParty = RevolutionsManagers.Party.CreateMobileParty(hero, settlement.GatePosition, settlement, !atWarWithLoyalFaction, true);

            var amountOfBasicTroops = Settings.Instance.RevoltsGeneralBaseArmy + (int)(settlement.Prosperity * Settings.Instance.RevoltsGeneralArmyProsperityMulitplier);
            mobileParty.MemberRoster.Add(RevolutionsManagers.Party.GenerateBasicTroopRoster(hero, amountOfBasicTroops, withTier4: false));

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

            this.Revolts.Add(new Revolt(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction));

            settlementInfo.HasRebellionEvent = true;

            FactionManager.DeclareWar(hero.MapFaction, settlement.MapFaction);
            Campaign.Current.FactionManager.RegisterCampaignWar(hero.MapFaction, settlement.MapFaction);
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, settlement.OwnerClan.Leader, -20, false);
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, settlement.OwnerClan.Kingdom.Leader, -20, false);
            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            SetPartyAiAction.GetActionForBesiegingSettlement(mobileParty, settlement);
        }
    }
}