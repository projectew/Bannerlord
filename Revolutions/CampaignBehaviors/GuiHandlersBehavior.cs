using Revolutions.Screens;
using System;
using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.Network;

namespace Revolutions.CampaignBehaviors
{
    internal class GuiHandlersBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
            
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            this.CreateLoyaltyMenu(obj);
        }

        private void Tick(float dt)
        {
            if (Input.IsKeyReleased(InputKey.Home))
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.SetTimeSpeed(0);
                Game.Current.GameStateManager.ActiveStateDisabledByUser = true;

                Event newEvent = new Event(1, new TextObject("hello"));
                newEvent.AddOption(1, new TextObject("Ok"));
                newEvent.Call();
            }
        }

        private void CreateLoyaltyMenu(CampaignGameStarter obj)
        {
            var menuName = new TextObject("{=Ts1iVN8d}Town Loyalty");
            obj.AddGameMenuOption("town", "town_enter_entr_option", menuName.ToString(), (args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                return true;
            }, (args) =>
            {
                var settlementInfo = Managers.Settlement.GetInfo(Settlement.CurrentSettlement);
                ScreenManager.PushScreen(new TownRevoltsScreen(settlementInfo, settlementInfo.CurrentFactionInfo));
            }, false, 4);
        }
    }
}