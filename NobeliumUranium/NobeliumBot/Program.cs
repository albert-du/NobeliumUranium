using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using NobeliumUranium.Bot.Services;
using System.IO;
using System.Text.Json;

namespace NobeliumUranium.Bot
{
    class Program
    {
        public const bool Development = true;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task<string> GetDiscordTokenAsync(bool isDevelopment)
        {
            var tokenFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..","..", "token.json");
            if (!File.Exists(tokenFile))
            {
                // create the file and stop the program
                using (FileStream fs = File.Create(tokenFile))
                {
                    var config = new TokenConfig
                    {
                        DiscordToken = "Set Token Here",
                        TestDiscordToken = "Set Development Token Here"
                    };
                    await JsonSerializer.SerializeAsync(fs, config);
                }

                Console.Write("Please set tokens in the Config file");
                Environment.Exit(0);
            }
            // read the file and return the token
            using (FileStream fs = File.OpenRead(tokenFile))
            {
                var Tokens = await JsonSerializer.DeserializeAsync<TokenConfig>(fs);
                return isDevelopment ? Tokens.TestDiscordToken : Tokens.DiscordToken;
            }
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot,await GetDiscordTokenAsync(Development));
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                services.GetRequiredService<DialogService>().Initialize();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<DialogService>()
                .BuildServiceProvider();
        }
    }
}
