using System;
using System.Collections.Generic;

namespace Automation.GenerativeAI.UX.Services
{
    /// <summary>
    /// Implements a services container class which is a singleton and provides some static methods
    /// to register and get the services of specific type.
    /// </summary>
    public class ServiceContainer
    {
        private static ServiceContainer instance;

        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        private ServiceContainer() 
        {
            services.Add(typeof(IDialogService), new DialogService());
        }

        private static ServiceContainer GetInstance()
        {
            if (instance == null) { instance = new ServiceContainer(); }
            return instance;
        }

        /// <summary>
        /// Allows to register service of a given type.
        /// </summary>
        /// <typeparam name="T">Type of the services</typeparam>
        /// <param name="service">Service instance</param>
        public static void Register<T>(T service)
        {
            var instance = GetInstance();
            instance.services.Add(typeof(T), service);
        }

        /// <summary>
        /// Resolves and returns a registered service instance of a given type.
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>service instance if registered else null</returns>
        public static T Resolve<T>()
        {
            var instance = GetInstance();
            Type type = typeof(T);
            foreach (var pair in instance.services)
            {
                if(type.IsAssignableFrom(pair.Key))
                {
                    return (T)pair.Value;
                }
            }

            return default(T);
        }
    }
}
