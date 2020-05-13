using Helpers;
using KNTLibrary.Helpers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    internal class CivilWarsBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        internal CivilWarsBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage = dataStorage;

            campaignGameStarter.AddBehavior(new CivilWarsDailyBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                if (dataStore.IsLoading)
                {
                    this.DataStorage.LoadCivilWarData();
                }

                if (dataStore.IsSaving)
                {
                    this.DataStorage.SaveCivilWarData();
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars.Data: SyncData failed (IsLoading: {dataStore.IsLoading} | IsSaving: {dataStore.IsSaving})!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }

        private void MapEventEnded(MapEvent mapEvent)
        {

        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            if(oldKingdom == null)
            {
                return;
            }

            var kingdomInfo = Managers.Kingdom.GetInfo(oldKingdom);
            if(kingdomInfo == null || !kingdomInfo.IsCivilWarKingdom)
            {
                return;
            }

            var clans = oldKingdom.Clans.Where(go => !go.IsUnderMercenaryService && !go.IsClanTypeMercenary);
            if(clans.Count() > 0)
            {
                return;
            }

            foreach (var currentClan in oldKingdom.Clans.ToList())
            {
                foreach (var enemyFaction in Campaign.Current.Factions.Where(go => go.IsAtWarWith(currentClan)))
                {
                    if (clan.IsAtWarWith(enemyFaction))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(enemyFaction, currentClan);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(currentClan, enemyFaction);
                    }
                }

                clan.ClanLeaveKingdom(false);
            }

            Managers.Kingdom.DestroyKingdom(oldKingdom);
        }
    }
}