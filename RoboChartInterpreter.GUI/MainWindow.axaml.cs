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

    Dictionary<string, StateMachineUpdate> lastUpdates = new();

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
                    Edge e = new(stateVisuals[state], stateVisuals[machine.Value.states[trans.to]], trans.ToString());
                    transVisuals.Add(trans, e);
                    graph.Edges.Add(e);
                }
            }

            graph.Edges.Add(new(new InitialStateVisual(), stateVisuals[machine.Value.states[machine.Value.initial]]));
        }

        graphPanel.Graph = graph;
    }

    public void SendEventClick(object? sender, RoutedEventArgs args)
    {
        Event e = eventNameInput.SelectedIndex != -1 ? new((string)eventNameInput.SelectedItem) : new("");

        SendEvent(e, (string?)stateMachineInput.SelectedItem);
    }

    public void SendEvent(Event e, string? machine = null)
    {
        var updates = interpreter.Step(e, machine);

        foreach (var update in updates)
        {
            if (lastUpdates.Keys.Contains(update.Key))
            {
                var lastUpdate = lastUpdates[update.Key];
                stateVisuals[interpreter.machines[update.Key].states[lastUpdate.active]].Colour = Brushes.LightGray;
            }

            stateVisuals[interpreter.machines[update.Key].states[update.Value.active]].Colour = Brushes.LightGreen;

            lastUpdates[update.Key] = update.Value;

            logBlock.Text += update.Value.ToString();
            logScrollViewer.ScrollToEnd();
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

        if (files.Count == 0) return;
        
        interpreter.LoadFromFile(files[0].Path.AbsolutePath);
        UpdateGraphPanel();
        logBlock.Text = "";
        eventNameInput.Items.Clear();
        foreach (string e in interpreter.events) eventNameInput.Items.Add(e);
        stateMachineInput.Items.Clear();
        List<string> machines = interpreter.machines.Keys.ToList();
        machines.Sort();
        foreach (string m in machines) stateMachineInput.Items.Add(m);
        SendEvent(new(""));
    }
}