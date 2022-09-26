﻿using System;
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

namespace Registry.Controllers
{
    public class RegistryController : ApiController
    {
        static string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        [HttpPost]
        public IHttpActionResult Publish(int token, ServiceDescription serviceDescription)
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

            foreach(ServiceDescription serviceDescription in serviceDescriptions)
            {
                // Iterate over every variable in ServiceDescription (Name, Description, API_Endpoint, Num_Operands, Operand_type)
                foreach(PropertyInfo prop in serviceDescription.GetType().GetProperties())
                {
                    // Gets the string value of the variable for comparison
                    string str = prop.GetValue(serviceDescription).ToString();
                    if(str.Contains(searchTerm))
                    {
                        // If there is a match, then add this service description to the list
                        // and break instead of checking the rest of the variables.
                        outputServiceDescriptions.Add(serviceDescription);
                        break;
                    }
                }

            }
            var output = JsonConvert.SerializeObject(outputServiceDescriptions);

            return Ok(output);
        }

        public IHttpActionResult AllServices(int token)
        {
            string fNameFile = folder + "/App_Data/service_description.txt";

            if (!Validate(token))
            {
                HttpResponseException httpResponseException = GetDeniedResponse();
                throw httpResponseException;
            }

            string contents = File.ReadAllText(fNameFile);

            var output = JsonConvert.SerializeObject(contents);

            return Ok(output);
        }

        public IHttpActionResult Unpublish(int token, string service_endpoint)
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
            var badOutcome = new ServiceCallOutcome()
            {
                Status = "Denied",
                Reason = "Authentication Error."
            };
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(badOutcome))
            };
            HttpResponseException httpResponseException = new HttpResponseException(response);

            return httpResponseException;
        }
    }
}