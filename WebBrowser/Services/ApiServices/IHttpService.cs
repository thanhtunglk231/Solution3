using CoreLib.Models;
using System.Data;

namespace WebBrowser.Services.ApiServices
{
    public interface IHttpService
    {
        Task<CResponseMessage1> DeleteResponseAsync(string url);
        Task<T> GetAsync<T>(string url);
        Task<DataRow> GetDataRowAsync(string url);
        Task<DataSet> GetDataSetFromResponseAsync(string url);
        Task<DataTable> GetDataTableAsync(string url);
        Task<List<T>> GetListAsync<T>(string url);
        Task<string> GetRawJsonAsync(string url);
        Task<CResponseMessage1> GetResponseAsync(string url);
        Task<DataRow> PostDataRowAsync(string url, object data);
        Task<DataTable> PostDataTableAsync(string url, object data);
        Task<CResponseMessage1> PostResponseAsync(string url, object data);
        Task<CResponseMessage1> PutResponseAsync(string url, object data);
        Task<T> PostAsync<T>(string url, object data);
        Task<List<T>> GetTableFromCResponseAsync<T>(string url);
        Task<CResponseMessage1> DeleteWithBodyResponseAsync(string url, object data);

    }
}