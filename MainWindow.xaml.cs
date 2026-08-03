using System;
using System.Windows;
using System.Windows.Input;
using Calculator.ViewModels;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            Loaded += (s, e) => MainRootGrid.Focus();
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    ToggleMaximize();
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool isHandled = true;

            switch (e.Key)
            {
                case Key.D0 or Key.NumPad0:
                    _viewModel.NumberCommand.Execute("0");
                    break;
                case Key.D1 or Key.NumPad1:
                    _viewModel.NumberCommand.Execute("1");
                    break;
                case Key.D2 or Key.NumPad2:
                    _viewModel.NumberCommand.Execute("2");
                    break;
                case Key.D3 or Key.NumPad3:
                    _viewModel.NumberCommand.Execute("3");
                    break;
                case Key.D4 or Key.NumPad4:
                    _viewModel.NumberCommand.Execute("4");
                    break;
                case Key.D5 or Key.NumPad5:
                    _viewModel.NumberCommand.Execute("5");
                    break;
                case Key.D6 or Key.NumPad6:
                    _viewModel.NumberCommand.Execute("6");
                    break;
                case Key.D7 or Key.NumPad7:
                    _viewModel.NumberCommand.Execute("7");
                    break;
                case Key.D8 or Key.NumPad8:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        _viewModel.OperationCommand.Execute("×");
                    else
                        _viewModel.NumberCommand.Execute("8");
                    break;
                case Key.D9 or Key.NumPad9:
                    _viewModel.NumberCommand.Execute("9");
                    break;
                case Key.OemPeriod or Key.Decimal or Key.OemComma:
                    _viewModel.NumberCommand.Execute(".");
                    break;
                case Key.Add:
                    _viewModel.OperationCommand.Execute("+");
                    break;
                case Key.Subtract or Key.OemMinus:
                    _viewModel.OperationCommand.Execute("-");
                    break;
                case Key.Multiply:
                    _viewModel.OperationCommand.Execute("×");
                    break;
                case Key.Divide or Key.OemQuestion:
                    _viewModel.OperationCommand.Execute("÷");
                    break;
                case Key.Enter:
                    _viewModel.EqualsCommand.Execute(null);
                    break;
                case Key.Back:
                    _viewModel.BackspaceCommand.Execute(null);
                    break;
                case Key.Escape:
                    _viewModel.ClearCommand.Execute(null);
                    break;
                default:
                    isHandled = false;
                    break;
            }

            if (isHandled)
            {
                e.Handled = true;
            }
        }
    }
}