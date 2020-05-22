using KNTLibrary.Components.Events;
using KNTLibrary.Helpers;
using Revolutions.Settings;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.CampaignBehaviors
{
    internal class RevolutionsBehavior : CampaignBehaviorBase
    {
        private int _currentTick = 0;

        internal RevolutionsBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new CleanupBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));

            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                DataStorage.LoadData();
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed to initialize base data.", ColorHelper.Red));

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                    InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                }
            }
        }

        private void TickEvent(float dt)
        {
            if (this._currentTick == 100)
            {
                if (!EventManager.Instance.InEvent && EventManager.Instance.Events.Count > 0)
                {
                    EventManager.Instance.Events.First().Invoke();
                }

                this._currentTick = 0;
            }

            this._currentTick++;
        }
    }
}