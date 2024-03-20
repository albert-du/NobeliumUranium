using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NobeliumUranium.Web.Models;

namespace NobeliumUranium.Web.Services
{
    public class TrainerService
    {
        private HttpClient _httpClient;
        public TrainerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async ValueTask<TrainingReturnStatus> TrainChatBotAsync(DialogCorpus corpus)
        {
            foreach (var convo in corpus.Conversations)
            {
                Task.Run(() =>
                {

                });
            }

            var values = new Dictionary<string, string>
            {
                {"data", corpus. }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync($"http://localhost:{10200}/train/", content); // assuming the default port, i give up
            return response.IsSuccessStatusCode ? TrainingReturnStatus.Success : TrainingReturnStatus.Failure;
        }
    }
    public enum TrainingReturnStatus
    {
        Success,
        Failure,
    }
}
