using Newtonsoft.Json;

namespace rsa.src.messagehandlerservice.jsonmessage
{
    /* The JsonMessage class represents a JSON message with email and content properties, and provides
      methods for serializing and deserializing the object. */
    public class JsonMessage
    {
        public JsonMessage(string email, string content)
        {
            this.email = email;
            this.content = content;
        }

        // Getter and Setter for Email property
        public string email { get; set; }

        // Getter and Setter for Base64EncodedKey property
        public string content { get; set; }

        // Function to convert the object to JSON
        public string SerializeJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public JsonMessage DeserializeJson()
        {
            return JsonConvert.DeserializeObject<JsonMessage>(content)!;
        }
    }

}