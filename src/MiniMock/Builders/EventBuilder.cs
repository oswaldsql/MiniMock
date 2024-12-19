namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Util;

/// <summary>
///     Represents a builder for events, implementing the ISymbolBuilder interface.
/// </summary>
internal class EventBuilder : ISymbolBuilder
{
    /// <inheritdoc />
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

        return BuildEvents(builder, symbols.OfType<IEventSymbol>());
    }

    /// <summary>
    ///     Builds the events based on the given symbols and adds them to the code builder.
    /// </summary>
    /// <param name="builder">The code builder to add the events to.</param>
    /// <param name="eventSymbols">The event symbols to build.</param>
    /// <returns>True if any events were built; otherwise, false.</returns>
    private static bool BuildEvents(CodeBuilder builder, IEnumerable<IEventSymbol> eventSymbols)
    {
        var enumerable = eventSymbols as IEventSymbol[] ?? eventSymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<HelperMethod>();

        builder.Add($"#region event : {name}");

        var eventCount = 1;
        foreach (var symbol in enumerable)
        {
            if (BuildEvent(builder, symbol, helpers, eventCount))
            {
                eventCount++;
            }
        }

        builder.Add(helpers.BuildHelpers(name));

        builder.Add("#endregion");

        return eventCount > 1;
    }

    /// <summary>
    ///     Builds an individual event and adds it to the code builder.
    /// </summary>
    /// <param name="builder">The code builder to add the event to.</param>
    /// <param name="symbol">The event symbol to build.</param>
    /// <param name="helpers">A list of helper methods to add to.</param>
    /// <param name="eventCount">The count of the event being built.</param>
    /// <returns>True if any events were built; otherwise, false.</returns>
    private static bool BuildEvent(CodeBuilder builder, IEventSymbol symbol, List<HelperMethod> helpers, int eventCount)
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

        return true;
    }

    /// <summary>
    ///     Builds helper methods for the given event symbol.
    /// </summary>
    /// <param name="symbol">The event symbol to build helpers for.</param>
    /// <param name="types">The types of the event parameters.</param>
    /// <param name="eventFunction">The name of the event function.</param>
    /// <returns>An enumerable of helper methods.</returns>
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
