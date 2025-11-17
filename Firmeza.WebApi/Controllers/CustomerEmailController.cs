using AdminDashboardApplication.DTOs.CustomersEmail;
using AdminDashboardApplication.UseCases.Customers;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.WebApi.Controllers
{
    [ApiController]
    [Route("apiv1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly RegisterCustomerHandler _registerHandler;

        public CustomersController(RegisterCustomerHandler registerHandler)
        {
            _registerHandler = registerHandler;
        }

        /// <summary>
        /// Registers a new customer.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _registerHandler.Execute(dto);

                return Created("", new
                {
                    message = "Customer registered successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Internal server error.",
                    detail = ex.Message
                });
            }
        }
    }
}