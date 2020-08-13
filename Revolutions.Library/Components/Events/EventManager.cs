using System.Collections.Generic;

namespace Revolutions.Library.Components.Events
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
            Events.Add(eventObj);
            InEvent = true;
        }

        public void EndEvent(Event eventObj)
        {
            Events.Remove(eventObj);
            InEvent = false;
        }
    }
}