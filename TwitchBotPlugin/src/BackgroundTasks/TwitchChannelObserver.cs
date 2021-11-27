using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchBotPlugin.Events;
using TwitchBotPlugin.src.Options;

using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.FollowerService;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

using YAB.Core.Contracts;
using YAB.Plugins;
using YAB.Plugins.Injectables;

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

        public async Task InitializeAsync(CancellationToken cancellation)
        {
            var API = new TwitchAPI();
            API.Settings.ClientId = _twitchOptions.TwitchBotClientId;
            API.Settings.Secret = _twitchOptions.TwitchBotSecret;
            API.Settings.AccessToken = API.Helix.Users.GetAccessToken();

            Module.TwitchAPI = new YAB.Plugins.Injectables.Lazy<TwitchLib.Api.Interfaces.ITwitchAPI>(() => API);
            var user = await API.Helix.Users.GetUsersAsync(logins: new List<string> { _twitchOptions.TwitchChannelToJoin });
            var followers = await API.Helix.Users.GetUsersFollowsAsync(toId: user.Users.First().Id);

            var viewers = await API.Undocumented.GetChattersAsync(_twitchOptions.TwitchChannelToJoin);

            Module.TwitchClient = new YAB.Plugins.Injectables.Lazy<TwitchLib.Client.Interfaces.ITwitchClient>(() => new TwitchClient());
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

            Module.TwitchFollowerService = new YAB.Plugins.Injectables.Lazy<FollowerService>(() => new FollowerService(Module.TwitchAPI.Value));
            Module.TwitchFollowerService.Value.SetChannelsByName(new List<string> { _twitchOptions.TwitchChannelToJoin });

            Module.TwitchFollowerService.Value.OnServiceTick += OnTwitchFollowerServiceUpdate;
            Module.TwitchFollowerService.Value.OnNewFollowersDetected += OnTwitchNewFollowers;
            Module.TwitchFollowerService.Value.Start();

            await Module.TwitchFollowerService.Value.UpdateLatestFollowersAsync(callEvents: false).ConfigureAwait(false);
        }

        public async Task RunUntilCancelledAsync(CancellationToken cancellationToken)
        {
            var client = Module.TwitchClient.Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5_000).ConfigureAwait(false);
            }

            client.OnLog -= OnTwitchClientLog;
            client.OnError -= OnTwitchClientError;
            client.OnMessageReceived -= OnTwitchClientMessageReceived;
            client.OnChatCommandReceived -= OnTwitchClientChatCommandReceived;
            client.OnNewSubscriber -= OnTwitchClientNewSubscriber;
            client.OnReSubscriber -= OnTwitchClientReSubscriber;

            Module.TwitchFollowerService.Value.OnServiceTick -= OnTwitchFollowerServiceUpdate;
            Module.TwitchFollowerService.Value.OnNewFollowersDetected -= OnTwitchNewFollowers;

            client.Disconnect();

            Module.TwitchFollowerService?.Value?.Stop();

            Module.TwitchAPI = null;
            Module.TwitchClient = null;
            Module.TwitchFollowerService = null;

            Console.WriteLine("Killed everything");
        }

        private void OnTwitchClientChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            _logger.LogInformation($"OnTwitchClientChatCommandReceived: {e.Command.CommandText}");

            _eventSender.SendEvent(new TwitchCommandEvent
            {
                Id = Guid.NewGuid(),
                Command = e.Command.CommandText,
                Arguments = e.Command.ArgumentsAsList == null ? new List<string>() : e.Command.ArgumentsAsList,
                User = new User
                {
                    DisplayName = e.Command.ChatMessage.DisplayName,
                    Id = e.Command.ChatMessage.UserId,
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
                }
            }, default);
        }

        private void OnTwitchFollowerServiceUpdate(object sender, OnServiceTickArgs e)
        {
            var knownFollows = Module.TwitchFollowerService.Value.KnownFollowers;
        }

        private void OnTwitchNewFollowers(object sender, OnNewFollowersDetectedArgs e)
        {
            _logger.LogInformation($"OnTwitchClientNewFollowers: {e.NewFollowers.Count}");

            foreach (var follower in e.NewFollowers)
            {
                _eventSender.SendEvent(new TwitchUserFollowedEvent
                {
                    Id = Guid.NewGuid(),
                    User = new User
                    {
                        DisplayName = follower.FromUserName,
                        Id = follower.FromUserId,
                    }
                }, default);
            }
        }
    }
}