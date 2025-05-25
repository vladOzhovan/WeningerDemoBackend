using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeningerDemoProject.Dtos.Customer;
using WeningerDemoProject.Helpers;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Mappers;
using WeningerDemoProject.Validators;

namespace WeningerDemoProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;

        public CustomerController(ICustomerRepository customRepo)
        {
            _customerRepo = customRepo;
        }
        
        /// <summary>
        /// Get all Customers
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var customers = await _customerRepo.GetAllAsync(query);
            var customerDtos = customers.Select(c => c.ToCustomerDto()).ToList();
            return Ok(customerDtos);
        }

        /// <summary>
        /// Get a Customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var customerModel = await _customerRepo.GetByIdAsync(id);

            if (customerModel == null)
                return NotFound("Customer not found");  

            return Ok(customerModel.ToCustomerDto());
        }

        /// <summary>
        /// Get a Customer by number
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        [HttpGet("by-number/{customerNumber}")]
        [Authorize]
        public async Task<IActionResult> GetByNumber(int customerNumber)
        {
            var customer = await _customerRepo.GetByNumberAsync(customerNumber);

            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer.ToCustomerDto());
        }

        /// <summary>
        /// Create a Customer
        /// </summary>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCustomerDto customerDto)
        {

            if(await _customerRepo.CustomerExists(customerDto.CustomerNumber))
                return Conflict($"Customer with number {customerDto.CustomerNumber} already exists");

            var customerModel = customerDto.ToCustomerFromCreateDto();
            await _customerRepo.CreateAsync(customerModel);
            return CreatedAtAction(nameof(GetById), new { id = customerModel.Id }, customerModel.ToCustomerDto());
        }

        /// <summary>
        /// Generate customers
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost("generate-customers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateCustomers(int count)
        {
            if (count <= 0)
                return BadRequest("Count must be greater than 0");

            var customers = await _customerRepo.GenerateCustomerListAsync(count);

            return Ok(customers.Select(c => c.ToCustomerDto()).ToList());
        }

        /// <summary>
        /// Delete a Customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var customerModel = await _customerRepo.DeleteAsync(id);

            if (customerModel == null)
                return NotFound("Customer not found");

            return NoContent();
        }

        /// <summary>
        /// Delete multiple
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("delete-multiple")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No customer IDs provided");

            foreach (var id in ids)
                await _customerRepo.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Update a Customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerDto customerDto)
        {
            var customerModel = await _customerRepo.UpdateAsync(id, customerDto);
            
            if (customerModel == null)
                return NotFound("Customer not found");

            return Ok(customerModel.ToCustomerDto());
        }
    }
}
