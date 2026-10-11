using System.Diagnostics.CodeAnalysis;
using System.Text;
using Wacc.Exceptions;

namespace Wacc.Validation;

public class IdentifierMap
{
    private static int ScopeCount = 0;

    public IdentifierMap? Parent = null;
    public string Name { get; init; }

    public IdentifierMap()
    {
        Name = $"Scope{++ScopeCount}";
    }

    public IdentifierMap(IdentifierMap m) : this()
    {
        Map = [];
        Parent = m;
    }

    private static int LoopCounter = 0;

    private readonly Dictionary<string, MapEntry> Map = [];
    private string? CurLoopLabel;

    public bool ContainsKey(string key) => Map.ContainsKey(key);

    public MapEntry this[string key]
    {
        get => Map[key];
        set => Map[key] = value;
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out MapEntry value, out bool inCurScope)
    {
        inCurScope = false;
        var scope = this;

        do
        {
            if (scope.Map.TryGetValue(key, out var v))
            {
                value = v;
                inCurScope = scope == this;
                return true;
            }
            scope = scope.Parent;
        } while (scope is not null);

        value = null!;
        return false;
    }

    public bool TryGetFromValues(string targetValue, [NotNullWhen(true)] out MapEntry value, out bool inCurScope)
    {
        inCurScope = false;
        var scope = this;

        do
        {
            var v = scope.Map.FirstOrDefault(pair => pair.Value.NewName == targetValue).Value;
            if (v is not null)
            {
                value = v;
                inCurScope = scope == this;
                return true;
            }
            scope = scope.Parent;
        } while (scope is not null);

        value = null!;
        return false;
    }

    public string? GetLoopLabel() => this switch
    {
        _ when CurLoopLabel is not null => CurLoopLabel,
        _ when Parent is null => null,
        _ => Parent.GetLoopLabel()
    };

    internal static string NewLoopLabelName() => $"$_loop{++LoopCounter:000}";

    public string NewLoopLabel()
    {
        if (CurLoopLabel is not null)
        {
            throw new ValidationError($"For current variable map, loop label {CurLoopLabel} already exists");
        }

        CurLoopLabel = NewLoopLabelName();
        return CurLoopLabel;
    }

    public override string ToString() => $"{Name} ({Map.Count})";

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        var cur = this;
        while (cur is not null)
        {
            foreach (var entry in cur.Map.OrderBy(kvp => kvp.Key))
            {
                sb.AppendLine(new string(' ', indent) + $"{entry.Key}: {entry.Value.NewName}");
            }
            cur = cur.Parent;
            indent++;
        }

        return sb.ToString();
    }
}