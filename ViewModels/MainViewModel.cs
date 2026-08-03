using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Calculator.Core.Models;
using Calculator.Core.Services;

namespace Calculator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICalculatorEngine _calculatorEngine;
        private readonly IHistoryService _historyService;
        private readonly IThemeService _themeService;

        private string _displayText = "0";
        private string _expressionText = "";
        private bool _isScientificMode = false;
        private bool _isHistoryOpen = false;
        private bool _isNewEntry = true;
        private double? _firstOperand = null;
        private string? _currentOperator = null;
        private bool _hasError = false;

        public MainViewModel() : this(new CalculatorEngine(), new HistoryService(), new ThemeService())
        {
        }

        public MainViewModel(ICalculatorEngine calculatorEngine, IHistoryService historyService, IThemeService themeService)
        {
            _calculatorEngine = calculatorEngine ?? throw new ArgumentNullException(nameof(calculatorEngine));
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

            History = new ObservableCollection<CalculationItem>();

            // Initialize Commands
            NumberCommand = new RelayCommand(param => OnNumberEntered(param?.ToString()));
            OperationCommand = new RelayCommand(param => OnOperationEntered(param?.ToString()));
            UnaryOperationCommand = new RelayCommand(param => OnUnaryOperationEntered(param?.ToString()));
            EqualsCommand = new RelayCommand(OnEqualsEntered);
            ClearCommand = new RelayCommand(OnClearAll);
            ClearEntryCommand = new RelayCommand(OnClearEntry);
            BackspaceCommand = new RelayCommand(OnBackspaceEntered);
            ToggleSignCommand = new RelayCommand(OnToggleSign);
            ToggleModeCommand = new RelayCommand(OnToggleMode);
            ToggleThemeCommand = new RelayCommand(OnToggleTheme);
            ToggleHistoryCommand = new RelayCommand(OnToggleHistory);
            SelectHistoryItemCommand = new RelayCommand(param => OnSelectHistoryItem(param as CalculationItem));
            ClearHistoryCommand = new RelayCommand(OnClearHistory);
            InsertConstantCommand = new RelayCommand(param => OnInsertConstant(param?.ToString()));
        }

        #region Properties

        public string DisplayText
        {
            get => _displayText;
            set => SetProperty(ref _displayText, value);
        }

        public string ExpressionText
        {
            get => _expressionText;
            set => SetProperty(ref _expressionText, value);
        }

        public bool IsScientificMode
        {
            get => _isScientificMode;
            set
            {
                if (SetProperty(ref _isScientificMode, value))
                {
                    OnPropertyChanged(nameof(ModeTitle));
                }
            }
        }

        public bool IsHistoryOpen
        {
            get => _isHistoryOpen;
            set => SetProperty(ref _isHistoryOpen, value);
        }

        public string ModeTitle => IsScientificMode ? "Scientific" : "Standard";

        public bool IsDarkTheme => _themeService.IsDarkTheme;
        public string ThemeTitle => IsDarkTheme ? "Dark" : "Light";
        public string ThemeIcon => IsDarkTheme ? "🌙" : "☀️";

        public ObservableCollection<CalculationItem> History { get; }

        #endregion

        #region Commands

        public ICommand NumberCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand UnaryOperationCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ToggleSignCommand { get; }
        public ICommand ToggleModeCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand ToggleHistoryCommand { get; }
        public ICommand SelectHistoryItemCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand InsertConstantCommand { get; }

        #endregion

        #region Logic Handlers

        private void OnNumberEntered(string? number)
        {
            if (string.IsNullOrEmpty(number)) return;

            if (_hasError)
            {
                OnClearAll();
            }

            if (number == ".")
            {
                if (_isNewEntry)
                {
                    DisplayText = "0.";
                    _isNewEntry = false;
                    return;
                }

                if (!DisplayText.Contains("."))
                {
                    DisplayText += ".";
                }
                return;
            }

            if (_isNewEntry || DisplayText == "0")
            {
                DisplayText = number;
                _isNewEntry = false;
            }
            else
            {
                if (DisplayText.Length < 16) // Max digits limit
                {
                    DisplayText += number;
                }
            }
        }

        private void OnOperationEntered(string? op)
        {
            if (string.IsNullOrEmpty(op) || _hasError) return;

            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentVal))
            {
                if (_firstOperand.HasValue && !_isNewEntry && _currentOperator != null)
                {
                    try
                    {
                        double result = _calculatorEngine.ExecuteBinaryOperation(_firstOperand.Value, currentVal, _currentOperator);
                        _firstOperand = result;
                        DisplayText = FormatResult(result);
                    }
                    catch (Exception ex)
                    {
                        SetErrorState(ex.Message);
                        return;
                    }
                }
                else
                {
                    _firstOperand = currentVal;
                }

                _currentOperator = op;
                ExpressionText = $"{FormatResult(_firstOperand.Value)} {op}";
                _isNewEntry = true;
            }
        }

        private void OnUnaryOperationEntered(string? op)
        {
            if (string.IsNullOrEmpty(op) || _hasError) return;

            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double operand))
            {
                try
                {
                    double result = _calculatorEngine.ExecuteUnaryOperation(operand, op);
                    string formattedResult = FormatResult(result);

                    string expr = op switch
                    {
                        "x²" or "sqr" => $"sqr({FormatResult(operand)})",
                        "√" or "sqrt" => $"√({FormatResult(operand)})",
                        "1/x" => $"1/({FormatResult(operand)})",
                        "sin" => $"sin({FormatResult(operand)}°)",
                        "cos" => $"cos({FormatResult(operand)}°)",
                        "tan" => $"tan({FormatResult(operand)}°)",
                        "ln" => $"ln({FormatResult(operand)})",
                        "log" => $"log({FormatResult(operand)})",
                        "n!" => $"fact({FormatResult(operand)})",
                        _ => $"{op}({FormatResult(operand)})"
                    };

                    ExpressionText = expr;
                    DisplayText = formattedResult;
                    _isNewEntry = true;

                    // Add unary operation to history
                    _historyService.AddItem(expr, formattedResult);
                    RefreshHistoryList();
                }
                catch (Exception ex)
                {
                    SetErrorState(ex.Message);
                }
            }
        }

        private void OnEqualsEntered()
        {
            if (_hasError || !_firstOperand.HasValue || string.IsNullOrEmpty(_currentOperator)) return;

            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double secondOperand))
            {
                try
                {
                    double result = _calculatorEngine.ExecuteBinaryOperation(_firstOperand.Value, secondOperand, _currentOperator);
                    string fullExpression = $"{FormatResult(_firstOperand.Value)} {_currentOperator} {FormatResult(secondOperand)} =";
                    string formattedResult = FormatResult(result);

                    ExpressionText = fullExpression;
                    DisplayText = formattedResult;

                    _historyService.AddItem(fullExpression, formattedResult);
                    RefreshHistoryList();

                    _firstOperand = null;
                    _currentOperator = null;
                    _isNewEntry = true;
                }
                catch (Exception ex)
                {
                    SetErrorState(ex.Message);
                }
            }
        }

        private void OnClearAll()
        {
            DisplayText = "0";
            ExpressionText = "";
            _firstOperand = null;
            _currentOperator = null;
            _isNewEntry = true;
            _hasError = false;
        }

        private void OnClearEntry()
        {
            DisplayText = "0";
            _isNewEntry = true;
            _hasError = false;
        }

        private void OnBackspaceEntered()
        {
            if (_hasError || _isNewEntry) return;

            if (DisplayText.Length > 1)
            {
                DisplayText = DisplayText[..^1];
                if (DisplayText == "-" || string.IsNullOrEmpty(DisplayText))
                {
                    DisplayText = "0";
                    _isNewEntry = true;
                }
            }
            else
            {
                DisplayText = "0";
                _isNewEntry = true;
            }
        }

        private void OnToggleSign()
        {
            if (_hasError || DisplayText == "0") return;

            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                val = -val;
                DisplayText = FormatResult(val);
            }
        }

        private void OnInsertConstant(string? constant)
        {
            if (_hasError) OnClearAll();

            double val = constant switch
            {
                "π" or "pi" => Math.PI,
                "e" => Math.E,
                _ => 0
            };

            DisplayText = FormatResult(val);
            _isNewEntry = true;
        }

        private void OnToggleMode()
        {
            IsScientificMode = !IsScientificMode;
        }

        private void OnToggleTheme()
        {
            _themeService.ToggleTheme();
            OnPropertyChanged(nameof(IsDarkTheme));
            OnPropertyChanged(nameof(ThemeTitle));
            OnPropertyChanged(nameof(ThemeIcon));
        }

        private void OnToggleHistory()
        {
            IsHistoryOpen = !IsHistoryOpen;
        }

        private void OnSelectHistoryItem(CalculationItem? item)
        {
            if (item == null) return;
            DisplayText = item.Result;
            _isNewEntry = true;
        }

        private void OnClearHistory()
        {
            _historyService.ClearHistory();
            RefreshHistoryList();
        }

        private void RefreshHistoryList()
        {
            History.Clear();
            foreach (var item in _historyService.History)
            {
                History.Add(item);
            }
        }

        private void SetErrorState(string message)
        {
            _hasError = true;
            DisplayText = message;
            _isNewEntry = true;
        }

        private static string FormatResult(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "Error";

            // Format cleanly without trailing zeros if integer
            if (Math.Abs(value) > 1e12 || (Math.Abs(value) < 1e-6 && value != 0))
            {
                return value.ToString("G8", CultureInfo.InvariantCulture);
            }

            return value.ToString("0.################", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
