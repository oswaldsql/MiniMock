namespace MiniMock.Tests.ConstructorTests;

public class WithParametersTests
{
    internal class ParameterClass
    {
        public string Name { get; }
        public int Age { get; }
        public DateTimeOffset Birth { get; } = DateTimeOffset.MinValue;

        public ParameterClass(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }

        public ParameterClass(string name, int age, DateTimeOffset birth)
        {
            this.Name = name;
            this.Age = age;
            this.Birth = birth;
        }

        public virtual bool IsAgeValid()
        {
            var calculateAge = CalculateAge(this.Birth);
            return this.Age == calculateAge;
        }

        public static int CalculateAge(DateTimeOffset birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age)) age--;

            return age;
        }
    }

    [Fact]
    [Mock<ParameterClass>]
    public void CanCreateMocksWithBaseConstructorParameters()
    {
        // Arrange

        // ACT
        var actual = Mock.ParameterClass("oswald", 25);
        var sut = Mock.ParameterClass("name", 10, DateTimeOffset.Now, config => config.IsAgeValid(false));

        // Assert
        Assert.Equal(25, actual.Age);
        Assert.Equal(actual.Name, "oswald");

        Assert.Equal(false, sut.IsAgeValid());
    }
}
