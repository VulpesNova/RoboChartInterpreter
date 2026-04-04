using YamlDotNet.Serialization;

namespace RoboChartInterpreter;

public class Transition
{
    public string to;
    public string type;
    [YamlMember(Alias = "event", ApplyNamingConventions = false)]
    public string eventType;

    public bool Condition(Event e)
    {
        return e.type == eventType;
    }

    public override string ToString()
    {
        return $"{eventType}";
    }
}