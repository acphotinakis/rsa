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

    public class CliAction
    {
        public class KeyGen : ICliAction
        {
            private readonly string[] _actionArgs;

            public KeyGen(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            public int KeySize { get; private set; }

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

            public void SetupInstance()
            {
                if (_actionArgs.Length < 2 || !int.TryParse(_actionArgs[1], out var keySize) || keySize % 8 != 0)
                {
                    throw new ArgumentException("Invalid key size. Ensure it is a multiple of 8.");
                }
                KeySize = keySize;
            }

            public static bool IsNumber(string input) => Regex.IsMatch(input, @"^\d+$");

            Task ICliAction.HandleAction()
            {
                throw new NotImplementedException();
            }
        }

        public class SendMsg : ICliAction
        {
            private readonly string[] _actionArgs;

            public SendMsg(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            public string RecipientEmail { get; private set; }
            public string PlainText { get; private set; }

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

        public class SendKey : ICliAction
        {
            private readonly string[] _actionArgs;

            public SendKey(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            public string Email { get; private set; }

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

            public void SetupInstance()
            {
                if (_actionArgs.Length < 2)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <Email>");
                }

                Email = _actionArgs[1];
            }
        }

        public class GetKey : ICliAction
        {
            private readonly string[] _actionArgs;

            public GetKey(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            public string Email { get; private set; }

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

            public void SetupInstance()
            {
                if (_actionArgs.Length < 2)
                {
                    throw new ArgumentException("Invalid arguments. Expected format: <Email>");
                }

                Email = _actionArgs[1];
            }
        }

        public class GetMessage : ICliAction
        {
            private readonly string[] _actionArgs;

            public GetMessage(string[] args)
            {
                _actionArgs = args ?? throw new ArgumentNullException(nameof(args));
            }

            public string Email { get; private set; }

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
