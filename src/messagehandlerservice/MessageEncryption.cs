using Newtonsoft.Json;
using System.Text;
using System.Numerics;

namespace rsa.src.messagehandlerservice.messageencrpytion
{
    /* The MessageEncryption class provides methods to encrypt a message using RSA encryption and
       deserialize a public key from a file. */
    public class MessageEncryption
    {
        public MessageEncryption(string messageToEncrypt, string filepath)
        {
            MessageToEncrypt = messageToEncrypt;
            FilePath = filepath;
            N = BigInteger.Zero;
            E = BigInteger.Zero;
        }

        /// <summary>
        /// The function takes a message, encrypts it using RSA encryption, and returns the encrypted message as
        /// a base64 string.
        /// </summary>
        /// <returns>
        /// The method is returning an encrypted message as a string.
        /// </returns>
        public string EncryptMessage()
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(MessageToEncrypt);
            BigInteger plainBig = new BigInteger(plainBytes);
            BigInteger ciphertextBigInt = BigInteger.ModPow(plainBig, E, N);
            byte[] cipherBytes = ciphertextBigInt.ToByteArray();
            string encrypted = Convert.ToBase64String(cipherBytes);
            return encrypted;
        }

        public string MessageToEncrypt { get; set; }

        public string FilePath { get; set; }

        public BigInteger N { get; set; }
        public BigInteger E { get; set; }

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
        /// The function DeserializeKey reads a file, deserializes its content into a PublicKey object,
        /// and extracts the values for N and E.
        /// </summary>
        public void DeserializeKey()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    throw new Exception("File " + FilePath + " does not exist");
                }
                string content = File.ReadAllText(FilePath);
                KeyTypes.PublicKey keyObject = JsonConvert.DeserializeObject<KeyTypes.PublicKey>(content)!;

                if (keyObject != null)
                {
                    byte[] data = Convert.FromBase64String(keyObject.key);

                    byte[] eBytesBigEndian = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        eBytesBigEndian[i] = data[i];
                    }
                    int e = ConvertToBigEndianInt(eBytesBigEndian);

                    byte[] EBytesLittleEndian = new byte[e];
                    int offset = 4;
                    int count = 0;
                    while (count < e)
                    {
                        EBytesLittleEndian[count] = data[offset + count];
                        count++;
                    }
                    BigInteger EBig = new BigInteger(EBytesLittleEndian, isUnsigned: true, isBigEndian: false);

                    offset = 4 + e;
                    byte[] nBytes = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        nBytes[i] = data[offset + i];
                    }
                    int n = ConvertToBigEndianInt(nBytes);
                    offset = 4 + e + 4;
                    byte[] NBytesLittleEndian = new byte[n];
                    for (int i = 0; i < n; i++)
                    {
                        NBytesLittleEndian[i] = data[offset + i];
                    }
                    BigInteger NBig = new BigInteger(NBytesLittleEndian, isUnsigned: true, isBigEndian: false);

                    N = NBig;
                    E = EBig;
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