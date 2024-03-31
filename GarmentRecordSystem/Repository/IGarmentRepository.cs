using System.Collections.Generic;
using GarmentRecordSystem.Models;

namespace GarmentRecordSystem.Repository;

public interface IGarmentRepository
{
    IEnumerable<GarmentModel> GetAllGarments();
    bool LoadGarmentsAndSetNewFilePath(string filePath);
    GarmentModel GetGarmentById(int garmentId);
    GarmentModel AddGarment(GarmentModel garment);
    GarmentModel UpdateGarment(GarmentModel updatedGarment);
    void SaveGarments(string? path = null);
    void DeleteGarmentById(int garmentId);
}