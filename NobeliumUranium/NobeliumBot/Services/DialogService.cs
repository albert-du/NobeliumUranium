using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using NobeliumUranium.Utils;

namespace NobeliumUranium.Bot.Services
{
    public class DialogService
    {
        private int port;
        private readonly HttpClient _http;
        private readonly string chatbotStartFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "NoUDialog");
        private readonly string chatbotStartFile = "app.py";
        public DialogService(IServiceProvider services)
        {
            _http = services.GetRequiredService<HttpClient>();
        }
        public void Initialize()
        {
            // find an open port and start the dialog engine
            port = Utils.Network.FindOpenPort(10200);
            if (port == 0)
                throw new Exception("No open port could be found");

            var dialogEngine = Path.Combine(chatbotStartFolder, chatbotStartFile);
            // ensure the file and environment exist
            if (!Directory.Exists(Path.Combine(chatbotStartFolder, "env"))) throw new FileNotFoundException("Could not find the Python Virtual Environment");
            if (!File.Exists(dialogEngine)) throw new FileNotFoundException("Could not find the Chatbot Dialog engine file");

            using (Process myProcess = new Process())
            {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                myProcess.StartInfo.FileName = Path.Combine(chatbotStartFolder, "env", isWindows ? "Scripts" : "bin", isWindows ? "python" : "python3");
                myProcess.StartInfo.Arguments = $"{dialogEngine} {port}";
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                myProcess.Start();
            }
        }

        public async Task<string> GetChatBotResponseAsync(string message, string channelContext)
        {
            var values = new Dictionary<string, string>
            {
                {"sender",channelContext },
                {"message", message }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await _http.PostAsync($"http://localhost:{port}/response/", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
