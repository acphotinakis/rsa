using System.Net;
using System.Text;

/* The `HttpHandler` class provides methods for sending and retrieving data from a server using HTTP
requests. */
public class HttpHandler
{
    /// <summary>
    /// The function `GetPublicKey` sends a GET request to a specified URL and returns the response content
    /// and status code.
    /// </summary>
    /// <param name="email">The `email` parameter is a string that represents the email address of the user
    /// for whom you want to retrieve the public key.</param>
    /// <returns>
    /// The method `GetPublicKey` returns a tuple containing two values: a string `content` and an
    /// `HttpStatusCode` `statusCode`.
    /// </returns>
    public static async Task<(string content, HttpStatusCode statusCode)> GetPublicKey(string email)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"REPLACE_WITH_SERVER_URL/Key/{email}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    return (jsonContent, response.StatusCode);
                }
                else
                {
                    return ($"Error: {response.StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ($"Error: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }

    /// <summary>
    /// The function `SendPublicKeyAsync` sends a serialized JSON object containing a public key to a server
    /// using a PUT request, and returns the response status code and content.
    /// </summary>
    /// <param name="email">The `email` parameter is a string that represents the email address of the user
    /// to whom the public key will be sent.</param>
    /// <param name="serializedJson">The `serializedJson` parameter is a string that represents a JSON
    /// object. It contains the data that needs to be sent to the server.</param>
    /// <returns>
    /// The method `SendPublicKeyAsync` returns a tuple containing two values: a string `content` and an
    /// `HttpStatusCode` `statusCode`.
    /// </returns>
    public static async Task SendPublicKeyAsync(string email, string serializedJson)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"REPLACE_WITH_SERVER_URL/Key/{email}";

                var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(url, content);

                HttpStatusCode code = response.StatusCode;
                if (response.IsSuccessStatusCode || code == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("Key saved\n");
                }
                else
                {
                    Console.WriteLine("Error: Key not sent to the server\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }

    /// <summary>
    /// The function `SendMessage` sends a serialized JSON message to a specified email address using an
    /// HTTP PUT request.
    /// </summary>
    /// <param name="email">The `email` parameter is a string that represents the email address of the
    /// recipient to whom the message will be sent.</param>
    /// <param name="serializedJson">The `serializedJson` parameter is a string that represents a JSON
    /// object. It is the data that you want to send to the server as the message content.</param>
    /// <returns>
    /// The method `SendMessage` returns a tuple containing two values: a string `content` and an
    /// `HttpStatusCode` `statusCode`.
    /// </returns>
    public static async Task<(string content, HttpStatusCode statusCode)> SendMessage(string email, string serializedJson)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"REPLACE_WITH_SERVER_URL/Message/{email}";

                var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return ($"Message written\n", response.StatusCode);
                }
                else
                {
                    return ($"Error: {response.StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ($"Error: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }

    /// <summary>
    /// The function `GetMessage` sends an HTTP GET request to a specified URL and returns the response
    /// content and status code.
    /// </summary>
    /// <param name="email">The `email` parameter is a string that represents the email address of the user
    /// for whom we want to retrieve a message.</param>
    /// <returns>
    /// The `GetMessage` method returns a tuple containing two values: a string `content` and an
    /// `HttpStatusCode` `statusCode`.
    /// </returns>
    public static async Task<(string content, HttpStatusCode statusCode)> GetMessage(string email)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"REPLACE_WITH_SERVER_URL/Message/{email}";
                HttpResponseMessage response = await client.GetAsync(url);

                HttpStatusCode statusCode = response.StatusCode;
                string jsonContent = await response.Content.ReadAsStringAsync();

                if (statusCode == HttpStatusCode.OK)
                {
                    return (jsonContent, statusCode);
                }
                else
                {
                    return ($"Error: {statusCode}", statusCode);
                }
            }
            catch (Exception ex)
            {
                return ($"Error: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}
