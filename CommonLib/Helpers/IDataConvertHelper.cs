using System.Data;

namespace CommonLib.Helpers
{
    public interface IDataConvertHelper
    {
        List<T> ConvertToList<T>(DataTable table) where T : new();
        T ConvertToObject<T>(DataRow row) where T : new();
    }
}