using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zen
{
    /// <summary>
    /// Cryptographic class for encryption and decryption of string values.
    /// </summary>
    public static class Crypto
    {
        // Arbitrary key and iv vector. 
        // You will want to generate (and protect) your own when using encryption.
        private const string DefaultKey = "EA81AA1D5FC1EC53E84F30AA746139EEBAFF8A9B76638895";
        private const string DefaultIV = "87AF7EA221F3FFF5";

        private static readonly TripleDESCryptoServiceProvider TripleDes3;

        /// <summary>
        /// Default constructor. Initializes the DES3 encryption provider. 
        /// </summary>
        static Crypto()
        {
            TripleDes3 = new TripleDESCryptoServiceProvider {Mode = CipherMode.CBC};
        }

        /// <summary>
        /// Generates a 24 byte Hex key.
        /// </summary>
        public static string GenerateKey()
        {            
            TripleDes3.GenerateKey();// Length is 24
            return BytesToHex(TripleDes3.Key);
        }

        /// <summary>
        /// Generates an 8 byte Hex IV (Initialization Vector).
        /// </summary>
        public static string GenerateIV()
        {
            // Length = 8
            TripleDes3.GenerateIV();
            return BytesToHex(TripleDes3.IV);
        }

        /// <summary>
        /// Encrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="iv">Encryption initialization vector.</param>
        /// <returns>Encrypted string.</returns>
        public static string Encrypt(string data, string key, string iv)
        {
            byte[] bdata = Encoding.ASCII.GetBytes(data);
            byte[] bkey = HexToBytes(key);
            byte[] biv = HexToBytes(iv);

            var stream = new MemoryStream();
            var encStream = new CryptoStream(stream,
                TripleDes3.CreateEncryptor(bkey, biv), CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return BytesToHex(stream.ToArray());
        }

        /// <summary>
        /// Decrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <param name="key">Original encryption key.</param>
        /// <param name="iv">Original initialization vector.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(string data, string key, string iv)
        {
            byte[] bdata = HexToBytes(data);
            byte[] bkey = HexToBytes(key);
            byte[] biv = HexToBytes(iv);

            var stream = new MemoryStream();
            var encStream = new CryptoStream(stream,
                TripleDes3.CreateDecryptor(bkey, biv), CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return Encoding.ASCII.GetString(stream.ToArray());
        }

        /// <summary>
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string data)
        {
            return Encrypt(data, DefaultKey, DefaultIV);
        }

        /// <summary>
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(string data)
        {
            return Decrypt(data, DefaultKey, DefaultIV);
        }


        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        private static byte[] HexToBytes(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length / 2; i++)
            {
                var code = hex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(code, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        private static string BytesToHex(IEnumerable<byte> bytes)
        {
            var hex = new StringBuilder();
            foreach (var b in bytes)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
    }
}