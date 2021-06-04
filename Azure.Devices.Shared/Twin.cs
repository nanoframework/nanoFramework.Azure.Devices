// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Twin Representation
    /// </summary>
    public class Twin
    {
        /// <summary>
        /// Creates an instance of <see cref="Twin"/>
        /// </summary>
        public Twin()
        {
            Tags = new TwinCollection();
            Properties = new TwinProperties();
        }

        /// <summary>
        /// Creates an instance of <see cref="Twin"/>
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        public Twin(string deviceId) : this()
        {
            DeviceId = deviceId;
        }

        /// <summary>
        /// Creates an instance of <see cref="Twin"/>
        /// </summary>
        /// <param name="twinProperties"></param>
        public Twin(TwinProperties twinProperties)
        {
            Tags = new TwinCollection();
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
        /// Gets and sets the <see cref="Twin" /> Module Id.
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> tags.
        /// </summary>
        public TwinCollection Tags { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> properties.
        /// </summary>
        public TwinProperties Properties { get; set; }

        /// <summary>
        /// Twin's ETag
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Twin's Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets the Twin as a JSON string
        /// </summary>
        /// <param name="formatting">Optional. Formatting for the output JSON string.</param
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
