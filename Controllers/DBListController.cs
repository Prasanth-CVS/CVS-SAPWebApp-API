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
    public class DBListController : ControllerBase
    {

        private readonly HanaDbHelper _hana;
        private readonly SapService _sapService;
        public DBListController(HanaDbHelper hana, SapService sapService) {

            _hana = hana;
            _sapService = sapService;
        }

        [HttpGet("Database")]
        public async Task<IActionResult> GetDatabase()
        {
            var sql = "SELECT \"dbName\", \"cmpName\" FROM SBOCOMMON.SRGC";
            var result = await _hana.QueryAsync(sql);
            return Ok(result);
        }

        [HttpGet("CompanyLogin")]
        public async Task<IActionResult> CompanyLogin([FromQuery] string username, [FromQuery] string password, [FromQuery] string database)
        {
            var data = await _sapService.CompanyLogin(username, password, database);
            return Ok(data);
        }
    }
}
