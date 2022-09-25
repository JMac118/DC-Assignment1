using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator_DLL
{
    [ServiceContract]
    public interface Authenticator_Interface
    {
        [OperationContract]
        string Register(string name, string password);
        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        int Login(string name, string password);
        [OperationContract]
        string Validate(int token);
    }
}
