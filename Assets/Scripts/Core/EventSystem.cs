using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class EventSystem : MonoBehaviour
    {
        // event handlers
        private static Dictionary<Type, IEventHandler> _genericHandlers = new Dictionary<Type, IEventHandler>();
        private static EventHandler _eventHandler = new EventHandler();
    
        // no params 
        public static void Subscribe(string key, Action func) => _eventHandler.Subscribe(key, func);
        public static void Unsubscribe(string key, Action func) => _eventHandler.Unsubscribe(key, func);
        public static void Invoke(string key) => _eventHandler.Invoke(key);
    
        // single param generics
        public static void Subscribe<T>(string key, Action<T> func )
        {
            // create a new handler when you use a new type
            if (!_genericHandlers.ContainsKey(typeof(T)))
            {
                _genericHandlers[typeof(T)] = new EventHandler<T>();
            }
        
            // grab the handler and subscribe
            var handler = (EventHandler<T>)_genericHandlers[typeof(T)];
            handler.Subscribe(key, func);
        }
    
        public static void Unsubscribe<T>(string key, Action<T> func )
        {
            // do nothing if no handler
            if (!_genericHandlers.ContainsKey(typeof(T))) return;
        
            // grab the handler and unsubscribe
            var handler = (EventHandler<T>)_genericHandlers[typeof(T)];
            handler.Unsubscribe(key, func);
        }
    
        public static void Invoke<T>(string key, T args)
        {
            // do nothing if no handler
            if (!_genericHandlers.ContainsKey(typeof(T))) return;
        
            // grab the handler and do thing
            var handler = (EventHandler<T>)_genericHandlers[typeof(T)];
            handler.Invoke(key, args);        
        }
    }
}