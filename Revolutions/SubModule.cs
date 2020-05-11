using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Revolts.CampaignBehaviors;
using TaleWorlds.Engine;
using KNTLibrary.Helpers;

namespace Revolts
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        internal static string BaseSavePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "Revolts", "Saves");

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            InformationManager.DisplayMessage(new InformationMessage("Revolts: Loaded Mod.", ColorHelper.Green));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (!(game.GameType is Campaign))
            {
                return;
            }

            this.InitializeMod(gameStarter as CampaignGameStarter);
        }

        private void InitializeMod(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                this._dataStorage = new DataStorage();
                this.AddBehaviours(campaignGameStarter);

                RevoltsManagers.Faction.DebugMode = Settings.Instance.DebugMode;
                RevoltsManagers.Kingdom.DebugMode = Settings.Instance.DebugMode;
                RevoltsManagers.Clan.DebugMode = Settings.Instance.DebugMode;
                RevoltsManagers.Party.DebugMode = Settings.Instance.DebugMode;
                RevoltsManagers.Character.DebugMode = Settings.Instance.DebugMode;
                RevoltsManagers.Settlement.DebugMode = Settings.Instance.DebugMode;
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolts: Failed to initialize!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new LuckyNationBehaviour());
            campaignGameStarter.AddBehavior(new RevoltBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new RevoltDailyBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());

            campaignGameStarter.AddModel(new Models.SettlementLoyaltyModel(ref this._dataStorage));
        }
    }
}