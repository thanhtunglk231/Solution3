using CoreLib.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;
using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.config;
using CoreLib.Models;

namespace WebBrowser.Services.Implementations
{
    public class DepartmentMvcService : IDepartmentMvcService
    {
        private readonly IHttpService _httpService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;
        private readonly HttpClient _client;

    
        //private const string GetAllDatasetUrl = "dep1/getalldataset";
        //private const string GetByIdDatasetUrl = "dep1/getbyiddataset/";


        public DepartmentMvcService(
            HttpClient client,
            IHttpService httpService,
            IDataConvertHelper dataConvertHelper,
            IErrorHandler errorHandler)
        {
            _client = client;
            _httpService = httpService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }

        //public async Task<List<DepartmentDto>> GetAll()
        //{
        //    const string func = "GetAll";
        //    _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

        //    try
        //    {
        //        return await _httpService.GetListAsync<DepartmentDto>(GetAllUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHandler.WriteToFile(ex);
        //        throw; // Cho controller xử lý
        //    }
        //}

        public async Task<List<DepartmentDto>> GetAllFromDataSet()
        {
            const string func = "GetAllFromDataSet";
            _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

            try
            {
                var dt = await _httpService.GetDataTableAsync(ApiRouter.GetAllDepartments);
                return _dataConvertHelper.ConvertToList<DepartmentDto>(dt);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }

        public async Task<List<DepartmentDto>> GetbyidDataset(int id)
        {
            const string func = "GetbyidDataset";
            _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

            try
            {
                var dt = await _httpService.GetDataTableAsync($"{ApiRouter.GetDepartmentById}{id}");
                return _dataConvertHelper.ConvertToList<DepartmentDto>(dt);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }

        public async Task<CResponseMessage1> Create(Department department)
        {
            const string func = "Create";
            _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

            try
            {
                var result = await _httpService.PostResponseAsync("Dep1/Create", department);
                if (result.code != "200")
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                }
                    return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }

        }

        public async Task<CResponseMessage1> Delete(string ma)
        {
            const string func = "Delete";
            _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

            try
            {
                var result = await _httpService.DeleteResponseAsync($"Dep1/Delete?maphg={ma}");
                if (result.code != "200")
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }

        }

        public async Task<CResponseMessage1> update(Department department)
        {
            const string func = "update";
            _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

            try
            {
                var result = await _httpService.PutResponseAsync($"Dep1/update",department);
                if (result.code != "200")
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }

        }

        //public async Task<DepartmentDto> GetById(int id)
        //{
        //    const string func = "GetById";
        //    _errorHandler.WriteStringToFuncion("DepartmentMvcService", func);

        //    try
        //    {
        //        var row = await _httpService.GetDataRowAsync($"{GetByIdUrl}{id}");
        //        return _dataConvertHelper.ConvertToObject<DepartmentDto>(row);
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHandler.WriteToFile(ex);
        //        throw;
        //    }
        //}
    }
}
