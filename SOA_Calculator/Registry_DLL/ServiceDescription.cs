using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry_DLL
{
    public class ServiceDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string API_Endpoint { get; set; }
        public string Num_Operands { get; set; }
        public string Operand_Type { get; set; }
    }
}
