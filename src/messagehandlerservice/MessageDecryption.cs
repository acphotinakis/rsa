using Newtonsoft.Json;
using System.Text;
using System.Numerics;
using rsa.src.messagehandlerservice.jsonmessage;

namespace rsa.src.messagehandlerservice.messagedecrpytion
{
    /* The `MessageDecryption` class provides methods to decrypt a message using RSA encryption. */
    public class MessageDecryption
    {
        public MessageDecryption(string email)
        {
            Email = email;
            N = BigInteger.Zero;
            D = BigInteger.Zero;
        }

        public string Email { get; set; }

        public string FilePath { get; set; }

        public BigInteger N { get; set; }
        public BigInteger D { get; set; }

        /// <summary>
        /// The function DecryptMessage takes a base64 encoded message, decrypts it using RSA encryption, and
        /// returns the plaintext message.
        /// </summary>
        /// <param name="messageToDecrypt">The `messageToDecrypt` parameter is a string that represents the
        /// encrypted message that needs to be decrypted.</param>
        /// <returns>
        /// The method is returning the decrypted plaintext message as a string.
        /// </returns>
        public string DecryptMessage(string messageToDecrypt)
        {
            JsonMessage jsonMessage = JsonConvert.DeserializeObject<JsonMessage>(messageToDecrypt);
            this.DeserializeKey("private.key");

            byte[] ciphertextBytes = Convert.FromBase64String(jsonMessage.content);

            // Convert the byte array to a BigInteger
            BigInteger ciphertextBigInt = new BigInteger(ciphertextBytes, isUnsigned: true, isBigEndian: false);

            // Perform RSA decryption: plaintext = (ciphertext ^ d) % n
            BigInteger plaintextBigInt = BigInteger.ModPow(ciphertextBigInt, D, N);

            // Convert the resulting BigInteger to a byte array
            byte[] plaintextBytes = plaintextBigInt.ToByteArray();

            // Convert the byte array to a string
            string plaintext = Encoding.UTF8.GetString(plaintextBytes);

            return plaintext;
        }

        /// <summary>
        /// The function takes a byte array and converts it to a big-endian integer.
        /// </summary>
        /// <param name="bytes">The parameter "bytes" is a byte array that represents a sequence of
        /// bytes.</param>
        /// <returns>
        /// The method is returning an integer value.
        /// </returns>
        public static int ConvertToBigEndianInt(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Byte array is null or empty");
            }

            int result = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                result = (result << 8) | bytes[i];
            }

            return result;
        }

        /// <summary>
        /// The function DeserializeKey reads a file, deserializes its content into a
        /// KeyTypes.PrivateKey object, and extracts specific values from the object to assign them to
        /// variables D and N.
        /// </summary>
        /// <param name="filePath">The `filePath` parameter is a string that represents the path to the
        /// file that contains the serialized key data.</param>
        public void DeserializeKey(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception();
                }
                else
                {
                    string content = File.ReadAllText(filePath);
                    KeyTypes.PrivateKey keyObject = JsonConvert.DeserializeObject<KeyTypes.PrivateKey>(content)!;
                    if (filePath != null)
                    {
                        byte[] data = Convert.FromBase64String(keyObject.key);

                        byte[] dBytesBigEndian = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            dBytesBigEndian[i] = data[i];
                        }
                        int d = ConvertToBigEndianInt(dBytesBigEndian);

                        byte[] DBytesLittleEndian = new byte[d];
                        int offset = 4;
                        int count = 0;
                        while (count < d)
                        {
                            DBytesLittleEndian[count] = data[offset + count];
                            count++;
                        }
                        BigInteger DBig = new BigInteger(DBytesLittleEndian, isUnsigned: true, isBigEndian: false);

                        offset = 4 + d;
                        byte[] nBytes = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            nBytes[i] = data[offset + i];
                        }
                        int n = ConvertToBigEndianInt(nBytes);
                        offset = 4 + d + 4;
                        byte[] NBytesLittleEndian = new byte[n];
                        for (int i = 0; i < n; i++)
                        {
                            NBytesLittleEndian[i] = data[offset + i];
                        }
                        BigInteger NBig = new BigInteger(NBytesLittleEndian, isUnsigned: true, isBigEndian: false);

                        D = DBig;
                        N = NBig;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n\nERROR:");
                Console.WriteLine($"e.Message: \n\t{e.Message}");
                Console.WriteLine($"\ne.StackTrace: \n\t{e.StackTrace}\n\n");
                System.Environment.Exit(1);
            }
        }
    }
}