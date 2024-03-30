using System;
using System.Collections.Generic;
using System.Linq;
using GarmentRecordSystem.Models;
using GarmentRecordSystem.Repository;

namespace GarmentRecordSystem.Service;


public class GarmentService : IGarmentService
{
    private readonly IGarmentRepository _garmentRepository;

    public GarmentService(IGarmentRepository garmentRepository)
    {
        _garmentRepository = garmentRepository;
    }

    public GarmentModel SearchGarment(int garmentId)
    {
        try
        {
            var result = _garmentRepository.GetGarmentById(garmentId);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<GarmentModel> SortGarments(string sortBy)
    {
        List<GarmentModel> garments = _garmentRepository.GetAllGarments().ToList();

        switch (sortBy.ToLower())
        {
            case "brandname":
                garments = garments.OrderBy(g => g.BrandName).ToList();
                break;
            case "purchasedate":
                garments = garments.OrderBy(g => g.PurchaseDate).ToList();
                break;
            case "size":
                garments = garments.OrderBy(g => g.Size).ToList();
                break;
            case "color":
                garments = garments.OrderBy(g => g.Color).ToList();
                break;
            default:
                throw new ArgumentException("Invalid sort criteria. Sort by BrandName, PurchaseDate, Size, or Color.");
        }

        return garments;
    }

    public void SaveGarment(string path)
    {
        try
        {
            var garments = _garmentRepository.GetAllGarments().ToList();
            _garmentRepository.SaveGarments(garments, path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public GarmentModel GetGarmentById(int garmentId)
    {
        try
        {
            return _garmentRepository.GetGarmentById(garmentId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public GarmentModel AddGarment(GarmentModel garment)
    {
        try
        {
            var result = _garmentRepository.AddGarment(garment);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public List<GarmentModel> GetAll()
    {
        try
        {
            return _garmentRepository.GetAllGarments().ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public GarmentModel EditGarment(GarmentModel garment)
    {
        try
        {
            var result = _garmentRepository.UpdateGarment(garment);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DeleteGarment(int garmentId)
    {
        try
        {
            _garmentRepository.DeleteGarmentById(garmentId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
