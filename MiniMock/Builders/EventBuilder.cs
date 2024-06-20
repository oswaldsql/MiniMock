namespace MiniMock.Builders;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal static class EventBuilder
{
    internal static void BuildEvent(CodeBuilder builder, IEventSymbol evnt)
    {
        var eventName = evnt.Name;
        var invokeMethod = evnt.Type.GetMembers().OfType<IMethodSymbol>().First(t => t.Name == "Invoke");
        var types = string.Join(" , ", invokeMethod.Parameters.Skip(1).Select(t => t.Type));

        builder.Add($$"""

                      #region {{evnt.Type}} {{eventName}}
                      {{evnt.AccessibilityString()}} event {{evnt.Type}} {{eventName}};
                      
                      public partial class Config
                      {
                          public void {{eventName}}(out System.Action<{{types}}> trigger)
                          {
                              trigger = args => target.{{eventName}}?.Invoke(target, args);
                          }
                      }

                      #endregion
                      """);
    }

    public static void BuildEvents(CodeBuilder builder, IEnumerable<IEventSymbol> eventSymbols)
    {
        foreach (var symbol in eventSymbols)
        {
            BuildEvent(builder, symbol);
        }
    }
}


