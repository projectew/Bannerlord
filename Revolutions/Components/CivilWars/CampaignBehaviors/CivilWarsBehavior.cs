using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using KNTLibrary.Helpers;
using System.Linq;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    public class CivilWarsBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public CivilWarsBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage = dataStorage;

            campaignGameStarter.AddBehavior(new CivilWarsDailyBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
        }

        private void MapEventEnded(MapEvent mapEvent)
        {

        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                this.DataStorage.SaveId = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

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
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }
    }
}