namespace MiniMock;

public static class Constants
{
    public const string MockAttributeCode = """
                                            namespace MiniMock { 
                                                [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
                                                public class Mock<T> : System.Attribute{}
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