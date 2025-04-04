using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

public class UpdateItemResult
{
    public bool Success { get; set; }
    public ItemDto Item { get; set; }
    public string Error { get; set; }
}