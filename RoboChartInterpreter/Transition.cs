using System.Diagnostics.CodeAnalysis;
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
    [YamlMember(Alias = "clock_resets", ApplyNamingConventions = false)]
    public List<string>? clockResets;

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

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        string temp = $"{eventType}";
        if (condition != null) temp += $"[{condition}]";
        if (clockResets != null)
            foreach (string clock in clockResets)
                temp += $" #{clock}";
        return temp;
    }
}