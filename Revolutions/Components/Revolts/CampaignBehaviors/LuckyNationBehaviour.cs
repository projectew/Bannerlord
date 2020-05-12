using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    public class LuckyNationBehaviour : CampaignBehaviorBase
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
            if (!Settings.Instance.RevoltsLuckyNationMechanic)
            {
                foreach (var info in RevolutionsManagers.Kingdom.Infos.Where(kingdomInfo => kingdomInfo.LuckyNation))
                {
                    info.LuckyNation = false;
                }

                return;
            }

            if (Settings.Instance.RevoltsLuckyNationRandom)
            {
                if (!RevolutionsManagers.Kingdom.Infos.Any(i => i.LuckyNation) && RevolutionsManagers.Kingdom.Infos.Count > 0)
                {
                    RevolutionsManagers.Kingdom.Infos.GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.RevoltsLuckyNationImperial)
            {
                var imperialNations = RevolutionsManagers.Kingdom.Infos.Where(i => i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!imperialNations.Any(i => i.LuckyNation) && imperialNations.Count() > 0)
                {
                    imperialNations.GetRandomElement().LuckyNation = true;
                }
            }

            if (Settings.Instance.RevoltsLuckyNationNonImperial)
            {
                var nonImperialNations = RevolutionsManagers.Kingdom.Infos.Where(i => !i.Kingdom.Culture.Name.ToString().ToLower().Contains("empire"));
                if (!nonImperialNations.Any(i => i.LuckyNation) && nonImperialNations.Count() > 0)
                {
                    nonImperialNations.GetRandomElement().LuckyNation = true;
                }
            }
        }
    }
}