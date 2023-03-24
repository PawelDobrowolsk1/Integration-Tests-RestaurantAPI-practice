using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    [Authorize]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public IActionResult Post([FromRoute]int restaurantId, [FromBody]CreateDishDto dto)
        {
           var newDishId = _dishService.Create(restaurantId,dto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);

        }

        [HttpGet("{dishId}")]
        public IActionResult Get([FromRoute] int restaurantId, [FromRoute] int dishId) 
        {
            DishDto dish = _dishService.GetById(restaurantId, dishId);

            return Ok(dish);
        }

        [HttpGet]
        public IActionResult Get([FromRoute] int restaurantId)
        {
            List<DishDto> result = _dishService.GetAll(restaurantId);

            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromRoute] int restaurantId)
        {
            _dishService.RemoveAll(restaurantId);

            return NoContent();
        }

    }
}
