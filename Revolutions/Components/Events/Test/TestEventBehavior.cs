using System;
using KNTLibrary.Components.Events;
using KNTLibrary.Components.Plots;
using Revolutions.Components.Events.Test;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;

namespace Revolutions.Components.Events
{
    public class TestEventBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void Tick(float dt)
        {
            int test = 2;
            
            if (Input.IsKeyReleased(InputKey.Home))
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.SetTimeSpeed(0);
                Game.Current.GameStateManager.ActiveStateDisabledByUser = true;

                if (test == 1)
                {
                    var id = "event_revolutions_testevent";
                    var description = "{=gkeDF4f0}This is a test event. It's here so that you can understand how to call them, add descriptions and options. I'm making this a bit long so that we can ensure text fits properly, and users get enough space for a bit of creative writing. Hopefully it works ok, because it's my first test, and I need something to raise my mood.";
                    var sprite = "Revolutions.PlottingLords";
                    var newEvent = new Event(id, description, sprite);
                    var optionOne = new TestEventOption("OptionOne", "{=glEji4Fc}I like turtles")
                    {
                        Information = "{=GdwerF78}Player likes turtles!"
                    };
                    var optionTwo = new TestEventOption("OptionTwo", "{=glEji4Fc}I don't like turtles")
                    {
                        Information = "{=GdwerF78}Player don't likes turtles!",
                    };

                    newEvent.AddOption(optionOne);
                    newEvent.AddOption(optionTwo);
                    newEvent.Invoke();
                }
                else if (test == 2)
                {
                    ScreenManager.PushScreen(new PlotScreen());;
                }
            }
        }
    }
}