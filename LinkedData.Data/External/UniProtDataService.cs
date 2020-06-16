using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LinkedData.Data.External
{
    public class UniProtDataService : BaseDataService
    {
        protected static readonly UniProtDataService _instance;
        public static UniProtDataService Instance
        {
            get
            {
                return _instance;
            }
        }

        static UniProtDataService() 
        {
            _instance = new UniProtDataService();
        }

        protected UniProtDataService()
            : base("https://www.uniprot.org")
        { }

        public IEnumerable<KeyValuePair<string, string>> MapIds(string dbNameFrom, string dbNameTo, 
            string[] proteinIds, out string requestInfo)
        {
            var path = "uploadlists";
            WebClient webClient = new WebClient();
            webClient.BaseAddress = ServerBaseAddress;
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.QueryString.Add("from", dbNameFrom);
            webClient.QueryString.Add("to", dbNameTo);
            webClient.QueryString.Add("format", "tab");
            webClient.QueryString.Add("query", string.Join("\t", proteinIds));

            var mappingResult = webClient.DownloadString(new Uri(ServerBaseAddress + "/" + path));
            //JArray mappedIdsJson = null;
            KeyValuePair<string, string>[] mappedIds = null;
            if (!string.IsNullOrEmpty(mappingResult))
            {
                mappedIds = mappingResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)
                    .Select(MapIds)
                    //.Select(kv => JObject.FromObject(kv))
                    .ToArray();
                //mappedIdsJson = JArray.FromObject(mappedIds);
            }

            requestInfo = webClient.ToString();
            return mappedIds;
        }

        private KeyValuePair<string, string> MapIds(string row)
        {
            var ids = row.Split('\t');
            return new KeyValuePair<string, string>(ids[0], ids[1]);
        }
    }
}