using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace tree;

public class MultibindingConverter : IMultiValueConverter
{
    public object? Convert( IList<object?> values, Type targetType, object? parameter, CultureInfo culture )
    {
        var topicObj = values[ 0 ];
        var messagesObj = values[ 1 ];
        var historyObj = values[ 2 ];

        int.TryParse( topicObj?.ToString(), out var topicsCount );
        int.TryParse( messagesObj?.ToString(), out var messagesCount );
        int.TryParse( historyObj?.ToString(), out var historyCount );
        
        var nodeTitle = historyCount == 1
            ? $"(in history: {historyCount} message)"
            : $"(in history: {historyCount} messages)";

        return topicsCount == 0 
            ? $"{nodeTitle}" 
            : $"({topicsCount} topics, {messagesCount} messages)";
    }
}