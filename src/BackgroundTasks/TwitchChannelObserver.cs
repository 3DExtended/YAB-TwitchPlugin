using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchBotPlugin.Events;

using TwitchLib.Api.Services.Events.FollowerService;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

using YAB.Core.Contracts;
using YAB.Plugins;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.BackgroundTasks
{
    public class TwitchChannelObserver : IBackgroundTask
    {
        private readonly IEventSender _eventSender;
        private readonly ILogger _logger;
        private readonly IPipelineStore _pipelineStore;
        private readonly TwitchOptions _twitchOptions;

        public TwitchChannelObserver(IPipelineStore pipelineStore, ILogger logger, TwitchOptions twitchOptions, IEventSender eventSender)
        {
            _twitchOptions = twitchOptions;
            _eventSender = eventSender;
            _logger = logger;
            _pipelineStore = pipelineStore;
        }

        public Task InitializeAsync(CancellationToken cancellation)
        {
            Module.TwitchClient = new Lazy<TwitchLib.Client.Interfaces.ITwitchClient>(() => new TwitchClient());
            var creds = new ConnectionCredentials(_twitchOptions.TwitchBotUsername, _twitchOptions.TwitchBotToken);

            var client = Module.TwitchClient.Value;
            client.Initialize(creds, _twitchOptions.TwitchChannelToJoin);

            client.OnLog += OnTwitchClientLog;
            client.OnError += OnTwitchClientError;
            client.OnMessageReceived += OnTwitchClientMessageReceived;
            client.OnChatCommandReceived += OnTwitchClientChatCommandReceived;
            client.OnNewSubscriber += OnTwitchClientNewSubscriber;
            client.OnReSubscriber += OnTwitchClientReSubscriber;
            client.Connect();

            // Module.TwitchFollowerService.Value.OnNewFollowersDetected += OnTwitchNewFollowers;
            return Task.CompletedTask;
        }

        public async Task RunUntilCancelledAsync(CancellationToken cancellationToken)
        {
            var client = Module.TwitchClient.Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Hello from Plugin 1");
                await Task.Delay(5_000).ConfigureAwait(false);
            }
        }

        private void OnTwitchClientChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            _logger.LogInformation($"OnTwitchClientChatCommandReceived: {e.Command.CommandText}");

            _eventSender.SendEvent(new TwitchCommandEvent
            {
                Id = Guid.NewGuid(),
                Command = e.Command.CommandText,
                User = new User
                {
                    DisplayName = e.Command.ChatMessage.DisplayName,
                    Id = e.Command.ChatMessage.UserId,
                    Plattform = YAB.Core.PluginPlattform.Twitch
                }
            }, default);
        }

        private void OnTwitchClientError(object sender, OnErrorEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        private void OnTwitchClientLog(object sender, OnLogArgs e)
        {
            _logger.LogDebug(e.ToString());
        }

        private void OnTwitchClientMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            _logger.LogInformation($"OnTwitchClientMessageReceived: {e.ChatMessage.Message}");

            _eventSender.SendEvent(new TwitchMessageEvent
            {
                Id = Guid.NewGuid(),
                Message = e.ChatMessage.Message,
                User = new User
                {
                    DisplayName = e.ChatMessage.DisplayName,
                    Id = e.ChatMessage.UserId,
                    Plattform = YAB.Core.PluginPlattform.Twitch
                }
            }, default);
        }

        private void OnTwitchClientNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            _logger.LogInformation($"OnTwitchClientNewSubscriber: {e.Subscriber.DisplayName}");

            _eventSender.SendEvent(new TwitchNewSubscribedEvent
            {
                Id = Guid.NewGuid(),
                User = new User
                {
                    DisplayName = e.Subscriber.DisplayName,
                    Id = e.Subscriber.UserId,
                    Plattform = YAB.Core.PluginPlattform.Twitch
                }
            }, default);
        }

        private void OnTwitchClientReSubscriber(object sender, OnReSubscriberArgs e)
        {
            _eventSender.SendEvent(new TwitchReSubscribedEvent
            {
                Id = Guid.NewGuid(),
                Month = e.ReSubscriber.Months,
                User = new User
                {
                    DisplayName = e.ReSubscriber.DisplayName,
                    Id = e.ReSubscriber.UserId,
                    Plattform = YAB.Core.PluginPlattform.Twitch
                }
            }, default);
        }

        private void OnTwitchNewFollowers(object sender, OnNewFollowersDetectedArgs e)
        {
            _logger.LogInformation($"OnTwitchClientNewSubscriber: {e.NewFollowers.Count}");

            foreach (var follower in e.NewFollowers)
            {
                _eventSender.SendEvent(new TwitchUserFollowedEvent
                {
                    Id = Guid.NewGuid(),
                    User = new User
                    {
                        DisplayName = follower.FromUserName,
                        Id = follower.FromUserId,
                        Plattform = YAB.Core.PluginPlattform.Twitch
                    }
                }, default);
            }
        }
    }
}