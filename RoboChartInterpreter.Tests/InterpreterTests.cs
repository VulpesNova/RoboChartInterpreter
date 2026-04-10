namespace RoboChartInterpreter.Tests;

using System.Collections;
using YamlDotNet.Serialization;

public class InterpreterTests
{
    [Fact]
    public void SimpleLoadTest()
    {
        Interpreter inter = new();
        inter.LoadFromFile("./Resources/simple.yaml");

        Interpreter interRef = new();

        StateMachine move = new()
        {
            initial = "A"
        };

        State a = new();
        a.transitions.Add(new() { to = "B", type = "event.simple", eventType = "AtoB" });

        move.states.Add("A", a);

        State b = new();
        b.transitions.Add(new() { to = "A", type = "event.simple", eventType = "BtoA" });

        move.states.Add("B", b);

        move.Initialize("movement");

        interRef.machines.Add("movement", move);

        interRef.events = ["AtoB", "BtoA"];

        Assert.Equivalent(interRef, inter, strict: true);
    }

    [Fact]
    public void CompleteMachineTest()
    {
        Interpreter inter = new();
        inter.LoadFromFile("./Resources/complete.yaml");

        Assert.Equal(0, inter.machines["mA"].visitor.clocks["T"]);
        Assert.Equal(0, inter.machines["mA"].visitor.clocks["C"]);
        Assert.Equal("sA", inter.machines["mA"].active);

        var updates = inter.Step(new("eA", "a"), "mA");

        Assert.Null(updates[0]["mA"].transitionTaken);
        Assert.Equal("sA", inter.machines["mA"].active);
        Assert.DoesNotContain("var", inter.machines["mA"].visitor.variables);

        updates = inter.TickClocks();

        Assert.Empty(updates);
        Assert.Equal(1, inter.machines["mA"].visitor.clocks["T"]);
        Assert.Equal(1, inter.machines["mA"].visitor.clocks["C"]);

        updates = inter.Step(new("eA", "a"), "mA");

        Assert.Null(updates[0]["mA"].transitionTaken);
        Assert.Equal("sA", inter.machines["mA"].active);

        updates = inter.TickClocks();

        Assert.Empty(updates);
        Assert.Equal(2, inter.machines["mA"].visitor.clocks["T"]);
        Assert.Equal(2, inter.machines["mA"].visitor.clocks["C"]);

        updates = inter.Step(new("eA", "a"), "mA");

        Assert.NotNull(updates[0]["mA"].transitionTaken);
        Assert.Equal("sB", inter.machines["mA"].active);

        updates = inter.TickClocks();

        Assert.Empty(updates);
        Assert.Equal(3, inter.machines["mA"].visitor.clocks["T"]);
        Assert.Equal(3, inter.machines["mA"].visitor.clocks["C"]);

        updates = inter.Step(new("reset"), "mA");

        Assert.Equal(2, updates.Count);
        Assert.NotNull(updates[0]["mA"].transitionTaken);
        Assert.NotNull(updates[1]["mA"].transitionTaken);
        Assert.Equal("sA", inter.machines["mA"].active);
        Assert.Equal(0, inter.machines["mA"].visitor.clocks["T"]);
        Assert.Equal(3, inter.machines["mA"].visitor.clocks["C"]);
    }
}