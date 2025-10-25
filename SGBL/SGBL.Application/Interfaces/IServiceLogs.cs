

using SGBL.Application.Dtos.Book;

namespace SGBL.Application.Interfaces
{
    public interface IServiceLogs 
    {
        void CreateLogInfo(string sLog);


        void CreateLogWarning(string sLog);

        void CreateLogError(string sLog);
         
    }
}
