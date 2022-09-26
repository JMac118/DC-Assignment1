using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Authenticator;

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
            //if(validate(token)
            return firstNumber + secondNumber;
        }

        [Route("ADDThreeNumbers/{firstNumber}/{secondNumber}/{thirdNumber}")]
        [Route("ADDThreeNumbers")]
        [HttpGet]
        public int ADDThreeNumbers(int firstNumber, int secondNumber, int thirdNumber)
        {
            return firstNumber + secondNumber + thirdNumber;
        }

        [Route("MulTwoNumbers/{firstNumber}/{secondNumber}")]
        [Route("MulTwoNumbers")]
        [HttpGet]
        public int MulTwoNumbers(int firstNumber, int secondNumber)
        {
            return firstNumber * secondNumber;
        }

        [Route("MulThreeNumbers/{firstNumber}/{secondNumber}/{thirdNumber}")]
        [Route("MulThreeNumbers")]
        [HttpGet]
        public int MulThreeNumbers(int firstNumber, int secondNumber, int thirdNumber)
        {
            return firstNumber * secondNumber * thirdNumber;
        }

    }
}
