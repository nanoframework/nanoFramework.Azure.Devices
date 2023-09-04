// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Web.Private;

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Cloud to device delegate function used for callback.
    /// </summary>
    /// <param name="sender">The <see cref="DeviceClient"/> class sender.</param>
    /// <param name="e">The device message event arguments.</param>
    public delegate void CloudToDeviceMessage(object sender, CloudToDeviceMessageEventArgs e);

    /// <summary>
    /// The device message event arguments.
    /// </summary>
    public class CloudToDeviceMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for device message event arguments.
        /// </summary>
        /// <param name="message">The string message.</param>
        /// <param name="properties">The properties.</param>
        public CloudToDeviceMessageEventArgs(string message, string properties)
        {
            Message = message;
            Properties = new Hashtable();
            string decoded = HttpUtility.UrlDecode(properties);
            // The properties look like that encoded and decoded:
            // %24.cid=aaaaa&%24.mid=idgenrated&%24.to=%2Fdevices%2FnanoEdgeTwin%2Fmessages%2Fdevicebound&%24.ct=aaaaa&%24.ce=utf-8&iothub-ack=positive&prop2=%20&prop3=42&prop4=something
            // $.cid=aaaaa&$.mid=idgenrated&$.to=/devices/nanoEdgeTwin/messages/devicebound&$.ct=aaaaa&$.ce=utf-8&iothub-ack=positive&prop2= &prop3=42&prop4=something
            if (!string.IsNullOrEmpty(decoded))
            {
                var props = decoded.Split('&');
                foreach (string prop in props)
                {
                    var items = prop.Split('=');
                    if (items.Length == 1)
                    {
                        Properties.Add(items[0], null);
                    }
                    else
                    {
                        Properties.Add(items[0], items[1]);
                    }
                }
            }
        }

        /// <summary>
        /// The properties.
        /// </summary>
        public Hashtable Properties { get; set; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; set; }

    }
}
