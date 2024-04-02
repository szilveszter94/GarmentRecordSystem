using GarmentRecordSystem.Models;
using GarmentRecordSystem.Models.Enums;
using GarmentRecordSystem.Repository;
using NUnit.Framework;

namespace GarmentRecordSystemTest;

public class GarmentRepositoryTest
{
    [Test]
    public void Constructor_WhenFileDoesNotExist_InitializesNextIdToOne()
    {
        // Arrange
        string filePath = "nonexistentfile.json";
        // Act
        var repository = new GarmentRepository(filePath);

        // Assert
        Assert.That(repository.NextId, Is.EqualTo(1));
    }
    
    [Test]
    public void GetAllGarments_ReturnsAllGarmentsFromFile()
    {
        // Arrange
        string filePath = "testData.json";
        File.Delete(filePath);
        File.WriteAllText(filePath,
            "{\n\"garments\": [\n {\n \"GarmentId\": 1,\n \"BrandName\": \"Nike\",\n \"PurchaseDate\": \"2024-03-30T23:27:41.0617978+01:00\",\n \"Color\": \"Green\",\n \"Size\": 1\n },\n {\n \"GarmentId\": 2,\n \"BrandName\": \"Adidas\",\n \"PurchaseDate\": \"2024-03-30T23:28:10.9193363+01:00\",\n \"Color\": \"Red\",\n \"Size\": 2\n }]\n}");

        // Act
        var repository = new GarmentRepository(filePath);
        var garments = repository.GetAllGarments();

        // Assert
        Assert.That(garments.Count(), Is.EqualTo(2));
        Assert.That(garments.First().GarmentId, Is.EqualTo(1));
        Assert.That(garments.First().BrandName, Is.EqualTo("Nike"));
        Assert.That(garments.Last().GarmentId, Is.EqualTo(2));
        Assert.That(garments.Last().BrandName, Is.EqualTo("Adidas"));
        File.Delete(filePath);
    }
    
    [Test]
    public void GetAllGarments_ReturnsEmptyIfReadingFails()
    {
        // Arrange
        string filePath = "invalidfile.json";
        File.Delete(filePath);
        File.WriteAllText(filePath, "[{\"invalidProperty\": \"invalidValue\"}]");

        // Act
        var repository = new GarmentRepository(filePath);
        var garments = repository.GetAllGarments();
        Assert.That(garments.Count(), Is.EqualTo(0));
        File.Delete(filePath);
    }
    
    [Test]
    public void GetGarmentById_ReturnsCorrectGarment()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var defaultId = 1;
        var expectedGarment = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        repository.AddGarment(expectedGarment);

        // Act
        var result = repository.GetGarmentById(defaultId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedGarment));
    }
    
    [Test]
    public void GetGarmentById_ReturnsNotFoundException()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var defaultId = 13;
        var expectedGarment = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        repository.AddGarment(expectedGarment);

        Assert.Throws<KeyNotFoundException>(() => repository.GetGarmentById(defaultId));
    }
    
    [Test]
    public void UpdateGarmentReturnsCorrectResult()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var expectedGarment = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        repository.AddGarment(expectedGarment);
        var modifiedGarment = new GarmentModel(){
            GarmentId = 1, BrandName = "Nike", PurchaseDate = new DateTime(2023, 02, 03), Size = SizeEnum.L,
            Color = "Yellow"
        };
        var result = repository.UpdateGarment(modifiedGarment);
        Assert.That(result, Is.EqualTo(modifiedGarment));
    }
    
    [Test]
    public void UpdateGarmentThrowsError_InvalidGarment()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var expectedGarment = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        repository.AddGarment(expectedGarment);
        var modifiedGarment = new GarmentModel(){
            GarmentId = 2, BrandName = "Nike", PurchaseDate = new DateTime(2023, 02, 03), Size = SizeEnum.L,
            Color = "Yellow"
        };
        Assert.Throws<KeyNotFoundException>(() => repository.UpdateGarment(modifiedGarment));
    }
    
    [Test]
    public void DeleteGarmentReturnsCorrectResult()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var deleteId = 1;
        var garment1 = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        var garment2 = new GarmentModel
        {
            BrandName = "Nike", PurchaseDate = new DateTime(2023, 01, 01), Size = SizeEnum.XXXL,
            Color = "Yellow"
        };
        repository.AddGarment(garment1);
        repository.AddGarment(garment2);
        repository.DeleteGarmentById(deleteId);
        var expectedResult = repository.GetAllGarments().ToList()[0];
        Assert.That(expectedResult, Is.EqualTo(garment2));
    }
    
    [Test]
    public void DeleteGarmentThrowsError_InvalidId()
    {
        // Arrange
        var repository = new GarmentRepository("validfile.json");
        var deleteId = 10;
        var garment1 = new GarmentModel
        {
            BrandName = "Puma", PurchaseDate = new DateTime(2022, 02, 02), Size = SizeEnum.S,
            Color = "Red"
        };
        repository.AddGarment(garment1);
        Assert.Throws<KeyNotFoundException>(() => repository.DeleteGarmentById(deleteId));
    }
    
    [Test]
    public void LoadGarmentsAndSetNewFilePath_ValidFilePath_Success()
    {
        // Arrange
        var filePath = "validfile.json";
        File.Delete(filePath);
        var repository = new GarmentRepository(filePath);
        var expectedGarments = new List<GarmentModel>()
        {
            new ()
            {
                GarmentId = 1, BrandName = "Nike", PurchaseDate = new DateTime(2024, 03, 30, 23, 28, 10, 919),
                Color = "Green", Size = SizeEnum.S
            },
            new (){GarmentId = 2, BrandName = "Adidas", PurchaseDate = new DateTime(2024, 03, 30, 23, 28, 10, 919), Color = "Red", Size = SizeEnum.M}
        };
        File.WriteAllText(filePath,
            "{\n\"garments\": [\n {\n \"GarmentId\": 1,\n \"BrandName\": \"Nike\",\n \"PurchaseDate\": \"2024-03-30T23:28:10.9193363+01:00\",\n \"Color\": \"Green\",\n \"Size\": 1\n },\n {\n \"GarmentId\": 2,\n \"BrandName\": \"Adidas\",\n \"PurchaseDate\": \"2024-03-30T23:28:10.9193363+01:00\",\n \"Color\": \"Red\",\n \"Size\": 2\n }]\n}");

        // Act
        bool result = repository.LoadGarmentsAndSetNewFilePath(filePath);
        // Assert
        var garmentsRetrieved = repository.GetAllGarments().ToList();
        Assert.IsTrue(result);
        Assert.That(garmentsRetrieved, Is.EquivalentTo(expectedGarments));
        File.Delete(filePath);
    }
    
    [Test]
    public void LoadGarmentsReturnsFalse()
    {
        // Arrange
        var filePath = "validfile.json";
        File.Delete(filePath);
        var repository = new GarmentRepository(filePath);
        File.WriteAllText(filePath,
            "invalidFile{\n\"garments\": [\n {\n \"0.9193363+01:00\",\n \"Color\": \"Green\",\n \"Siz\"Size\": 2\n }]\n}");

        // Act
        bool result = repository.LoadGarmentsAndSetNewFilePath(filePath);
        // Assert
        
        Assert.IsFalse(result);
        File.Delete(filePath);
    }
    
}