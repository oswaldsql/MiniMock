namespace MiniMock.Tests.EventTests;

using System.ComponentModel;

public class TriggerEventFromConfiguration
{
    [Fact]
    [Mock<INotifyDTO>]
    public void ItShouldBePossibleToTriggerEventsFromConfiguration()
    {
        // arrange
        var actual = "";
        var sut = Mock.INotifyDTO(config => config.Value(() => "test", s =>
        {
            config.PropertyChanged(new PropertyChangedEventArgs("Value"));
        }));

        sut.PropertyChanged += (_, args) => actual = args.PropertyName;

        // act
        sut.Value = "dummy";

        // assert
        Assert.Equal("Value", actual);
    }

    public interface INotifyDTO : INotifyPropertyChanged
    {
        public string Value { get; set; }
    }
}
