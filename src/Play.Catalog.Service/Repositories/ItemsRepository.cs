using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Play.Catalog.Service.Models;
using MongoDB.Driver;

namespace Play.Catalog.Service.Repositories
{
    public class ItemRepository
    {
        private readonly IMongoCollection<Item> _items;
        public ItemRepository(MongoDBContext context)
        {
            _items = context.GetCollection<Item>("Items");
        }

        public async Task<List<Item>> GetItemsAsync()
        {
            return await _items.Find(_ => true).ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(Guid Id)
        {
            return await _items.Find(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(paramName: item.Name, message: "Item parameter cannot be null");
            }
            await _items.InsertOneAsync(item);
        }

        public async Task<ReplaceOneResult> UpdateItemAsync(Guid id, Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(paramName: item.Name, message: "Item parameter cannot be null");
            }
            return await _items.ReplaceOneAsync(x => x.Id == id, item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            await _items.DeleteOneAsync(x => x.Id == id);
        }
    }
}