using System.ComponentModel;
using Avalonia.Media;

namespace RoboChartInterpreter.GUI.StateMachineVisuals;

public class StateVisual : INotifyPropertyChanged
{
    public StateVisual(string _name, IBrush _colour)
    { 
        Name = _name; 
        Colour = _colour;
    }
    public string Name { get; private set; }
    private IBrush _colour;
    public IBrush Colour {
        get {return _colour;} 
        set
        {
            _colour = value;
            OnPropertyChanged(nameof(Colour));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
