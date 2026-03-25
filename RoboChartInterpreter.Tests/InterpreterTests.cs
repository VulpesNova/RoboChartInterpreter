namespace RoboChartInterpreter.Tests;

using System.Collections;

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
        a.transitions.Add(new() { to = "B", type = "simple", eventName = "AtoB" });

        move.states.Add("A", a);

        State b = new();
        b.transitions.Add(new() { to = "A", type = "simple", eventName = "BtoA" });

        move.states.Add("B", b);
        
        move.Initialize();

        interRef.machines.Add("movement", move);

        Assert.Equivalent(interRef, inter, strict: true);
    }
}