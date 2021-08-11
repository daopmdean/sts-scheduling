using sts_scheduling.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace sts_scheduling.Utils
{
    public class SendHttpRequest
    {
        public static async Task SendScheduleResult(ScheduleResponse schedule)
        {
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new(clientHandler);
            //client.BaseAddress = new Uri("http://35.72.3.192:8080/");
            client.BaseAddress = new Uri("https://sts-project.azurewebsites.net/");
            //client.BaseAddress = new Uri("https://localhost:44301/");
            //client.BaseAddress = new Uri("http://localhost:8090/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMinutes(4);

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/shift-schedule", schedule);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();
                error.Message = "From Schedule server: " + error.Message;
                throw new Exception();
            }
        }
    }
}
