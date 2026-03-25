using YamlDotNet.Serialization;

namespace Interpreter;

public class Transition
{
    public string to;
    public string type;
    [YamlMember(Alias = "event", ApplyNamingConventions = false)]
    public string eventName;

    public bool Condition(Event e)
    {
        return e.name == eventName;
    }
}