namespace MiniMock.Demo.BusinessLogic;

public interface IEMailService
{
    Task SendMail(string to, string subject, string body);
}
