using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
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
        
        private void AddNewGarment(object sender, RoutedEventArgs e)
        {
            var addWindow = new GarmentEditorWindow();
            addWindow.ShowDialog();
        }
    }
}