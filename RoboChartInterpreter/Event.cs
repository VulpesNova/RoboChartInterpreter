namespace RoboChartInterpreter;

public class Event
{
    public string? type;
    public object? value;

    public Event(string? _type, object? _value = null)
    {
        type = _type;
        value = _value;
    }
}