using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<GarmentModel> Garments { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _isSaveEnabled = "False";
        public string IsSaveEnabled
        {
            get => _isSaveEnabled;
            set  
            {  
                if (value != _isSaveEnabled)  
                {  
                    _isSaveEnabled = value;  
                    OnPropertyChanged();
                }  
            }  
        }
        private bool _isAutoSaveEnabled;

        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set
            {
                if (_isAutoSaveEnabled != value)
                {
                    _isAutoSaveEnabled = value;
                    CheckAutoSave();
                    OnPropertyChanged();
                }
            }
        }
        private readonly IGarmentService _garmentService;
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
            var key = (string)((Button)sender).Tag;
            if (key == "Save")
            {
                _garmentService.SaveGarment(_filePath);
                IsSaveEnabled = "False";
                return;
            }
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.FileName = "garments.json";
            
            bool? result = saveFileDialog.ShowDialog();
            
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                _garmentService.SaveGarment(filePath);
                MessageBox.Show($"Garments successfully saved to the location {filePath}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void LoadGarments(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            bool? result = openFileDialog.ShowDialog();
            
            if (result == true)
            {
                string newFilePath = openFileDialog.FileName;
                var isValid = _garmentService.SetNewFilePathAndLoad(newFilePath);
                if (!isValid)
                {
                    MessageBox.Show($"Failed to load garments. Invalid file format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Garments loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshGarmentList(_garmentService.GetAll());
                    _filePath = newFilePath;
                    IsSaveEnabled = "False";
                }
                
            }
        }
        
        private void AddNewGarment(object sender, RoutedEventArgs e)
        {
            var addWindow = new GarmentEditorWindow(_garmentService);
            if (addWindow.ShowDialog() == true)
            {
                RefreshGarmentList(_garmentService.GetAll());
                MessageBox.Show("Garment successfully created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckAutoSave();
            }
        }
        
        private void UpdateGarment(object sender, RoutedEventArgs e)
        {
            var garmentId = (int)((Button)sender).CommandParameter;
            var addWindow = new GarmentEditorWindow(_garmentService, garmentId);
            if (addWindow.ShowDialog() == true)
            {
                RefreshGarmentList(_garmentService.GetAll());
                MessageBox.Show("Garment successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckAutoSave();
            }
        }
        
        private void SearchGarments(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTermTextBox.Text.ToLower();
            Int32.TryParse(searchText, out int number);
            var filteredGarments = new List<GarmentModel>();
            foreach (var garment in _garmentService.GetAll())
            {
                if (garment.BrandName.ToLower().Contains(searchText) || garment.Color.ToLower().Contains(searchText) || garment.GarmentId.ToString().Contains(number.ToString()) )
                {
                    filteredGarments.Add(garment);
                }
            }
            RefreshGarmentList(filteredGarments);
        }

        private void SortGarments(object sender, RoutedEventArgs e)
        {
            var key = (string)((TextBlock)sender).Tag;
            bool isAscending = !sortFlags[key];
            sortFlags[key] = isAscending;
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
                    sortedGarments = _garmentService.GetAll().OrderBy(g => g.PurchaseDate);
                    break;
                default:
                    return;
            }

            if (!isAscending)
            {
                sortedGarments = sortedGarments.Reverse();
            }

            RefreshGarmentList(sortedGarments);
        }

        private void DeleteGarment(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this garment?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var garmentId = (int)((Button)sender).CommandParameter;
                _garmentService.DeleteGarment(garmentId);
                var garmentToRemove = Garments.FirstOrDefault(g => g.GarmentId == garmentId);
                if (garmentToRemove != null)
                {
                    Garments.Remove(garmentToRemove);
                    CheckAutoSave();
                }
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void CheckAutoSave()
        {
            if (IsAutoSaveEnabled)
            {
                IsSaveEnabled = "False";
                _garmentService.SaveGarment(_filePath);
            }
            else
            {
                IsSaveEnabled = "True";
            }
        }

        void RefreshGarmentList(IEnumerable<GarmentModel> updatedList)
        {
            Garments.Clear();
            foreach (var garment in updatedList)
            {
                Garments.Add(garment);
            }
        }
    }
}