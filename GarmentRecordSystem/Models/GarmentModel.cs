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
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        GarmentModel other = (GarmentModel)obj;

        return GarmentId == other.GarmentId &&
               BrandName == other.BrandName &&
               PurchaseDate.Date == other.PurchaseDate.Date &&
               Color == other.Color &&
               Size == other.Size;
    }
    
}