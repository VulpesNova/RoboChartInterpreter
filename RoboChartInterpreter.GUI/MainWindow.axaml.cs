using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using AvaloniaGraphControl;
using RoboChartInterpreter.GUI.StateMachineVisuals;

namespace RoboChartInterpreter.GUI;

public partial class MainWindow : Window
{
    Interpreter interpreter = new();

    Dictionary<State, StateVisual> stateVisuals = new();
    Dictionary<Transition, Edge> transVisuals = new();

    Dictionary<string, StateMachineUpdate> updates = new();

    Graph graph = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    public void UpdateGraphPanel()
    {
        graph = new();

        foreach (var machine in interpreter.machines)
        {
            foreach (var state in machine.Value.states)
            {
                stateVisuals.Add(state.Value, new(state.Key, Brushes.LightGray));
            }

            foreach (State state in machine.Value.states.Values)
            {
                foreach (Transition trans in state.transitions)
                {
                    Edge e = new(stateVisuals[state], stateVisuals[machine.Value.states[trans.to]], trans.eventName);
                    transVisuals.Add(trans, e);
                    graph.Edges.Add(e);
                }
            }

            graph.Edges.Add(new(new InitialStateVisual(), stateVisuals[machine.Value.states[machine.Value.initial]]));
        }

        graphPanel.Graph = graph;
    }

    public void SendEvent(object? sender, RoutedEventArgs args)
    {
        foreach (var update in updates)
        {
            stateVisuals[interpreter.machines[update.Key].states[update.Value.active]].Colour = Brushes.LightGray;
        }

        updates = interpreter.Step(new(eventNameInput.Text));

        foreach (var update in updates)
        {
            stateVisuals[interpreter.machines[update.Key].states[update.Value.active]].Colour = Brushes.LightGreen;
        }
    }

    public void OpenYaml(object? sender, System.EventArgs args)
    {
        OpenYamlAsync();
    }

    public async Task OpenYamlAsync()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open RoboChart Yaml",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            interpreter.LoadFromFile(files[0].Path.AbsolutePath);
            UpdateGraphPanel();
        }
    }
}