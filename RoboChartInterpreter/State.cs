using System.Text;
using System.Transactions;

namespace RoboChartInterpreter;

public class State
{
    public List<Transition> transitions = new();
    public string name;

    public Transition? Step(Event e)
    {
        foreach (Transition t in transitions)
            if (t.Condition(e))
                return t;

        return null;
    }

    public HashSet<string> GetEvents()
    {
        HashSet<string> temp = new();
        
        foreach (Transition t in transitions) temp.Add(t.eventType);
        
        return temp; 
    }
}
