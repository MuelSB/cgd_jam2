using System;
using System.Collections.Generic;

namespace Core
{
    // Interface
    public interface IEventHandler
    {
    }

// Default
    public class EventHandler : IEventHandler
    {
        private Dictionary<string, Action> _eventList = new Dictionary<string, Action>();
    
        public void Subscribe(string key, Action func )
        {
            // add to dictionary if event doesnt exist
            if (!_eventList.ContainsKey(key))
            {
                _eventList[key] = func;
                return;
            }
        
            // subscribe
            _eventList[key] += func;
        }
    
        public void Unsubscribe(string key, Action func )
        {
            // do nothing if the event doesnt exist
            if (!_eventList.ContainsKey(key)) return;
        
            // if no subscribers remove from the dictionary
            if (_eventList[key].GetInvocationList().Length < 1)
            {
                _eventList.Remove(key);
                return;
            }
        
            // unsubscribe
            _eventList[key] -= func;
        }
    
        public void Invoke(string key)
        {
            // return if no subscribers
            if (!_eventList.ContainsKey(key)) return;
        
            // do the thing
            _eventList[key].Invoke();
        }
    }

// Generic <T>
    public class EventHandler<T> : IEventHandler
    {
        private Dictionary<string, Action<T>> _eventList = new Dictionary<string, Action<T>>();
    
        public void Subscribe(string key, Action<T> func )
        {
            // add to dictionary if event doesnt exist
            if (!_eventList.ContainsKey(key))
            {
                _eventList[key] = func;
                return;
            }
        
            // subscribe
            _eventList[key] += func;    
        }
    
        public void Unsubscribe(string key, Action<T> func )
        {
            // do nothing if the event doesnt exist
            if (!_eventList.ContainsKey(key)) return;
        
            // if no subscribers remove from the dictionary
            if (_eventList[key].GetInvocationList().Length < 1)
            {
                _eventList.Remove(key);
                return;
            }
        
            // unsubscribe
            _eventList[key] -= func;
        }
    
        public void Invoke(string key, T args)
        {
            // return if no subscribers
            if (!_eventList.ContainsKey(key)) return;
        
            // do the thing
            _eventList[key].Invoke(args);    
        }
    }
}