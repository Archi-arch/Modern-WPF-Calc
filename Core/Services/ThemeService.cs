using System.Windows;
using System.Windows.Media;

namespace Calculator.Core.Services
{
    public class ThemeService : IThemeService
    {
        public bool IsDarkTheme { get; private set; } = true;

        public void ToggleTheme()
        {
            ApplyTheme(!IsDarkTheme);
        }

        public void ApplyTheme(bool isDark)
        {
            IsDarkTheme = isDark;
            var resources = Application.Current.Resources;

            if (isDark)
            {
                // Dark Theme Colors (Obsidian Glass)
                SetBrush(resources, "BgObsidianBrush", "#0F1017");
                SetBrush(resources, "BgCardBrush", "#181A26");
                SetBrush(resources, "BgCardHoverBrush", "#222536");
                SetBrush(resources, "BgCardPressedBrush", "#2A2E44");
                SetBrush(resources, "BgGlassPanelBrush", "#141622");
                SetBrush(resources, "BorderGlowBrush", "#2E334D");

                SetBrush(resources, "TextPrimaryBrush", "#FFFFFF");
                SetBrush(resources, "TextSecondaryBrush", "#8E95B3");
                SetBrush(resources, "TextMutedBrush", "#5C6280");

                // Accent Icon Colors (Vibrant Neon)
                SetBrush(resources, "NeonCyanBrush", "#00E5FF");
                SetBrush(resources, "AccentPurpleBrush", "#7C4DFF");
                SetBrush(resources, "DangerRedBrush", "#FF5252");

                SetGradient(resources, "WindowHeaderGradient", "#12131C", "#181A26");
                SetGradient(resources, "ScientificGradientBrush", "#1E2235", "#151726");
            }
            else
            {
                // Muted Slate Light Theme Colors (High Contrast Icon & Symbol Visibility)
                SetBrush(resources, "BgObsidianBrush", "#DCE1ED");
                SetBrush(resources, "BgCardBrush", "#EDF1F8");
                SetBrush(resources, "BgCardHoverBrush", "#DCE3F0");
                SetBrush(resources, "BgCardPressedBrush", "#CBD5E8");
                SetBrush(resources, "BgGlassPanelBrush", "#E6EBF5");
                SetBrush(resources, "BorderGlowBrush", "#B8C2D4");

                SetBrush(resources, "TextPrimaryBrush", "#1E2436");
                SetBrush(resources, "TextSecondaryBrush", "#4A546E");
                SetBrush(resources, "TextMutedBrush", "#76829E");

                // Accent Icon Colors (Deep, High Contrast Tones for Light Backgrounds)
                SetBrush(resources, "NeonCyanBrush", "#0284C7");     // Deep Ocean Cyan/Blue for operators (+, -, ×, ÷)
                SetBrush(resources, "AccentPurpleBrush", "#6D28D9"); // Deep Rich Purple for scientific functions
                SetBrush(resources, "DangerRedBrush", "#DC2626");    // Deep Crimson Red for clear buttons (C, CE, ⌫)

                SetGradient(resources, "WindowHeaderGradient", "#D1D7E6", "#DFE4F0");
                SetGradient(resources, "ScientificGradientBrush", "#DCE2EF", "#D0D7E6");
            }
        }

        private static void SetBrush(ResourceDictionary resources, string key, string hexColor)
        {
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            resources[key] = new SolidColorBrush(color);
        }

        private static void SetGradient(ResourceDictionary resources, string key, string startHex, string endHex)
        {
            var startColor = (Color)ColorConverter.ConvertFromString(startHex);
            var endColor = (Color)ColorConverter.ConvertFromString(endHex);
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };
            brush.GradientStops.Add(new GradientStop(startColor, 0));
            brush.GradientStops.Add(new GradientStop(endColor, 1));
            resources[key] = brush;
        }
    }
}
