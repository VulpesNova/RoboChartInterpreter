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
    [InlineData("0.1 < 0.1", false)]
    [InlineData("0.1 < 0.2", true)]
    [InlineData("0.1 <= 0.1", true)]
    [InlineData("0.1 >= 0.1", true)]
    [InlineData("0.1 > 0.1", false)]
    [InlineData("0.1 > 1", false)]
    [InlineData("0.1 < 1", true)]
    [InlineData("0.1 + 0.2 == 0.3", true)]
    [InlineData("0.5 - 0.2 == 0.3", true)]
    public void ComparisonTest(string input, bool expected)
    {
        ExpressionInterpreter visitor = new();
        Assert.Equal(expected, (bool)visitor.Interpret(input));
    }

    [Theory]
    [InlineData("2+8", 10)]
    [InlineData("2*8", 16)]
    [InlineData("2/8", 0)]
    [InlineData("9/8", 1)]
    [InlineData("2 - 10", -8)]
    public void IntArithmeticTest(string input, int expected)
    {
        ExpressionInterpreter visitor = new();
        Assert.Equal(expected, (int)visitor.Interpret(input));
    }

    [Theory]
    [InlineData("2+8.0", 10.0)]
    [InlineData("2.0*8", 16.0)]
    [InlineData("2/8.0", 0.25)]
    [InlineData("9.0/8", 1.125)]
    [InlineData("2.0 - 10", -8.0)]
    public void MixedArithmeticTest(string input, double expected)
    {
        ExpressionInterpreter visitor = new();
        Assert.Equal(expected, (double)visitor.Interpret(input), 10);
    }

    [Theory]
    [InlineData("2-(8.0-2)", -4.0)]
    [InlineData("(2-8.0)-2", -8.0)]
    [InlineData("(4.0 / 2) / 2", 1.0)]
    [InlineData("4.0 / (2 / 2)", 4.0)]
    public void ParenthisedTest(string input, double expected)
    {
        ExpressionInterpreter visitor = new();
        Assert.Equal(expected, (double)visitor.Interpret(input), 10);
    }
}