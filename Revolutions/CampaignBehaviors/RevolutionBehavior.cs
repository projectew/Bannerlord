using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Actions;
using KNTLibrary;
using Revolutions.Components.Settlements;
using Revolutions.Components.Revolutions;
using HarmonyLib;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));

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
                    this.DataStorage.InitializeData();
                    this.DataStorage.LoadData();
                }

                if (dataStore.IsSaving)
                {
                    this.DataStorage.SaveData();
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})!", ColorManager.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorManager.Red));
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter obj)
        {
            this.DataStorage.InitializeData();
        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            var involvedParty = mapEvent.InvolvedParties.Intersect(RevolutionManager.Instance.GetParties()).FirstOrDefault();
            if (involvedParty == null)
            {
                return;
            }

            var currentRevolution = RevolutionsManagers.RevolutionManager.GetRevolutionByPartyId(involvedParty.Id);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                RevolutionsManagers.RevolutionManager.EndFailedRevolution(currentRevolution);
            }
            else
            {
                RevolutionsManagers.RevolutionManager.EndSucceededRevoluton(currentRevolution);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = RevolutionsManagers.SettlementManager.GetInfo(settlement);
            settlementInfo.UpdateOwnerRevolution(newOwner.MapFaction);
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            if(RevolutionsManagers.KingdomManager.GetInfo(kingdom)?.IsCustomKingdom == true)
            {
                foreach (var clan in kingdom.Clans)
                {
                    RevolutionsManagers.CharacterManager.RemoveAndDestroyCharacter(clan.Leader.CharacterObject);
                    RevolutionsManagers.ClanManager.RemoveAndDestroyClan(clan);
                }
            }

            RevolutionsManagers.KingdomManager.RemoveKingdom(kingdom);
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var info = RevolutionsManagers.ClanManager.GetInfo(clan);
            if(info == null || !info.IsCustomClan)
            {
                return;
            }

            if(clan.Kingdom.Clans.Where(go => go.StringId != clan.StringId).Count() == 0)
            {
                RevolutionsManagers.KingdomManager.RemoveAndDestroyKingdom(clan.Kingdom);
            }

            RevolutionsManagers.ClanManager.RemoveClan(clan);
        }

        private void OnPartyRemovedEvent(PartyBase party)
        {
            RevolutionsManagers.PartyManager.RemoveInfo(party.Id);
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = RevolutionsManagers.ClanManager.GetInfo(clan);

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