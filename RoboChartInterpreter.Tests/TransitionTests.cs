namespace RoboChartInterpreter.Tests;

using System.Collections;

public class TransitionTests
{
    [Fact]
    public void SimpleTransitionTest()
    {
        Interpreter inter = new();
        inter.LoadFromFile("./Resources/simple.yaml");

        Event AtoB = new("AtoB");
        Event BtoA = new("BtoA");

        StateMachineUpdate update = inter.Step(BtoA)["movement"];

        Assert.Null(update.previous);
        Assert.Null(update.transitionTaken);
        Assert.Equal("A", update.active);

        update = inter.Step(AtoB)["movement"];

        Assert.Equal("A", update.previous);
        Assert.NotNull(update.transitionTaken);
        Assert.Equal("B", update.active);

        update = inter.Step(AtoB)["movement"];

        Assert.Null(update.previous);
        Assert.Null(update.transitionTaken);
        Assert.Equal("B", update.active);
    }
}