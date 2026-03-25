using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RoboChartInterpreter;

public class Interpreter
{
    public Dictionary<string, StateMachine> machines = new();

    public Dictionary<string, StateMachineUpdate> Step(Event e)
    {
        Dictionary<string, StateMachineUpdate> updates = new();

        foreach (string key in machines.Keys)
        {
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
        
        machines = deserializer.Deserialize<Dictionary<string, StateMachine>>(yaml);
        foreach (StateMachine machine in machines.Values)
        {
            machine.Initialize();
        }
    }
}
