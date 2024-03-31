using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GarmentRecordSystem.Models;
using Newtonsoft.Json;

namespace GarmentRecordSystem.Repository;

public class GarmentRepository : IGarmentRepository
{
    private string _filePath;
    private List<GarmentModel>? _garments;
    public int NextId { get; set; }
    
    public GarmentRepository(string filePath)
    {
        _garments = new List<GarmentModel>();
        _filePath = filePath;
        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            NextId = 1;
            return;
        }
        InitializeRepository();
    }
    
    public IEnumerable<GarmentModel> GetAllGarments()
    {
        try
        {
            return _garments;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to read garments from file.", ex);
        }
    }
    
    public GarmentModel GetGarmentById(int garmentId)
    {
        try
        {
            var selectedGarment = _garments.FirstOrDefault(garment => garment.GarmentId == garmentId);
            if (selectedGarment != null)
            {
                return selectedGarment;
            }

            throw new KeyNotFoundException();
        }
        catch (KeyNotFoundException e)
        {
            throw new KeyNotFoundException("Garment not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occured, cannot get the garment.");
        }
    }
    
    public GarmentModel AddGarment(GarmentModel garment)
    {
        try
        {
            garment.GarmentId = NextId++;
            _garments.Add(garment);
            return garment;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to write garment to file.");
        }
    }
    
    public GarmentModel UpdateGarment(GarmentModel updatedGarment)
    {
        try
        {
            var existingGarment = _garments.FirstOrDefault(g => g.GarmentId == updatedGarment.GarmentId);

            if (existingGarment != null)
            {
                existingGarment.BrandName = updatedGarment.BrandName;
                existingGarment.PurchaseDate = updatedGarment.PurchaseDate;
                existingGarment.Color = updatedGarment.Color;
                existingGarment.Size = updatedGarment.Size;

                return existingGarment;
            }
            {
                throw new KeyNotFoundException($"Garment with ID {updatedGarment.GarmentId} not found.");
            }
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the garment.", ex);
        }
    }

    public bool LoadGarmentsAndSetNewFilePath(string filePath)
    {
        var result = InitializeRepository(filePath);
        if (result)
        {
            _filePath = filePath;
            return true;
        }

        return false;
    }
    
    public void SaveGarments(string? path = null)
    {
        string json = JsonConvert.SerializeObject(new { garments = _garments }, Formatting.Indented);

        try
        {
            if (path != null)
            {
                File.WriteAllText(path, json);
                return;
            }
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save garments to file.", ex);
        }
    }
    
    public void DeleteGarmentById(int garmentId)
    {
        try
        {
            var garmentToRemove = _garments.FirstOrDefault(garment => garment.GarmentId == garmentId);

            if (garmentToRemove != null)
            {
                _garments.Remove(garmentToRemove);
                Console.WriteLine($"Garment with ID {garmentId} deleted successfully.");
            }
            else
            {
                throw new KeyNotFoundException($"Garment with ID {garmentId} not found.");
            }
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting the garment.", ex);
        }
    }

    private bool InitializeRepository(string? insertedFilePath = null)
    {
        var filePath = insertedFilePath ?? _filePath;
        try
        {
            string json = File.ReadAllText(filePath);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            List<GarmentModel> garments =
                JsonConvert.DeserializeObject<List<GarmentModel>>(jsonObject.garments.ToString());
            NextId = garments.Count > 0 ? garments.Max(g => g.GarmentId) + 1 : 1;
            _garments = garments;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load the file.");
            return false;
        }
    }
}