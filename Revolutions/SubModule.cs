using KNTLibrary.Helpers;
using Revolutions.CampaignBehaviors;
using Revolutions.Components.CivilWars.CampaignBehaviors;
using Revolutions.Components.Revolts.CampaignBehaviors;
using Revolutions.Settings;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        internal static string BaseSavePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "Revolutions", "Saves");

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (!(game.GameType is Campaign))
            {
                return;
            }

            this.InitializeRevolutions(gameStarter as CampaignGameStarter);
        }

        private void InitializeRevolutions(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                this._dataStorage = new DataStorage();
                this.AddBehaviours(campaignGameStarter);
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed to initialize!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            if (RevolutionsSettings.Instance.EnableRevolts || RevolutionsSettings.Instance.EnableCivilWars)
            {
                campaignGameStarter.AddModel(new Models.SettlementLoyaltyModel());

                campaignGameStarter.AddBehavior(new RevolutionsBehavior(ref this._dataStorage, campaignGameStarter));
            }

            if (RevolutionsSettings.Instance.EnableRevolts)
            {
                campaignGameStarter.AddBehavior(new RevoltBehavior(ref this._dataStorage, campaignGameStarter));
            }

            if (RevolutionsSettings.Instance.EnableCivilWars)
            {
                campaignGameStarter.AddBehavior(new CivilWarsBehavior(ref this._dataStorage, campaignGameStarter));
            }
        }
    }
}