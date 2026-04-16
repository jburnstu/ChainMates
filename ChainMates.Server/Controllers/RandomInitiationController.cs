using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs;
using ChainMates.Server.Services;
using System.Diagnostics;
using System.Net;


namespace ChainMates.Server.Controllers
{
    [ApiController]
    [Route("chainmates/randomcreation")]
    public class RandomInitiationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RandomInitiationService _service;

        public RandomInitiationController (AppDbContext context)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new RandomInitiationService(context);
        }


        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RandomCreationDto dto)
        {
            Debug.WriteLine("INSIDE POST");
            await _service.CreateRandomAuthorsAndSegments(dto);
            return Ok(dto);

        }

    }
}
