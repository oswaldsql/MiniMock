using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MiniMock.Demo.VBNet")]
[assembly:InternalsVisibleTo("MiniMock.Demo.FSharp")]

namespace MiniMock.Demo.OtherLanguages;

using BusinessLogic;
[Mock<ICustomerRepository>]
[Mock<IEMailService>]
public class Class1
{
}
