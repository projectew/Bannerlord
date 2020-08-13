using System;
using System.Linq;
using Revolutions.Library.Components.Events;
using Revolutions.Library.Helpers;
using Revolutions.Module.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Module.CampaignBehaviors
{
    internal class RevolutionsBehavior : CampaignBehaviorBase
    {
        private int _currentTick;

        internal RevolutionsBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new CleanupBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunchedEvent);

            CampaignEvents.TickEvent.AddNonSerializedListener(this, TickEvent);
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
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed to initialize data. Turn on Debug Mode for detailed information.", ColorHelper.Red));

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                    InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                }
            }
        }

        private void TickEvent(float dt)
        {
            if (_currentTick == 100)
            {
                if (!EventManager.Instance.InEvent && EventManager.Instance.Events.Count > 0)
                {
                    EventManager.Instance.Events.First().Invoke();
                }

                _currentTick = 0;
            }

            _currentTick++;
        }
    }
}