using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using POC.Orders.Commands;
using Volo.Abp.AspNetCore.Mvc;

namespace POC.Controllers
{
    [ApiController]
    [Route("api/Orders")]
    public class OrderController: POCController
    {
        private readonly IMediator _meditor;

        public OrderController(IMediator meditor)
        {
            _meditor = meditor;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var result = await _meditor.Send(command);
                return Ok(result);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
