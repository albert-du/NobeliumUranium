using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NobeliumUranium.Bot.Services
{
    //https://github.com/discord-net/Discord.Net/blob/dev/samples/02_commands_framework/Services/CommandHandlingService.cs
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DialogService _dialog;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _dialog = services.GetRequiredService<DialogService>();
            _services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var context = new SocketCommandContext(_discord, message);


            // This value holds the offset where the prefix ends
            var argPos = 0;


            if (message.HasMentionPrefix(_discord.CurrentUser, ref argPos)|| context.Channel.Name.ToLower().Replace(" ","") is "nouchat")
            {
                using (context.Channel.EnterTypingState())
                {
                    var filteredMessage = Utils.Preprocessing.FilterUserMessage(message.Content);
                    var response = await _dialog.GetChatBotResponseAsync(filteredMessage, context.Channel.Id.ToString());
                    await context.Channel.SendMessageAsync(response);
                }
                return;
            }

            // Perform prefix check. You may want to replace this with
            if (!message.HasCharPrefix('.', ref argPos)) return;
            // for a more traditional command format like !help.
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                return;
            }
            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                return;
            }

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
