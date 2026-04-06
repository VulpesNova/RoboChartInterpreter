using System;

namespace RoboChartInterpreter.GUI.StateMachineVisuals;

public class StateMachineVisual
{
    public StateMachineVisual(string _name)
    { 
        Name = _name;
    }
    public string Name { get; private set; }
}