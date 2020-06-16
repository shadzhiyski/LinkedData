using System;
using System.Collections.Generic;
using System.Net;
using LinkedData.Data.Models;
using Newtonsoft.Json.Linq;

namespace LinkedData.Data.External
{
    public class EbiDataService : BaseDataService
    {
        protected static readonly EbiDataService _instance;
        public static EbiDataService Instance
        {
            get
            {
                return _instance;
            }
        }

        static EbiDataService() 
        {
            _instance = new EbiDataService();
        }

        protected EbiDataService() 
            : base("https://www.ebi.ac.uk")
        {
            
        }

        public JArray GetGeneRelatedData(string geneName)
        {
            var path = "proteins/api/proteins";

            WebClient webClient = new WebClient();
            webClient.BaseAddress = ServerBaseAddress;
            webClient.Headers.Add("Accept", "application/json");
            webClient.QueryString.Add("size", 100.ToString());
            webClient.QueryString.Add("gene", geneName);

            var contentString = webClient.DownloadString(new Uri(ServerBaseAddress + "/" + path));

            var jsonArray = JArray.Parse(contentString);
            return jsonArray;
        }
    }
}