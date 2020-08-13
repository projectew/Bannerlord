using System;
using System.Linq;
using Revolutions.Library;
using Revolutions.Module.Components.Factions;
using Revolutions.Module.Components.Kingdoms;
using Revolutions.Module.Components.Settlements;
using Revolutions.Module.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using GameTexts = Revolutions.Module.Components.Revolts.Localization.GameTexts;

namespace Revolutions.Module.Components.Revolts.CampaignBehaviors
{
    internal class RevoltBehavior : CampaignBehaviorBase
    {
        public RevoltBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new GuiHandlerBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunchedEvent);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTickEvent);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, MapEventEnded);
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChangedEvent);
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, ClanChangedKingdom);
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
                var kingdomInfos = imperialNations as KingdomInfo[] ?? imperialNations.ToArray();

                if (kingdomInfos.Any() && !kingdomInfos.Any(i => i.LuckyNation))
                {
                    var imperialNation = kingdomInfos.GetRandomElement();
                    if (imperialNation != null)
                    {
                        imperialNation.LuckyNation = true;
                    }
                }
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationNonImperial)
            {
                var nonImperialNations = Managers.Kingdom.Infos.Where(i => !i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                var kingdomInfos = nonImperialNations as KingdomInfo[] ?? nonImperialNations.ToArray();

                if (kingdomInfos.Any() && !kingdomInfos.Any(i => i.LuckyNation))
                {
                    var nonImperialNation = kingdomInfos.GetRandomElement();
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

            var revolt = Managers.Revolt.GetRevoltByParty(involvedParty);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                EndFailedRevolt(revolt);
            }
            else
            {
                EndSucceededRevolt(revolt);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = Managers.Settlement.Get(settlement);
            settlementInfo.UpdateOwnerRevolt(newOwner.MapFaction);

            if (capturedHero?.PartyBelongedTo?.Party == null)
            {
                return;
            }

            var revolt = RevoltManager.Instance.GetRevoltByParty(capturedHero.PartyBelongedTo.Party);
            if (revolt == null || RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic || !revolt.IsMinorFaction)
            {
                return;
            }

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

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            foreach (var settlement in clan.Settlements)
            {
                var settlementInfo = Managers.Settlement.Get(settlement);
                settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
                settlementInfo.CurrentFactionId = clan.MapFaction.StringId;
            }

            var clanInfo = Managers.Clan.Get(clan);

            if (clan.StringId == Clan.PlayerClan.StringId || clanInfo.CanChangeKingdom || !clanInfo.IsRevoltClan || clan.StringId == newKingdom.RulingClan.StringId
                || clan.Culture.Name.ToString().ToLower().Contains("empire") && newKingdom.Culture.Name.ToString().ToLower().Contains("empire")
                || clan.Culture.Name.ToString() == newKingdom.Culture.Name.ToString())
            {
                return;
            }

            clan.ClanLeaveKingdom();
        }

        private void DailyTickEvent()
        {
            foreach (var info in Managers.Settlement.Infos)
            {
                if (info.Settlement.Parties.Any(party => party.IsLordParty && party.Party.Owner.Clan == info.Settlement.OwnerClan))
                {
                    info.Settlement.Town.Loyalty += RevolutionsSettings.Instance.RevoltsGeneralOwnerInTownLoyaltyIncrease;

                    if (info.Settlement.OwnerClan.StringId == Hero.MainHero.Clan.StringId)
                    {
                        var textObject = new TextObject(GameTexts.RevoltsLoyaltyIncrease);
                        textObject.SetTextVariable("SETTLEMENT", info.Settlement.Name);
                        InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
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
                    StartRevolt(info.Settlement);
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
            var textObject = new TextObject(GameTexts.RevoltsRevoltStart);
            textObject.SetTextVariable("SETTLEMENT", settlement.Name);
            InformationManager.AddNotice(new SettlementRebellionMapNotification(settlement, textObject));
            InformationManager.AddQuickInformation(textObject);

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

                var bannerInfo = Managers.Banner.GetBanner(settlementInfo);
                var banner = bannerInfo != null ? new Banner(bannerInfo.BannerId) : null;

                var clan = Managers.Clan.CreateClan(leader, null, null, banner);
                Managers.Clan.Get(leader.Clan).IsRevoltClan = true;

                textObject = new TextObject(GameTexts.RevoltsMinorFactionKingdom);
                textObject.SetTextVariable("SETTLEMENT", settlement.Name);
                Managers.Kingdom.CreateKingdom(leader, textObject, textObject, banner ?? clan.Banner);
                Managers.Kingdom.Get(leader.Clan.Kingdom).IsRevoltKingdom = true;
            }

            var mobileParty = Managers.Party.CreateMobileParty(leader, null, settlement.GatePosition, settlement, !atWarWithLoyalFaction);

            var amountOfTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseSize + (int)(settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralProsperityMulitplier)) / 3;
            var basicUnits = new TroopRoster();
            basicUnits.AddToCounts(leader.Culture.RangedMilitiaTroop, amountOfTroops);
            basicUnits.AddToCounts(leader.Culture.MeleeMilitiaTroop, amountOfTroops * 2);
            mobileParty.MemberRoster.Add(basicUnits);

            if (settlement.MilitaParty != null && settlement.MilitaParty.CurrentSettlement == settlement && settlement.MilitaParty.MapEvent == null)
            {
                foreach (var troopRosterElement in settlement.MilitaParty.MemberRoster)
                {
                    mobileParty.AddElementToMemberRoster(troopRosterElement.Character, troopRosterElement.Number);
                }

                settlement.MilitaParty.RemoveParty();
            }

            mobileParty.ChangePartyLeader(mobileParty.Party.Owner.CharacterObject);

            if (!FactionManager.IsAtWarAgainstFaction(leader.MapFaction, settlement.MapFaction))
            {
                DeclareWarAction.Apply(leader.MapFaction, settlement.MapFaction);
            }

            var revolt = new Revolt(mobileParty.Party.Id, settlement, !atWarWithLoyalFaction);
            Managers.Revolt.Revolts.Add(revolt);
            settlementInfo.HasRebellionEvent = true;

            if (settlementInfo.Garrison == null || Math.Abs(settlementInfo.Garrison.TotalStrength) < 1)
            {
                EndSucceededRevolt(revolt);
                return;
            }

            mobileParty.Ai.SetDoNotMakeNewDecisions(true);
            StartBattleAction.Apply(mobileParty.Party, settlementInfo.Garrison);
        }

        internal static void EndFailedRevolt(Revolt revolt)
        {
            var textObject = new TextObject(GameTexts.RevoltsRevoltEnd);
            textObject.SetTextVariable("SETTLEMENT", revolt.Settlement.Name);
            InformationManager.AddQuickInformation(textObject);

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionFailed(revolt.Settlement);

            if (revolt.IsMinorFaction)
            {
                Managers.Kingdom.DestroyKingdom(revolt.Party.Owner.Clan.Kingdom);
                KillCharacterAction.ApplyByExecution(revolt.Party.Owner, revolt.Settlement.OwnerClan?.Kingdom?.Leader ?? revolt.Settlement.OwnerClan?.Leader);

                if (revolt.Party?.MobileParty != null)
                {
                    DestroyPartyAction.Apply(revolt.SettlementInfo.Garrison, revolt.Party.MobileParty);
                }
            }

            Managers.Revolt.Revolts.Remove(revolt);
        }

        internal static void EndSucceededRevolt(Revolt revolt)
        {
            var textObject = new TextObject(GameTexts.RevoltsRevoltEnd);
            textObject.SetTextVariable("SETTLEMENT", revolt.Settlement.Name);
            InformationManager.AddQuickInformation(textObject);

            revolt.SettlementInfo.CurrentFactionInfo.CityRevoltionSucceeded(revolt.Settlement);

            if (RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic && revolt.SettlementInfo.IsCurrentFactionOfImperialCulture && !revolt.SettlementInfo.IsLoyalFactionOfImperialCulture)
            {
                revolt.Settlement.OwnerClan.AddRenown(-RevolutionsSettings.Instance.RevoltsImperialRenownLoss);
            }

            if (RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
            {
                ChangeOwnerOfSettlementAction.ApplyByRevolt(revolt.Party.LeaderHero, revolt.Settlement);
                revolt.Party.LeaderHero.Clan.AddRenown(RevolutionsSettings.Instance.RevoltsMinorFactionsRenownGainOnWin);

                foreach (var notable in revolt.Settlement.Notables.Concat(revolt.Settlement.BoundVillages.SelectMany(s => s.Settlement.Notables)))
                {
                    notable.SetPersonalRelation(revolt.Party.Leader.HeroObject, new Random().Next(5, 25));
                }

                var amountOTroops = (RevolutionsSettings.Instance.RevoltsGeneralBaseSize + (int)(revolt.Settlement.Prosperity * RevolutionsSettings.Instance.RevoltsGeneralProsperityMulitplier)) / 3;
                var eliteUnits = new TroopRoster();
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.RangedEliteMilitiaTroop, amountOTroops);
                eliteUnits.AddToCounts(revolt.Party.Leader.Culture.MeleeEliteMilitiaTroop, amountOTroops * 2);
                revolt.Party.MobileParty.MemberRoster.Add(eliteUnits);

                SetPartyAiAction.GetActionForPatrollingAroundSettlement(revolt.Party.MobileParty, revolt.Settlement);
                revolt.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);

                Managers.Revolt.Revolts.Remove(revolt);
            }

            if (!RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic || revolt.IsMinorFaction)
            {
                return;
            }

            revolt.Party.MobileParty.Ai.SetDoNotMakeNewDecisions(false);
            Managers.Revolt.Revolts.Remove(revolt);
        }
    }
}