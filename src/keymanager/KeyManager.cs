using KeyTypes;
using Newtonsoft.Json;


/* The KeyManager class provides methods for writing and retrieving public and private keys to and from
files. */
public class KeyManager
{
    /// <summary>
    /// The function writes a JSON string to a file with the name being the email address followed by the
    /// extension ".key" and prints a message indicating that the public key for the email has been
    /// retrieved and stored locally.
    /// </summary>
    /// <param name="json">A string containing the JSON representation of the public key.</param>
    /// <param name="email">The email parameter is a string that represents the email address of the
    /// user.</param>
    public static void WriteGetPublicKeyToFile(string json, string email)
    {
        string filePath = String.Concat(email, ".key");
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// The function writes a public key in JSON format to a file and prints a message indicating the email
    /// associated with the key.
    /// </summary>
    /// <param name="json">A string containing the JSON representation of the public key.</param>
    /// <param name="email">The email parameter is a string that represents the email address associated
    /// with the public key.</param>
    public static void WritePublicKeyToFile(string json, string email)
    {
        string filePath = "public.key";
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// The function writes a JSON string to a file named "private.key" and prints a success message with
    /// the email and JSON.
    /// </summary>
    /// <param name="json">The `json` parameter is a string that represents the private key in JSON
    /// format.</param>
    /// <param name="email">The email parameter is a string that represents the email address associated
    /// with the private key.</param>
    public static void WritePrivateKeyToFile(string json, string email)
    {
        string filePath = "private.key";
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// The function checks if a file exists in a specified directory.
    /// </summary>
    /// <param name="directoryPath">The directory path is the path to the directory where the file is
    /// located. It can be an absolute path or a relative path.</param>
    /// <param name="fileName">The name of the file you want to check for existence.</param>
    /// <returns>
    /// The method returns a boolean value indicating whether the file exists or not.
    /// </returns>
    public static bool DoesFileExist(string directoryPath, string fileName)
    {
        string filePath = Path.Combine(directoryPath, fileName);
        return File.Exists(filePath);
    }

    /// <summary>
    /// The function retrieves a local public key from a file.
    /// </summary>
    /// <returns>
    /// The method is returning a PublicKey object.
    /// </returns>
    public static PublicKey GetLocalPublicKey()
    {
        string filePath = "public.key";
        try
        {
            if (!File.Exists(filePath)) throw new Exception();
            else
            {
                string content = File.ReadAllText(filePath);
                KeyTypes.PublicKey localPublicKey = JsonConvert.DeserializeObject<KeyTypes.PublicKey>(content)!;
                return localPublicKey;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("public.key file does not exist\n");
            System.Environment.Exit(1);
            return null;
        }

    }

    /// <summary>
    /// The function retrieves a local private key from a file.
    /// </summary>
    /// <returns>
    /// The method `GetLocalPrivateKey` returns a `PrivateKey` object.
    /// </returns>
    public static PrivateKey GetLocalPrivateKey()
    {
        string filePath = "private.key";
        try
        {
            if (!File.Exists(filePath)) throw new Exception();
            else
            {
                string content = File.ReadAllText(filePath);
                KeyTypes.PrivateKey localPrivateKey = JsonConvert.DeserializeObject<KeyTypes.PrivateKey>(content)!;
                return localPrivateKey;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("private.key file does not exist\n");
            System.Environment.Exit(1);
            return null;
        }


    }

}