using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    Dictionary<StateMachine, StateMachineVisual> stateMachineVisuals = new();
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

        InvisibleState i = new();

        foreach (var machine in interpreter.machines)
        {
            StateMachineVisual smv = new(machine.Key);

            stateMachineVisuals.Add(machine.Value, smv);

            foreach (var state in machine.Value.states)
            {
                StateVisual sv = state.Key == "_final" ? new FinalStateVisual() : new StateVisual(state.Key, Brushes.LightGray);

                stateVisuals.Add(state.Value, sv);

                graph.Parent[sv] = smv;
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

            InitialStateVisual isv = new();

            graph.Edges.Add(new(isv, stateVisuals[machine.Value.states[machine.Value.initial]]));

            graph.Parent[isv] = smv;

            graph.Edges.Add(new InvisibleEdge(i, smv));
        }

        graphPanel.Graph = graph;
    }

    public void SendEventClick(object? sender, RoutedEventArgs args)
    {
        object? value;
        if (int.TryParse(eventValueInput.Text, out int i))
            value = i;
        else if (double.TryParse(eventValueInput.Text, out double d))
            value = d;
        else value = eventValueInput.Text;
        Event e = eventNameInput.SelectedIndex != -1 ? new((string)eventNameInput.SelectedItem, value) : new("");

        eventValueInput.Text = "";

        SendEvent(e, (string?)stateMachineInput.SelectedItem);
    }

    public async void SendEvent(Event e, string? machine = null)
    {
        var updates = interpreter.Step(e, machine);

        foreach (var innerUpdates in updates)
        {
            foreach (var innerUpdate in innerUpdates)
            {
                if (lastUpdates.Keys.Contains(innerUpdate.Key))
                {
                    var lastUpdate = lastUpdates[innerUpdate.Key];
                    if (lastUpdate.previous != null)
                        stateVisuals[interpreter.machines[innerUpdate.Key].states[lastUpdate.previous]].Colour = Brushes.LightGray;
                    stateVisuals[interpreter.machines[innerUpdate.Key].states[lastUpdate.active]].Colour = Brushes.LightGray;
                }

                if (innerUpdate.Value.previous != null)
                    stateVisuals[interpreter.machines[innerUpdate.Key].states[innerUpdate.Value.previous]].Colour = Brushes.Yellow;
                stateVisuals[interpreter.machines[innerUpdate.Key].states[innerUpdate.Value.active]].Colour = Brushes.LightGreen;

                lastUpdates[innerUpdate.Key] = innerUpdate.Value;

                logBlock.Text += innerUpdate.Value.ToString();
                logScrollViewer.ScrollToEnd();
            }

            if ((bool)multipleTransDelayCheck.IsChecked) await Task.Delay(1000);
        }
    }

    public async void OpenYaml(object? sender, System.EventArgs args)
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
        foreach (string m in machines)
        {
            stateMachineInput.Items.Add(m);
            StateMachine machine = interpreter.machines[m];
            stateVisuals[machine.states[machine.initial]].Colour = Brushes.LightGreen;
        }

    }
}