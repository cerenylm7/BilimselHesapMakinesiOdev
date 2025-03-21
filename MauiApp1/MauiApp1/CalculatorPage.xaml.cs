using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class CalculatorPage : ContentPage
    {
        public CalculatorPage()
        {
            InitializeComponent();
            BindingContext = new CalculatorViewModel();

        }
    }
}


