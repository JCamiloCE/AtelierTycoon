using System;
using System.Collections.Generic;

namespace JCC.GameplayEventSystem
{
    public class EventPool 
    {
        private readonly Dictionary<Type, List<EventBase>> p_events_dictionary = new ();

        internal TEvent GetInstance<TEvent>(params object[] parameters) where TEvent : EventBase, new()
        {
            if (p_events_dictionary.ContainsKey(typeof(TEvent)) == false) 
                p_events_dictionary.Add(typeof(TEvent), new List<EventBase>());

            TEvent event_data = (TEvent)GetAvailableEventFromList<TEvent>(p_events_dictionary[typeof(TEvent)]);
            event_data.SetParameters(parameters);
            return event_data;
        }

        private EventBase GetAvailableEventFromList<TEvent>(List<EventBase> event_list) where TEvent : EventBase, new() 
        {
            if (event_list.Exists(x => x.IsAvailable)) 
                return event_list.Find(x => x.IsAvailable);

            TEvent new_event_data = new ();
            event_list.Add(new_event_data);
            return new_event_data;
        }
    }
}