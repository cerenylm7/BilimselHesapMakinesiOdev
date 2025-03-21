using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace MauiApp1
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private string _inputExpression = "";
        private bool _isNewEntry = true;
        private CultureInfo _culture = CultureInfo.CurrentCulture;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public ICommand NumberCommand => new Command<string>(OnNumberPressed);
        public ICommand OperationCommand => new Command<string>(OnOperatorPressed);
        public ICommand EqualsCommand => new Command(OnEqualsPressed);
        public ICommand ScientificOperationCommand => new Command<string>(OnScientificOperation);
        public ICommand ClearCommand => new Command(OnClear);
        public ICommand ClearEntryCommand => new Command(OnClearEntry);
        public ICommand BackspaceCommand => new Command(OnBackspace);
        public ICommand NegateCommand => new Command(OnNegate);
        public ICommand DecimalCommand => new Command(OnDecimal);
        public ICommand SinCommand => new Command(OnSin);
        public ICommand CosCommand => new Command(OnCos);
        public ICommand TanCommand => new Command(OnTan);
        public ICommand CotCommand => new Command(OnCot);

        private void OnNumberPressed(string number)
        {
            if (_isNewEntry || DisplayText == "0")
            {
                DisplayText = number;
                _isNewEntry = false;
            }
            else
            {
                DisplayText += number;
            }
        }

        private void OnOperatorPressed(string op)
        {
            if (string.IsNullOrEmpty(_inputExpression) && DisplayText == "0") return;
            if (_inputExpression.EndsWith("+ ") || _inputExpression.EndsWith("- ") || _inputExpression.EndsWith("* ") || _inputExpression.EndsWith("/ "))
            {
                _inputExpression = _inputExpression.Substring(0, _inputExpression.Length - 2) + op + " ";
            }
            else
            {
                _inputExpression += $"{DisplayText} {op} ";
            }

            DisplayText = "0";
            _isNewEntry = true;
        }

        private void OnEqualsPressed()
        {
            try
            {
                _inputExpression += DisplayText;
                var result = EvaluateExpression(_inputExpression);
                DisplayText = result.ToString(CultureInfo.CurrentCulture);
                _inputExpression = "";
                _isNewEntry = true;
            }
            catch (DivideByZeroException)
            {
                DisplayText = "Sıfıra Bölünme Hatası";
                _inputExpression = "";
                _isNewEntry = true;
            }
            catch (Exception)
            {
                DisplayText = "Hata";
                _inputExpression = "";
                _isNewEntry = true;
            }
        }

        private void OnScientificOperation(string operation)
        {
            try
            {
                double result = 0;

                if (operation == "pi")
                {
                    result = Math.PI;
                    DisplayText = result.ToString("N10", _culture);
                    _isNewEntry = true;
                    return;
                }
                if (operation == "e")
                {
                    result = Math.E;
                    DisplayText = result.ToString("N10", _culture);
                    _isNewEntry = true;
                    return;
                }

                if (!double.TryParse(DisplayText, NumberStyles.Any, _culture, out double value))
                {
                    DisplayText = "Hata";
                    return;
                }

                switch (operation)
                {
                    case "log":
                        if (value <= 0) { DisplayText = "Hata"; return; }
                        DisplayText = Math.Log10(value).ToString(_culture);
                        break;
                    case "ln":
                        if (value <= 0) { DisplayText = "Hata"; return; }
                        DisplayText = Math.Log(value).ToString(_culture);
                        break;
                    case "pow":
                        DisplayText = Math.Pow(value, 2).ToString(_culture);
                        break;
                    case "sqrt":
                        if (value < 0) { DisplayText = "Hata"; return; }
                        DisplayText = Math.Sqrt(value).ToString(_culture);
                        break;
                    case "mod":
                        DisplayText = (value / 100).ToString(_culture);
                        break;
                    case "reciprocal":
                        if (value == 0) { DisplayText = "Hata"; return; }
                        DisplayText = (1 / value).ToString(_culture);
                        break;
                    case "abs":
                        DisplayText = Math.Abs(value).ToString(_culture);
                        break;
                    case "exp":
                        DisplayText = Math.Exp(value).ToString(_culture);
                        break;
                    case "power":
                        break;
                    case "tenPower":
                        DisplayText = Math.Pow(10, value).ToString(_culture);
                        break;
                    case "factorial":
                        if (value < 0 || value != (int)value) { DisplayText = "Hata"; return; }
                        DisplayText = Factorial(value).ToString(_culture);
                        break;
                    case "2nd":
                        break;
                }

                _isNewEntry = true;
            }
            catch (Exception)
            {
                DisplayText = "Hata";
            }
        }

        private double Factorial(double n)
        {
            if (n == 0) return 1;
            double result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        private void OnClearEntry()
        {
            _inputExpression = string.IsNullOrEmpty(_inputExpression) ? "" : GetUpdatedExpression(_inputExpression);
            DisplayText = "0";
            _isNewEntry = true;
        }

        private string GetUpdatedExpression(string inputExpression)
        {
            var parts = inputExpression.Split(' ');
            return parts.Length > 2 ? string.Join(" ", parts, 0, parts.Length - 2) : "";
        }

        private void OnClear()
        {
            DisplayText = "0";
            _inputExpression = "";
            _isNewEntry = true;
        }

        private void OnBackspace()
        {
            if (!string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
                if (string.IsNullOrEmpty(DisplayText))
                {
                    DisplayText = "0";
                    _isNewEntry = true;
                }
            }
        }

        private void OnNegate()
        {
            if (DisplayText != "0")
            {
                if (DisplayText.StartsWith("-"))
                {
                    DisplayText = DisplayText.Substring(1);
                }
                else
                {
                    DisplayText = "-" + DisplayText;
                }
            }
        }

        private void OnDecimal()
        {
            if (!DisplayText.Contains("."))
            {
                DisplayText += ".";
            }
        }

        private double EvaluateExpression(string expression)
        {
            try
            {
                if (expression.Contains("/ 0"))
                {
                    throw new DivideByZeroException();
                }

                var result = new System.Data.DataTable().Compute(expression, null);
                return Convert.ToDouble(result, _culture);
            }
            catch (DivideByZeroException)
            {
                throw; // This will be caught by your `OnEqualsPressed` method to display the error message.
            }
            catch (Exception)
            {
                throw; // Handle other exceptions as needed.
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSin()
        {
            DisplayText = "Sinüs Fonksiyonu";
            _isNewEntry = true;
        }

        private void OnCos()
        {
            DisplayText = "Kosinüs Fonksiyonu";
            _isNewEntry = true;
        }

        private void OnTan()
        {
            DisplayText = "Tanjant Fonksiyonu";
            _isNewEntry = true;
        }

        private void OnCot()
        {
            DisplayText = "Kotanjant Fonksiyonu";
            _isNewEntry = true;
        }
    }
}