using System;
using TaleWorlds.CampaignSystem;
using Revolutions.Components.BaseComponents.Settlements;
using Revolutions.Components.BaseComponents.Factions;

namespace Revolts.CampaignBehaviors
{
    public class RevoltDailyBehavior : CampaignBehaviorBase
    {
        private readonly DataStorage DataStorage;

        public RevoltDailyBehavior(ref DataStorage dataStorage)
        {
            this.DataStorage = dataStorage;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            RevoltsManagers.Revolt.IncreaseDailyLoyaltyForSettlement();
            RevoltsManagers.Revolt.CheckRevoltProgress();
            this.UpdateSettlementInfos();
        }

        private void UpdateSettlementInfos()
        {
            foreach (var factionInfo in RevoltsManagers.Faction.Infos)
            {
                factionInfo.DailyUpdate();
            }

            foreach (var settlementInfo in RevoltsManagers.Settlement.Infos)
            {
                settlementInfo.DailyUpdate();
            }
        }
    }
}