using System;
using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;
using TaleWorlds.TwoDimension.Standalone;

namespace Revolutions.CampaignBehaviors
{
    public class EventsBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //
        }

        private void Tick(float dt)
        {
            if (Input.IsKeyReleased(InputKey.Home))
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.SetTimeSpeed(0);
                Game.Current.GameStateManager.ActiveStateDisabledByUser = true;

                TextObject description = new TextObject("{=gkeDF4f0}This is a test event. It's here so that you can understand how to call them, add descriptions and options. I'm making this a bit long so that we can ensure text fits properly, and users get enough space for a bit of creative writing. Hopefully it works ok, because it's my first test, and I need something to raise my mood.");
                Event newEvent = new Event(1, description, "Revolutions.PlottingLords");

                MyOption myOption = new MyOption();
                myOption.Information = new TextObject("{=GdwerF78}Player likes turtles!");
                myOption.Id = "TurtlesOption";
                myOption.Text = new TextObject("{=glEji4Fc}I like turtles");
                newEvent.AddOption(myOption);
                
                MyOption myOptio2 = new MyOption();
                myOption.Information = new TextObject("{=GdwerF78}Player likes turtles!");
                myOption.Id = "TurtlesOption2";
                myOption.Text = new TextObject("{=glEji4Fc}I like turtles");
                newEvent.AddOption(myOption);

                newEvent.Call();
            }
        }

        [Serializable]
        public class MyOption : Option
        {
            public TextObject Information;

            public MyOption() : base()
            {

            }

            public MyOption(string id, TextObject text) : base(id, text)
            {

            }

            public override void Result()
            {
                InformationManager.AddQuickInformation(Information);
            }
        }
    }
}