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
        private readonly IOrderActionService _orderActionService;
        public OrderController(
            IOrderRepository orderRepo, 
            ICustomerRepository customRepo,
            IOrderActionService orderActionService,
            UserManager<AppUser> userManager)
        {
            _orderRepo = orderRepo;
            _customerRepo = customRepo;
            _orderActionService = orderActionService;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User) ??
            throw new UnauthorizedAccessException("User is not authenticated");

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

            if (!orders.Any())
                return NotFound("No orders found for this customer number");

            var ordersDto = orders.Select(o => o.ToOrderDto());
            return Ok(ordersDto);
        }

        //[HttpGet("user-order-list")]
        //[Authorize]
        //public async Task<IActionResult> GetUserOrderList()
        //{
        //    var userId = _userManager.GetUserId(User);

        //    var user = await _userManager.Users.Include(u => u.Orders).
        //        ThenInclude(o => o.Customer).
        //        ThenInclude(c => c.Address).
        //        FirstOrDefaultAsync(u => u.Id == userId);

        //    if (user == null)
        //        return Unauthorized();

        //    var orderList = user.Orders.OrderByDescending(o => o.CreatedOn).ToList();

        //    var orderListDto = orderList.Select(o => o.ToOrderDto()).ToList();

        //    return Ok(orderListDto);
        //}

        [HttpGet("user-order-list")]
        [Authorize]
        public async Task<IActionResult> GetUserOrderList(
            [FromQuery] string? sortBy, [FromQuery] bool isDescending = false)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users.Include(u => u.Orders).
                ThenInclude(o => o.Customer).
                ThenInclude(c => c.Address).
                FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var ordersQuery = user.Orders.AsQueryable();
            ordersQuery.ApplySorting(sortBy, isDescending);
            var orderListDto = ordersQuery.Select(o => o.ToOrderDto()).ToList();
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

            var orderModel = orderDto.ToOrderFromCreateDto(customerId.Value);
            await _orderRepo.CreateAsync(orderModel);
            var createdOrder = await _orderRepo.GetByIdAsync(orderModel.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder?.Id }, createdOrder?.ToOrderDto());
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

            var success = await _orderRepo.DeleteMultipleAsync(ids);

            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Failed to delete all specified orders. No changes were applied");

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
        [Authorize(Roles = "User")]
        public async Task<IActionResult> TakeOrder([FromRoute] int orderId)
        {
            try
            {
                var dto = await _orderActionService.TakeOrderAsync(orderId, CurrentUserId);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("release/{orderId:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReleaseOrder([FromRoute] int orderId)
        {
            try
            {
                var dto = await _orderActionService.ReleaseOrderAsync(orderId, CurrentUserId);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("complete/{orderId:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CompleteOrder([FromRoute] int orderId)
        {
            try
            {
                var dto = await _orderActionService.CompleteOrderAsync(orderId, CurrentUserId);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cancel/{orderId:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelOrder([FromRoute] int orderId)
        {
            try
            {
                var dto = await _orderActionService.CancelOrderAsync(orderId, CurrentUserId);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
