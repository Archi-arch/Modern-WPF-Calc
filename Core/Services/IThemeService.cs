namespace Calculator.Core.Services
{
    public interface IThemeService
    {
        bool IsDarkTheme { get; }
        void ToggleTheme();
        void ApplyTheme(bool isDark);
    }
}
