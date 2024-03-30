using System;
using GarmentRecordSystem.Models.Enums;

namespace GarmentRecordSystem.Models;

public class GarmentModel
{
    public int GarmentId { get; set; }
    public string BrandName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Color { get; set; }
    public SizeEnum Size { get; set; }

    public override string ToString()
    {
        return $"Id: {GarmentId}, Brand: {BrandName}, Purchased: {PurchaseDate}, Color: {Color}, Size: {Size}";
    }
}