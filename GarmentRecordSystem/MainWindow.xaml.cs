﻿using System;
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