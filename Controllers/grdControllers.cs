using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using grdApi.Data;
using grdApi.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace grdApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class grdApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public grdApiController(AppDbContext db) => _db = db;

        [HttpPost("fnGetMerchData")]
       
        public async Task<IActionResult> fnGetMerchData([FromBody] MerchDataRequest request)
        { 
            MerchDataResponse _out= new MerchDataResponse();
              if (request is null) return BadRequest("Request body is required.");
              var connString = _db.Database.GetConnectionString();
              DataAccess da = new DataAccess();
              _out = da.fnGetMerchData(request,connString);
              // _out.error_desc = "in main controller page";              
              //_out.error_desc =connString;

            return Ok( _out);
        }
        
        [HttpPost("fnGetAttendanceDtl")]
        public async Task<IActionResult> fnGetAttendanceDtl([FromBody] clsiAttendanceDtl request)
        { 
            clsoAttendance _out= new clsoAttendance();
              if (request is null) return BadRequest("Request body is required.");
              var connString = _db.Database.GetConnectionString();
              DataAccess da = new DataAccess();
              _out = da.fnGetAttendanceDtl(request,connString);
              // _out.error_desc = "in main controller page";              
              //_out.error_desc =connString;

            return Ok( _out);
        }

    }
}