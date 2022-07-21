namespace CollapsedLight.WebApp.Data
{
    public class DataPoints<T> : List<DataPoint<T>>
    { } 

    public class DataPoint<T> 
    {
        public DateTime Time {get; set;}
        public T Value {get; set;}

        public DataPoint(T value)
        {
            Time = DateTime.Now;
            Value = value;   
        }
    }

}