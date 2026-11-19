using CommunityToolkit.Mvvm.ComponentModel;

namespace Tyt.Wpf.ViewModels;

public partial class PlaceholderViewModel : ObservableObject
{
    public string Title { get; }

    public string Description { get; }

    public PlaceholderViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
