namespace MiniMock.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class EventBuilder
{
    public static void BuildEvents(CodeBuilder builder, IEnumerable<IEventSymbol> eventSymbols)
    {
        var enumerable = eventSymbols as IEventSymbol[] ?? eventSymbols.ToArray();
        var name = enumerable.First().Name;
        var helpers = new List<MethodSignature>();

        builder.Add($"#region event : {name}");

        int eventCount = 0;
        foreach (var symbol in enumerable)
        {
            eventCount++;
            BuildEvent(builder, symbol, helpers, eventCount);
        }

        helpers.BuildHelpers(builder, name);

        builder.Add($"#endregion");
    }

    private static void BuildEvent(CodeBuilder builder, IEventSymbol symbol, List<MethodSignature> helpers, int eventCount)
    {
        var eventName = symbol.Name;
        var invokeMethod = symbol.Type.GetMembers().OfType<IMethodSymbol>().First(t => t.Name == "Invoke");
        var types = string.Join(" , ", invokeMethod.Parameters.Skip(1).Select(t => t.Type));
        var typeSymbol = symbol.Type.ToString().Trim('?');

        var eventFunction = eventCount == 1 ? eventName : $"{eventName}_{eventCount}";

        var (containingSymbol, accessibilityString, _) = symbol.Overwrites();

        var (parameterList, typeList, nameList) = invokeMethod.ParameterStrings();

        builder.Add($$"""

                      private event {{typeSymbol}}? {{eventFunction}};
                      {{accessibilityString}} event {{typeSymbol}}? {{containingSymbol}}{{eventName}}
                      {
                          add => this.{{eventFunction}} += value;
                          remove => this.{{eventFunction}} -= value;
                      }
                      private void trigger_{{eventFunction}}({{parameterList}})
                      {
                          {{eventFunction}}?.Invoke({{nameList}});
                      }

                      """);

        if (types == "System.EventArgs")
        {
            helpers.Add(new($"out System.Action trigger",
                $"trigger = () => this.{eventName}();",
                $"Returns an action that can be used for triggering {eventName}."));

            helpers.Add(new("",
                $"target.trigger_{eventFunction}(target, System.EventArgs.Empty);",
                $"Trigger {eventName} directly."));
        }
        else
        {
            helpers.Add(new($"out System.Action<{types}> trigger",
                $"trigger = args => this.{eventName}(args);",
                $"Returns an action that can be used for triggering {eventName}."));

            helpers.Add(new(types + " eventArgs",
                $"target.trigger_{eventFunction}(target, eventArgs);",
                $"Trigger {eventName} directly."));
        }
    }
}
