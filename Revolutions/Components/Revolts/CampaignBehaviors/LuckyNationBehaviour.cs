using Revolutions.Settings;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class LuckyNationBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoadedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunchedEvent(CampaignGameStarter starter)
        {
            this.SetLuckyNations();
        }

        private void OnGameLoadedEvent(CampaignGameStarter starter)
        {
            this.SetLuckyNations();
        }

        private void SetLuckyNations()
        {
            if(Managers.Kingdom.Infos.Count == 0)
            {
                return;
            }

            if (!RevolutionsSettings.Instance.RevoltsLuckyNationMechanic)
            {
                foreach (var info in Managers.Kingdom.Infos.Where(kingdomInfo => kingdomInfo.LuckyNation))
                {
                    info.LuckyNation = false;
                }

                return;
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationRandom)
            {
                if (!Managers.Kingdom.Infos.Any(i => i.LuckyNation))
                {
                    Managers.Kingdom.Infos.GetRandomElement().LuckyNation = true;
                }
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationImperial)
            {
                var imperialNations = Managers.Kingdom.Infos.Where(i => i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!imperialNations.Any(i => i.LuckyNation) && imperialNations.Count() > 0)
                {
                    imperialNations.GetRandomElement().LuckyNation = true;
                }
            }

            if (RevolutionsSettings.Instance.RevoltsLuckyNationNonImperial)
            {
                var nonImperialNations = Managers.Kingdom.Infos.Where(i => !i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!nonImperialNations.Any(i => i.LuckyNation) && nonImperialNations.Count() > 0)
                {
                    nonImperialNations.GetRandomElement().LuckyNation = true;
                }
            }
        }
    }
}