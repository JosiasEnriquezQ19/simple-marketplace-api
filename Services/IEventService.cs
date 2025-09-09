using System;
namespace SimpleMarketplace.Api.Services
{
    public interface IEventService
    {
        // Método para suscribirse a un evento
        void Subscribe<T>(string eventName, Action<T> handler);
        
        // Método para publicar un evento
        void Publish<T>(string eventName, T data);
        
        // Método para darse de baja de un evento
        void Unsubscribe<T>(string eventName, Action<T> handler);
    }
}
