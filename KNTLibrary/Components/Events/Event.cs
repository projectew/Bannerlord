using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace KNTLibrary.Components.Events
{
    [Serializable]
    public class Event
    {
        public Event() { }

        public Event(int id, TextObject description)
        {
            Id = id;
            Description = description;
        }

        public readonly int Id;
        public readonly TextObject Description;
        public readonly List<Option> Options = new List<Option>();

        public void AddOption(int id, TextObject text)
        {
            Options.Add(new Option(id, text));
        }

        public void Call()
        {
            ScreenManager.PushScreen(new EventScreen(Options, Description));
        }
    }

    [Serializable]
    public class Option
    {
        public Option() { }

        public Option(int id, TextObject text)
        {
            Id = id;
            Text = text;
        }
        
        public readonly int Id;
        public readonly TextObject Text;
    }
}