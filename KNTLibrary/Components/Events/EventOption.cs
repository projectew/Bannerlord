using System;

namespace KNTLibrary.Components.Events
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
            this.Id = id;
            this.Text = text;
        }

        public virtual void Invoke()
        {

        }
    }
}