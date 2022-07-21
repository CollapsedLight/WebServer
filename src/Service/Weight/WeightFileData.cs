using CollapsedLight.WebApp.Data;
using System.Text.Json;

namespace CollapsedLight.WebApp.Service;

public class WeightFileData : IWeightDataService, IDisposable
{
    // Create a dataset
    private Dictionary<DateTime, float> _data = new();
    private const string _file = "weight.json";
    private readonly string _path;

   public WeightFileData(AppPath appPath)
    {
        _path = Path.Combine(appPath(), _file);
        if (!File.Exists(_path))
            WriteData();

        _data = ReadData();
    }

    public void AddWeight(float value)
    {
        _data[DateTime.UtcNow] = value;
        WriteData();
    }

    public IEnumerable<float> GetWeight(TimeSpan timespan)
    {
        return _data.Values;
    }

    public void Dispose()
    {
        WriteData();
    }

    private void WriteData()
        => File.WriteAllText(_path, JsonSerializer.Serialize(_data));

    private Dictionary<DateTime, float>? ReadData()
         => JsonSerializer.Deserialize<Dictionary<DateTime, float>>(File.ReadAllText(_path));

    public IEnumerable<WeightDataPoint> GetAll()
    {
        return _data.Select(k => new WeightDataPoint(k.Key, k.Value));
    }
}

