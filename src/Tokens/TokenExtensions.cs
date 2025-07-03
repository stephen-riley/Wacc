using System.ComponentModel;
using System.Reflection;

namespace Wacc.Tokens;

public static class TokenExtensions
{
    public static string Description<TEnum>(this TEnum val) where TEnum : Enum
    {
        var field = typeof(TEnum).GetField(val.ToString()) ?? throw new InvalidOperationException($"{typeof(TEnum).Name}.{val} has no FieldInfo");
        var attributes = field.GetCustomAttributes<DescriptionAttribute>(false).ToArray();
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}