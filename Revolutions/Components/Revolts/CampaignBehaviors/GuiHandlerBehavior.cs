using Revolutions.Components.Revolts.Localization;
using Revolutions.Components.Revolts.Screens;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class GuiHandlerBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            this.CreateLoyaltyMenu(campaignGameStarter);
        }

        private void CreateLoyaltyMenu(CampaignGameStarter campaignGameStarter)
        {
            var menuName = new TextObject(GameTexts.RevoltsTownLoyalty);
            campaignGameStarter.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), (args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, (args) =>
            {
                var settlementInfo = Managers.Settlement.Get(Settlement.CurrentSettlement);
                ScreenManager.PushScreen(new TownRevoltsScreen(settlementInfo));
            }, false, 4);
        }
    }
}