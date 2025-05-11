using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Order
{
    public class UpdateOrderDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Title must be 2 characters")]
        [MaxLength(25, ErrorMessage = "Title cannot be over 25 characters")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10, ErrorMessage = "Description must be 10 characters")]
        [MaxLength(300, ErrorMessage = "Description cannot be over 300 characters")]
        public string? Description { get; set; } = string.Empty;
    }
}
