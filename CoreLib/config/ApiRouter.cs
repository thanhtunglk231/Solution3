using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.config
{
    public class ApiRouter
    {
        // ===================== Employee =====================
        public const string GetAllEmployees = "emp/getall";
        public const string AddEmployee = "emp/add";
        public const string DeleteEmployee = "emp/delete";
        public const string UpdateSalary = "emp/update-salary";
        public const string UpdateCommission = "emp/update-commission";
        public const string GetEmpHistory = "emp/history";

        // ===================== Job =====================
        public const string AddJob = "job1/add";
        public const string DeleteJob = "job1/delete";
        public const string GetAllJobs = "job1/getall";

        // ===================== Department =====================
        public const string GetAllDepartments = "dep1/getalldataset";
        public const string GetDepartmentById = "dep1/getbyiddataset/";

        // ===================== User & Auth =====================
        public const string Login = "login";
        public const string Register = "login/register";
        public const string GetAllUsers = "login/getall";
        public const string UpdateUser = "login/update";
       
        // ===================== Acconut=====================
        public const string UserPermissions = "account/getall";
    }
}