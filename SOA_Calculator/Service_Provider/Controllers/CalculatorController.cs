using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using Authenticator_DLL;

namespace Service_Provider.Controllers
{
    [RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
        [Route("{token}/ADDTwoNumbers/{firstNumber}/{secondNumber}")]
        [Route("ADDTwoNumbers")]
        [HttpGet]
        public int ADDTwoNumbers(int token, int firstNumber, int secondNumber)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            Authenticator_Interface authenticator;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            if (authenticator.Validate(token).Equals("validated"))
            {
                return firstNumber + secondNumber;
            }

            return -1;
        }

        [Route("{token}/ADDThreeNumbers/{firstNumber}/{secondNumber}/{thirdNumber}")]
        [Route("ADDThreeNumbers")]
        [HttpGet]
        public int ADDThreeNumbers(int token, int firstNumber, int secondNumber, int thirdNumber)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            Authenticator_Interface authenticator;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            if (authenticator.Validate(token).Equals("validated"))
            {
                return firstNumber +secondNumber + thirdNumber;
            }

            return -1;

            
        }

        [Route("{token}/MulTwoNumbers/{firstNumber}/{secondNumber}")]
        [Route("MulTwoNumbers")]
        [HttpGet]
        public int MulTwoNumbers(int token, int firstNumber, int secondNumber)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            Authenticator_Interface authenticator;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            if (authenticator.Validate(token).Equals("validated"))
            {
                return firstNumber * secondNumber;
            }

            return -1;
        }

        [Route("{token}/MulThreeNumbers/{firstNumber}/{secondNumber}/{thirdNumber}")]
        [Route("MulThreeNumbers")]
        [HttpGet]
        public int MulThreeNumbers(int token, int firstNumber, int secondNumber, int thirdNumber)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            Authenticator_Interface authenticator;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            if (authenticator.Validate(token).Equals("validated"))
            {
                return firstNumber * secondNumber * thirdNumber;
            }
            return -1;
        }

    }
}
