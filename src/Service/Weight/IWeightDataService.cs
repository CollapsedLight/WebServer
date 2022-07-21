using CollapsedLight.WebApp.Data;

namespace CollapsedLight.WebApp.Service;

public interface IWeightDataService
{
    public IEnumerable<float> GetWeight(TimeSpan timespan);
    public void AddWeight(float value);
    public IEnumerable<WeightDataPoint> GetAll();
}

