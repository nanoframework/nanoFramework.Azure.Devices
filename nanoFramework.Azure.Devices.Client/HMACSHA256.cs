//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

//  
// Originally by Elze Kool see http://www.microframework.nl/2009/09/05/shahmac-digest-class/ 
// Adjustments by Laurent Ellerbach laurelle@microsoft.com 2021/05/31
//   

namespace System.Security.Cryptography
{
    using System;

    /// <summary> 
    /// Computes a Hash-based Message Authentication Code (HMAC) by using the System.Security.Cryptography.SHA256 
    /// hash function.
    /// </summary> 
    public class HMACSHA256
    {
        /// <summary>
        /// Gets or sets the key to use in the HMAC calculation.
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Initializes a new instance of the System.Security.Cryptography.HMACSHA256 class
        /// with the specified key data.
        /// </summary>
        /// <param name="key">The secret key for System.Security.Cryptography.HMACSHA256 encryption. The key
        /// can be any length. However, the recommended size is 64 bytes max.</param>
        public HMACSHA256(byte[] key)
        {
            Key = key;
        }

        /// <summary>
        /// Computes the hash value for the specified byte array.
        /// </summary>
        /// <param name="buffer">The input to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        /// <exception cref="ArgumentNullException">buffer is null.</exception>
        public byte[] ComputeHash(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            return ComputeHMACSHA256(Key, buffer);
        }

        /// <summary> 
        ///   Compute HMAC SHA-256 
        /// </summary> 
        /// <param name = "secret">Secret</param> 
        /// <param name = "value">Password</param> 
        /// <returns>32 byte HMAC_SHA256</returns> 
        private static byte[] ComputeHMACSHA256(byte[] secret, byte[] value)
        {
            // Create two arrays, bi and bo 
            var bi = new byte[64 + value.Length];
            var bo = new byte[64 + 32];

            // Copy secret to both arrays 
            Array.Copy(secret, bi, secret.Length);
            Array.Copy(secret, bo, secret.Length);

            for (var i = 0; i < 64; i++)
            {
                // Xor bi with 0x36 
                bi[i] = (byte)(bi[i] ^ 0x36);

                // Xor bo with 0x5c 
                bo[i] = (byte)(bo[i] ^ 0x5c);
            }

            // Append value to bi 
            Array.Copy(value, 0, bi, 64, value.Length);

            var sha256 = SHA256.Create();
            // Append SHA256(bi) to bo 
            var sha_bi = sha256.ComputeHash(bi);
            Array.Copy(sha_bi, 0, bo, 64, 32);

            // Return SHA256(bo) 
            return sha256.ComputeHash(bo);
        }        
    }
}
