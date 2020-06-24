using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LinkedData.Data.External
{
    public class EnsemblDataService : BaseDataService
    {
        public EnsemblDataService()
            :base("http://rest.ensembl.org")
        { }
        
        public async Task<string> GetGeneIdByNameAsync(string geneName) 
        {
            string geneId = null;
            var path = string.Format("xrefs/symbol/homo_sapiens/{0}", geneName);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var contentString =  await response.Content.ReadAsStringAsync();
                JArray content = JArray.Parse(contentString);

                geneId = content[0]["id"].ToString();
            }

            return geneId;
        }

        public async Task<JArray> GetGeneVariationsAsync(string geneName) 
        {
            var path = string.Format("/phenotype/gene/homo_sapiens/{0}?include_associated=1;include_overlap=1", geneName);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var contentString =  await response.Content.ReadAsStringAsync();
                JArray content = JArray.Parse(contentString);

                return content;
            }

            return null;
        }
    }
}
