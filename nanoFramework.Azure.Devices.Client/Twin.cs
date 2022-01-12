﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using nanoFramework.Json;

namespace nanoFramework.Azure.Devices.Shared
{
    /// <summary>
    /// Twin Representation.
    /// </summary>
    public class Twin
    {
        /// <summary>
        /// Creates an instance of <see cref="Twin"/>.
        /// </summary>
        public Twin()
        {
            Properties = new TwinProperties();
        }

        /// <summary>
        /// Creates an instance of <see cref="Twin"/>.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        public Twin(string deviceId) : this()
        {
            DeviceId = deviceId;
        }

        /// <summary>
        /// Creates an instance of <see cref="Twin"/>.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        /// <param name="jsonTwin">The json twin.</param>
        public Twin(string deviceId, string jsonTwin)
        {
            DeviceId = deviceId;
            Hashtable props = (Hashtable)JsonConvert.DeserializeObject(jsonTwin, typeof(Hashtable));
            Properties = new TwinProperties((Hashtable)props["desired"], (Hashtable)props["reported"]);
        }

        /// <summary>
        /// Creates an instance of <see cref="Twin"/>.
        /// </summary>
        /// <param name="twinProperties">The twin properties.</param>
        public Twin(TwinProperties twinProperties)
        {
            Properties = twinProperties;
        }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> Id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// The DTDL model Id of the device.
        /// </summary>
        /// <remarks>
        /// The value will be null for a non-pnp device.
        /// The value will be null for a pnp device until the device connects and registers with the model Id.
        /// </remarks>
        public string ModelId { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> properties.
        /// </summary>
        public TwinProperties Properties { get; set; }

        /// <summary>
        /// Twin's Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets the Twin as a JSON string.
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            Hashtable ser = new();
            ser.Add("properties", Properties);

            if (!string.IsNullOrEmpty(ModelId))
            {
                ser.Add("modelid", ModelId);
            }

            if (!string.IsNullOrEmpty(DeviceId))
            {
                ser.Add("deviceid", DeviceId);
            }
            
            if (Version !=0)
            {
                ser.Add("$version", Version);
            }

            return JsonConvert.SerializeObject(ser);
        }
    }
}
