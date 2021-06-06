// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Microsoft.Azure.Devices.Client.Extensions;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Authentication method that uses the symmetric key associated with the device in the device registry.
    /// </summary>
    public sealed class DeviceAuthenticationWithRegistrySymmetricKey : IAuthenticationMethod
    {
        private string _deviceId;
        private string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceAuthenticationWithRegistrySymmetricKey"/> class.
        /// </summary>
        /// <param name="deviceId">Device identifier.</param>
        /// <param name="key">Symmetric key associated with the device.</param>
        public DeviceAuthenticationWithRegistrySymmetricKey(string deviceId, string key)
        {
            SetDeviceId(deviceId);
            SetKeyFromBase64String(key);
        }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        public string DeviceId
        {
            get => _deviceId;
            set => SetDeviceId(value);
        }

        /// <summary>
        /// Gets or sets the key associated with the device.
        /// </summary>
        public byte[] Key
        {
            get => Convert.FromBase64String(_key);
            set => SetKey(value);
        }

        /// <summary>
        /// Gets or sets the Base64 formatted key associated with the device.
        /// </summary>
        public string KeyAsBase64String
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// Populates an <see cref="IotHubConnectionStringBuilder"/> instance based on the properties of the current instance.
        /// </summary>
        /// <param name="iotHubConnectionStringBuilder">Instance to populate.</param>
        /// <returns>The populated <see cref="IotHubConnectionStringBuilder"/> instance.</returns>
        public IotHubConnectionStringBuilder Populate(IotHubConnectionStringBuilder iotHubConnectionStringBuilder)
        {
            if (iotHubConnectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(iotHubConnectionStringBuilder));
            }

            Debug.WriteLine("Populate");
            iotHubConnectionStringBuilder.DeviceId = DeviceId;
            iotHubConnectionStringBuilder.SharedAccessKey = KeyAsBase64String;
            iotHubConnectionStringBuilder.SharedAccessKeyName = null;
            iotHubConnectionStringBuilder.SharedAccessSignature = null;

            return iotHubConnectionStringBuilder;
        }

        private void SetKey(byte[] key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            _key = Convert.ToBase64String(key);
        }

        private void SetKeyFromBase64String(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!StringValidationHelper.IsBase64String(key))
            {
                throw new ArgumentException("Key must be base64 encoded");
            }
            
            _key = key;
        }

        private void SetDeviceId(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            _deviceId = deviceId;
        }
    }
}
