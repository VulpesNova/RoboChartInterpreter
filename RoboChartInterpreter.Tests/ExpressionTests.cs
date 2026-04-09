using Antlr4.Runtime;
using RoboChartInterpreter.Expressions;
[assembly: CaptureConsole]

namespace RoboChartInterpreter.Tests;

public class ExpressionTests
{
    [Theory]
    [InlineData("\"a\" == \"a\"", true)]
    [InlineData("\"a\" == \"b\"", false)]
    [InlineData("\"a\" != \"a\"", false)]
    [InlineData("\"a\" != \"b\"", true)]
    [InlineData("1 == 1", true)]
    [InlineData("1 == 2", false)]
    [InlineData("1 != 1", false)]
    [InlineData("1 != 2", true)]
    [InlineData("1.0 == 1", true)]
    [InlineData("0.1 == 0.1", true)]
    [InlineData("0.1 == 0.2", false)]
    [InlineData("0.1 != 0.1", false)]
    [InlineData("0.1 != 0.2", true)]
    public void ComparisonTest(string input, bool expected)
    {
        ExpressionInterpreter visitor = new();
        Assert.Equal(expected, (bool)visitor.Interpret(input));
    }
}