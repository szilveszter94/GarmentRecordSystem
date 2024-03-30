using System.Collections.Generic;
using GarmentRecordSystem.Models;

namespace GarmentRecordSystem.Service;

public interface IGarmentService
{
    GarmentModel SearchGarment(int garmentId);
    List<GarmentModel> SortGarments(string sortBy);
    void SaveGarment(string path);
    GarmentModel GetGarmentById(int garmentId);
    GarmentModel AddGarment(GarmentModel garment);
    List<GarmentModel> GetAll();
    GarmentModel EditGarment(GarmentModel garment);
    void DeleteGarment(int garmentId);
}