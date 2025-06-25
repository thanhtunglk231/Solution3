namespace DataServiceLib.unuse.Interfaces.unuse
{
    public interface ICDepartmentDataProvider
    {
        Task<List<Dictionary<string, object>>> getall();
        Task<List<Dictionary<string, object>>> getbyid(int id);
    }
}