using KNTLibrary.Helpers;
using Revolutions.CampaignBehaviors;
using Revolutions.Components.CivilWars.CampaignBehaviors;
using Revolutions.Components.Revolts.CampaignBehaviors;
using Revolutions.Settings;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.TwoDimension;
using TaleWorlds.MountAndBlade;

namespace Revolutions
{
    public class SubModule : MBSubModuleBase
    {
        private DataStorage _dataStorage;

        internal static string BaseSavePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "Revolutions", "Saves");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var uiResourceDepot = UIResourceManager.UIResourceDepot;
            var resourceContext = UIResourceManager.ResourceContext;
            var spriteData1 = UIResourceManager.SpriteData;
            var spriteData2 = new SpriteData("RevolutionsSpriteData");
            spriteData2.Load(uiResourceDepot);
            var texture = new TaleWorlds.TwoDimension.Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/Revolutions/GUI/SpriteSheets/", "revolutions-ui-1.png")));
            spriteData1.SpriteCategories.Add("revolutions_events", spriteData2.SpriteCategories["revolutions_events"]);
            spriteData1.SpriteNames.Add("Revolutions.PlottingLords", (Sprite)new SpriteGeneric("Revolutions.PlottingLords", spriteData2.SpritePartNames["Revolutions.PlottingLords"]));
            spriteData1.SpriteNames.Add("Revolutions.Whatever", (Sprite)new SpriteGeneric("Revolutions.Whatever", spriteData2.SpritePartNames["Revolutions.Whatever"]));

            var spriteCategory = spriteData1.SpriteCategories["revolutions_events"];
            spriteCategory.SpriteSheets.Add(texture);
            spriteCategory.Load(resourceContext, uiResourceDepot);
            UIResourceManager.BrushFactory.Initialize();
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
                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
                }
            }
        }

        private void AddBehaviours(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddModel(new Components.General.Models.SettlementLoyaltyModel());
            campaignGameStarter.AddBehavior(new RevolutionsBehavior(ref this._dataStorage, campaignGameStarter));

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