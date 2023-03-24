using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class UpdateRestaurantDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool HasDelivery { get; set; }
    }
}
