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
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoadedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnGameLoadedEvent(CampaignGameStarter obj)
        {
            DataStorage.LoadBaseData();
        }
    }
}