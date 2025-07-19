using CommonLib.Handles;
using CommonLib.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CommonLib.Helpers
{
    public class DataConvertHelper : ErrorHandler, IDataConvertHelper
    {
        public DataConvertHelper(ISerilogProvider logger) : base(logger) { }

        public List<T> ConvertToList<T>(DataTable table) where T : new()
        {
            var result = new List<T>();

            if (table == null || table.Rows.Count == 0)
                return result;

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var obj = ConvertToObject<T>(row);
                    if (obj != null)
                        result.Add(obj);
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex, "Lỗi khi convert DataRow sang {TypeName}", typeof(T).Name);
                    WriteToFile(ex);
                }
            }

            return result;
        }

        public T ConvertToObject<T>(DataRow row) where T : new()
        {
            if (row == null) return default;

            T obj = new T();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            this.WriteStringToFuncion("DataConvertHelper", "ConvertToObject"); // Gọi 1 lần mỗi dòng

            foreach (var prop in properties)
            {
                try
                {
                    var column = GetColumnMatch(row.Table, prop.Name);
                    if (column == null)
                    {
                        Logger.Warning("Không tìm thấy cột tương ứng cho thuộc tính {PropertyName}", prop.Name);
                        continue;
                    }

                    var value = row[column];
                    if (value == DBNull.Value) continue;

                    object safeValue = ConvertValue(value, prop.PropertyType);
                    prop.SetValue(obj, safeValue);
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex, "Lỗi khi ánh xạ cột sang thuộc tính {Property}", prop.Name);
                    WriteToFile(ex);
                }
            }

            return obj;
        }


        private object ConvertValue(object value, Type targetType)
        {
            try
            {
                this.WriteStringToFuncion("DataConvertHelper", "ConvertValue");
                if (value == null || value == DBNull.Value)
                    return null;

                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value.ToString());

                if (targetType == typeof(Guid))
                    return Guid.Parse(value.ToString());

                if (targetType == typeof(DateTime))
                    return Convert.ToDateTime(value);

                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Không thể chuyển đổi giá trị {Value} sang kiểu {Type}", value, targetType.Name);
                WriteToFile(ex);
                return null;
            }
        }

   
        private DataColumn GetColumnMatch(DataTable table, string propName)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (col.ColumnName.Trim().Equals(propName, StringComparison.OrdinalIgnoreCase))
                    return col;
            }
            return null;
        }
    }
}
