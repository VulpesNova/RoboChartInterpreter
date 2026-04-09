using System.Dynamic;
using System.Text;

namespace RoboChartInterpreter;

public class StateMachine
{
    public Dictionary<string, State> states = new();
    public string name;
    public string active;
    public string initial;
    public Dictionary<string, object> context = new();

    public void Initialize(string _name)
    {
        name = _name;
        active = initial;
        states["_final"] = new();
    }

    public StateMachineUpdate Step(Event e)
    {
        Transition? trans = states[active].Step(e, context);

        if (trans == null) return new(name, active, e);

        string prev = active;

        active = trans.to;

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
}

public class StateMachineUpdate(string _machine, string _active, Event _e)
{
    public string? previous;
    public Transition? transitionTaken;
    public string machine = _machine;
    public string active = _active;
    public Event e = _e;

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