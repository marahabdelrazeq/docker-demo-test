using CommonLibrary.Utilities.FilterHelper;

namespace CommonLibrary.Utilities.LogHelpers
{
    public class LogSearch : FilterSearchWithNullableValue
    {
        public int EntityId { get; set; }
        public string ActionName { get; set; }

    }
}
