using Revolutions.Components.Base.Factions;
using Revolutions.Components.Base.Settlements;
using System;
using TaleWorlds.CampaignSystem;

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
            Managers.Revolt.IncreaseDailyLoyaltyForSettlement();
            Managers.Revolt.CheckRevoltProgress();
            this.DailyUpdates();
        }

        private void DailyUpdates()
        {
            foreach (var factionInfo in Managers.Faction.Infos)
            {
                factionInfo.DailyUpdate();
            }

            foreach (var settlementInfo in Managers.Settlement.Infos)
            {
                settlementInfo.DailyUpdate();
            }
        }
    }
}