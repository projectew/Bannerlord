using KNTLibrary;
using Revolutions.Components.Factions;
using Revolutions.Components.Settlements;
using Revolutions.Settings;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class RevoltBehavior : CampaignBehaviorBase
    {
        public RevoltBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new GuiHandlerBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunchedEvent(CampaignGameStarter starter)
        {
            if (!RevolutionsSettings.Instance.RevoltsLuckyNationMechanic && Managers.Kingdom.Infos.Count > 0)
            {
                foreach (var info in Managers.Kingdom.Infos.Where(kingdomInfo => kingdomInfo.LuckyNation))
                {
                    info.LuckyNation = false;
                }

                return;
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationRandom)
            {
                if (Managers.Kingdom.Infos.Count > 0 && !Managers.Kingdom.Infos.Any(i => i.LuckyNation))
                {
                    var randomNation = Managers.Kingdom.Infos.GetRandomElement();
                    if (randomNation != null)
                    {
                        randomNation.LuckyNation = true;
                    }
                }
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationImperial)
            {
                var imperialNations = Managers.Kingdom.Infos.Where(i => i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (imperialNations.Count() > 0 && !imperialNations.Any(i => i.LuckyNation))
                {
                    var imperialNation = imperialNations.GetRandomElement();
                    if (imperialNation != null)
                    {
                        imperialNation.LuckyNation = true;
                    }
                }
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationNonImperial)
            {
                var nonImperialNations = Managers.Kingdom.Infos.Where(i => !i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (nonImperialNations.Count() > 0 && !nonImperialNations.Any(i => i.LuckyNation))
                {
                    var nonImperialNation = nonImperialNations.GetRandomElement();
                    if (nonImperialNation != null)
                    {
                        nonImperialNation.LuckyNation = true;
                    }
                }
            }
        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            var involvedParty = mapEvent.InvolvedParties.Intersect(RevoltManager.Instance.GetParties()).FirstOrDefault();
            if (involvedParty == null)
            {
                return;
            }

            var currentRevolt = Managers.Revolt.GetRevoltByPartyId(involvedParty.Id);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                RevoltBehavior.EndFailedRevolt(currentRevolt);
            }
            else
            {
                RevoltBehavior.EndSucceededRevolt(currentRevolt);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = Managers.Settlement.Get(settlement);
            settlementInfo.UpdateOwnerRevolt(newOwner.MapFaction);

            if (capturedHero?.PartyBelongedTo?.Party != null)
            {
                var revolt = RevoltManager.Instance.GetRevoltByParty(capturedHero.PartyBelongedTo.Party);
                if (revolt != null && !RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
                {
                    if (revolt.SettlementInfo.CurrentFaction == revolt.SettlementInfo.LoyalFaction)
                    {
                        var previousFactionOwner = BaseManagers.Faction.GetLordWithLeastFiefs(revolt.SettlementInfo.PreviousFaction).HeroObject;
                        ChangeOwnerOfSettlementAction.ApplyByRevolt(previousFactionOwner, settlement);
                    }
                    else
                    {
                        var loyalFactionOwner = BaseManagers.Faction.GetLordWithLeastFiefs(revolt.SettlementInfo.LoyalFaction).HeroObject;
                        ChangeOwnerOfSettlementAction.ApplyByRevolt(loyalFactionOwner, settlement);
                    }

                    Managers.Kingdom.DestroyKingdom(capturedHero.Clan.Kingdom);
                    Managers.Clan.DestroyClan(capturedHero.Clan);
                    capturedHero.PartyBelongedTo.RemoveParty();
                    KillCharacterAction.ApplyByRemove(capturedHero);

                    RevoltManager.Instance.Revolts.Remove(revolt);
                }
            }
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = Managers.Clan.Get(clan);

            if (clan.StringId == Clan.PlayerClan.StringId || clanInfo.CanChangeKingdom || !clanInfo.IsRevoltClan || clan.StringId == newKingdom.RulingClan.StringId
                || clan.Culture.Name.ToString().ToLower().Contains("empire") && newKingdom.Culture.Name.ToString().ToLower().Contains("empire")
                || clan.Culture.Name.ToString() == newKingdom.Culture.Name.ToString())
            {
                return;
            }

            clan.ClanLeaveKingdom(false);
        }

        private void DailyTickEvent()
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
                            textObject.SetTextVariable("SETTLEMENT", info.Settlement.Name);
                            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                        }

                        break;
                    }
                }

                if (!info.Settlement.IsFortification)
                {
                    continue;
                }

                if (info.LoyalFaction.StringId == info.CurrentFactionId)
                {
                    continue;
                }

                if (info.CurrentFactionInfo?.CanRevolt == false || info.HasRebellionEvent)
                {
                    info.RevoltProgress = 0;
                    continue;
                }

                info.RevoltProgress -= info.Settlement.Town.LoyaltyChange;

                if (info.RevoltProgress >= 100 && !info.Settlement.IsUnderSiege)
                {
                    RevoltBehavior.StartRevolt(info.Settlement);
                    continue;
                }

                if (info.RevoltProgress < 0)
                {
                    info.RevoltProgress = 0;
                }

                info.DaysOwnedByOwner++;
            }

            foreach (var factionInfo in Managers.Faction.Infos)
            {
                factionInfo.DaysSinceLastRevolt++;

                if (factionInfo.DaysSinceLastRevolt > RevolutionsSettings.Instance.RevoltsGeneralCooldownTime)
                {
                    factionInfo.CanRevolt = true;
                }
            }
        }

        internal static void StartRevolt(Settlement settlement)
        {
            var information = new TextObject("{=dRoS0maD}{SETTLEMENT} is revolting!");
            information.SetTextVariable("SETTLEMENT", settlement.Name);
            InformationManager.AddNotice(new SettlementRebellionMapNotification(settlement, information));
            InformationManager.AddQuickInformation(information);

            var settlementInfo = Managers.Settlement.Get(settlement);
            var atWarWithLoyalFaction = settlementInfo.CurrentFaction.IsAtWarWith(settlementInfo.LoyalFaction);

            Hero leader;

            if (atWarWithLoyalFaction)
            {
                leader = Managers.Faction.GetLordWithLeastFiefs(settlementInfo.LoyalFaction).HeroObject;
            }
            else
            {
                leader = Managers.Character.CreateRandomLeader(settlementInfo.LoyalFaction?.Leader?.Clan ?? settlement.OwnerClan, settlementInfo);
                Managers.Character.Get(leader.CharacterObject).IsRevoltKingdomLeader = true;

                var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);
                var banner = bannerInfo != null ? new Banner(bannerInfo.BannerId) : null;

                var clan = Managers.Clan.CreateClan(leader, null, null, banner);
                Managers.Clan.Get(leader.Clan).IsRevoltClan = true;

                Managers.Kingdom.CreateKingdom(leader, new TextObject($"Kingdom of {settlement.Name}"), new TextObject($"Kingdom of {settlement.Name}"), banner ?? clan.Banner, false);
                Managers.Kingdom.Get(leader.Clan.Kingdom).IsRevoltKingdom = true;
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

            mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject, false);

            if (!FactionManager.IsAtWarAgainstFaction(leader.MapFaction, settlement.MapFaction))
            {
                DeclareWarAction.Apply(leader.MapFaction, settlement.MapFaction);
            }

            var revolt = new Revolt(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction);
            Managers.Revolt.Revolts.Add(revolt);
            settlementInfo.HasRebellionEvent = true;

            if (settlementInfo.Garrision == null || settlementInfo.Garrision.TotalStrength == 0)
            {
                RevoltBehavior.EndSucceededRevolt(revolt);
                return;
            }

            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            StartBattleAction.Apply(mobileParty.Party, settlementInfo.Garrision);
        }

        internal static void EndFailedRevolt(Revolt revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolt.Settlement.Name);
            InformationManager.AddQuickInformation(information);

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionFailed(revolt.Settlement);

            if (revolt.IsMinorFaction)
            {
                Managers.Kingdom.DestroyKingdom(revolt.Party.Owner.Clan.Kingdom);
                KillCharacterAction.ApplyByExecution(revolt.Party.Owner, revolt.Settlement.OwnerClan?.Kingdom?.Leader ?? revolt.Settlement.OwnerClan.Leader, true);

                if (revolt.Party?.MobileParty != null)
                {
                    DestroyPartyAction.Apply(revolt.SettlementInfo.Garrision, revolt.Party.MobileParty);
                }
            }

            Managers.Revolt.Revolts.Remove(revolt);
        }

        internal static void EndSucceededRevolt(Revolt revolt)
        {
            var information = new TextObject("{=dkpS074R}The revolt in {SETTLEMENT} has ended.");
            information.SetTextVariable("SETTLEMENT", revolt.Settlement.Name);
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

                Managers.Revolt.Revolts.Remove(revolt);
            }
            else if (!revolt.IsMinorFaction)
            {
                revolt.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
                Managers.Revolt.Revolts.Remove(revolt);
            }
        }
    }
}