using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using KNTLibrary.Helpers;

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

        private void ClanChangedKingdom(Clan arg1, Kingdom arg2, Kingdom arg3, bool arg4, bool arg5)
        {
            //throw new NotImplementedException();
        }
    }
}