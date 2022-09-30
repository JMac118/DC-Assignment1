using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Authenticator_DLL;
using Newtonsoft.Json;
using Registry_DLL;
using RestSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Service_Publishing_Console_App
{
    internal class Program
    {
        static Authenticator_Interface authenticator;
        static string registryApi = "https://localhost:44329/";
        static void Main(string[] args)
        {
            ChannelFactory<Authenticator_Interface> authFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost/AuthenticationService";
            authFactory = new ChannelFactory<Authenticator_Interface>(tcp, URL);
            authenticator = authFactory.CreateChannel();

            MainMenu();
        }

        private static void MainMenu()
        {
            int token = -1;
            while(true)
            {
                Console.WriteLine("Service Publishing Console App.");
                Console.WriteLine("The following operations are available:");
                Console.WriteLine("1: Registration");
                Console.WriteLine("2: Login");
                Console.WriteLine("3: Publish Service");
                Console.WriteLine("4: Unpublish Service");
                Console.WriteLine("5: Close Server");
                Console.Write("Please input number choice: ");
                int choice = -1;
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice < 1 || choice > 5)
                    {
                        Console.Clear();
                        Console.WriteLine("Not a valid input. Please enter a number from 1-5");
                    }
                    else
                    {
                        switch (choice)
                        {
                            case 1:
                                Register();
                                break;
                            case 2:
                                token = Login();
                                break;
                            case 3:
                                Publish(token);
                                break;
                            case 4:
                                Unpublish(token);
                                break;
                            case 5: return;
                        }
                    }
                }
                catch(Exception)
                {
                    Console.Clear();
                    Console.WriteLine("Not a valid input. Please enter a number from 1-5");
                }
                
            }
        }

        private static void Register()
        {
            string username;
            string password;
            Console.Clear();
            Console.WriteLine("Register Operation.");
            Console.Write("Please enter a username: ");
            username = Console.ReadLine();
            if(username == "")
            {
                Console.Clear();
                Console.WriteLine("A username must be input to register.");
                return;
            }
            Console.Write("Please enter a password: ");
            password = Console.ReadLine();
            if (password == "")
            {
                Console.Clear();
                Console.WriteLine("A password must be input to register.");
                return;
            }
            try
            {
                string outcome = authenticator.Register(username, password);
                if (outcome.Equals("successfully registered"))
                {
                    Console.Clear();
                    Console.WriteLine("User Successfully Registered.");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Something went wrong. Failed to register.");
                }
            }
            catch(Exception e)
            {
                Console.Clear();
                Console.WriteLine("Something went wrong. Failed to register. Error: " + e.Message);
            }
        }
        private static int Login()
        {
            int token = -1;
            string username;
            string password;
            Console.Clear();
            Console.WriteLine("Login Operation.");
            Console.Write("Please enter a username: ");
            username = Console.ReadLine();
            if (username == "")
            {
                Console.Clear();
                Console.WriteLine("A username must be input to log in.");
                return token;
            }
            Console.Write("Please enter a password: ");
            password = Console.ReadLine();
            if (password == "")
            {
                Console.Clear();
                Console.WriteLine("A password must be input to log in.");
                return token;
            }
            try
            {
                token = authenticator.Login(username, password);

                Console.Clear();
                Console.WriteLine("User Successfully Logged In.");
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Something went wrong. Failed to login. Error: " + e.Message);
            }
            return token;
        }
        private static void Publish(int token)
        {
            Console.Clear();
            Console.WriteLine("Publish Operation.");
            

            string name, desc, endpoint, num_operands, operand_type;

            Console.Write("Please enter the name of the service: ");
            name = Console.ReadLine();
            if (name == "")
            {
                Console.Clear();
                Console.WriteLine("A name must be input to publish.");
                return;
            }

            Console.Write("Please enter the description of the service: ");
            desc = Console.ReadLine();
            if (desc == "")
            {
                Console.Clear();
                Console.WriteLine("A description must be input to publish.");
                return;
            }

            Console.Write("Please enter the API endpoint of the service: ");
            endpoint = Console.ReadLine();
            if (endpoint == "")
            {
                Console.Clear();
                Console.WriteLine("An endpoint must be input to publish.");
                return;
            }

            Console.Write("Please enter the number of operands used in the service: ");
            num_operands = Console.ReadLine();
            if (num_operands == "")
            {
                Console.Clear();
                Console.WriteLine("The number of operands must be input to publish.");
                return;
            }

            Console.Write("Please enter the types of operands used in the service: ");
            operand_type = Console.ReadLine();
            if (operand_type == "")
            {
                Console.Clear();
                Console.WriteLine("A type must be input to publish.");
                return;
            }

            ServiceDescription serviceDescription = new ServiceDescription();
            serviceDescription.Name = name;
            serviceDescription.Description = desc;
            serviceDescription.API_Endpoint = endpoint;
            serviceDescription.Num_Operands = num_operands;
            serviceDescription.Operand_Type = operand_type;

            RestClient restClient = new RestClient(registryApi);
            RestRequest request = new RestRequest("api/registry/Publish/" + token);

            var body = JsonConvert.SerializeObject(serviceDescription);
            request.AddParameter("Application/Json", body, ParameterType.RequestBody);
            
            RestResponse restResponse = restClient.ExecutePost(request);
            if(restResponse.IsSuccessStatusCode)
            {
                Console.Clear();
                ServiceCallOutcome serviceCallOutcome = JsonConvert.DeserializeObject<ServiceCallOutcome>(restResponse.Content);
                Console.WriteLine(serviceCallOutcome.Reason);
            }
            else
            {
                Console.Clear();
                
                Exception exc = new Exception(restResponse.Content);
                Console.WriteLine(exc.Message);
            }
        }
        private static void Unpublish(int token)
        {
            Console.Clear();
            Console.WriteLine("Unpublish Operation.");

            string endpoint;

            Console.Write("Please enter the API endpoint of the service: ");
            endpoint = Console.ReadLine();
            if (endpoint == "")
            {
                Console.Clear();
                Console.WriteLine("An endpoint must be input to publish.");
                return;
            }

            RestClient restClient = new RestClient(registryApi);
            RestRequest request = new RestRequest("api/registry/Unpublish/" + token);
            String body = JsonConvert.SerializeObject(endpoint);
            //request.AddStringBody(endpoint, "text/plain");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            //RestResponse restResponse = restClient.ExecutePost(request);
            RestResponse restResponse = restClient.ExecutePost(request);
            if (restResponse.IsSuccessStatusCode)
            {
                Console.Clear();
                ServiceCallOutcome serviceCallOutcome = JsonConvert.DeserializeObject<ServiceCallOutcome>(restResponse.Content);
                Console.WriteLine(serviceCallOutcome.Reason);
            }
            else
            {
                Console.Clear();
                Exception exc = new Exception(restResponse.Content);
                Console.WriteLine(exc.Message);
            }
        }
    }
}
