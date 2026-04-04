using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RoboChartInterpreter;

public class Interpreter
{
    public Dictionary<string, StateMachine> machines = new();
    public List<string> events = new();

    public Dictionary<string, StateMachineUpdate> Step(Event e, string? machine = null)
    {
        Dictionary<string, StateMachineUpdate> updates = new();

        foreach (string key in machines.Keys)
        {
            if (machine != null && key != machine) continue;

            updates.Add(key, machines[key].Step(e));
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
