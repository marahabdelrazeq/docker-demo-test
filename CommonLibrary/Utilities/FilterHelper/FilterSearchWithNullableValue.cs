using FilterBuilder;

namespace CommonLibrary.Utilities.FilterHelper
{
    public class FilterSearchWithDefaultValue : FilterSearch
    {
        public FilterSearchWithDefaultValue()
        {
            NullValue = new List<(Type, object)> {
                (typeof(int),0),
                (typeof(long),0),
                (typeof(float),0),
                (typeof(double),0),
                (typeof(string),"")
            };
        }

    }
    public class FilterSearchWithNullableValue : FilterSearch
    {
        public FilterSearchWithNullableValue()
        {
            NullValue = new List<(Type, object)> {
                (typeof(int),null),
                (typeof(long),null),
                (typeof(float),null),
                (typeof(double),null),
                (typeof(string),null)
            };
        }

    }
}
