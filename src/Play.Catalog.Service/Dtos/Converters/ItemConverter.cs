using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Models;

namespace Play.Catalog.Service
{
    public static class ItemConverter
    {
        public static ItemDto ToDto(Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }

        public static Item ToModel(ItemDto item)
        {
            var itemModel = new Item();
            itemModel.Id = item.Id;
            itemModel.Name = item.Name;
            itemModel.Description = item.Description;
            itemModel.Price = item.Price;
            itemModel.CreatedDate = item.CreatedDate;

            return itemModel;
        }
    }


}