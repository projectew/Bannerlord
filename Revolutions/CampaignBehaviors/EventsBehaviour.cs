using System;
using KNTLibrary.Components.Events;
using SandBox.Quests.QuestBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

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
                Event newEvent = new Event(1, description);
                
                TurtlesOption turtlesOption = new TurtlesOption();
                
                newEvent.AddOption(turtlesOption);
                newEvent.Call();
            }
        }

        [Serializable]
        public class TurtlesOption : Option
        {
            public TurtlesOption() : base()
            {
                Id = "TurtlesEvent";
                Text = new TextObject("{=GdwerF78}Player likes turtles!");
            }
            
            public TurtlesOption(string id, TextObject text) : base(id, text)
            {
                
            }

            public override void Result()
            {
                var information = new TextObject("{=GdwerF78}Player likes turtles!");
                InformationManager.AddQuickInformation(information);
            }
        }
    }
}