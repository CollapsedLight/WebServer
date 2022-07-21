using CollapsedLight.WebApp.Data;

namespace CollapsedLight.WebApp.Service;

public interface IFuelService
{
    public void PushRecord(MileageDataRecord record);
    
    public IEnumerable<MileageDataRecord?> GetAll();
    public IEnumerable<MileageDataRecord?> Get(int count);
}
