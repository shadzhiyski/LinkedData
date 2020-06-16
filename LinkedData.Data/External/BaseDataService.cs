using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LinkedData.Data.External
{
    public class BaseDataService
    {
        protected readonly HttpClient client = new HttpClient();

        protected BaseDataService(string serverBaseAddress)
        {
            ServerBaseAddress = serverBaseAddress;

            InitHttpClient();
        }

        public string ServerBaseAddress { get; protected set; }

        protected virtual void InitHttpClient()
        {
            client.BaseAddress = new Uri(ServerBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}