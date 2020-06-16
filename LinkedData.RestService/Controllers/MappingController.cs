using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using LinkedData.Data.External;
//using RestService.Models;

namespace LinkedData.RestService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MappingController : ControllerBase
    {
        private readonly Neo4jDataService _dataService;

        public MappingController(Neo4jDataService dataService)
        {
            _dataService = dataService;
        }
        // GET api/mapping/dbFrom&dbTo&proteinIds
        [HttpGet]
        public ActionResult<JArray> Uniprot(string dbFrom, string dbTo, string[] proteinIds)
        {
            string requestInfo;
            var mappedIds = JArray.FromObject(UniProtDataService.Instance.MapIds(dbFrom, dbTo, proteinIds, out requestInfo)
                .Select(kv => JObject.FromObject(kv)));
            return mappedIds;
        }

        // GET api/mapping/linkuniprot?geneName
        [HttpGet]
        public ActionResult<JArray> LinkUniprot(string geneName)
        {
            _dataService.LinkGeneWithProteins(geneName);
            return null;
        }
    }
}