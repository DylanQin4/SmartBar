using SmartBar.Tablet.Models;
using SmartBar.Tablet.PageModels;

namespace SmartBar.Tablet.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}