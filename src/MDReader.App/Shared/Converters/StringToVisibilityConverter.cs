// MIT License - see LICENSE file in the repository root.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MDReader.App.Shared.Converters;

/// <summary>
/// Converts a string value to a <see cref="Visibility"/> value.
/// Non-empty string → Visible, Null or empty string → Collapsed.
/// </summary>
public sealed class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool invert = parameter?.ToString() == "Invert";
        bool hasText = value is string str && !string.IsNullOrWhiteSpace(str);
        return (hasText ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
