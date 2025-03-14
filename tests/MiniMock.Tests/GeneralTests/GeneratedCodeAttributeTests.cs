﻿namespace MiniMock.Tests.GeneralTests;

using System.CodeDom.Compiler;
using System.Reflection;

public class GeneratedCodeAttributeTests
{
    private const string CurrentVersion = "0.9.14";

    [Fact]
    public void MockAttributeShouldHaveAttribute()
    {
        // Arrange
        var sut = typeof(MockAttribute);

        // ACT
        var actual = sut.GetCustomAttributes<GeneratedCodeAttribute>();

        // Assert
        var actualAttribute = Assert.Single(actual);
        Assert.Equal("MiniMock", actualAttribute.Tool);
        Assert.Equal(CurrentVersion, actualAttribute.Version);
    }

    [Fact]
    public void MockClassShouldHaveAttribute()
    {
        // Arrange
        var sut = typeof(Mock);

        // ACT
        var actual = sut.GetCustomAttributes<GeneratedCodeAttribute>();

        // Assert
        var actualAttribute = Assert.Single(actual);
        Assert.Equal("MiniMock", actualAttribute.Tool);
        Assert.Equal(CurrentVersion, actualAttribute.Version);
    }


    [Fact]
    [Mock<IGeneratorTarget>]
    public void GeneratedMockClassShouldHaveAttribute()
    {
        // Arrange
        var sut = typeof(MockOf_IGeneratorTarget);

        // ACT
        var actual = sut.GetCustomAttributes<GeneratedCodeAttribute>();

        // Assert
        var actualAttribute = Assert.Single(actual);
        Assert.Equal("MiniMock", actualAttribute.Tool);
        Assert.Equal(CurrentVersion, actualAttribute.Version);
    }

    public interface IGeneratorTarget
    {

    }
}
