using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private static int requestCounter = 0;

        public ItemsController(IRepository<Item> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }
        // GET /items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllItemsAsync()
        {
            requestCounter++;

            Console.WriteLine("Request {0}: Starting... ", requestCounter);

            if (requestCounter <= 2)
            {
                Console.WriteLine("Request {0}: Delaying... ", requestCounter);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounter <= 4)
            {
                Console.WriteLine("Request {0}: 500 (Internal Server Error) ", requestCounter);
                return StatusCode(500);
            }
            
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            Console.WriteLine("Request {0}: 200 (OK). ", requestCounter);
            
            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemByIDAsync(Guid id)
        {
            var item = (await itemsRepository.GetAsync(id)).AsDto();

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        // POST /items
        [HttpPost]
        public async Task<IActionResult> CreateItemAsync(CreateItemDto createItemDto)
        {
            //Create an item entities out the CreateItemDto
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetItemByIDAsync), new { id = item.Id }, item);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.DeleteAsync(item.Id);

            return NoContent();
        }
    }
}