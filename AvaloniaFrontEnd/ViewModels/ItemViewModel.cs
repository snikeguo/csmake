using CSConfig;
using ReactiveUI;
using System;

namespace AvaloniaFrontEnd.ViewModels;

public enum ItemType
{
    Config,
    Choice,
    Menu,
}
public class ItemViewModel : ViewModelBase
{
    private ItemType _SelectedShape;

    /// <summary>
    /// Gets or sets the selected ShapeType
    /// </summary>
    public ItemType SelectedShape
    {
        get { return _SelectedShape; }
        set { this.RaiseAndSetIfChanged(ref _SelectedShape, value); }
    }

    /// <summary>
    ///  Gets an array of all available ShapeTypes
    /// </summary>
    public ItemType[] AvailableShapes { get; } = Enum.GetValues<ItemType>();
}

public class ConfigComboBoxItemModel
{
    public IItem Source { get; set; }
    public string Name=>Source.DisplayName;
}