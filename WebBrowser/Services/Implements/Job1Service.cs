using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class Job1Service : IJob1Service
    {
        private readonly IHttpService _httpService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public Job1Service(IHttpService httpService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _httpService = httpService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }


        public async Task<List<Job>> GetAllFromDataSet()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Job1Service", "GetAllFromDataSet");
                var dt = await _httpService.GetDataTableAsync(ApiRouter.GetAllJobs);
                return _dataConvertHelper.ConvertToList<Job>(dt);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<Job>();
            }
        }


        public async Task<CResponseMessage1> AddJob(Addjob emp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Job1Service", "AddJob");
                return await _httpService.PostResponseAsync(ApiRouter.AddJob, emp);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi thêm Job" };
            }
        }


        public async Task<CResponseMessage1> DeleteJob(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("Job1Service", "DeleteJob");
                return await _httpService.DeleteResponseAsync($"{ApiRouter.DeleteJob}/{manv}");
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1     { code = "500", message = "Lỗi khi xóa Job" };
            }
        }

    }
}
