using RoboChartInterpreter.Expressions;
using YamlDotNet.Serialization;

namespace RoboChartInterpreter;

public class Transition
{
    public string to;
    public string type;
    [YamlMember(Alias = "event", ApplyNamingConventions = false)]
    public string? eventType;
    public string? variable;
    public string? condition;

    public bool Check(Event e, ExpressionInterpreter visitor)
    {
        if (condition != null && !(bool)visitor.Interpret(condition)) return false;

        switch (type)
        {
            case "event.simple":
                return e.type == eventType;
            case "event.input":
                if (e.type == eventType)
                {
                    visitor.variables[variable] = e.value;
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
        string temp = $"{eventType}";
        if (condition != null) temp += $"[{condition}]";
        return temp;
    }
}