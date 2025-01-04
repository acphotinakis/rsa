# SecureComm

![C# Badge](https://img.shields.io/badge/C%23%20.NET-7.0-blue?style=flat-square&logo=csharp)

This project is a command-line RSA encryption tool that allows users to securely generate RSA key pairs, exchange public keys, and send or retrieve encrypted messages. By using simple commands like keyGen to generate keys, sendKey and getKey to exchange public keys, and sendMsg and getMsg to encrypt and decrypt messages, users can easily set up encrypted communication. The application ensures input validation, checks for valid email addresses, and handles errors with clear instructions, making it simple to use for secure message exchange.


## Application

The `Application` namespace contains the entry point for the program, which handles various command-line interface (CLI) actions such as key generation, sending and retrieving keys, and sending and retrieving messages. The main class, also named `Application`, validates the arguments passed to the program and performs the appropriate action based on the specified command.

### CLI Actions

The program supports the following actions, each with specific argument formats:

1. **keyGen**: Generate RSA keys with a specified size (in bits).
   - Arguments: `dotnet run keyGen <bits>`
   - Validates that the key size is a positive integer, a multiple of 8, and at least 2 bits.

2. **sendKey**: Send a public key to a recipient via email.
   - Arguments: `dotnet run sendKey <email>`
   - Requires the recipient's email address.

3. **getKey**: Retrieve the public key for a recipient using their email address.
   - Arguments: `dotnet run getKey <email>`
   - Requires the recipient's email address.

4. **sendMsg**: Encrypt and send a message to a recipient.
   - Arguments: `dotnet run sendMsg <email> "<plaintext"`
   - Requires the recipient's email address and the message to be sent.

5. **getMsg**: Retrieve and decrypt a message for a recipient.
   - Arguments: `dotnet run getMsg <email>`
   - Requires the recipient's email address.

### Method Descriptions

- **IsNumber**: Validates if a string input is a valid number.
  - Returns `true` if the input is a valid number, otherwise `false`.
  
- **ValidateCliActionsArguments**: Validates the arguments based on the specified action.
  - Ensures the correct number of arguments and checks their validity based on the action type (e.g., key generation, message sending).
  
- **Main**: The entry point of the application.
  - Validates the command-line arguments and performs the corresponding action based on the first argument.
  - Supports actions like key generation, sending and receiving keys, and sending and receiving messages.

### Error Handling

- If invalid arguments are passed or required arguments are missing, the program will display an error message and exit.
- The program also handles invalid action commands by listing the supported commands and their required arguments.

### Example Usage

1. Generate RSA key:
   ```bash
   dotnet run keyGen 2048
    ```
    This command generates an RSA key with the specified key size (2048 bits). The key size must be a positive multiple of 8 and at least 2.

2. Send a public key to a recipient
    ```bash
    dotnet run sendKey recipient@example.com
    ```
    This command sends the public key to the recipient with the given email address.

3. Retrieve a public key
    ```bash
    dotnet run getKey recipient@example.com
    ```
    This command retrieves the public key for the recipient with the specified email address.

4. Send an encrypted message
    ```bash
    dotnet run sendMsg recipient@example.com "Hello, this is a secret message!"
    ```
    This command encrypts and sends the message to the recipient with the given email address.

5. Retrieve and decrypt a message
    ```bash
    dotnet run getMsg recipient@example.com
    ```
    This command retrieves and decrypts the message for the recipient with the given email address.

### Notes
    - Ensure that the key size provided for the keyGen action is a positive multiple of 8 and at least 2.
    - The email address provided for the sendKey, getKey, and sendMsg actions should be in a valid string format.



## HttpHandler

The `HttpHandler` class provides methods for sending and retrieving data from a server via HTTP requests. The class contains the following functions:

- **GetPublicKey**: Sends a GET request to retrieve the public key associated with a user's email.
- **SendPublicKeyAsync**: Sends a serialized JSON object containing a public key to a server using a PUT request.
- **SendMessage**: Sends a serialized JSON message to a specified email address using an HTTP PUT request.
- **GetMessage**: Sends a GET request to retrieve a message associated with a user's email.

Each method handles errors gracefully, returning appropriate status codes and error messages where necessary. The class utilizes `HttpClient` for making asynchronous HTTP requests and returns either content or status code based on the server's response.


## KeyManager

The `KeyManager` class provides methods for writing and retrieving public and private keys to and from files. The class includes the following functions:

- **WriteGetPublicKeyToFile**: Writes a JSON string (public key) to a file named after the user's email address with the `.key` extension.
- **WritePublicKeyToFile**: Writes a public key in JSON format to a file named `public.key`.
- **WritePrivateKeyToFile**: Writes a private key in JSON format to a file named `private.key`.
- **DoesFileExist**: Checks if a file exists in a specified directory.
- **GetLocalPublicKey**: Retrieves a local public key from the `public.key` file and returns it as a `PublicKey` object.
- **GetLocalPrivateKey**: Retrieves a local private key from the `private.key` file and returns it as a `PrivateKey` object.

The class uses `JsonConvert` from the `Newtonsoft.Json` library to deserialize the JSON data into appropriate `PublicKey` and `PrivateKey` objects. It also provides error handling for missing key files and prints relevant messages for successful operations.


## MessageDecryption

The `MessageDecryption` class provides methods to decrypt messages using RSA encryption. The class includes the following functions:

- **DecryptMessage**: Takes a base64 encoded message, decrypts it using RSA encryption, and returns the plaintext message. The decryption process involves converting the message to a BigInteger and applying the RSA formula `(ciphertext ^ d) % n`, where `d` is the private key and `n` is the modulus.

- **ConvertToBigEndianInt**: Converts a byte array to a big-endian integer. This method handles the byte order conversion necessary for RSA operations.

- **DeserializeKey**: Reads a file, deserializes its content into a `PrivateKey` object, and extracts the `D` (private exponent) and `N` (modulus) values, which are used for the decryption process.

### Constructor

- **MessageDecryption(string email)**: Initializes a new instance of the `MessageDecryption` class with the user's email address, and sets default values for `N` and `D`.

### Properties

- **Email**: The email address associated with the decryption process.
- **FilePath**: The path to the private key file (typically `private.key`).
- **N**: The RSA modulus (BigInteger).
- **D**: The RSA private exponent (BigInteger).

The class uses `JsonConvert` from the `Newtonsoft.Json` library to deserialize JSON content into `JsonMessage` and `PrivateKey` objects. It also provides error handling for missing key files and prints relevant error messages for failed operations.

### Error Handling

If the decryption or key deserialization fails (e.g., the key file does not exist or the format is incorrect), the program will output an error message and exit.



## MessageEncryption

The `MessageEncryption` class provides methods to encrypt a message using RSA encryption and deserialize a public key from a file. The class includes the following functions:

- **EncryptMessage**: Takes a message, encrypts it using RSA encryption, and returns the encrypted message as a base64 string. The encryption is performed using the RSA formula `ciphertext = (plaintext ^ e) % n`, where `e` is the public exponent and `n` is the modulus.

- **ConvertToBigEndianInt**: Converts a byte array to a big-endian integer. This method handles the byte order conversion necessary for RSA operations.

- **DeserializeKey**: Reads a file, deserializes its content into a `PublicKey` object, and extracts the `E` (public exponent) and `N` (modulus) values, which are used for the encryption process.

### Constructor

- **MessageEncryption(string messageToEncrypt, string filepath)**: Initializes a new instance of the `MessageEncryption` class with the message to encrypt and the file path of the public key.

### Properties

- **MessageToEncrypt**: The message that needs to be encrypted.
- **FilePath**: The path to the public key file (typically `public.key`).
- **N**: The RSA modulus (BigInteger).
- **E**: The RSA public exponent (BigInteger).

The class uses `JsonConvert` from the `Newtonsoft.Json` library to deserialize JSON content into `PublicKey` objects. It also provides error handling for missing key files and prints relevant error messages for failed operations.

### Error Handling

If the encryption or key deserialization fails (e.g., the key file does not exist or the format is incorrect), the program will output an error message and exit.



# Prime Extension for BigInteger

This C# code provides a set of extension methods for the `BigInteger` class to facilitate prime number generation and primality testing. The methods include:

## Key Methods

### `IsProbablyPrime(BigInteger n, int k = 10)`
- **Purpose**: Checks if a given `BigInteger` is probably prime using the Miller-Rabin primality test.
- **Parameters**:
  - `n` (BigInteger): The number to check.
  - `k` (int): The number of iterations for the test. Default is 10.
- **Returns**: `true` if the number is probably prime, otherwise `false`.

### `GetRandomA(BigInteger n)`
- **Purpose**: Generates a random `BigInteger` between 2 and `n - 2`.
- **Parameters**:
  - `n` (BigInteger): Upper limit for the generated random number.
- **Returns**: A randomly generated `BigInteger` in the range `[2, n-2]`.

### `CheckPrime(BigInteger candidate)`
- **Purpose**: Checks if a given `BigInteger` is prime by iterating through a list of pre-generated prime numbers and checking divisibility.
- **Parameters**:
  - `candidate` (BigInteger): The number to check.
- **Returns**: `true` if the candidate is prime, otherwise `false`.

### `GeneratePrime(int bits)`
- **Purpose**: Generates a prime number with the specified number of bits using parallel processing.
- **Parameters**:
  - `bits` (int): The number of bits for the generated prime number.
- **Returns**: A `BigInteger` representing the generated prime number.

### `GeneratePrimesNaive(int n)`
- **Purpose**: Generates a list of prime numbers up to a specified limit using a naive approach (trial division).
- **Parameters**:
  - `n` (int): The number of prime numbers to generate.
- **Returns**: A list of prime numbers (`autoGeneratedPrimes`).

## Key Concepts

- **Miller-Rabin Primality Test**: A probabilistic test used to determine if a number is likely prime. The `IsProbablyPrime` method implements this test for efficient primality checking.
- **Parallel Processing**: The `GeneratePrime` method utilizes parallel processing (`Parallel.For`) to speed up the prime number generation process for large numbers.
- **Secure Random Generation**: A secure random number generator (`secureRandom`) is used to generate cryptographically secure random numbers.

## Thread Safety
- **Locking**: The code uses a locking mechanism (`lockObj`) to ensure thread safety when accessing and modifying shared resources, such as the `autoGeneratedPrimes` list, during parallel execution.

## Usage
This extension can be used in cryptographic algorithms like RSA, where prime number generation and primality testing are essential. The `autoGeneratedPrimes` list provides pre-generated primes for efficient primality checks, and methods like `GeneratePrime` allow for the generation of large prime numbers.

## Example Usage

```csharp
BigInteger prime = PrimeExtension.GeneratePrime(2048); // Generate a 2048-bit prime number
bool isPrime = prime.IsProbablyPrime(); // Check if the generated number is prime
```


# RSA CLI Actions

This C# code defines several Command-Line Interface (CLI) actions for RSA encryption and key management. The actions handle the key generation, sending and receiving encrypted messages, and managing public keys for RSA encryption.

## Key Components

### 1. **KeyGen Class**
   - **Purpose**: Handles the process of generating RSA public and private keys.
   - **Key Operations**:
     - Generates RSA keys of a specified size.
     - Saves the public and private keys to files.
   - **Methods**:
     - `HandleAction()`: Generates keys and writes them to files.
     - `SetupInstance()`: Validates the key size and initializes the instance.
     - `IsNumber()`: Validates if a string is a valid number.

### 2. **SendMsg Class**
   - **Purpose**: Handles the process of sending an encrypted message to a recipient.
   - **Key Operations**:
     - Retrieves the recipient’s public key file.
     - Encrypts the message and sends it using an HTTP request.
   - **Methods**:
     - `HandleAction()`: Encrypts the message and sends it to the recipient.
     - `SetupInstance()`: Sets up the recipient email and plain text message.

### 3. **SendKey Class**
   - **Purpose**: Sends the public key to a recipient's email address.
   - **Key Operations**:
     - Retrieves and sends the public key to the recipient.
     - Updates the local private key with the recipient's email.
   - **Methods**:
     - `HandleAction()`: Sends the public key via HTTP.
     - `SetupInstance()`: Sets up the recipient's email address.

### 4. **GetKey Class**
   - **Purpose**: Retrieves a public key for a given email address.
   - **Key Operations**:
     - Retrieves the public key from a remote server.
     - Saves the retrieved key to a local file.
   - **Methods**:
     - `HandleAction()`: Retrieves and saves the public key.
     - `SetupInstance()`: Sets up the email address for key retrieval.

### 5. **GetMessage Class**
   - **Purpose**: Retrieves and decrypts an encrypted message for a given email address.
   - **Key Operations**:
     - Retrieves the encrypted message.
     - Decrypts the message using the associated private key.
   - **Methods**:
     - `HandleAction()`: Retrieves and decrypts the message.
     - `SetupInstance()`: Sets up the email address for message retrieval.

## Dependencies

- **RSA_**: Class responsible for RSA encryption key generation.
- **KeyManager**: Manages key storage and retrieval.
- **MessageEncryption & MessageDecryption**: Handles the encryption and decryption of messages.
- **HttpHandler**: Handles HTTP communication, such as sending messages and keys.
- **JsonMessage**: Used for structuring messages in JSON format.

## Example Usage

```bash
# Generate RSA keys with a key size of 2048 bits
dotnet run KeyGen 2048

# Send an encrypted message to a recipient
dotnet run SendMsg recipient@example.com "Hello, this is a secret message."

# Send the public key to a recipient's email
dotnet run SendKey recipient@example.com

# Retrieve the public key for a given email address
dotnet run GetKey recipient@example.com

# Retrieve and decrypt an encrypted message for the given email address
dotnet run GetMessage recipient@example.com
```

### Error Handling

The following error handling mechanisms are in place for the RSA CLI actions:

- **Key File Not Found**: If a key file is not found for a recipient, the operation will terminate with an error message indicating that the key file could not be located.
  
- **Public Key Retrieval Failure**: If the public key retrieval process fails, the operation will display a failure message, informing the user of the failure and suggesting potential next steps.

- **Message Decryption or Retrieval Failure**: If the decryption or retrieval of a message fails, an error message will be shown, including the relevant status code to help identify the issue.


### Notes

- **RSA Key Sizes**: RSA key sizes should be multiples of 8 (e.g., 2048, 4096).
  
- **Key Storage**: Public and private keys are stored in separate files, with the filenames being the recipient's email address. These files are used for encryption and decryption processes.














