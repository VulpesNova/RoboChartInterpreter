using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RoboChartInterpreter;

public class Interpreter
{
    public Dictionary<string, StateMachine> machines = new();
    public List<string> events = new();

    public List<Dictionary<string, StateMachineUpdate>> TickClocks()
    {
        foreach (StateMachine m in machines.Values)
        {
            m.TickClocks();
        }

        return Step(new(null));
    }

    public List<Dictionary<string, StateMachineUpdate>> Step(Event e, string? machine = null)
    {
        List<Dictionary<string, StateMachineUpdate>> updates = new();

        int repeats = 0;

        while (true)
        {
            Dictionary<string, StateMachineUpdate> innerUpdates = new();

            int transTaken = 0;

            foreach (string key in machines.Keys)
            {
                if (machine != null && key != machine) continue;

                StateMachineUpdate smu = machines[key].Step(e);
                innerUpdates.Add(key, smu);
                if (smu.transitionTaken != null) transTaken++;
            }

            if (transTaken == 0 && e.type == null) break;
            else updates.Add(innerUpdates);

            e = new(null);
            repeats++;
            if (repeats > 20) break;
        }

        return updates;
    }

    public void LoadFromFile(string path) => LoadYaml(File.ReadAllText(path));

    public void LoadYaml(string yaml)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        HashSet<string> temp = new();

        machines = deserializer.Deserialize<Dictionary<string, StateMachine>>(yaml);
        foreach (KeyValuePair<string, StateMachine> machine in machines)
        {
            machine.Value.Initialize(machine.Key);
            temp.UnionWith(machine.Value.GetEvents());
        }

        events = temp.ToList();
        events.Sort();
    }
}
