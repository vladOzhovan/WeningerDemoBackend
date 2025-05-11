using System.ComponentModel.DataAnnotations;
using WeningerDemoProject.Models;
using WeningerDemoProject.Validators;

namespace WeningerDemoProject.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [EnumValueValidation(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }
    }
}
