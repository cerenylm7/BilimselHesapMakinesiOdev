using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace MauiApp1
{
    public class StandardCalculatorViewModel : INotifyPropertyChanged
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
        public ICommand ClearCommand => new Command(OnClear);
        public ICommand ClearEntryCommand => new Command(OnClearEntry);
        public ICommand BackspaceCommand => new Command(OnBackspace);
        public ICommand NegateCommand => new Command(OnNegate);
        public ICommand DecimalCommand => new Command(OnDecimal);
        public ICommand PercentCommand => new Command(OnPercent);
        public ICommand ReciprocalCommand => new Command(OnReciprocal);
        public ICommand SquareCommand => new Command(OnSquare);
        public ICommand SquareRootCommand => new Command(OnSquareRoot);

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

          
            if (op == "/" && DisplayText == "0")
            {
                DisplayText = "Sıfıra Bölünme Hatası";
                _inputExpression = "";
                _isNewEntry = true;
                return;
            }

            if (_inputExpression.EndsWith("+ ") || _inputExpression.EndsWith("- ") ||
                _inputExpression.EndsWith("* ") || _inputExpression.EndsWith("/ "))
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
                if (_inputExpression.EndsWith("/ 0 ") || (_inputExpression.EndsWith("/ ") && DisplayText == "0"))
                {
                    throw new DivideByZeroException();
                }

                _inputExpression += DisplayText;
                var result = EvaluateExpression(_inputExpression);

                // Sonsuz kontrolü
                if (double.IsInfinity(result))
                {
                    throw new DivideByZeroException();
                }

                DisplayText = result.ToString(_culture);
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

        private void OnClearEntry()   //CE
        {
           
            if (!_isNewEntry)
            {
                DisplayText = "0";
                _isNewEntry = true;
            }
            else
            {
               
                OnClear();
            }
        }

        private void OnClear()    // C: bütün işlemleri siler
        {
           
            DisplayText = "0";
            _inputExpression = "";
            _isNewEntry = true;
        }

        private void OnBackspace()   // Sil: son rakamı silen
        {
            
            if (DisplayText.Length > 1)
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }
            else
            {
                DisplayText = "0";
                _isNewEntry = true;
            }
        }

        private void OnNegate()    // +/- 
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

        private void OnDecimal()  //virgülden ondalığa geçis icin
        {
            
            if (!DisplayText.Contains(".") && !DisplayText.Contains(","))
            {
                DisplayText += ",";
            }
        }

        private void OnPercent()    // % 
        {
            
            try
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, _culture, out double value))
                {
                    
                    if (!string.IsNullOrEmpty(_inputExpression))
                    {
                        
                        string[] parts = _inputExpression.Trim().Split(' ');
                        if (parts.Length >= 2 && double.TryParse(parts[0], out double previousValue))
                        {
                            
                            double percentResult = previousValue * (value / 100);
                            DisplayText = percentResult.ToString(_culture);
                        }
                        else
                        {
                            
                            DisplayText = (value / 100).ToString(_culture);
                        }
                    }
                    else
                    {
                  
                        DisplayText = (value / 100).ToString(_culture);
                    }
                    _isNewEntry = true;
                }
            }
            catch (Exception)
            {
                DisplayText = "Hata";
                _isNewEntry = true;
            }
        }

        private void OnReciprocal()  // 1/x 
        {
            
            try
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, _culture, out double value))
                {
                    if (value == 0)
                    {
                        DisplayText = "Sıfıra Bölünme Hatası";
                    }
                    else
                    {
                        DisplayText = (1 / value).ToString(_culture);
                    }
                    _isNewEntry = true;
                }
            }
            catch (Exception)
            {
                DisplayText = "Hata";
                _isNewEntry = true;
            }
        }

        private void OnSquare()   // x²
        {
            
            try
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, _culture, out double value))
                {
                    DisplayText = Math.Pow(value, 2).ToString(_culture);
                    _isNewEntry = true;
                }
            }
            catch (Exception)
            {
                DisplayText = "Hata";
                _isNewEntry = true;
            }
        }

        private void OnSquareRoot()  // √x 
        {
            
            try
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, _culture, out double value))
                {
                    if (value < 0)
                    {
                        DisplayText = "Hata";
                    }
                    else
                    {
                        DisplayText = Math.Sqrt(value).ToString(_culture);
                    }
                    _isNewEntry = true;
                }
            }
            catch (Exception)
            {
                DisplayText = "Hata";
                _isNewEntry = true;
            }
        }

        private double EvaluateExpression(string expression)
        {
            try
            {
                // Özel sıfıra bölme durumu kontrolü
                string[] parts = expression.Split(' ');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    if (parts[i] == "/" && parts[i + 1] == "0")
                    {
                        throw new DivideByZeroException();
                    }
                }

                var result = new System.Data.DataTable().Compute(expression, null);
                double doubleResult = Convert.ToDouble(result, _culture);

                // Sonuçta sonsuz kontrolü
                if (double.IsInfinity(doubleResult))
                {
                    throw new DivideByZeroException();
                }

                return doubleResult;
            }
            catch (DivideByZeroException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}