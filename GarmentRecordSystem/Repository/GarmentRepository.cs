using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GarmentRecordSystem.Models;
using Newtonsoft.Json;

namespace GarmentRecordSystem.Repository;

public class GarmentRepository : IGarmentRepository
{
    private readonly string _filePath;
    private int NextId { get; set; }
    
    public GarmentRepository(string filePath)
    {
        _filePath = filePath;
        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            NextId = 1;
            return;
        }
        string json = File.ReadAllText(_filePath);
        var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
        List<GarmentModel> garments = JsonConvert.DeserializeObject<List<GarmentModel>>(jsonObject.garments.ToString());
        NextId = garments.Count > 0 ? garments.Max(g => g.GarmentId) + 1 : 1;
    }
    
    public IEnumerable<GarmentModel> GetAllGarments()
    {
        try
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                return new List<GarmentModel>();
            }
            
            string json = File.ReadAllText(_filePath);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            var garments = JsonConvert.DeserializeObject<List<GarmentModel>>(jsonObject.garments.ToString());
        
            return garments;
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
            var garments = GetAllGarments();
            var selectedGarment = garments.FirstOrDefault(garment => garment.GarmentId == garmentId);
            if (selectedGarment != null)
            {
                return selectedGarment;
            }

            throw new KeyNotFoundException();
        }
        catch (KeyNotFoundException e)
        {
            throw new Exception("Garment not found.");
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
            SaveGarment(garment);
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
            // Load all garments from the file
            var garments = GetAllGarments().ToList();
            
            var existingGarment = garments.FirstOrDefault(g => g.GarmentId == updatedGarment.GarmentId);

            if (existingGarment != null)
            {
                existingGarment.BrandName = updatedGarment.BrandName;
                existingGarment.PurchaseDate = updatedGarment.PurchaseDate;
                existingGarment.Color = updatedGarment.Color;
                existingGarment.Size = updatedGarment.Size;
                
                SaveGarments(garments);

                Console.WriteLine($"Garment with ID {updatedGarment.GarmentId} updated successfully.");
                return existingGarment;
            }
            else
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
    
    public void SaveGarments(List<GarmentModel> garments, string? path = null)
    {
        string json = JsonConvert.SerializeObject(new { garments = garments }, Formatting.Indented);

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
            var garments = GetAllGarments().ToList();
            var garmentToRemove = garments.FirstOrDefault(garment => garment.GarmentId == garmentId);

            if (garmentToRemove != null)
            {
                garments.Remove(garmentToRemove);
                SaveGarments(garments);

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
    
    private void SaveGarment(GarmentModel garment)
    {
        List<GarmentModel> existingGarments = GetAllGarments().ToList();
        existingGarments.Add(garment);
        
        string json = JsonConvert.SerializeObject(new { garments = existingGarments }, Formatting.Indented);

        try
        {
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save garments to file.", ex);
        }
    }
}