using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using KNTLibrary.Helpers;
using Revolutions.Components.Revolts.CampaignBehaviors;
using Revolutions.CampaignBehaviors;
using Revolts;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        internal static string BaseSavePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "Revolutions", "Saves");

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            InformationManager.DisplayMessage(new InformationMessage("Revolutions: Loaded Mod.", ColorHelper.Green));
        }

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
            campaignGameStarter.AddBehavior(new RevolutionsBehavior(ref this._dataStorage, campaignGameStarter));

            if (Settings.Instance.EnableRevolts)
            {
                campaignGameStarter.AddBehavior(new RevoltBehavior(ref this._dataStorage, campaignGameStarter));
            }

            if (Settings.Instance.EnableRevolts || Settings.Instance.EnableCivilWars)
            {
                campaignGameStarter.AddModel(new Models.SettlementLoyaltyModel(ref this._dataStorage));
            }
        }
    }
}