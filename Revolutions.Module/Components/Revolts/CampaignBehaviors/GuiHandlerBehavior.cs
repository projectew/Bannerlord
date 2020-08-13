using Revolutions.Module.Components.Revolts.Localization;
using Revolutions.Module.Components.Revolts.Screens;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace Revolutions.Module.Components.Revolts.CampaignBehaviors
{
    internal class GuiHandlerBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            CreateLoyaltyMenu(campaignGameStarter);
        }

        private void CreateLoyaltyMenu(CampaignGameStarter campaignGameStarter)
        {
            var menuName = new TextObject(GameTexts.RevoltsTownLoyalty);
            campaignGameStarter.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), args =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, args =>
            {
                var settlementInfo = Managers.Settlement.Get(Settlement.CurrentSettlement);
                ScreenManager.PushScreen(new TownRevoltsScreen(settlementInfo));
            }, false, 4);
        }
    }
}