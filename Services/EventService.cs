using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SimpleMarketplace.Api.Services
{
    public class EventService : IEventService
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _handlers = new ConcurrentDictionary<string, List<Delegate>>();

        public void Subscribe<T>(string eventName, Action<T> handler)
        {
            var handlers = _handlers.GetOrAdd(eventName, _ => new List<Delegate>());
            lock (handlers)
            {
                handlers.Add(handler);
            }
        }

        public void Publish<T>(string eventName, T data)
        {
            if (_handlers.TryGetValue(eventName, out var handlers))
            {
                List<Delegate> currentHandlers;
                lock (handlers)
                {
                    // Hacer una copia de los handlers para evitar problemas de concurrencia
                    currentHandlers = new List<Delegate>(handlers);
                }

                foreach (var handler in currentHandlers)
                {
                    if (handler is Action<T> typedHandler)
                    {
                        try
                        {
                            typedHandler(data);
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with other handlers
                            Console.WriteLine($"Error executing handler for event {eventName}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public void Unsubscribe<T>(string eventName, Action<T> handler)
        {
            if (_handlers.TryGetValue(eventName, out var handlers))
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
            }
        }
    }
}
