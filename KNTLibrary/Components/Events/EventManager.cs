using System.Collections.Generic;

namespace KNTLibrary.Components.Events
{
    public class EventManager
    {
        #region Singleton

        static EventManager()
        {
            Instance = new EventManager();
        }

        public static EventManager Instance { get; private set; }

        #endregion

        public List<Event> Events { get; set; } = new List<Event>();

        public bool InEvent { get; private set; }

        public void StartEvent(Event eventObj)
        {
            this.Events.Add(eventObj);
            this.InEvent = true;
        }

        public void EndEvent(Event eventObj)
        {
            this.Events.Remove(eventObj);
            this.InEvent = false;
        }
    }
}