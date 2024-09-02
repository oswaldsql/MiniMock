Imports System
Imports System.Threading
Imports MiniMock.Demo.BusinessLogic
Imports Xunit
imports MiniMock

Namespace MiniMock.Demo.VBNet
    Public Class UnitTest1
        <Fact>
        Sub TestSub()
            dim sut = Mock.ICustomerRepository(AddressOf Config)
            dim actual = sut.CreateCustomer("name", "Email@mail.com", CancellationToken.None).Result
            Assert.Equal(10, actual)
        End Sub

        Private Sub Config(config As ICustomerRepositoryMock.Config)
            config.CreateCustomer(10)
        End Sub
    End Class
End Namespace


