using CvsServiceLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;

namespace CvsServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        ServicesLayerHana _ServicesLayerHana;
        public MasterController(IConfiguration configuration) {

            _ServicesLayerHana = new ServicesLayerHana(configuration);
        }

        /*[HttpGet("partners")]
        public async Task<IActionResult> GetPartners()
        {
            var data = await _ServicesLayerHana.GetBusinessPartnersAsync();
            return Ok(data);
        }*/

        


    }
}
