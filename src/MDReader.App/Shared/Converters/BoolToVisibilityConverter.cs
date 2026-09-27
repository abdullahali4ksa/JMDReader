// MIT License - see LICENSE file in the repository root.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MDReader.App.Shared.Converters;

/// <summary>
/// Converts a boolean value to a <see cref="Visibility"/> value.
/// True → Visible, False (or Invert=true) → Collapsed.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean to Visibility.
    /// Pass "Invert" as the parameter to reverse the logic.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool invert = parameter?.ToString() == "Invert";
        bool boolValue = (bool)value;
        return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Converts a Visibility back to a boolean.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        bool invert = parameter?.ToString() == "Invert";
        bool isVisible = (Visibility)value == Visibility.Visible;
        return isVisible ^ invert;
    }
}
