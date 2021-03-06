﻿using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    internal class PushNotificationBackgroundTask : IHostedService
    {
        public PushNotificationBackgroundTask(
            IPushNotificationsQueue messagesQueue,
            IEnumerable<IPushNotificationRecipientProvider> recipientProviders,
            IPushNotificationService notificationService,
            ILogger<PushNotificationBackgroundTask> logger
            )
        {
            _recipientProviders = recipientProviders;
            _messagesQueue = messagesQueue;
            _notificationService = notificationService;
            _log = logger;
        }

        private readonly IPushNotificationsQueue _messagesQueue;
        private readonly IEnumerable<IPushNotificationRecipientProvider> _recipientProviders;
        private readonly IPushNotificationService _notificationService;
        
        private readonly ILogger _log;
        private Task _dequeueMessagesTask;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                try
                {
                    PushQueueItem queueItem = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                    if (!_stopTokenSource.IsCancellationRequested)
                    {
                        var recipientProvider = GetRecipientProvider(queueItem.RecipientProviderName);
                        if (recipientProvider != null)
                        {
                            var subscriptions = await recipientProvider.GetRecipients(queueItem, _stopTokenSource.Token);
                            foreach (var subscription in subscriptions)
                            {
                                var pushMessage = FromModel(queueItem.Message);
                                await _notificationService.SendNotificationAsync(subscription, pushMessage, _stopTokenSource.Token);
                            }
                        }
                        else
                        {
                            _log.LogWarning($"failed to send notification because IPushNotificationRecipientProvider with name {queueItem.RecipientProviderName} was not found");
                        }
                    }
                }
                catch(Exception ex)
                {
                    _log.LogError($"{ex.Message}:{ex.StackTrace}");
                }

                
            }

        }

        private PushMessage FromModel(PushMessageModel model)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
                
            };
            var serializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };

            var serialized = JsonConvert.SerializeObject(model, serializationSettings);
            var content = new StringContent(serialized, Encoding.UTF8, "application/json");

            var pushMessage = new PushMessage(content)
            {
                //Topic = message.Topic,
                //Urgency = message.Urgency
            };

            return pushMessage;

        }

        private IPushNotificationRecipientProvider GetRecipientProvider(string providerName)
        {
            foreach(var p in _recipientProviders)
            {
                if (p.Name == providerName) return p;
            }

            return null;
        }

    }
}
