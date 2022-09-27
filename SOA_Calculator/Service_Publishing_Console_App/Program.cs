using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authenticator_DLL;

namespace Service_Publishing_Console_App
{
    internal class Program
    {
        static Authenticator_Interface authenticator;
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
            

        }
        private static void Unpublish(int token)
        {

        }
    }
}
