using Treadmill.Infrastructure;
using Treadmill.Maui.Views;

namespace Treadmill.Maui;

public partial class MainPage : ContentPage
{
  public MainView MainView { get; set; }

  public MainPage(ICompositionRoot root)
  {
    MainView = root.Get<MainView>();
    InitializeComponent();
  }
}

