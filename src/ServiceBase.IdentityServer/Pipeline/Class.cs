﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ServiceBase.IdentityServer.Pipeline
{
    /// <summary>
    /// Models a recipient of notification of events
    /// </summary>
    public interface IPipeService
    {
        void Execute<TMessage>(TMessage message);
        IPipeService AddListener<TMessage, TListener>(); 
    }

    /// <summary>
    /// Default implementation of the event service. Write events raised to the log.
    /// </summary>
    public class DefaultPipeService : IPipeService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, List<Type>> _listeners;

        public DefaultPipeService(ILogger<DefaultPipeService> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _listeners = new Dictionary<Type, List<Type>>();
        }

        public void Execute<TMessage>(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var listenerTypes = GetListenerTypes<TMessage>();

            foreach (var listenerType in listenerTypes)
            {
                var listener = _provider.GetService(listenerType) as IEventListener<TMessage>;

                if (listener != null)
                {
                    listener.Execute(message); 
                }
            }            
        }

        public IPipeService AddListener<TMessage, TListener>()
        {
            GetListenerTypes<TMessage>().Add(typeof(TListener));

            return this;
        }

        List<Type> GetListenerTypes<TMessage>()
        {
            List<Type> listenersByMessageType = null;
            if (!_listeners.TryGetValue(typeof(TMessage), out listenersByMessageType))
            {
                listenersByMessageType = new List<Type>();
                _listeners.Add(typeof(TMessage), listenersByMessageType);
            }

            return listenersByMessageType;
        }
    }

    public interface IEventListener<TMessage>
    {
        void Execute<TMessage>(TMessage message);
    }


    // User registration case 

    public class UserRegisteredInfo
    {
        // ...
    }

    public class OnUserRegisteredSendEmail : IEventListener<UserRegisteredInfo>
    {
        public void Execute<TMessage>(TMessage message)
        {

        }
    }

    public class OnUserRegisteredSavePrimaryStore : IEventListener<UserRegisteredInfo>
    {
        public void Execute<TMessage>(TMessage message)
        {

        }
    }
    

    // Application bootstrapping 

    public class StartupFoo
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPipeService, DefaultPipeService>();
            services.AddTransient<OnUserRegisteredSendEmail>();
            services.AddTransient<OnUserRegisteredSavePrimaryStore>();
        }

        public void Configure(IPipeService eventService)
        {
            eventService
                .AddListener<UserRegisteredInfo, OnUserRegisteredSendEmail>()
                .AddListener<UserRegisteredInfo, OnUserRegisteredSavePrimaryStore>();
        }
    }
}
