﻿using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using Revolutions.CampaignBehaviors;
using Revolutions.Models;
using ModLib;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        public static string ModuleDataPath = Path.Combine(BasePath.Name, "Modules", "Revolutions", "ModuleData");

        protected override void OnSubModuleLoad()
        {
            try
            {
                FileDatabase.Initialise("Revolutions");

                Settings settings = FileDatabase.Get<Settings>("RevolutionsSettings");
                if (settings == null)
                {
                    settings = new Settings();
                }

                SettingsDatabase.RegisterSettings(settings);
                SettingsDatabase.SaveSettings(Settings.Instance);
            }
            catch (Exception exception)
            {
                var errorMessage = "Revolutions: Settings could not be loaded! ";
                InformationManager.DisplayMessage(new InformationMessage(errorMessage + exception?.ToString()));
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            try
            {
                InformationManager.DisplayMessage(new InformationMessage("Loaded Revolutions.", Color.White));
            }
            catch (Exception exception)
            {
                var errorMessage = "Revolutions: Could not be loaded! ";
                InformationManager.DisplayMessage(new InformationMessage(errorMessage + exception?.ToString()));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            try
            {
                base.OnGameStart(game, gameStarter);

                if (!(game.GameType is Campaign))
                {
                    return;
                }

                this.InitializeMod(gameStarter as CampaignGameStarter);
            }
            catch (Exception exception)
            {
                var exceptionMessage = "Revolutions: Failed to load on game start! ";
                InformationManager.DisplayMessage(new InformationMessage(exceptionMessage + exception?.ToString(), Color.FromUint(4282569842U)));
            }
        }

        private void InitializeMod(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                campaignGameStarter.LoadGameTexts(Path.Combine(SubModule.ModuleDataPath, "global_strings.xml"));

                this._dataStorage = new DataStorage();
                this.AddBehaviours(campaignGameStarter);
            }
            catch (Exception exception)
            {
                var exceptionMessage = "Revolutions: Failed to initialize! ";
                InformationManager.DisplayMessage(new InformationMessage(exceptionMessage + exception?.ToString(), Color.FromUint(4282569842U)));
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new RevolutionBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new RevolutionDailyBehavior(ref this._dataStorage));
            campaignGameStarter.AddBehavior(new GuiHandlersBehavior());
            campaignGameStarter.AddBehavior(new CleanupBehavior());

            campaignGameStarter.AddModel(new LoyaltyModel(ref this._dataStorage));
        }
    }
}