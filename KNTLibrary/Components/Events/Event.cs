using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Screens;

namespace KNTLibrary.Components.Events
{
    [Serializable]
    public class Event
    {
        public string Id { get; }

        public string Description { get; }

        public string Sprite { get; }

        public List<EventOption> Options { get; }

        public Event()
        {
        }

        public Event(string id, string description, string sprite, List<EventOption> options = null)
        {
            this.Id = id;
            this.Description = description;
            this.Sprite = sprite;
            this.Options = options ?? new List<EventOption>();
        }

        public void AddOption(string id, string text)
        {
            this.Options.Add(new EventOption(id, text));
        }

        public void AddOption(EventOption option)
        {
            this.Options.Add(option);
        }

        public void Invoke()
        {
            ScreenManager.PushScreen(new EventScreen(this.Description, this.Sprite, this.Options));
        }
    }
}