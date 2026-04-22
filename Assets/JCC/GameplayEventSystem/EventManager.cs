using System;
using System.Collections.Generic;
using System.Linq;

namespace JCC.GameplayEventSystem
{
    public static class EventManager
    {
        private static Dictionary<Type, HashSet<IEventListenerBase>> s_event_listeners = new ();
        private static EventPool s_event_pool = new ();

        public static void AddListener<TEvent> (IEventListener<TEvent> new_listener) where TEvent : EventBase 
        {
            if(new_listener == null)
                throw new ArgumentNullException (nameof (new_listener));

            if (s_event_listeners.ContainsKey(typeof(TEvent)) == false)
                s_event_listeners.Add(typeof(TEvent), new HashSet<IEventListenerBase>());

            s_event_listeners[typeof(TEvent)].Add(new_listener);
        }

        public static void TriggerEvent<TEvent>(params object[] parameters) where TEvent : EventBase, new()
        {
            EventBase event_data = parameters.Length > 0 ? s_event_pool.GetInstance<TEvent>(parameters) : s_event_pool.GetInstance<NullObjectEvent>();

            if (s_event_listeners.ContainsKey(typeof(TEvent)) == false)
                return;

            event_data.IsAvailable = false;
            s_event_listeners[typeof(TEvent)].ToList().ForEach(listener => ((IEventListener<TEvent>)listener).OnEvent(event_data as TEvent));
            event_data.IsAvailable = true;
        }

        public static void RemoveListener<TEvent>(IEventListener<TEvent> listener) where TEvent : EventBase 
        {
            if (s_event_listeners.ContainsKey(typeof(TEvent)) == false)
                return;

            s_event_listeners[typeof(TEvent)].Remove(listener);
        }

        public static void ClearListener() 
        {
            s_event_listeners = new();
        }
    }
}