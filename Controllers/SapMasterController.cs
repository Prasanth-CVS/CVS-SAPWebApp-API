using CvsServiceLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvsServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SapMasterController : ControllerBase
    {

        private readonly SapService _sapService;

        public SapMasterController(SapService sapService)
        {
            _sapService = sapService;
        }

        [HttpGet("partners")]
        public async Task<IActionResult> GetPartners([FromQuery] string cardType, [FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var data = await _sapService.GetBusinessPartnersAsync(cardType, pageSize, pageNumber);
            return Ok(data);
        }

        [HttpPost("partner")]
        public async Task<IActionResult> CreatePartner([FromBody] object partnerData)
        {
            var result = await _sapService.CreateBusinessPartnerAsync(partnerData);
            return Ok(result);
        }
        
        [HttpPost("items")]
        public async Task<IActionResult> GetItemMasterDataAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await _sapService.GetItemMasterDataAsync(pageSize, pageNumber);
            return Ok(result);
        }
    }
}
