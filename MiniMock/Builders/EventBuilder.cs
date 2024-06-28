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

        BuildHelpers(builder, helpers, name);
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

    private static void BuildHelpers(CodeBuilder builder, List<MethodSignature> helpers, string name)
    {
        if (helpers.Count == 0)
        {
            return;
        }

        var signatures = helpers.ToLookup(t => t.Signature);

        builder.Add("public partial class Config {").Indent();

        foreach (var grouping in signatures)
        {
            builder.Add($"""

                         /// <summary>
                         """);
            grouping.Select(t => t.Documentation).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList().ForEach(t => builder.Add("///     " + t));
            builder.Add($"""
                         /// </summary>
                         /// <returns>The updated configuration.</returns>
                         """);

            builder.Add($"public Config {name}({grouping.Key}) {{").Indent();
            foreach (var code in grouping.Select(t => t.Code).Distinct())
            {
                builder.Add(code);
            }

            builder.Unindent().Add("    return this;");
            builder.Add("}");
            builder.Add();
        }

        builder.Unindent().Add("}");
    }
}
