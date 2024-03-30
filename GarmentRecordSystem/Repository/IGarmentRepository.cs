using System.Collections.Generic;
using GarmentRecordSystem.Models;

namespace GarmentRecordSystem.Repository;

public interface IGarmentRepository
{
    IEnumerable<GarmentModel> GetAllGarments();
    GarmentModel GetGarmentById(int garmentId);
    GarmentModel AddGarment(GarmentModel garment);
    GarmentModel UpdateGarment(GarmentModel updatedGarment);
    void SaveGarments(List<GarmentModel> garments, string? path = null);
    void DeleteGarmentById(int garmentId);
}