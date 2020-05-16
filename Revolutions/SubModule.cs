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
using TaleWorlds.Library;
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

            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            SpriteData spriteData1 = UIResourceManager.SpriteData;
            SpriteData spriteData2 = new SpriteData("RevolutionsSpriteData");
            spriteData2.Load(uiResourceDepot);
            TaleWorlds.TwoDimension.Texture texture = new TaleWorlds.TwoDimension.Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/Revolutions/GUI/SpriteSheets/", "entrepreneur-ui-1.png")));
            spriteData1.SpriteCategories.Add("entrepreneur_icons", spriteData2.SpriteCategories["entrepreneur_icons"]);
            spriteData1.SpritePartNames.Add("FinancesIcon", spriteData2.SpritePartNames["FinancesIcon"]);
            spriteData1.SpriteNames.Add("FinancesIcon", (Sprite)new SpriteGeneric("FinancesIcon", spriteData2.SpritePartNames["FinancesIcon"]));
            spriteData1.SpritePartNames.Add("MapbarLeftFrame", spriteData2.SpritePartNames["MapbarLeftFrame"]);
            spriteData1.SpriteNames.Add("MapbarLeftFrame", (Sprite)new SpriteGeneric("MapbarLeftFrame", spriteData2.SpritePartNames["MapbarLeftFrame"]));
            spriteData1.SpritePartNames.Add("Entrepreneur.EmptyField", spriteData2.SpritePartNames["Entrepreneur.EmptyField"]);
            spriteData1.SpriteNames.Add("Entrepreneur.EmptyField", (Sprite)new SpriteGeneric("Entrepreneur.EmptyField", spriteData2.SpritePartNames["Entrepreneur.EmptyField"]));
            spriteData1.SpritePartNames.Add("Entrepreneur.WorkingField", spriteData2.SpritePartNames["Entrepreneur.WorkingField"]);
            spriteData1.SpriteNames.Add("Entrepreneur.WorkingField", (Sprite)new SpriteGeneric("Entrepreneur.WorkingField", spriteData2.SpritePartNames["Entrepreneur.WorkingField"]));
            spriteData1.SpritePartNames.Add("Entrepreneur.VillagePropertyIcon", spriteData2.SpritePartNames["Entrepreneur.VillagePropertyIcon"]);
            spriteData1.SpriteNames.Add("Entrepreneur.VillagePropertyIcon", (Sprite)new SpriteGeneric("Entrepreneur.VillagePropertyIcon", spriteData2.SpritePartNames["Entrepreneur.VillagePropertyIcon"]));
            SpriteCategory spriteCategory = spriteData1.SpriteCategories["entrepreneur_icons"];
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
            if (RevolutionsSettings.Instance.EnableRevolts || RevolutionsSettings.Instance.EnableCivilWars)
            {
                campaignGameStarter.AddModel(new Components.General.Models.SettlementLoyaltyModel());
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

            campaignGameStarter.AddBehavior(new EventsBehaviour());
        }
    }
}