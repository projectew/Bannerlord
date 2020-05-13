using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolutions.Settings;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class RevoltBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        internal RevoltBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage = dataStorage;

            campaignGameStarter.AddBehavior(new RevoltsDailyBehavior());
            campaignGameStarter.AddBehavior(new LuckyNationBehaviour());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));

            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));

            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyedEvent));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartyRemovedEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                if (dataStore.IsLoading)
                {
                    this.DataStorage.LoadRevoltData();
                }

                if (dataStore.IsSaving)
                {
                    this.DataStorage.SaveRevoltData();
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.Revolts.Data: SyncData failed (IsLoading: {dataStore.IsLoading} | IsSaving: {dataStore.IsSaving})!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
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
                Managers.Revolt.EndFailedRevolt(currentRevolt);
            }
            else
            {
                Managers.Revolt.EndSucceededRevolut(currentRevolt);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            settlementInfo.UpdateOwnerRevolt(newOwner.MapFaction);

            if (capturedHero != null && Managers.Character.GetInfo(capturedHero.CharacterObject).IsRevoltKingdomLeader)
            {
                var revolt = RevoltManager.Instance.GetRevoltByParty(capturedHero.PartyBelongedTo.Party);
                if (!RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
                {
                    var noble = KNTLibrary.BaseManagers.Faction.GetLordWithLeastFiefs(revolt.SettlementInfo.LoyalFaction).HeroObject;
                    ChangeOwnerOfSettlementAction.ApplyBySiege(noble, noble, settlement);
                    Managers.Kingdom.DestroyKingdom(capturedHero.Clan.Kingdom);
                    Managers.Clan.DestroyClan(capturedHero.Clan);
                    capturedHero.PartyBelongedTo.RemoveParty();
                    RevoltManager.Instance.Revolts.Remove(revolt);
                }
            }
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            if (Managers.Kingdom.GetInfo(kingdom)?.IsCustomKingdom == true)
            {
                foreach (var clan in kingdom.Clans)
                {
                    Managers.Character.RemoveAndDestroyCharacter(clan.Leader.CharacterObject);
                    Managers.Clan.DestroyClan(clan);
                }
            }

            Managers.Kingdom.RemoveKingdom(kingdom);
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var info = Managers.Clan.GetInfo(clan);
            if (info == null || !info.IsCustomClan)
            {
                return;
            }

            if (clan.Kingdom.Clans.Where(go => go.StringId != clan.StringId).Count() == 0)
            {
                Managers.Kingdom.DestroyKingdom(clan.Kingdom);
            }

            Managers.Clan.RemoveClan(clan);
        }

        private void OnPartyRemovedEvent(PartyBase party)
        {
            Managers.Party.RemoveInfo(party.Id);
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = Managers.Clan.GetInfo(clan);

            if (clan.StringId == Clan.PlayerClan.StringId || clanInfo.CanChangeKingdom || !clanInfo.IsRevoltClan || clan.StringId == newKingdom.RulingClan.StringId
                || clan.Culture.Name.ToString().ToLower().Contains("empire") && newKingdom.Culture.Name.ToString().ToLower().Contains("empire")
                || clan.Culture.Name.ToString() == newKingdom.Culture.Name.ToString())
            {
                return;
            }

            clan.ClanLeaveKingdom(false);
        }
    }
}