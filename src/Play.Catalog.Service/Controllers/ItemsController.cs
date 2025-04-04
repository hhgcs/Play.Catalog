using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        // private static readonly List<ItemDto> items = new()
        // {
        //     new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Antidote", "Cures poisoning", 7, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
        // };
        private readonly ItemRepository _itemRepository;

        public ItemsController(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var internalItems = await _itemRepository.GetItemsAsync();
            return internalItems.Select(x => ItemConverter.ToDto(x)).ToArray();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var internalItem = await _itemRepository.GetItemByIdAsync(id);
            if (internalItem == null)
            {
                return NotFound();
            }
            return ItemConverter.ToDto(internalItem);
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostItemAsync(CreateItemDto item)
        {
            var newPublicItem = new ItemDto
            (
                Id: Guid.NewGuid(),
                Name: item.Name,
                Description: item.Description,
                Price: item.Price,
                CreatedDate: DateTimeOffset.UtcNow
            );

            var newInternalItem = ItemConverter.ToModel(newPublicItem);
            await _itemRepository.CreateItemAsync(newInternalItem);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = newPublicItem.Id }, newPublicItem);
        }

        [HttpPut]
        // public ActionResult<ActionResult<ItemDto>[]> PutItems(UpdateItemDto[] updateItems)
        // {
        //     var results = new List<ActionResult<ItemDto>>();
        //     var itemsDict = new Dictionary<Guid, int>();
        //     for (var i = 0; i < items.Count; i++)
        //     {
        //         itemsDict.Add(items[i].Id, i);
        //     }

        //     foreach (var updateItem in updateItems)
        //     {
        //         var id = updateItem.Id;
        //         var itemReplacement = new ItemDto(id, updateItem.Name, updateItem.Description, updateItem.Price, DateTimeOffset.UtcNow);
        //         if (itemsDict.TryGetValue(id, out int index))
        //         {
        //             items[index] = itemReplacement;
        //             results.Add(Ok(itemReplacement));
        //         }
        //         else
        //         {
        //             results.Add(NotFound($"Item with id {id} not found."));
        //         }
        //     }
        //     return results.ToArray();
        // }
        public async Task<ActionResult<IEnumerable<UpdateItemResult>>> PutItemsAsync(UpdateItemDto[] updateItems)
        {
            var results = new List<UpdateItemResult>();

            foreach (var updatedItem in updateItems)
            {
                var existingInternalItem = await _itemRepository.GetItemByIdAsync(updatedItem.Id);
                if (existingInternalItem != null)
                {
                    var updatedItemDto = new ItemDto(updatedItem.Id, updatedItem.Name, updatedItem.Description, updatedItem.Price, existingInternalItem.CreatedDate);
                    var internalItem = ItemConverter.ToModel(updatedItemDto);
                    try
                    {
                        await _itemRepository.UpdateItemAsync(internalItem.Id, internalItem);
                        results.Add(new UpdateItemResult()
                        {
                            Success = true,
                            Item = updatedItemDto,
                            Error = null
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
                else
                {
                    results.Add(
                        new UpdateItemResult()
                        {
                            Success = false,
                            Item = null,
                            Error = $"item with id:{updatedItem.Id} not found."
                        }
                    );
                }
            }

            // Return the results, which include both successful and failed updates
            return Ok(results);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var internalItem = _itemRepository.GetItemByIdAsync(id);
            if (internalItem == null)
            {
                return NotFound();
            }
            await _itemRepository.DeleteItemAsync(id);
            return NoContent();
        }



    }
}