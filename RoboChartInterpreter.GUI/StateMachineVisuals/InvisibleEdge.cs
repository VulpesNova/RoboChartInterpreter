using System;
using AvaloniaGraphControl;

namespace RoboChartInterpreter.GUI.StateMachineVisuals;

public class InvisibleEdge : Edge
{
    public InvisibleEdge(object tail, object head) : base(tail, head, null, Symbol.None, Symbol.None)
    {
    }
}
