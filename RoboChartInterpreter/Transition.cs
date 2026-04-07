using YamlDotNet.Serialization;

namespace RoboChartInterpreter;

public class Transition
{
    public string to;
    public string type;
    [YamlMember(Alias = "event", ApplyNamingConventions = false)]
    public string? eventType;
    public string? variable;

    public bool Condition(Event e, Dictionary<string, object> context)
    {
        switch (type)
        {
            case "event.simple":
                return e.type == eventType;
            case "event.input":
                if (e.type == eventType)
                {
                    context[variable] = e.value;
                    return true;
                }
                return false;
            case "notrigger":
                return true;
            default:
                return false;
        }
    }

    public override string ToString()
    {
        return $"{eventType}";
    }
}