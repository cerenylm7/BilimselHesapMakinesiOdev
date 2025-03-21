
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class StandartPage : ContentPage
    {
        public StandartPage()
        {
            InitializeComponent();
            BindingContext = new StandardCalculatorViewModel();
        }
    }
}