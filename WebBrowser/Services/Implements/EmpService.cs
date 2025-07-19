using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class EmpService : IEmpService
    {
        private readonly IHttpService _httpService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public EmpService(IHttpService httpService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _httpService = httpService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }

      
        public async Task<List<Employee>> GetAllFromDataSet()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "GetAllFromDataSet");
                var dt = await _httpService.GetDataTableAsync(ApiRouter.GetAllEmployees);
                return _dataConvertHelper.ConvertToList<Employee>(dt);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<Employee>();
            }
        }

     
        public async Task<CResponseMessage1> AddEmployee(Employee emp)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "AddEmployee");
                return await _httpService.PostResponseAsync(ApiRouter.AddEmployee, emp);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi thêm nhân viên" };
            }
        }

      
        public async Task<CResponseMessage1> DeleteEmployee(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "DeleteEmployee");
                return await _httpService.DeleteResponseAsync($"{ApiRouter.DeleteEmployee}?manv={manv}");
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi xóa nhân viên" };
            }
        }

      
        public async Task<CResponseMessage1> UpdateSalary()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "UpdateSalary");
                return await _httpService.PutResponseAsync(ApiRouter.UpdateSalary, null);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi cập nhật lương" };
            }
        }

       
        public async Task<CResponseMessage1> UpdateCommission(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "UpdateCommission");
                var url = $"{ApiRouter.UpdateCommission}?manv={manv}";
                return await _httpService.PutResponseAsync(url, null);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1 { code = "500", message = "Lỗi khi cập nhật hoa hồng" };
            }
        }

       
        public async Task<List<HistoryDto>> GetJobHistory(string manv)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("EmpService", "GetJobHistory");
                var dt = await _httpService.GetDataTableAsync($"{ApiRouter.GetEmpHistory}/{manv}");
                return _dataConvertHelper.ConvertToList<HistoryDto>(dt);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<HistoryDto>();
            }
        }
    }
}
