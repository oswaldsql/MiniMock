Imports System.Threading
Imports MiniMock.Demo.BusinessLogic
Imports Xunit

Namespace MiniMock.Demo.VBNet
    Public Class UnitTest1
        <Fact>
        Sub TestSub()
            dim sut = Mock.ICustomerRepository(AddressOf Config)
            dim actual = sut.CreateCustomer("name", "Email@mail.com", CancellationToken.None).Result
            Assert.Equal(10, actual)
        End Sub

        Private Sub Config(config As MockOf_ICustomerRepository.Config)
            config.CreateCustomer(10)
        End Sub
    End Class
End Namespace


