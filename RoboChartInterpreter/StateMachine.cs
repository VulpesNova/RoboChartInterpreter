using System.Text;

namespace RoboChartInterpreter;

public class StateMachine
{
    public Dictionary<string, State> states = new();
    public string name;
    public string active;
    public string initial;

    public void Initialize()
    {
        active = initial;
    }

    public StateMachineUpdate Step(Event e)
    {
        Transition? trans = states[active].Step(e);

        if (trans == null) return new(active);
        
        string prev = active;

        active = trans.to;

        return new(active)
        {
            previous = prev,
            transitionTaken = trans
        };
    }
}

public class StateMachineUpdate
{
    public string? previous;
    public Transition? transitionTaken;
    public string active;

    public StateMachineUpdate(string _active)
    {
        active = _active;
    }
}