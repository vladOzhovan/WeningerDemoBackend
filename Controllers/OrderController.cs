using Microsoft.AspNetCore.Mvc;
using WeningerDemoProject.Mappers;
using WeningerDemoProject.Dtos.Order;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Validators;
using WeningerDemoProject.Models;
using Microsoft.AspNetCore.Identity;
using WeningerDemoProject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WeningerDemoProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly UserManager<AppUser> _userManager;
        public OrderController(IOrderRepository orderRepo, ICustomerRepository customRepo, UserManager<AppUser> userManager)
        {
            _orderRepo = orderRepo;
            _customerRepo = customRepo;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all Orders
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var orders = await _orderRepo.GetAllAsync(query);
            var orderDto = orders.Select(o => o.ToOrderDto()).ToList();
            return Ok(orderDto);
        }

        /// <summary>
        /// Get an Order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order.ToOrderDto());
        }

        /// <summary>
        /// Get Orders by Customer Number
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        [HttpGet("by-customer/{customerNumber:int}")]
        [Authorize]
        public async Task<IActionResult> GetByCustomerNumber([FromRoute] int customerNumber)
        {
            var orders = await _orderRepo.GetByCustomerNumberAsync(customerNumber);

            //if (!orders.Any())
            //    return NotFound("No orders found for this customer number");

            var ordersDto = orders.Select(o => o.ToOrderDto());

            return Ok(ordersDto);
        }

        [HttpGet("user-order-list")]
        [Authorize]
        public async Task<IActionResult> GetUserOrderList()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var orderList = user.Orders.OrderByDescending(o => o.CreatedOn).ToList();

            var orderListDto = orderList.Select(o => o.ToOrderDto()).ToList();

            return Ok(orderListDto);
        }

        /// <summary>
        /// Create an Order
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPost("by-number/{customerNumber:int}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromRoute] int customerNumber, [FromBody] CreateOrderDto orderDto)
        {
            var customerId = await _customerRepo.GetCustomerIdByNumberAsync(customerNumber);

            if (customerId == null)
                return NotFound("Customer does not exists");

            var orderModel = orderDto.ToOrderFromCreateDto(customerNumber, customerId.Value);
            
            await _orderRepo.CreateAsync(orderModel);

            return CreatedAtAction(nameof(GetById), new { id = orderModel.Id }, orderModel.ToOrderDto());
        }

        /// <summary>
        /// Delete an Order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var orderModel = await _orderRepo.DeleteAsync(id);

            if (orderModel == null)
                return NotFound("Order not found");

            return Ok(orderModel);
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
            if (ids == null || ids.Count == 0)
                return BadRequest("No order IDs provided");

            foreach (var id in ids)
                await _orderRepo.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Update an Order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOrderDto orderDto)
        {
            var orderInDb = await _orderRepo.UpdateAsync(id, orderDto);

            if (orderInDb == null)
                return NotFound("Order not found");

            return Ok(orderInDb.ToOrderDto());
        }

        /// <summary>
        /// Change Order status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusDto"></param>
        /// <returns></returns>
        [HttpPatch("{id}/update-status")]
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            var orderInDb = await _orderRepo.UpdateOrderStatusAsync(id, statusDto);

            if (orderInDb == null)
                return NotFound("Order not found");

            return Ok(orderInDb.ToOrderDto());
        }

        [HttpPut("take/{orderId:int}")]
        [Authorize]
        public async Task<IActionResult> TakeOrder([FromRoute] int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.TakenByUserId != null)
                return BadRequest("Order is already taken by someone else");

            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            order.Take(userId);

            await _orderRepo.SaveChangesAsync();

            return Ok(order.ToOrderDto());
        }

        [HttpPut("release/{orderId:int}")]
        [Authorize]
        public async Task<IActionResult> ReleaseOrder([FromRoute] int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            if (order.TakenByUserId != userId)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You can only release orders you have taken"
                });
            }

            order.Release();
            await _orderRepo.SaveChangesAsync();

            return Ok(order.ToOrderDto());
        }

        [HttpPut("complete/{orderId:int}")]
        [Authorize]
        public async Task<IActionResult> CompleteOrder([FromRoute] int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            if (order.TakenByUserId != userId)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You can only complete orders you have taken"
                });
            }

            if (order.Status == OrderStatus.Completed)
                return BadRequest("Order is already completed");

            order.Complete();
            await _orderRepo.SaveChangesAsync();

            return Ok(order.ToOrderDto());
        }
    }
}
