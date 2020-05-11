using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using Helpers;
using KNTLibrary;
using Revolutions.Components.Factions;

namespace Revolutions.Components.Revolutions
{
    public class RevolutionManager
    {
        #region Singleton

        private RevolutionManager() { }

        static RevolutionManager()
        {
            RevolutionManager.Instance = new RevolutionManager();
        }

        public static RevolutionManager Instance { get; private set; }

        #endregion

        public HashSet<Revolution> Revolutions = new HashSet<Revolution>();

        public Revolution GetRevolutionByPartyId(string id)
        {
            return this.Revolutions.FirstOrDefault(r => r.PartyId == id);
        }

        public Revolution GetRevolutionByParty(PartyBase party)
        {
            return this.GetRevolutionByPartyId(party.Id);
        }

        public Revolution GetRevolutionBySettlementId(string id)
        {
            return this.Revolutions.FirstOrDefault(r => r.SettlementId == id);
        }

        public Revolution GetRevolutionBySettlement(Settlement settlement)
        {
            return this.GetRevolutionBySettlementId(settlement.StringId);
        }

        public List<Settlement> GetSettlements()
        {
            return this.Revolutions.Select(r => r.Settlement).ToList();
        }

        public List<PartyBase> GetParties()
        {
            return this.Revolutions.Select(r => r.Party).ToList();
        }

        public void IncreaseDailyLoyaltyForSettlement()
        {
            foreach (var info in RevolutionsManagers.SettlementManager.Infos)
            {
                foreach (var party in info.Settlement.Parties)
                {
                    if (party.IsLordParty && party.Party.Owner.Clan == info.Settlement.OwnerClan)
                    {
                        info.Settlement.Town.Loyalty += Settings.Instance.PlayerInTownLoyaltyIncrease;

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

        public void CheckRevolutionProgress()
        {
            foreach (var settlementInfo in RevolutionsManagers.SettlementManager.Infos)
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

                if (!settlementInfo.CurrentFactionInfoRevolutions.CanRevolt || settlementInfo.HasRebellionEvent)
                {
                    settlementInfo.RevolutionProgress = 0;
                    continue;
                }

                settlementInfo.RevolutionProgress += Settings.Instance.MinimumObedienceLoyalty - settlement.Town.Loyalty;

                if (settlementInfo.RevolutionProgress >= 100 && !settlement.IsUnderSiege)
                {
                    this.StartRebellionEvent(settlement);
                    continue;
                }

                if (settlementInfo.RevolutionProgress < 0)
                {
                    settlementInfo.RevolutionProgress = 0;
                }
            }
        }

        public void EndFailedRevolution(Revolution revolution)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolution.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Yellow));

            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionFailed(revolution.Settlement);

            if (revolution.IsMinorFaction)
            {
                var kingdom = revolution.Party.Owner.Clan.Kingdom;
                var mapFaction = kingdom.MapFaction;
                foreach (var faction in Campaign.Current.Factions.Where(go => go.IsAtWarWith(mapFaction)))
                {
                    if (kingdom.MapFaction.IsAtWarWith(faction))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(faction, mapFaction);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(mapFaction, faction);
                    }
                }

                KillCharacterAction.ApplyByExecution(revolution.Party.Owner, revolution.Settlement.OwnerClan?.Kingdom.Leader ?? revolution.Settlement.OwnerClan.Leader);
                RevolutionsManagers.KingdomManager.RemoveKingdom(kingdom);
            }

            if(revolution.Party?.MobileParty != null)
            {
                DestroyPartyAction.Apply(revolution.SettlementInfoRevolutions.Garrision, revolution.Party.MobileParty);
            }

            this.Revolutions.Remove(revolution);
        }

        public void EndSucceededRevoluton(Revolution revolution)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolution.Settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Yellow));

            revolution.SettlementInfoRevolutions.CurrentFactionInfoRevolutions.CityRevoltionSucceeded(revolution.Settlement);

            if (Settings.Instance.EmpireLoyaltyMechanics && revolution.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolution.SettlementInfoRevolutions.IsLoyalFactionOfImperialCulture)
            {
                revolution.Settlement.OwnerClan.AddRenown(-Settings.Instance.ImperialRenownLossOnWin);
            }

            if (Settings.Instance.AllowMinorFactions && revolution.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyBySiege(revolution.Party.LeaderHero, revolution.Party.LeaderHero, revolution.Settlement);
                revolution.Party.LeaderHero.Clan.AddRenown(Settings.Instance.RenownGainOnWin);

                var companion = RevolutionsManagers.CharacterManager.CreateRandomLeader(revolution.Party.LeaderHero.Clan, revolution.SettlementInfo);
                RevolutionsManagers.CharacterManager.GetInfo(companion.CharacterObject);
                RevolutionsManagers.ClanManager.CreateClan(companion, companion.Name, companion.Name);
                var mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(companion, revolution.Settlement.GatePosition, revolution.Settlement, true, true);
                ChangeKingdomAction.ApplyByJoinToKingdom(companion.Clan, revolution.Party.LeaderHero.Clan.Kingdom, true);

                RevolutionsManagers.ClanManager.GetInfo(companion.Clan).CanJoinOtherKingdoms = false;

                var amountOfEliteTroops = (Settings.Instance.BaseRevoltArmySize + (int)(revolution.Settlement.Prosperity * Settings.Instance.ArmyProsperityMulitplier)) / 2;
                mobileParty.MemberRoster.Add(RevolutionsManagers.PartyManager.GenerateEliteTroopRoster(mobileParty.LeaderHero, amountOfEliteTroops));

                revolution.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                mobileParty.Ai.SetDoNotMakeNewDecisions(false);

                SetPartyAiAction.GetActionForPatrollingAroundSettlement(mobileParty, revolution.Settlement);
            }

            this.Revolutions.Remove(revolution);
        }

        public void StartRebellionEvent(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(information.ToString(), ColorManager.Yellow));

            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero hero;

            if (atWarWithLoyalFaction)
            {
                hero = RevolutionsManagers.FactionManager.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                hero = RevolutionsManagers.CharacterManager.CreateRandomLeader(settlement.OwnerClan, settlementInfo);
                RevolutionsManagers.CharacterManager.GetInfo(hero.CharacterObject).IsRevoltKingdomLeader = true;
                RevolutionsManagers.ClanManager.CreateClan(hero, hero.Name, hero.Name);
                RevolutionsManagers.KingdomManager.CreateKingdom(hero, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"));

                RevolutionsManagers.ClanManager.GetInfo(hero.Clan).CanJoinOtherKingdoms = false;
            }

            var mobileParty = RevolutionsManagers.PartyManager.CreateMobileParty(hero, settlement.GatePosition, settlement, !atWarWithLoyalFaction, true);

            var amountOfBasicTroops = Settings.Instance.BaseRevoltArmySize + (int)(settlement.Prosperity * Settings.Instance.ArmyProsperityMulitplier);
            mobileParty.MemberRoster.Add(RevolutionsManagers.PartyManager.GenerateBasicTroopRoster(hero, amountOfBasicTroops, withTier4: false));

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

            this.Revolutions.Add(new Revolution(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction));

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