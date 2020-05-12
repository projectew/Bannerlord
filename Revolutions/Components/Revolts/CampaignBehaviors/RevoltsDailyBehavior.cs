using System;
using TaleWorlds.CampaignSystem;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.Base.Factions;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class RevoltsDailyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            RevolutionsManagers.Revolt.IncreaseDailyLoyaltyForSettlement();
            RevolutionsManagers.Revolt.CheckRevoltProgress();
            this.DailyUpdates();
        }

        private void DailyUpdates()
        {
            foreach (var factionInfo in RevolutionsManagers.Faction.Infos)
            {
                factionInfo.DailyUpdate();
            }

            foreach (var settlementInfo in RevolutionsManagers.Settlement.Infos)
            {
                settlementInfo.DailyUpdate();
            }
        }
    }
}