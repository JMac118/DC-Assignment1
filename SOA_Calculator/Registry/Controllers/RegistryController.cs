using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json;
using Registry_DLL;
using Authenticator_DLL;
using System.ServiceModel;
using System.Web.Http.Description;
using System.Web;

namespace Registry.Controllers
{
    [RoutePrefix("api/registry")]
    public class RegistryController : ApiController
    {
        //static string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string folder = HttpRuntime.AppDomainAppPath;

        [Route("Publish/{token:int}")]
        [HttpPost]
        [ResponseType(typeof(ServiceCallOutcome))]
        public IHttpActionResult Publish(int token, [FromBody] ServiceDescription serviceDescription)
        {
            string fNameFile = folder + "/App_Data/service_description.txt";

            if(!Validate(token))
            {
                HttpResponseException httpResponseException = GetDeniedResponse();
                throw httpResponseException;
            }


            // Get contents of text file which may or may not have anything in it,
            // but we are expecting Json in ServiceDescription format.
            string contents = File.ReadAllText(fNameFile);

            /*
             * Deserializes the contents of the text file into ServiceDescription objects
             * and puts them into a list. If there is no ServiceDescription json read from
             * the file (its empty) then the '??' line will create a new empty list.
             */
            List<ServiceDescription> serviceDescriptions = 
                JsonConvert.DeserializeObject<List<ServiceDescription>>(contents) 
                ?? new List<ServiceDescription>();

            // Adds the input ServiceDescription object to the list
            serviceDescriptions.Add(serviceDescription);

            // Serializes the list into json format.
            contents = JsonConvert.SerializeObject(serviceDescriptions);

            // Writes the Json to the file.
            File.WriteAllText(fNameFile, contents);

            var outcome = new ServiceCallOutcome()
            {
                Status = "Success",
                Reason = "Published Service Description."
            };


            return Ok(outcome);
        }

        [Route("Search/{token:int}/{searchTerm}")]
        [HttpPost]
        [ResponseType(typeof(List<ServiceDescription>))]
        public IHttpActionResult Search(int token, string searchTerm)
        {
            string fNameFile = folder + "/App_Data/service_description.txt";

            if (!Validate(token))
            {
                HttpResponseException httpResponseException = GetDeniedResponse();
                throw httpResponseException;
            }

            string contents = File.ReadAllText(fNameFile);

            // New list to hold the matching service descriptions
            List<ServiceDescription> outputServiceDescriptions = new List<ServiceDescription>();

            // List of all the stored service descriptions.
            List <ServiceDescription> serviceDescriptions =
                JsonConvert.DeserializeObject<List<ServiceDescription>>(contents)
                ?? new List<ServiceDescription>();
            try
            {
                foreach (ServiceDescription serviceDescription in serviceDescriptions)
                {
                    // Iterate over every variable in ServiceDescription (Name, Description, API_Endpoint, Num_Operands, Operand_type)
                    foreach (PropertyInfo prop in serviceDescription.GetType().GetProperties())
                    {
                        // Gets the string value of the variable for comparison
                        string str = prop.GetValue(serviceDescription).ToString().ToLower();
                        //if (str.Contains(searchTerm))
                        if(str.Length > searchTerm.Length)
                        {
                            if (str.Contains(searchTerm.ToLower()))
                            {
                                // If there is a match, then add this service description to the list
                                // and break instead of checking the rest of the variables.
                                outputServiceDescriptions.Add(serviceDescription);
                                break;
                            }
                        }
                        else if(str.Equals(searchTerm.ToLower()))
                        {
                            outputServiceDescriptions.Add(serviceDescription);
                            break;
                        }
                    }

                }
            }
            catch(Exception exc)
            {
                var message = exc.ToString();
            }
            //var output = JsonConvert.SerializeObject(outputServiceDescriptions);

            return Ok(outputServiceDescriptions);
        }

        [Route("AllServices/{token:int}")]
        [HttpGet]
        [ResponseType(typeof(List<ServiceDescription>))]
        public IHttpActionResult AllServices(int token)
        {
            string fNameFile = folder + "/App_Data/service_description.txt";

            if (!Validate(token))
            {
                HttpResponseException httpResponseException = GetDeniedResponse();
                throw httpResponseException;
            }

            string contents = File.ReadAllText(fNameFile);

            // List of all the stored service descriptions.
            List<ServiceDescription> serviceDescriptions =
                JsonConvert.DeserializeObject<List<ServiceDescription>>(contents)
                ?? new List<ServiceDescription>();

            //var output = JsonConvert.SerializeObject(serviceDescriptions);

            //return Ok(output);
            return Ok(serviceDescriptions);
        }

        [Route("Unpublish/{token}")]
        [HttpPost]
        [ResponseType(typeof(ServiceCallOutcome))]
        public IHttpActionResult Unpublish(int token, [FromBody] String serviceEndpoint)
        //public IHttpActionResult Unpublish(int token)
        {
            string fNameFile = folder + "/App_Data/service_description.txt";

            if (!Validate(token))
            {
                HttpResponseException httpResponseException = GetDeniedResponse();
                throw httpResponseException;
            }

            string contents = File.ReadAllText(fNameFile);

            // List of all the stored service descriptions.
            List<ServiceDescription> serviceDescriptions =
                JsonConvert.DeserializeObject<List<ServiceDescription>>(contents)
                ?? new List<ServiceDescription>();

            foreach (ServiceDescription serviceDescription in serviceDescriptions)
            {
                if(serviceDescription.API_Endpoint.Equals(serviceEndpoint))
                {
                    serviceDescriptions.Remove(serviceDescription);
                    break;
                }
            }

            string json = JsonConvert.SerializeObject(serviceDescriptions);

            File.WriteAllText(fNameFile, json);

            var outcome = new ServiceCallOutcome()
            {
                Status = "Success",
                Reason = "Published Service Description."
            };

            return Ok(outcome);
        }

        private bool Validate(int token)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            Authenticator_Interface authenticator;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            if(authenticator.Validate(token).Equals("validated"))
            {
                return true;
            }

            return false;
        }

        private HttpResponseException GetDeniedResponse()
        {
            /*var badOutcome = new ServiceCallOutcome()
            {
                Status = "Denied",
                Reason = "Authentication Error."
            };*/
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Authentication Error.")
            };
            HttpResponseException httpResponseException = new HttpResponseException(response);

            return httpResponseException;
        }
    }
}
