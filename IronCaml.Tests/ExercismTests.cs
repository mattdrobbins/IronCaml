using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml.Tests
{
    public class ExercismTests
    {
        [Fact]
        public void HelloWorld()
        {
            var code = File.ReadAllText("ExercismExamples/hello_world.ml");
            var result = IronCaml.Execute<Func<string>>(code);
            Assert.Equal("Hello, World!", result());
        }

        [Theory]
        [InlineData(2015, false)]
        [InlineData(1970, false)]
        [InlineData(1996, true)]
        [InlineData(1960, true)]
        [InlineData(2100, false)]
        [InlineData(1900, false)]
        [InlineData(2000, true)]
        [InlineData(2400, true)]
        [InlineData(1800, false)]

        public void Leap(int year, bool leapYear)
        {
            var code = File.ReadAllText("ExercismExamples/leap.ml");
            var isLeapYear = IronCaml.Execute<Func<long, bool>>(code);
            Assert.Equal(leapYear, isLeapYear(year));
        }
    }
}