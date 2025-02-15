﻿namespace MiniMock.Tests.EventTests;

using System.ComponentModel;

public class TriggerEventFromConfiguration
{
    [Fact]
    [Mock<INotifyDto>]
    public void ItShouldBePossibleToTriggerEventsFromConfiguration()
    {
        // arrange
        var actual = "";
        var sut = Mock.INotifyDto(config => config.Value(() => "test", _ =>
        {
            config.PropertyChanged(new PropertyChangedEventArgs("Value"));
        }));

        sut.PropertyChanged += (_, args) => actual = args.PropertyName;

        // act
        sut.Value = "dummy";

        // assert
        Assert.Equal("Value", actual);
    }

    [Fact]
    [Mock<INotifyDto>]
    public void ItShouldBePossibleToTriggerEventsFromAExposedConfiguration()
    {
        // Arrange
        Version? actual = null;
        MockOf_IVersionLibrary.Config? exposedConfig = null;

        var versionLibrary = Mock.IVersionLibrary(config => exposedConfig = config);

        versionLibrary.NewVersionAdded += (_, version) => actual = version;

        // ACT
        exposedConfig?.NewVersionAdded(new Version(2, 0, 0, 0));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(new Version(2, 0, 0, 0), actual);
    }

    public interface INotifyDto : INotifyPropertyChanged
    {
        public string Value { get; set; }
    }
}
