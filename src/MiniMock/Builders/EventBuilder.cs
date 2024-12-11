namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

internal class EventBuilder : ISymbolBuilder
{
    public bool TryBuild(CodeBuilder builder, IGrouping<string, ISymbol> symbols)
    {
        var first = symbols.First();

        if (first is IMethodSymbol { MethodKind: MethodKind.EventAdd or MethodKind.EventRaise or MethodKind.EventRemove })
        {
            return true;
        }

        if (first is not IEventSymbol)
        {
            return false;
        }

        if (!(first.IsAbstract || first.IsVirtual) || first.IsStatic)
        {
            return true;
        }

        return this.BuildEvents(builder, symbols.OfType<IEventSymbol>());
    }

    private bool BuildEvents(CodeBuilder builder, IEnumerable<IEventSymbol> eventSymbols)
    {
        var enumerable = eventSymbols as IEventSymbol[] ?? eventSymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<HelperMethod>();

        builder.Add($"#region event : {name}");

        var eventCount = 0;
        foreach (var symbol in enumerable)
        {
            eventCount++;
            BuildEvent(builder, symbol, helpers, eventCount);
        }

        builder.Add(helpers.BuildHelpers(name));

        builder.Add("#endregion");

        return eventCount > 0;
    }

    private static void BuildEvent(CodeBuilder builder, IEventSymbol symbol, List<HelperMethod> helpers, int eventCount)
    {
        var eventName = symbol.Name;
        var invokeMethod = symbol.Type.GetMembers().OfType<IMethodSymbol>().First(t => t.Name == "Invoke");
        var types = string.Join(" , ", invokeMethod.Parameters.Skip(1).Select(t => t.Type));
        var typeSymbol = symbol.Type.ToString().Trim('?');

        var eventFunction = eventCount == 1 ? eventName : $"{eventName}_{eventCount}";

        var (containingSymbol, accessibilityString, _) = symbol.Overwrites();

        var (_, parameterList, _, nameList) = invokeMethod.ParameterStrings();

        builder.Add($$"""

                      private event {{typeSymbol}}? _{{eventFunction}};
                      {{accessibilityString}} event {{typeSymbol}}? {{containingSymbol}}{{eventName}}
                      {
                          add => this._{{eventFunction}} += value;
                          remove => this._{{eventFunction}} -= value;
                      }
                      private void trigger_{{eventFunction}}({{parameterList}})
                      {
                          _{{eventFunction}}?.Invoke({{nameList}});
                      }

                      """);

        helpers.AddRange(BuildHelpers(symbol, types, eventFunction));
    }

    private static IEnumerable<HelperMethod> BuildHelpers(IEventSymbol symbol, string types, string eventFunction)
    {
        var seeCref = symbol.ToString();
        var eventName = symbol.Name;

        if (types == "System.EventArgs")
        {
            yield return new HelperMethod("out System.Action trigger",
                $"trigger = () => this.{eventName}();",
                $"Returns an action that can be used for triggering {eventName}.", seeCref);

            yield return new HelperMethod("",
                $"target.trigger_{eventFunction}(target, System.EventArgs.Empty);",
                $"Trigger {eventName} directly.", seeCref);
        }
        else
        {
            yield return new HelperMethod($"out System.Action<{types}> trigger",
                $"trigger = args => this.{eventName}(args);",
                $"Returns an action that can be used for triggering {eventName}.", seeCref);

            yield return new HelperMethod(types + " raise",
                $"target.trigger_{eventFunction}(target, raise);",
                $"Trigger {eventName} directly.", seeCref);
        }
    }
}
