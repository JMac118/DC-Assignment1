using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Authenticator_DLL;

namespace Authenticator
{
    /*
        The Authenticator Project [2 Marks]
        It is a .NET WCF/remoting server. “net.tcp://localhost/AuthenticationService” is an
        example of a fixed service endpoint. It has three operations open as service functions:
        1. String Register (String name, String Password): It expects two operands, i.e.,
        name and password from an actor. It saves these values in a local text file. If
        successful it returns “successfully registered”. [0.5 Mark]
        2. int Login (String name, String Password): It expects two operands, i.e., name
        and password from an actor. It checks these values in a local text file. If a match
        is found, it creates a token (random integer), saves it into another local text file,
        and returns it to the actor who calls this function. [0.5 Mark]
        3. String validate (int token): It expects a token and checks whether the token is
        already generated. If the token could be validated, the return is “validated”, else
        “not validated”. [0.5 Mark]
        4. There is an internal function that clears the saved tokens every ‘x’ minutes.
        When you run the authentication server, it will ask for the number of minutes
        for the periodical clean-up in the console (using multithreading). [0.5 Marks] 
     */
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Authentication Service.");
            
            NetTcpBinding tcp = new NetTcpBinding();

            AuthenticationServer authServer = new AuthenticationServer();
            ServiceHost host = new ServiceHost(authServer);

            host.AddServiceEndpoint(typeof(Authenticator_Interface), tcp, "net.tcp://localhost/AuthenticationService");


            //Task cleanupTask = authServer.SetTokenCleanupPeriod(numMinutes);
            //cleanupTask.Start();

            host.Open();

            int numMinutes = PromptUserForMinutes();

            //Task.Run(() =>
            //{
                authServer.SetTokenCleanupPeriod(numMinutes);
            //});

            Console.WriteLine("Service is Online. \nCleaning saved tokens every " + numMinutes + " minutes.\nEnter any key to close server...");
            Console.ReadLine();

            host.Close();
        }

        private static int PromptUserForMinutes()
        {
            string input = null;
            int output = -1;
            while (input == null)
            {
                Console.WriteLine("How often should periodical token clean-up occur? (minutes)");
                try
                {
                    input = Console.ReadLine();
                    output = int.Parse(input);

                    if (output <= 0)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Not a valid input");
                    input = null;
                }
            }
            return output;
        }
    }
}
