using System.Collections.Generic;

namespace KNTLibrary.Components.Events
{
    public class EventManager
    {
        #region Singleton

        static EventManager()
        {
            EventManager.Instance = new EventManager();
        }

        public static EventManager Instance { get; private set; }

        #endregion

        public List<Event> Events { get; set; } = new List<Event>();

        public bool InEvent { get; private set; }

        public void StartEvent(Event eventobj)
        {
            this.Events.Add(eventobj);
            this.InEvent = true;
        }

        public void EndEvent(Event eventobj)
        {
            this.Events.Remove(eventobj);
            this.InEvent = false;
        }
    }
}