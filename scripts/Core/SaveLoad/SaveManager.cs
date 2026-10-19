using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class SaveManager
{
    private const string _savePath = "save.json";
    private static List<ISaveable> _saveables = new();

    public static void Register(ISaveable saveable)
    {
        _saveables.Add(saveable);
    }


    public static void Save()
    {
        Dictionary<string, object> data = new();

        foreach (ISaveable saveable in _saveables)
        {
            data[saveable.ID] = saveable.CaptureState();
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(_savePath, json);
    }

    public static void Load()
    {
        if (!File.Exists(_savePath)) return;

        string json = File.ReadAllText(_savePath);
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        foreach (ISaveable saveable in _saveables)
        {
            if (data.TryGetValue(saveable.ID, out object state))
            {
                saveable.RestoreState(state);
            }
        }
    }
}