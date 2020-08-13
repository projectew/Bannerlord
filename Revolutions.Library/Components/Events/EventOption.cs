using System;

namespace Revolutions.Library.Components.Events
{
    [Serializable]
    public class EventOption
    {
        public string Id { get; }

        public string Text { get; }

        public EventOption()
        {

        }

        public EventOption(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public virtual void Invoke()
        {

        }
    }
}