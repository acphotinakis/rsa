using Newtonsoft.Json;

namespace KeyTypes
{
    /* The PublicKey class represents a public key with an email and a base64 encoded key, and provides
    a method to convert the object to JSON. */
    public class PublicKey
    {
        public PublicKey(string email, string encoded)
        {
            this.email = email;
            this.key = encoded;
        }

        // Getter and Setter for Email property
        public string email { get; set; }

        // Getter and Setter for Base64EncodedKey property
        public string key { get; set; }

        // Function to convert the object to JSON
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


    /* The PrivateKey class represents a private key with a list of emails and a base64 encoded key,
    and provides methods to get and set the properties and convert the object to JSON. */
    public class PrivateKey
    {
        public PrivateKey(List<String> emails, string base64EncodedKey)
        {
            this.email = emails;
            this.key = base64EncodedKey;
        }

        // Getter and Setter for Emails property
        public List<String> email { get; set; }

        // Getter and Setter for Base64EncodedKey property
        public string key { get; set; }

        // Function to convert the object to JSON
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


}
