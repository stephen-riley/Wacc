using Wacc.Validation.Types;

namespace Wacc.Validation.Tools;

public record MapEntry(string NewName, WaccType Type, bool FromCurrentScope = true, bool HasLinkage = false);