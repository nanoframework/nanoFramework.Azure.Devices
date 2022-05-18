// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace nanoFramework.Azure.Devices
{
    internal static class Helper
    {
        public static string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            // http://msdn.microsoft.com/en-us/library/azure/dn170477.aspx
            // the canonical Uri scheme is http because the token is not amqp specific
            // signature is computed from joined encoded request Uri string and expiry string

            var exp = DateTime.UtcNow.ToUnixTimeSeconds() + (long)tokenTimeToLive.TotalSeconds;

            string expiry = exp.ToString();
            string encodedUri = HttpUtility.UrlEncode(resource);

            var hmacsha256 = new HMACSHA256(Convert.FromBase64String(sharedAccessKey));
            byte[] hmac = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(encodedUri + "\n" + expiry));
            string sig = Convert.ToBase64String(hmac);

            if (keyName != null)
            {
                return String.Format(
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                encodedUri,
                HttpUtility.UrlEncode(sig),
                HttpUtility.UrlEncode(expiry),
                HttpUtility.UrlEncode(keyName));
            }
            else
            {
                return String.Format(
                    "SharedAccessSignature sr={0}&sig={1}&se={2}",
                    encodedUri,
                    HttpUtility.UrlEncode(sig),
                    HttpUtility.UrlEncode(expiry));
            }
        }

        public static void ComposeTelemetryInformation(ref string userName)
        {
            // compose product info
            string productInfo = "nano;";
            productInfo += $"azrsdk{ThisAssembly.AssemblyVersion};";
            productInfo += $"{Runtime.Native.SystemInfo.TargetName}";

            // add to user name
            userName += $"&DeviceClientType={HttpUtility.UrlEncode(productInfo)}";
        }
    }
}
