using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;

namespace KNTLibrary.Components.Events
{
    [Serializable]
    public class Event
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string Sprite { get; set; }

        public List<EventOption> Options { get; set; } = new List<EventOption>();

        public Event()
        {
        }

        public Event(string id, string description, string sprite)
        {
            this.Id = id;
            this.Description = description;
            this.Sprite = sprite;

        }

        public void AddOption(string id, string text)
        {
            this.Options.Add(new EventOption(id, text));
        }

        public void AddOption(EventOption option)
        {
            this.Options.Add(option);
        }

        public void Invoke(bool pause = true)
        {
            if (EventManager.Instance.InEvent)
            {
                EventManager.Instance.StartEvent(this);
            }
            else
            {
                EventManager.Instance.StartEvent(this);

                if (pause)
                {
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                    Campaign.Current.SetTimeSpeed(0);
                    Game.Current.GameStateManager.ActiveStateDisabledByUser = true;
                }

                ScreenManager.PushScreen(new EventScreen(this.Description, this.Sprite, this.Options, this));
            }
        }
    }
}