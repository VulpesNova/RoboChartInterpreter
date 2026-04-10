using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;
using RoboChartInterpreter.Expressions;
using YamlDotNet.Serialization;

namespace RoboChartInterpreter;

public class StateMachine
{
    public Dictionary<string, State> states = new();
    public string name;
    public string active;
    public string initial;
    [YamlMember(Alias = "initial_vars", ApplyNamingConventions = false)]
    public Dictionary<string, string> initialVars = new();
    public List<string> clocks = new();
    public ExpressionInterpreter visitor = new();

    public void Initialize(string _name)
    {
        name = _name;
        active = initial;
        states["_final"] = new();
        foreach (string clock in clocks)
        {
            visitor.clocks[clock] = 0;
        }
        foreach (var v in initialVars)
        {
            object? value;
            if (int.TryParse(v.Value, out int i))
                value = i;
            else if (double.TryParse(v.Value, out double d))
                value = d;
            else value = v.Value;
            visitor.variables.Add(v.Key, value);
        }
    }

    public void TickClocks()
    {
        foreach (string clock in clocks)
        {
            visitor.clocks[clock]++;
        }
    }

    public StateMachineUpdate Step(Event e)
    {
        Transition? trans = states[active].Step(e, visitor);

        if (trans == null) return new(name, active, e);

        string prev = active;

        active = trans.to;
        if (trans.clockResets != null)
            foreach (string clock in trans.clockResets)
                visitor.clocks[clock] = 0;

        return new(name, active, e)
        {
            previous = prev,
            transitionTaken = trans
        };
    }

    public HashSet<string> GetEvents()
    {
        HashSet<string> temp = new();

        foreach (State s in states.Values) temp.UnionWith(s.GetEvents());

        return temp;
    }

    [ExcludeFromCodeCoverage]
    public string ClockString()
    {
        return $"{{{string.Join(", ", visitor.clocks.Select(kv => kv.Key + ": " + kv.Value).ToArray())}}}";
    }
}

public class StateMachineUpdate(string _machine, string _active, Event _e)
{
    public string? previous;
    public Transition? transitionTaken;
    public string machine = _machine;
    public string active = _active;
    public Event e = _e;

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        string message = $"{e.type} ({machine}):\n";
        if (previous != null)
        {
            message += $"├ transition taken: {transitionTaken}\n";
            message += $"├ previous state: {previous}\n";
            if (transitionTaken.type == "event.input")
            {
                message += $"├ set variable '{transitionTaken.variable}' to: {e.value}\n";
            }
        }
        else
        {
            message += $"├ no transition taken\n";
        }
        message += $"└ Active State: {active}\n\n";
        return message;
    }
}