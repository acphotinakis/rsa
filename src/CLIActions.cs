using KeyTypes;
using rsa.src.messagehandlerservice.messagedecrpytion;
using System.Net;
using Newtonsoft.Json;
using rsa.src.messagehandlerservice.messageencrpytion;
using rsa.src.messagehandlerservice.jsonmessage;
using System.Text.RegularExpressions;
using rsa.src.primekeygeneration.primeextension;
using rsa.src.http;

namespace rsa.src
{
    /// <summary>
    /// Represents CLI actions related to RSA encryption and key management.
    /// </summary>
    public class CliAction
    {
        /// <summary>
        /// Handles the key generation process for RSA encryption.
        /// </summary>
        public class KeyGen : ICliAction
        {
            private readonly string[] _actionArgs;

            /// <summary>
            /// Initializes a new instance of the KeyGen class.
            /// </summary>
            /// <param name="args">Arguments passed to the CLI action.</param>
            public KeyGen(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            /// <summary>
            /// Gets or sets the size of the RSA key to be generated.
            /// </summary>
            public int KeySize { get; private set; }

            /// <summary>
            /// Handles the action of generating public and private RSA keys.
            /// </summary>
            public void HandleAction()
            {
                PrimeExtension.GeneratePrimesNaive(10000);

                var rsa = new RSA_.RSA(KeySize);
                var publicKey = rsa.GeneratePublicKey();
                var privateKey = rsa.GeneratePrivateKey();

                KeyManager.WritePublicKeyToFile(publicKey.ToJson(), "public");
                KeyManager.WritePrivateKeyToFile(privateKey.ToJson(), "private");
                Console.WriteLine("\nKeys generated successfully.");
            }

            /// <summary>
            /// Sets up the key size for the key generation process based on the provided arguments.
            /// </summary>
            public void SetupInstance()
            {
                if (_actionArgs.Length < 2 || !int.TryParse(_actionArgs[1], out var keySize) || keySize % 8 != 0)
                {
                    throw new ArgumentException("Invalid key size. Ensure it is a multiple of 8.");
                }
                KeySize = keySize;
            }

            /// <summary>
            /// Determines if a string is a valid number.
            /// </summary>
            /// <param name="input">The input string.</param>
            /// <returns>True if the input string is a number; otherwise, false.</returns>
            public static bool IsNumber(string input) => Regex.IsMatch(input, @"^\d+$");

            Task ICliAction.HandleAction()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Handles the process of sending an encrypted message to a recipient.
        /// </summary>
        public class SendMsg : ICliAction
        {
            private readonly string[] _actionArgs;

            /// <summary>
            /// Initializes a new instance of the SendMsg class.
            /// </summary>
            /// <param name="args">Arguments passed to the CLI action.</param>
            public SendMsg(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            /// <summary>
            /// Gets or sets the recipient's email address.
            /// </summary>
            public string RecipientEmail { get; private set; }

            /// <summary>
            /// Gets or sets the plain text message to be sent.
            /// </summary>
            public string PlainText { get; private set; }

            /// <summary>
            /// Handles the action of sending an encrypted message to a recipient.
            /// </summary>
            public async Task HandleAction()
            {
                if (!File.Exists($"{RecipientEmail}.key"))
                {
                    Console.WriteLine($"Key file for {RecipientEmail} does not exist.");
                    Environment.Exit(1);
                }

                var encryption = new MessageEncryption(PlainText, $"{RecipientEmail}.key");
                encryption.DeserializeKey();
                var encryptedMessage = encryption.EncryptMessage();
                var jsonMessage = new JsonMessage(RecipientEmail, encryptedMessage);

                var result = await HttpHandler.SendMessage(RecipientEmail, jsonMessage.SerializeJson());

                if (result.statusCode == HttpStatusCode.OK || result.statusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("Message sent successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send message: {result.statusCode}");
                }
            }

            /// <summary>
            /// Sets up the recipient email and plain text message based on the provided arguments.
            /// </summary>
            public void SetupInstance()
            {
                if (_actionArgs.Length < 3)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <RecipientEmail> <PlainText>");
                }

                RecipientEmail = _actionArgs[1];
                PlainText = _actionArgs[2];
            }
        }

        /// <summary>
        /// Handles the process of sending a public key to a recipient's email address.
        /// </summary>
        public class SendKey : ICliAction
        {
            private readonly string[] _actionArgs;

            /// <summary>
            /// Initializes a new instance of the SendKey class.
            /// </summary>
            /// <param name="args">Arguments passed to the CLI action.</param>
            public SendKey(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            /// <summary>
            /// Gets or sets the recipient's email address.
            /// </summary>
            public string Email { get; private set; }

            /// <summary>
            /// Handles the action of sending the public key to a recipient.
            /// </summary>
            public async Task HandleAction()
            {
                var privateKey = KeyManager.GetLocalPrivateKey();

                if (!privateKey.email.Contains(Email))
                {
                    privateKey.email.Add(Email);
                    KeyManager.WritePrivateKeyToFile(JsonConvert.SerializeObject(privateKey), Email);
                }

                var publicKey = KeyManager.GetLocalPublicKey();
                publicKey.email = Email;

                await HttpHandler.SendPublicKeyAsync(Email, JsonConvert.SerializeObject(publicKey));
                Console.WriteLine($"Public key for {Email} sent successfully.");
            }

            /// <summary>
            /// Sets up the email address based on the provided arguments.
            /// </summary>
            public void SetupInstance()
            {
                if (_actionArgs.Length < 2)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <Email>");
                }

                Email = _actionArgs[1];
            }
        }

        /// <summary>
        /// Handles the process of retrieving a public key for a given email address.
        /// </summary>
        public class GetKey : ICliAction
        {
            private readonly string[] _actionArgs;

            /// <summary>
            /// Initializes a new instance of the GetKey class.
            /// </summary>
            /// <param name="args">Arguments passed to the CLI action.</param>
            public GetKey(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            /// <summary>
            /// Gets or sets the email address to retrieve the public key for.
            /// </summary>
            public string Email { get; private set; }

            /// <summary>
            /// Handles the action of retrieving a public key for a given email address.
            /// </summary>
            public async Task HandleAction()
            {
                var (key, statusCode) = await HttpHandler.GetPublicKey(Email);

                if (statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(key))
                {
                    var publicKey = JsonConvert.DeserializeObject<PublicKey>(key);
                    publicKey.email = Email;
                    KeyManager.WriteGetPublicKeyToFile(publicKey.ToJson(), Email);
                    Console.WriteLine($"Public key for {Email} retrieved successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve key for {Email}. Status: {statusCode}");
                }
            }

            /// <summary>
            /// Sets up the email address based on the provided arguments.
            /// </summary>
            public void SetupInstance()
            {
                if (_actionArgs.Length < 2)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <Email>");
                }

                Email = _actionArgs[1];
            }
        }

        /// <summary>
        /// Handles the action of retrieving an encrypted message and decrypting it.
        /// </summary>
        public class GetMessage : ICliAction
        {
            private readonly string[] _actionArgs;

            /// <summary>
            /// Initializes a new instance of the GetMessage class.
            /// </summary>
            /// <param name="args">Arguments passed to the CLI action.</param>
            public GetMessage(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            /// <summary>
            /// Gets or sets the email address associated with the message.
            /// </summary>
            public string Email { get; private set; }

            /// <summary>
            /// Handles the action of retrieving and decrypting a message for the given email address.
            /// </summary>
            public async Task HandleAction()
            {
                var decryption = new MessageDecryption(Email);
                var result = await HttpHandler.GetMessage(Email);

                if (result.statusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Decrypted Message: " + decryption.DecryptMessage(result.content));
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve message. Status: {result.statusCode}");
                }
            }

            /// <summary>
            /// Sets up the email address based on the provided arguments.
            /// </summary>
            public void SetupInstance()
            {
                if (_actionArgs.Length < 2)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <Email>");
                }

                Email = _actionArgs[1];
            }
        }
    }
}
