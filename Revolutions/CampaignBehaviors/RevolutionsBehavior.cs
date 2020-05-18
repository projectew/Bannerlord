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

        }

        public override void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsLoading)
            {
                DataStorage.LoadBaseData();
            }
        }
    }
}