using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class EventSystem : MonoBehaviour
    {
        // map for all events
        private static Dictionary<string, UnityEvent> _events = new Dictionary<string, UnityEvent>();
        
        // delegate used as func parameter
        public delegate void Fnc();
        //public delegate void Fnc<in T>(T param);

        public static void Subscribe(string name, Fnc fnc)
        {
            // manages memory assignment 
            if (!_events.ContainsKey(name))
            {
                _events[name] = new UnityEvent();
            }
            // subscribes the function
            _events[name].AddListener(fnc.Invoke);
        }
        
        public static void Unsubscribe(string name, Fnc fnc)
        {
            // early exit if it doesnt exist
            if (!_events.ContainsKey(name)) return;
            
            // unsubscribes 
            _events[name].RemoveListener(fnc.Invoke);
            
            // removes mem if not being used
            if (_events[name].GetPersistentEventCount() < 1)
            {
                _events.Remove(name);
            }
        }

        public static bool Invoke(string name)
        {
            // return false if no exist
            if (!_events.ContainsKey(name)) return false;
            
            // invoke event and return true
            _events[name].Invoke();
            return true;
        }
    }
    
}
