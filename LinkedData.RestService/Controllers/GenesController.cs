using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using LinkedData.Data.External;
using LinkedData.Data.Models;
using LinkedData.Data.Repositories;
using LinkedData.RestService.Models;
using System.Net.Http;
using System.Net;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace LinkedData.RestService.Controllers
{
    [Route("api/[controller]")]
    //[Produces("application/json")]
    [ApiController]
    public class GenesController : ControllerBase
    {
        protected GenesRepository _repository;
        
        protected ProteinsRepository _proteinsRepository;

        public GenesController(GenesRepository repository,
            ProteinsRepository proteinsRepository)
        {
            _repository = repository;
            _proteinsRepository = proteinsRepository;
        }
        // GET api/genes/all
        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<Gene>> GetGenes()
        {
            var genes = _repository.GetAll();
            
            if (genes == null)
            {
                return BadRequest();
            }
            
            return Ok(genes);
        }

        // GET api/genes/{geneName}
        [HttpGet("{geneName}")]
        public ActionResult<Gene> GetGene(string geneName)
        {
            var searchedGene = _repository.Get((Gene gene) => gene.Name == geneName);
            
            if (searchedGene == null)
            {
                return BadRequest();
            }

            return Ok(searchedGene);
        }

        // GET api/genes/{geneName}/proteins
        [HttpGet("{geneName}/proteins")]
        public ActionResult<IEnumerable<Protein>> GetGeneProteins(string geneName)
        {
            var searchedProteins = _proteinsRepository.GetAll((Gene gene) => gene.Name == geneName,
                new GeneProteinRelationship());
            
            if (searchedProteins == null)
            {
                return BadRequest();
            }

            return Ok(searchedProteins);
        }

        // PUT api/genes
        [HttpPut]
        public ActionResult<Gene> PutGene([FromBody] GenePutModel gene)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // try 
            // {
                var newGene = new Gene()
                {
                    Name = gene.Name
                };

                _repository.Put(newGene);
                _repository.PutRelated(newGene);
            //}
            // catch (Exception ex)
            // {
            //     return BadRequest(ex.Message);
            // }

            return Ok(gene);
        }

        // PUT api/genes
        // [HttpPost]
        // public async Task<ActionResult<Gene>> PutGene(IFormFile file)
        // {
        //     if (file == null)
        //     {
        //         return BadRequest();
        //     }

        //     long size = file.Length;

        //     // full path to file in temp location
        //     var filePath = Path.GetTempFileName();

        //     if (file.Length > 0)
        //     {
        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await file.CopyToAsync(stream);
        //         }
        //     }

        //     // process uploaded files
        //     // Don't rely on or trust the FileName property without validation.
        //     return Ok(new { size, filePath });
        // }
    }
}
