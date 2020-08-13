using System;
using HarmonyLib;
using Revolutions.Library.Helpers;
using Revolutions.Module.CampaignBehaviors;
using Revolutions.Module.Components.CivilWars.CampaignBehaviors;
using Revolutions.Module.Components.Revolts.CampaignBehaviors;
using Revolutions.Module.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;
using Path = System.IO.Path;
using SettlementLoyaltyModel = Revolutions.Module.Components.Revolts.Models.SettlementLoyaltyModel;
using Texture = TaleWorlds.Engine.Texture;

namespace Revolutions.Module
{
    public class SubModule : MBSubModuleBase
    {
        internal static string BaseSavePath => Path.Combine(Utilities.GetConfigsPath(), "Revolutions", "Saves");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var uiResourceDepot = UIResourceManager.UIResourceDepot;
            var resourceContext = UIResourceManager.ResourceContext;
            var spriteData1 = UIResourceManager.SpriteData;
            var spriteData2 = new SpriteData("RevolutionsSpriteData");
            spriteData2.Load(uiResourceDepot);
            var texture = new TaleWorlds.TwoDimension.Texture(new EngineTexture(Texture.CreateTextureFromPath("../../Modules/Revolutions/GUI/SpriteSheets/", "revolutions-ui-1.png")));
            spriteData1.SpriteCategories.Add("revolutions_events", spriteData2.SpriteCategories["revolutions_events"]);
            spriteData1.SpriteNames.Add("Revolutions.PlottingLords", new SpriteGeneric("Revolutions.PlottingLords", spriteData2.SpritePartNames["Revolutions.PlottingLords"]));
            spriteData1.SpriteNames.Add("Revolutions.Whatever", new SpriteGeneric("Revolutions.Whatever", spriteData2.SpritePartNames["Revolutions.Whatever"]));

            var spriteCategory = spriteData1.SpriteCategories["revolutions_events"];
            spriteCategory.SpriteSheets.Add(texture);
            spriteCategory.Load(resourceContext, uiResourceDepot);
            UIResourceManager.BrushFactory.Initialize();

            var harmony = new Harmony(GetType().Namespace);
            harmony.PatchAll(GetType().Assembly);
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (!(game.GameType is Campaign))
            {
                return;
            }

            InitializeRevolutions(gameStarter as CampaignGameStarter);
        }

        private void InitializeRevolutions(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                AddBehaviours(campaignGameStarter);
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
            campaignGameStarter.AddBehavior(new RevolutionsBehavior(campaignGameStarter));

            if (RevolutionsSettings.Instance.EnableRevolts)
            {
                campaignGameStarter.AddModel(new SettlementLoyaltyModel());
                campaignGameStarter.AddBehavior(new RevoltBehavior(campaignGameStarter));
            }

            if (RevolutionsSettings.Instance.EnableCivilWars)
            {
                campaignGameStarter.AddBehavior(new CivilWarsBehavior(campaignGameStarter));
            }
        }
    }
}