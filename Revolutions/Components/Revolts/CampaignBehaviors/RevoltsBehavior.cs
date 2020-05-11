using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using HarmonyLib;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolts;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    public class RevoltBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevoltBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
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
                this.DataStorage.SaveId = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

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
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.Revolt: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})!", ColorHelper.Red));
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

            var currentRevolt = RevoltsManagers.Revolt.GetRevoltByPartyId(involvedParty.Id);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                RevoltsManagers.Revolt.EndFailedRevolt(currentRevolt);
            }
            else
            {
                RevoltsManagers.Revolt.EndSucceededRevoluton(currentRevolt);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.UpdateOwnerRevolt(newOwner.MapFaction);
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            if (RevoltsManagers.Kingdom.GetInfo(kingdom)?.IsCustomKingdom == true)
            {
                foreach (var clan in kingdom.Clans)
                {
                    RevoltsManagers.Character.RemoveAndDestroyCharacter(clan.Leader.CharacterObject);
                    RevoltsManagers.Clan.RemoveAndDestroyClan(clan);
                }
            }

            RevoltsManagers.Kingdom.RemoveKingdom(kingdom);
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var info = RevoltsManagers.Clan.GetInfo(clan);
            if (info == null || !info.IsCustomClan)
            {
                return;
            }

            if (clan.Kingdom.Clans.Where(go => go.StringId != clan.StringId).Count() == 0)
            {
                RevoltsManagers.Kingdom.RemoveAndDestroyKingdom(clan.Kingdom);
            }

            RevoltsManagers.Clan.RemoveClan(clan);
        }

        private void OnPartyRemovedEvent(PartyBase party)
        {
            RevoltsManagers.Party.RemoveInfo(party.Id);
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = RevoltsManagers.Clan.GetInfo(clan);

            if (clanInfo != null && newKingdom != null &
                !clanInfo.CanJoinOtherKingdoms && newKingdom.RulingClan.StringId != clan.StringId
                && !(clan.Culture.Name.ToString().ToLower().Contains("empire") && newKingdom.Culture.Name.ToString().ToLower().Contains("empire"))
                && !(clan.Culture.Name.ToString() == newKingdom.Culture.Name.ToString())
                && clan.StringId != Clan.PlayerClan.StringId)
            {
                clan.ClanLeaveKingdom(false);
            }
        }
    }
}