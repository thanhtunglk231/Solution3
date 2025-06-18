using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.config
{
    public class ApiRouter
    {
        public const string GetDeptbyid = "department/";

        //Job
        public const string Addjob = "job/addjob";
        public const string UpdateCommission = "Employee/UpdateCommission";
        public const string add_emp = "Employee/add_emp";
        public const string getall_emp = "Employee/getall";
        public const string delete_emp = "employee/delete";
        public const string UpdateSalary = "employee/UpdateSalary";
        public const string His_emp = "HisEmp";
        public const string Get_all_dept = "getall";
        public const string get_all_job = "job/getall";
        public const string add_job = "job/add";
        public const string del_job = "job/delete";
        public const string log_in = "Login";
        public const string register = "login/register";
        public const string user_getall = "login/getall";
    }
}