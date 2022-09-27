using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authenticator_DLL;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        InstanceContextMode = InstanceContextMode.Single)]
    internal class AuthenticationServer : Authenticator_Interface
    {
        // Gets the local filepath to this built project
        static string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Random random = new Random();
        public int Login(string name, string password)
        {
            string fNameFile = folder + "/Data/registered_users.txt";
            // Combine the username and password into single string: "user:pass"
            string inputUser = name + ":" + password;

            string[] userPassPairs;

            try
            {
                userPassPairs = File.ReadAllLines(fNameFile);

                foreach(string user in userPassPairs)
                {
                    if(inputUser.Equals(user))
                    {
                        return GenerateToken();
                    }
                }
            }
            catch(Exception exc)
            {
                //Throws if something goes wrong
                throw new FaultException<ApplicationException>(new ApplicationException("Something went wrong: " + exc.Message));
            }
            //Throws if no user found
            throw new FaultException<InvalidUserFault>(new InvalidUserFault("Failed to authenticate user: " + name));
        }

        public string Register(string name, string password)
        {
            string fNameFile = folder + "/Data/registered_users.txt";
            string input = name + ":" + password;
            string output = "";
            try
            {
                File.AppendAllText(fNameFile, input);
                output = "successfully registered";
            }
            catch(Exception)
            {
                output = "failed to register";
            }
            return output;
        }

        public string Validate(int token)
        {
            string fNameFile = folder + "/Data/generated_tokens.txt";
            string[] tokens;
            string valid = "not validated";

            try
            {
                tokens = File.ReadAllLines(fNameFile);
                foreach (string tempToken in tokens)
                {
                    int temp = int.Parse(tempToken);
                    if (temp == token)
                    {
                        valid = "validated";
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                //Throws if something goes wrong
                throw new FaultException<ApplicationException>(new ApplicationException("Something went wrong: " + exc.Message));
            }

            return valid;
        }

        /*
         * Creates a new token then checks if its already in the generated_tokens.txt file. 
         * If it is, then generate another and check again.
         */
        private int GenerateToken()
        {
            int token = random.Next();

            while(Validate(token).Equals("validated"))
            {
                token = random.Next();
            }

            return token;
        }

        /*
         * Calculates milliseconds from input minute variable, sets Threading Timer to 
         * repeat call the token cleanup.
         */
        internal void SetTokenCleanupPeriod(int minutes)
        {
            int mseconds = (minutes * 60000);
            Timer timer = new Timer(CleanupTokens, null, mseconds, mseconds);
            Console.WriteLine("Timer task set.");
        }

        private void CleanupTokens(object state)
        {
            string fNameFile = folder + "/Data/generated_tokens.txt";
            File.WriteAllText(fNameFile, String.Empty);
            Console.WriteLine("Cleared tokens");
        }
    }
}
