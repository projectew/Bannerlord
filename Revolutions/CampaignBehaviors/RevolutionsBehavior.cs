using KNTLibrary.Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Revolutions.CampaignBehaviors
{
    internal class RevolutionsBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        internal RevolutionsBehavior(ref DataStorage dataStorage, CampaignGameStarter campaignGameStarter)
        {
            this.DataStorage = dataStorage;

            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());
            Managers.Banner.AddBanners(BasePath.Name + "Modules/Revolutions/ModuleData/Banners");
        }

        public override void RegisterEvents()
        {

        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
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
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.Base.Data: SyncData failed (IsLoading: {dataStore.IsLoading} | IsSaving: {dataStore.IsSaving})!", ColorHelper.Red));
                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
                }
            }
        }
    }
}