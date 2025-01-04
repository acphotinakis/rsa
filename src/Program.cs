using System.Text.RegularExpressions;
using rsa.src;

/* The code is defining a C# namespace called "Application" and within that namespace, there is a
static class called "Application". This class contains a Main method, which is the entry point of
the program. */
namespace Application
{
    /* The Application class handles different command line actions based on the arguments passed to
    the Main method. */
    static class Application
    {
        /// <summary>
        /// Checks if the input string is a valid number.
        /// </summary>
        /// <param name="input">The string to be checked.</param>
        /// <returns>Returns true if the input is a valid number, otherwise false.</returns>
        public static bool IsNumber(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }

        /// <summary>
        /// Validates the arguments for different CLI actions based on the action type.
        /// </summary>
        /// <param name="actionArgs">The arguments passed to the program.</param>
        /// <param name="action">The action to be validated (e.g., keygen, sendkey).</param>
        /// <returns>Returns true if the arguments are valid for the specified action, otherwise false.</returns>
        static bool ValidateCliActionsArguments(string[] actionArgs, string action)
        {
            switch (action.ToLower())
            {
                case "keygen":
                    if (actionArgs.Length != 2)
                    {
                        Console.WriteLine("Invalid number of arguments, type the following format:");
                        Console.WriteLine("\tdotnet run keyGen <bits>\n");
                        return false;
                    }
                    else if (IsNumber(actionArgs[1]))
                    {
                        int keySize = Int32.Parse(actionArgs[1]);
                        if (keySize < 2)
                        {
                            Console.WriteLine("Invalid key size, must be at least 2\n");
                        }
                        else if (keySize % 8 != 0)
                        {
                            Console.WriteLine("Invalid key size, must be a multiple of 8\n");
                        }
                        else
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Key size must be an integer, multiple of 8, and >= 2\n");
                        return false;
                    }

                case "sendkey":
                    if (actionArgs.Length != 2)
                    {
                        Console.WriteLine("Invalid number of arguments, type the following format:");
                        Console.WriteLine("\tdotnet run sendKey <email>\n");
                        return false;
                    }
                    else if (actionArgs[1] is string)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid arguments");
                        return false;
                    }

                case "getkey":
                    if (actionArgs.Length != 2)
                    {
                        Console.WriteLine("Invalid number of arguments, type the following format:");
                        Console.WriteLine("\tdotnet run getKey <email>\n");
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                case "sendmsg":
                    if (actionArgs.Length != 3)
                    {
                        Console.WriteLine("Invalid number of arguments, type the following format:");
                        Console.WriteLine("\tdotnet run sendMsg <email> \"<plaintext\"\n");
                        return false;
                    }
                    else if (actionArgs[0] is string && actionArgs[1] is string)
                    {
                        return true;
                    }
                    Console.WriteLine("Invalid arguments, type the following format:");
                    Console.WriteLine("\tdotnet run sendMsg <email> \"<plaintext\"\n");
                    return false;

                case "getmsg":
                    if (actionArgs.Length != 2)
                    {
                        Console.WriteLine("Invalid number of arguments, type the following format:");
                        Console.WriteLine("\tdotnet run getMsg <email>\n");
                        return false;
                    }
                    else if (actionArgs[0] is string && actionArgs[1] is string)
                    {
                        return true;
                    }
                    return false;

                default:
                    return false;
            }

        }

        /// <summary>
        /// The main entry point for the application.
        /// It validates the command line arguments and performs the corresponding action based on the first argument.
        /// </summary>
        /// <param name="args">The command line arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            // Validate the command line arguments based on the action
            if (!ValidateCliActionsArguments(args, args[0]))
            {
                System.Environment.Exit(1);
            }

            // Perform the appropriate action based on the first argument
            if (args[0].Equals("keyGen"))
            {
                CliAction.KeyGen keyGen = new CliAction.KeyGen(args);
                keyGen.SetupInstance();
                keyGen.HandleAction();
            }
            else if (args[0].Equals("sendKey"))
            {
                CliAction.SendKey sendKey = new CliAction.SendKey(args);
                sendKey.SetupInstance(); // Call SetupInstance
                sendKey.HandleAction().Wait();
            }
            else if (args[0].Equals("getKey"))
            {
                CliAction.GetKey getKey = new CliAction.GetKey(args);
                getKey.SetupInstance(); // Call SetupInstance
                getKey.HandleAction().Wait();
            }
            else if (args[0].Equals("sendMsg"))
            {
                CliAction.SendMsg sendMsg = new CliAction.SendMsg(args);
                sendMsg.SetupInstance(); // Call SetupInstance
                sendMsg.HandleAction().Wait();
            }
            else if (args[0].Equals("getMsg"))
            {
                CliAction.GetMessage getMessage = new CliAction.GetMessage(args);
                getMessage.SetupInstance(); // Call SetupInstance
                getMessage.HandleAction().Wait();
            }

            else
            {
                // Inform the user about the invalid action command
                Console.WriteLine("Action command not supported");
                Console.WriteLine("Options are the following: ");
                Console.WriteLine("\tdotnet run keyGen <bits>");
                Console.WriteLine("\tdotnet run sendKey <email>");
                Console.WriteLine("\tdotnet run getKey <email>");
                Console.WriteLine("\tdotnet run sendMsg <email> \"<plaintext\"");
                Console.WriteLine("\tdotnet run getMsg <email>\n");
            }

        }
    }
}
