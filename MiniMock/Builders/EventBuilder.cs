namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class EventBuilder
{
    private static int eventCount;

    public static void BuildEvents(CodeBuilder builder, IEnumerable<IEventSymbol> eventSymbols)
    {
        var enumerable = eventSymbols as IEventSymbol[] ?? eventSymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<MethodSignature>();

        void AddHelper(string signature, string code, string documentation)
        {
            helpers.Add(new(signature, code, documentation));
        }

        foreach (var symbol in enumerable)
        {
            BuildEvent(builder, symbol, AddHelper);
        }

        helpers.BuildHelpers(builder, name);
    }

    internal static void BuildEvent(CodeBuilder builder, IEventSymbol evnt, Action<string, string, string> addHelper)
    {
        eventCount++;

        var eventName = evnt.Name;
        var invokeMethod = evnt.Type.GetMembers().OfType<IMethodSymbol>().First(t => t.Name == "Invoke");
        var types = string.Join(" , ", invokeMethod.Parameters.Skip(1).Select(t => t.Type));
        var typeSymbol = evnt.Type.ToString().Trim('?');

        var containingSymbol = "";
        var accessibilityString = evnt.AccessibilityString();
        if (evnt.ContainingType.TypeKind == TypeKind.Interface)
        {
            containingSymbol = evnt.ContainingSymbol + ".";
            accessibilityString = "";
        }

        var pa = invokeMethod.ToString(symbol => $"{symbol.Type} {symbol.Name}");
        var na = invokeMethod.ToString(symbol => $"{symbol.Name}");

        builder.Add($$"""

                      #region {{evnt.Type}} {{eventName}}
                      internal event {{typeSymbol}}? {{eventName}}_{{eventCount}};
                      {{accessibilityString}} event {{typeSymbol}}? {{containingSymbol}}{{eventName}}
                      {
                          add => this.{{eventName}}_{{eventCount}} += value;
                          remove => this.{{eventName}}_{{eventCount}} -= value;
                      }
                      internal void trigger_{{eventName}}_{{eventCount}}({{pa}})
                      {
                          {{eventName}}_{{eventCount}}?.Invoke({{na}});
                      }

                      #endregion
                      """);

        addHelper($"out System.Action<{types}> trigger", $"trigger = args => {eventName}(target, args);", $"Returns a action that can be used for triggering {eventName}.");
        addHelper(pa, $"target.trigger_{eventName}_{eventCount}({na});", $"Trigger {eventName} directly.");
    }
}
