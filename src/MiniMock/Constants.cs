namespace MiniMock;

public static class Constants
{
    public const string MiniMockVersion = "0.9.14";

    public const string MockAttributeCode = $$"""
                                              namespace MiniMock {
                                                  /// <summary>
                                                  /// Indicates the interface or class that should be mocked.
                                                  /// </summary>
                                                  /// <typeparam name="T">Interface or class to be mocked.</typeparam>
                                                  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
                                                  internal class MockAttribute<T> : System.Attribute{}

                                                  [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false)]
                                                  [System.CodeDom.Compiler.GeneratedCode("MiniMock","{{MiniMockVersion}}")]
                                                  internal class MockAttribute : System.Attribute
                                                  {
                                                      public string MockFactoryName { get; set; } = "Mock";
                                                  }
                                              }
                                              """;


    public const string MockCallStackCode = """
                                            namespace MiniMock {
                                                internal class CallEvents //: System.Collections.Generic.IEnumerable<CallEvent>
                                                {
                                                    private readonly System.Collections.Generic.List<CallEvent> store = new();
                                                    private int index = 0;
                                                    public void Add(string source, string method, CallEventType type) => this.store.Add(new CallEvent(this.index++, source, method,     type));
                                                    public System.Collections.Generic.IEnumerator<CallEvent> GetEnumerator() => this.store.GetEnumerator();

                                                    //System.Collections.Generic.IEnumerator<CallEvent> IEnumerable.GetEnumerator() => this.GetEnumerator();
                                                }

                                                internal class CallEvent
                                                {
                                                    public CallEvent(int Index, string Source, string Method, CallEventType Type)
                                                    {
                                                        this.Index = Index;
                                                        this.Source = Source;
                                                        this.Method = Method;
                                                        this.Type = Type;
                                                    }

                                                    public int Index { get; }
                                                    public string Source { get; }
                                                    public string Method { get; }
                                                    public CallEventType Type { get; }

                                                    public void Deconstruct(out int Index, out string Source, out string Method, out CallEventType Type)
                                                    {
                                                        Index = this.Index;
                                                        Source = this.Source;
                                                        Method = this.Method;
                                                        Type = this.Type;
                                                    }
                                                }

                                                internal enum CallEventType
                                                {
                                                    Setup = 0,
                                                    Call = 1,
                                                    Get = 2,
                                                    Set = 3,
                                                    Raise = 4,
                                                }
                                            }
                                            """;
}
