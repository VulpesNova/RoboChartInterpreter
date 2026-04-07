using YamlDotNet.Serialization;

namespace RoboChartInterpreter;

public class Transition
{
    public string to;
    public string type;
    [YamlMember(Alias = "event", ApplyNamingConventions = false)]
    public string? eventType;

    public bool Condition(Event e)
    {
        switch (type)
        {
            case "event.simple":
                return e.type == eventType;
            default:
                return false;
        }
    }

    public override string ToString()
    {
        return $"{eventType}";
    }
}