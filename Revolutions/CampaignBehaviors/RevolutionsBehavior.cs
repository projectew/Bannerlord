using System;
using System.Runtime.Versioning;
using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;

namespace Revolutions.CampaignBehaviors
{
    internal class RevolutionsBehavior : CampaignBehaviorBase
    {
        internal RevolutionsBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
        {
            DataStorage.ClearData();
            DataStorage.ClearRevoltData();
            DataStorage.ClearCivilWarData();
        }

        public override void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsLoading)
            {
                DataStorage.ClearData();
                DataStorage.LoadBaseData();
            }
        }
    }
}