using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICJob1DataProvider
    {
        CResponseMessage1 Addjob(Addjob addjob);
        CResponseMessage1 Deletejob(string manv);
        DataSet GetAll();
    }
}