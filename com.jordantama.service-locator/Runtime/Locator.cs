using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JordanTama.ServiceLocator
{
    public class Locator
    {
        private readonly Dictionary<string, IService> services = new Dictionary<string, IService>();

        private static Locator instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            instance = new Locator();
            
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => typeof(IService).IsAssignableFrom(p) && p != typeof(IService)).ToArray();

            foreach (Type type in types)
            {
                dynamic service = Convert.ChangeType(Activator.CreateInstance(type), type);
                Register(service);
            }
        }

        public static void Register<T>(T service) where T : IService
        {
            string key = typeof(T).Name;
            
            if (instance.services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to register service of type {key} which has already been registered.");
                return;
            }

            instance.services.Add(key, service);
            service.OnServiceRegistered();
        }

        public static void Unregister<T>() where T : IService
        {
            string key = typeof(T).Name;

            if (!instance.services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered.");
                return;
            }

            instance.services.Remove(key);
        }

        public static T Get<T>() where T : IService
        {
            string key = typeof(T).Name;

            if (!instance.services.ContainsKey(key))
                throw new Exception($"{key} is not a registered service.");

            return (T) instance.services[key];
        }

        public static bool Get<T>(out T service) where T : IService
        {
            service = Get<T>();
            return service != null;
        }
    }
}