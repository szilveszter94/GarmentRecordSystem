using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GarmentRecordSystem.Models;
using GarmentRecordSystem.Repository;
using GarmentRecordSystem.Service;
using GarmentRecordSystem.Ui;

namespace GarmentRecordSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GarmentModel> Garments { get; set; }
        private IGarmentService _garmentService;
        private string _filePath;
        private Dictionary<string, bool> sortFlags = new Dictionary<string, bool>
        {
            { "GarmentId", false },
            { "BrandName", false },
            { "Color", false },
            { "PurchaseDate", false },
            { "Size", false }
        };
        public MainWindow()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(location, "database.json");
            _filePath = filePath;
            var garmentRepository = new GarmentRepository(filePath);
            var garmentService = new GarmentService(garmentRepository);
            _garmentService = garmentService;
            Garments = new ObservableCollection<GarmentModel>(_garmentService.GetAll());
            DataContext = this;
            InitializeComponent();
        }
        
        private void SaveGarments(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.FileName = "garments.json";
            
            bool? result = saveFileDialog.ShowDialog();
            
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                _garmentService.SaveGarment(filePath);
                MessageBox.Show($"Items successfully saved to the location {filePath}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void AddNewGarment(object sender, RoutedEventArgs e)
        {
            var addWindow = new GarmentEditorWindow(_garmentService);
            if (addWindow.ShowDialog() == true)
            {
                Garments.Clear();
                foreach (var garment in _garmentService.GetAll())
                {
                    Garments.Add(garment);
                }
            }
            MessageBox.Show("Item successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void UpdateGarment(object sender, RoutedEventArgs e)
        {
            var garmentId = (int)((Button)sender).CommandParameter;
            var addWindow = new GarmentEditorWindow(_garmentService, garmentId);
            if (addWindow.ShowDialog() == true)
            {
                Garments.Clear();
                foreach (var garment in _garmentService.GetAll())
                {
                    Garments.Add(garment);
                }
                MessageBox.Show("Item successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void SearchGarments(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTermTextBox.Text.ToLower();
            Int32.TryParse(searchText, out int number);
            Garments.Clear();
            var filteredGarments = new List<GarmentModel>();
            foreach (var garment in _garmentService.GetAll())
            {
                if (garment.BrandName.ToLower().Contains(searchText) || garment.Color.ToLower().Contains(searchText) || garment.GarmentId == number)
                {
                    filteredGarments.Add(garment);
                }
            }
            foreach (var garment in filteredGarments)
            {
                Garments.Add(garment);
            }
        }

        private void SortGarments(object sender, RoutedEventArgs e)
        {
            var key = (string)((TextBlock)sender).Tag;
            bool isAscending = !sortFlags[key];
            sortFlags[key] = isAscending;

            Garments.Clear();
            IEnumerable<GarmentModel> sortedGarments;

            switch (key)
            {
                case "GarmentId":
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.GarmentId);
                    break;
                case "BrandName":
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.BrandName, StringComparer.CurrentCultureIgnoreCase);
                    break;
                case "Color":
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.Color, StringComparer.CurrentCultureIgnoreCase);
                    break;
                case "Size":
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.Size);
                    break;
                case "PurchaseDate":
                    Console.WriteLine("ok");
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.PurchaseDate);
                    break;
                default:
                    return;
            }

            if (!isAscending)
            {
                sortedGarments = sortedGarments.Reverse();
            }

            foreach (var garment in sortedGarments)
            {
                Garments.Add(garment);
            }
        }

        private void DeleteGarment(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var garmentId = (int)((Button)sender).CommandParameter;
                _garmentService.DeleteGarment(garmentId);
                var garmentToRemove = Garments.FirstOrDefault(g => g.GarmentId == garmentId);
                if (garmentToRemove != null)
                {
                    Garments.Remove(garmentToRemove);
                }
            }
        }
    }
}