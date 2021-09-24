﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ev.ServiceBus.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using NJsonSchema;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.AsyncApiSchema.v2.Bindings.Amqp;
using Saunter.Generation.Filters;
using Saunter.Generation.SchemaGeneration;

namespace Ev.ServiceBus.AsyncApi
{
    public class DocumentFilter : IDocumentFilter
    {
        private readonly IOptions<ServiceBusOptions> _options;

        public DocumentFilter(IOptions<ServiceBusOptions> options)
        {
            _options = options;
        }

        public void Apply(AsyncApiDocument document, DocumentFilterContext context)
        {
            var resolver = (AsyncApiSchemaResolver)context.SchemaResolver;
            document.DefaultContentType = "application/json";
            ProcessConnectionSettings(_options.Value.Settings.ConnectionSettings, document);

            document.Components.CorrelationIds.Add("default", new CorrelationId("$message.header#/correlationId")
            {
                Description = "Default Correlation ID"
            });

            foreach (var sender in _options.Value.Senders)
            {
                switch (sender.ClientType)
                {
                    case ClientType.Queue: ProcessQueueSender((QueueOptions)sender, document); break;
                    case ClientType.Topic: ProcessTopicSender((TopicOptions)sender, document); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var receiver in _options.Value.Receivers)
            {
                switch (receiver.ClientType)
                {
                    case ClientType.Queue: ProcessQueueReceiver((QueueOptions)receiver, document); break;
                    case ClientType.Subscription: ProcessSubscriptionReceiver((SubscriptionOptions)receiver, document); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var dispatch in _options.Value.DispatchRegistrations)
            {
                ProcessDispatch(dispatch, document, context, resolver);
            }

            foreach (var reception in _options.Value.ReceptionRegistrations)
            {
                ProcessReception(reception, document, context, resolver);
            }
        }

        private void ProcessReception(MessageReceptionRegistration reg, AsyncApiDocument document, DocumentFilterContext context, AsyncApiSchemaResolver asyncApiSchemaResolver)
        {
            var channelName = reg.Options.OriginalResourceId;
            var channel = GetOrCreateChannel(document, channelName);

            if (channel.Subscribe == null)
            {
                channel.Subscribe = CreateSubscribeOperation(channelName, reg.Options);
            }

            if (channel.Subscribe.Message == null)
            {
                channel.Subscribe.Message = new Messages();
            }
            ((Messages)channel.Subscribe.Message).OneOf.Add(GenerateMessage(reg.PayloadTypeId, reg.PayloadType, context, asyncApiSchemaResolver));
        }

        private Operation CreateSubscribeOperation(string channelName, ClientOptions options)
        {
            var operation = new Operation()
            {
                OperationId = "sub/" + channelName,
                Description = $"Reception of messages through the {options.OriginalResourceId} {options.ClientType}",
                Summary = $"{options.OriginalResourceId}",
                Tags = GetOperationTags(options)
            };
            return operation;
        }

        private void ProcessDispatch(MessageDispatchRegistration reg, AsyncApiDocument document, DocumentFilterContext context, AsyncApiSchemaResolver asyncApiSchemaResolver)
        {
            var channelName = reg.Options.OriginalResourceId + "/" + reg.PayloadTypeId;
            var channel = GetOrCreateChannel(document, channelName);

            if (channel.Publish == null)
            {
                channel.Publish = CreatePublishOperation(channelName, reg.Options);
            }

            if (channel.Publish.Message == null)
            {
                channel.Publish.Message = new Messages();
            }

            ((Messages)channel.Publish.Message).OneOf.Add(GenerateMessage(reg.PayloadTypeId, reg.PayloadType, context, asyncApiSchemaResolver));
        }

        private Operation CreatePublishOperation(string channelName, ClientOptions options)
        {
            var operation = new Operation()
            {
                OperationId = "pub/" + channelName,
                Description = $"Dispatch of messages through the {options.OriginalResourceId} {options.ClientType}",
                Summary = $"{options.OriginalResourceId}",
                Tags = GetOperationTags(options)
            };
            return operation;
        }

        private void ProcessSubscriptionReceiver(SubscriptionOptions options, AsyncApiDocument document)
        {
            ProcessConnectionSettings(options.ConnectionSettings, document);
            var channel = GetOrCreateChannel(document, $"{options.TopicName}/{options.SubscriptionName}");
        }

        private void ProcessQueueReceiver(QueueOptions options, AsyncApiDocument document)
        {
            ProcessConnectionSettings(options.ConnectionSettings, document);
            var channel = GetOrCreateChannel(document, options.QueueName);
        }

        private void ProcessTopicSender(TopicOptions options, AsyncApiDocument document)
        {
            ProcessConnectionSettings(options.ConnectionSettings, document);
            var channel = GetOrCreateChannel(document, options.TopicName);
        }

        private void ProcessQueueSender(QueueOptions options, AsyncApiDocument document)
        {
            ProcessConnectionSettings(options.ConnectionSettings, document);
            var channel = GetOrCreateChannel(document, options.QueueName);
        }

        private static IMessage GenerateMessage(string payloadTypeId, Type payloadType, DocumentFilterContext context, AsyncApiSchemaResolver asyncApiSchemaResolver)
        {
            var message = new Saunter.AsyncApiSchema.v2.Message()
            {
                Name = payloadTypeId,
                Payload = GetOrCreatePayloadSchema(payloadType, context),
                ContentType = "application/json",
                Title = payloadTypeId,
                Tags = new HashSet<Tag>()
                {
                    GetEvServiceBusTag()
                },
                // CorrelationId = new CorrelationIdReference("default"),
                Bindings = new MessageBindings()
                {
                    Amqp = new AmqpMessageBinding()
                    {
                        ContentEncoding = "UTF-8",
                        MessageType = payloadTypeId
                    }
                }
            };
            return asyncApiSchemaResolver.GetMessageOrReference(message);
        }

        private static JsonSchema GetOrCreatePayloadSchema(Type payloadType, DocumentFilterContext context)
        {
            JsonSchema schema;
            if (context.SchemaResolver.Schemas.Any(o => o.Title == payloadType.Name))
            {
                schema = context.SchemaResolver.GetSchema(payloadType, payloadType.IsEnum);
            }
            else
            {
                schema = context.SchemaGenerator.Generate(payloadType);
                context.SchemaResolver.AddSchema(payloadType, payloadType.IsEnum, schema);
            }

            return schema;
        }

        private ISet<Tag> GetOperationTags(ClientOptions clientOptions)
        {
            var tags = new HashSet<Tag>();

            tags.Add(new Tag(clientOptions.ClientType.ToString())
            {
                Description = "The type of client",
                ExternalDocs = new ExternalDocumentation("https://github.com/EcovadisCode/Ev.ServiceBus")
                {
                    // Description = "Ev.ServiceBus repository"
                }
            });
            tags.Add(GetEvServiceBusTag());
            return tags;
        }

        private static Tag GetEvServiceBusTag()
        {
            return new Tag("Ev.ServiceBus")
            {
                Description = "Generated by Ev.ServiceBus",
                ExternalDocs = new ExternalDocumentation("https://github.com/EcovadisCode/Ev.ServiceBus")
                {
                    Description = "Ev.ServiceBus repository"
                }
            };
        }

        private ChannelItem GetOrCreateChannel(AsyncApiDocument document, string name)
        {
            if (document.Channels.ContainsKey(name))
            {
                return document.Channels[name];
            }

            var channel = new ChannelItem();
            channel.Bindings = new ChannelBindings()
            {
                Amqp = new AmqpChannelBinding()
                {
                    Is = AmqpChannelBindingIs.Queue,
                    Queue = new AmqpChannelBindingQueue()
                    {
                        Durable = true,
                        Exclusive = false,
                        AutoDelete = false,
                        Name = name
                    }
                }
            };
            document.Channels.Add(name, channel);
            return channel;
        }

        private void ProcessConnectionSettings(ConnectionSettings? connectionSettings, AsyncApiDocument document)
        {
            if (connectionSettings == null)
            {
                return;
            }

            string endpoint;
            if (connectionSettings.Connection != null)
            {
                endpoint = connectionSettings.Connection.Endpoint.ToString();
            }
            else if (connectionSettings.ConnectionStringBuilder != null)
            {
                endpoint = connectionSettings.ConnectionStringBuilder.Endpoint;
            }
            else if (connectionSettings.ConnectionString != null)
            {
                var builder = new ServiceBusConnectionStringBuilder(connectionSettings.ConnectionString);
                endpoint = builder.Endpoint;
            }
            else
            {
                return;
            }

            if (endpoint == null || document.Servers.Any(o => o.Value.Url == endpoint))
            {
                return;
            }

            document.Servers.Add(endpoint, new Server(endpoint, "amqp"));
        }
    }
}
