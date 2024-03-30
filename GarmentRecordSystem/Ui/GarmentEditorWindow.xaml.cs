using System;
using System.Windows;
using GarmentRecordSystem.Models;
using GarmentRecordSystem.Models.Enums;
using GarmentRecordSystem.Service;

namespace GarmentRecordSystem.Ui;

public partial class GarmentEditorWindow : Window
{
    private readonly IGarmentService _garmentService;
    private readonly int? _garmentId;
    public GarmentEditorWindow(IGarmentService garmentService, int? garmentId = null)
    {
        _garmentService = garmentService;
        _garmentId = garmentId;
        InitializeComponent();
        if (_garmentId.HasValue)
        {
            var garment = _garmentService.GetGarmentById(_garmentId.Value);
            NameTextBox.Text = garment.BrandName;
            ColorTextBox.Text = garment.Color;
            SizeComboBox.SelectedValue = garment.Size;
            EditorWindow.Title = "Garment editor";
            EditorTitle.Text = "Edit garment";
        }
    }
    
    private void SaveGarment(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text;
        string color = ColorTextBox.Text;
        SizeEnum size = Enum.Parse<SizeEnum>(SizeComboBox.SelectedValue.ToString() ?? "");
        var garment = new GarmentModel()
            { BrandName = name, Color = color, PurchaseDate = DateTime.Now, Size = size };
        if (_garmentId != null)
        {
            garment.GarmentId = _garmentId ?? 0;
            _garmentService.EditGarment(garment);
        }
        else
        {
            _garmentService.AddGarment(garment);
        }
        
        DialogResult = true;
        Close();
    }
    
    private void CancelFromEditorWindow(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}