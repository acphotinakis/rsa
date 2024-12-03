using System.Numerics;
using KeyTypes;
using rsa.src.primekeygeneration.primeextension;

namespace RSA_
{
    public class RSA
    {

        /* The `RSA` constructor is initializing the RSA object with a specified number of bits. */
        public RSA(int bits)
        {
            Bits = bits;
            int pBits = (bits / 2) + GetRandomOffset(bits / 2);
            PSize = pBits;
            QSize = bits - pBits;
            P = PrimeExtension.GeneratePrime(PSize);
            Q = PrimeExtension.GeneratePrime(QSize);
            N = BigInteger.Multiply(P, Q);
            R = BigInteger.Multiply(BigInteger.Subtract(P, 1), BigInteger.Subtract(Q, 1));
            E = 65537; // Constant value 
            D = modInverse(E, R);
        }

        // Getter and Setter for pSize
        public int PSize { get; set; }

        // Getter and Setter for qSize
        public int QSize { get; set; }

        // Getter and Setter for bits
        public int Bits { get; set; }

        // Getter and Setter for p
        public BigInteger P { get; set; }

        // Getter and Setter for q
        public BigInteger Q { get; set; }

        // Getter and Setter for N
        public BigInteger N { get; set; }

        // Getter and Setter for r
        public BigInteger R { get; set; }

        // Getter and Setter for E
        public BigInteger E { get; set; }

        // Getter and Setter for D
        public BigInteger D { get; set; }

        /// <summary>
        /// The function generates a public key by converting the values of E and N to byte arrays,
        /// concatenating them, and then base64 encoding the resulting byte array.
        /// </summary>
        /// <returns>
        /// The method is returning a PublicKey object.
        /// </returns>
        public PublicKey GeneratePublicKey()
        {

            int e = E.GetByteCount();
            byte[] eBytes = BitConverter.GetBytes(e);
            Array.Reverse(eBytes);

            byte[] EBytes = E.ToByteArray();

            int n = N.GetByteCount();
            byte[] nBytes = BitConverter.GetBytes(n);
            Array.Reverse(nBytes);

            byte[] NBytes = N.ToByteArray();

            // Construct the byte array for the public key
            byte[] publicKeyBytes = new byte[4 + E.GetByteCount() + 4 + N.GetByteCount()];

            Array.Copy(eBytes, publicKeyBytes, 4);
            Array.Copy(EBytes, 0, publicKeyBytes, 4, E.GetByteCount());
            Array.Copy(nBytes, 0, publicKeyBytes, 4 + E.GetByteCount(), 4);
            Array.Copy(NBytes, 0, publicKeyBytes, 4 + E.GetByteCount() + 4, N.GetByteCount());

            // Base64 encode the byte array
            string base64EncodedKey = Convert.ToBase64String(publicKeyBytes);

            KeyTypes.PublicKey publicKey = new KeyTypes.PublicKey("", base64EncodedKey);

            return publicKey;
        }

        /// <summary>
        /// The function generates a private key by converting the values of D and N to byte arrays,
        /// concatenating them, and then base64 encoding the resulting byte array.
        /// </summary>
        /// <returns>
        /// The method is returning a PrivateKey object.
        /// </returns>
        public PrivateKey GeneratePrivateKey()
        {
            int d = D.GetByteCount();
            byte[] dBytes = BitConverter.GetBytes(d);
            Array.Reverse(dBytes);
            byte[] DBytes = D.ToByteArray();

            int n = N.GetByteCount();
            byte[] nBytes = BitConverter.GetBytes(n);
            Array.Reverse(nBytes);

            byte[] NBytes = N.ToByteArray();

            // Construct the byte array for the private key
            byte[] privateKeyBytes = new byte[4 + D.GetByteCount() + 4 + N.GetByteCount()];

            Array.Copy(dBytes, privateKeyBytes, 4);
            Array.Copy(DBytes, 0, privateKeyBytes, 4, D.GetByteCount());
            Array.Copy(nBytes, 0, privateKeyBytes, 4 + D.GetByteCount(), 4);
            Array.Copy(NBytes, 0, privateKeyBytes, 4 + D.GetByteCount() + 4, N.GetByteCount());

            // Base64 encode the byte array
            string base64EncodedKey = Convert.ToBase64String(privateKeyBytes);

            KeyTypes.PrivateKey privateKey = new KeyTypes.PrivateKey(new List<String>(), base64EncodedKey);

            return privateKey;
        }

        /// <summary>
        /// Calculates the modular multiplicative inverse of a number 'a' modulo 'n'.
        /// </summary>
        /// <param name="a">The number for which the modular inverse is to be calculated.</param>
        /// <param name="n">The modulo value.</param>
        /// <returns>
        /// The modular multiplicative inverse of 'a' modulo 'n'.
        /// If the modular inverse does not exist, returns 0.
        /// </returns>
        static BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        private static int GetRandomOffset(int size)
        {
            Random random = new Random();
            return random.Next((int)(size * 0.2), (int)(size * 0.3));
        }

    }
}
