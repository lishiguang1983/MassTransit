﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using Configuration;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.EntityNameFormatters;
    using MassTransit.Topology.Topologies;


    public static class ActiveMqBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for RabbitMQ
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IActiveMqBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(MessageTopology);
            var busConfiguration = new ActiveMqBusConfiguration(topologyConfiguration);
            var busEndpoingConfiguration = busConfiguration.CreateEndpointConfiguration();

            var configurator = new ActiveMqBusFactoryConfigurator(busConfiguration, busEndpoingConfiguration);

            configure(configurator);

            return configurator.Build();
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTopologyConfigurator> MessageTopologyValue =
                new Lazy<IMessageTopologyConfigurator>(() => new MessageTopology(_entityNameFormatter),
                    LazyThreadSafetyMode.PublicationOnly);

            static readonly IEntityNameFormatter _entityNameFormatter;

            static Cached()
            {
                _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(new ActiveMqMessageNameFormatter());
            }
        }
    }
}