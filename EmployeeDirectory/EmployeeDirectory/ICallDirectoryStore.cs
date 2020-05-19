using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDirectory
{
    public interface ICallDirectoryStore
    {
        Task<RegistrationResponse> Store(IOrderedEnumerable<KeyValuePair<long, string>> sorted);
    }

    public class RegistrationResponse
    {
        public enum RegistrationResponseEnum
        {
            Ok,
            NotActivated,
            Error
        };
        public RegistrationResponseEnum Response;
        public string Reason;
    }
}
