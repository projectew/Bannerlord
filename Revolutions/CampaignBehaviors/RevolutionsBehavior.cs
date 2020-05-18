using System;
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

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
        {
            DataStorage.LoadBaseData();
        }
    }
}