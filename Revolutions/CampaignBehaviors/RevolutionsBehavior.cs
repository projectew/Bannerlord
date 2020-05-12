using HarmonyLib;
using KNTLibrary.Helpers;
using System;
using System.IO;
using System.Threading;
using KNTLibrary.Components.Banners;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Revolutions.CampaignBehaviors
{
    public class RevolutionsBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevolutionsBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage = dataStorage;

            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());
            RevolutionsManagers.Banner.AddBanners(BasePath.Name + "Modules/Revolutions/ModuleData", "Banners");
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                this.DataStorage.SaveId = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();
                Thread.Sleep(1000);

                if (dataStore.IsLoading)
                {
                    this.DataStorage.LoadBaseData();
                }

                if (dataStore.IsSaving)
                {
                    this.DataStorage.SaveBaseData();
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.Base: SyncData failed ({dataStore.IsLoading} | {dataStore.IsSaving} | {this.DataStorage.SaveId})!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage.InitializeBaseData();
        }
    }
}