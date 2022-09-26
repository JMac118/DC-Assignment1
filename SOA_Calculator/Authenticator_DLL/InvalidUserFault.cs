using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator_DLL
{
    [DataContract]
    [Serializable]
    public class InvalidUserFault
    {
        private string report;

        public InvalidUserFault(string message)
        {
            this.report = message;
        }

        [DataMember]
        public string Message
        {
            get { return this.report; } 
            set { this.report = value; }
        }
    }
}
