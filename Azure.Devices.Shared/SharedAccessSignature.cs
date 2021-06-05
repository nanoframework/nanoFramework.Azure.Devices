// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client.Extensions;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.Azure.Devices.Client
{
    internal sealed class SharedAccessSignature
    {
        private readonly string _encodedAudience;
        private readonly string _expiry;

        private SharedAccessSignature(string iotHubName, DateTime expiresOn, string expiry, string keyName, string signature, string encodedAudience)
        {
            if (string.IsNullOrEmpty(iotHubName))
            {
                throw new ArgumentNullException(nameof(iotHubName));
            }

            ExpiresOn = expiresOn;

            if (IsExpired())
            {
                throw new Exception($"The specified SAS token is expired on {ExpiresOn}.");
            }

            IotHubName = iotHubName;
            Signature = signature;
            Audience = HttpUtility.UrlDecode(encodedAudience);
            _encodedAudience = encodedAudience;
            _expiry = expiry;
            KeyName = keyName ?? string.Empty;
        }

        public string IotHubName { get; }

        public DateTime ExpiresOn { get; private set; }

        public string KeyName { get; private set; }

        public string Audience { get; private set; }

        public string Signature { get; private set; }

        public static SharedAccessSignature Parse(string iotHubName, string rawToken)
        {
            if (string.IsNullOrEmpty(iotHubName))
            {
                throw new ArgumentNullException(nameof(iotHubName));
            }

            if (string.IsNullOrEmpty(rawToken))
            {
                throw new ArgumentNullException(nameof(rawToken));
            }

            Hashtable parsedFields = ExtractFieldValues(rawToken);

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.SignatureFieldName, out string signature))
            {
                throw new FormatException($"Missing field: {SharedAccessSignatureConstants.SignatureFieldName}");
            }

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.ExpiryFieldName, out string expiry))
            {
                throw new FormatException($"Missing field: {SharedAccessSignatureConstants.ExpiryFieldName}");
            }

            // KeyName (skn) is optional.
            parsedFields.TryGetValue(SharedAccessSignatureConstants.KeyNameFieldName, out string keyName);

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.AudienceFieldName, out string encodedAudience))
            {
                throw new FormatException($"Missing field: {SharedAccessSignatureConstants.AudienceFieldName}");
            }

            return new SharedAccessSignature(iotHubName, DateTime.FromUnixTimeSeconds((long)double.Parse(expiry)), expiry, keyName, signature, encodedAudience);
        }

        public static bool IsSharedAccessSignature(string rawSignature)
        {
            if (string.IsNullOrEmpty(rawSignature))
            {
                return false;
            }

            try
            {
                Hashtable parsedFields = ExtractFieldValues(rawSignature);
                bool isSharedAccessSignature = parsedFields.TryGetValue(SharedAccessSignatureConstants.SignatureFieldName, out string signature);
                return isSharedAccessSignature;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool IsExpired()
        {
            return ExpiresOn + SharedAccessSignatureConstants.MaxClockSkew < DateTime.UtcNow;
        }

        public string ComputeSignature(byte[] key)
        {
            string value = $"{_encodedAudience}\n{_expiry}";
            return Sign(key, value);
        }

        internal static string Sign(byte[] key, string value)
        {
            var algorithm = new HMACSHA256(key);

            return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        private static Hashtable ExtractFieldValues(string sharedAccessSignature)
        {
            string[] lines = sharedAccessSignature.Split();

            if (!string.Equals(lines[0].Trim(), SharedAccessSignatureConstants.SharedAccessSignature) || lines.Length != 2)
            {
                throw new FormatException("Malformed signature");
            }

            Hashtable parsedFields = new();
            string[] fields = lines[1].Trim().Split(SharedAccessSignatureConstants.PairSeparator);

            foreach (string field in fields)
            {
                if (!string.IsNullOrEmpty(field))
                {
                    string[] fieldParts = field.Split(SharedAccessSignatureConstants.KeyValueSeparator);
                    if (string.Equals(fieldParts[0], SharedAccessSignatureConstants.AudienceFieldName))
                    {
                        // We need to preserve the casing of the escape characters in the audience,
                        // so defer decoding the URL until later.
                        parsedFields.Add(fieldParts[0], fieldParts[1]);
                    }
                    else
                    {
                        parsedFields.Add(fieldParts[0], HttpUtility.UrlDecode(fieldParts[1]));
                    }
                }
            }

            return parsedFields;
        }
    }
}
