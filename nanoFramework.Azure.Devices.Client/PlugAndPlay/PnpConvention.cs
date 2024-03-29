﻿//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Azure.Devices.Provisioning.Client.PlugAndPlay
{
    /// <summary>
    /// A helper class for formatting the DPS device registration payload, per plug and play convention.
    /// </summary>
    public static class PnpConvention
    {
        /// <summary>
        /// Create the DPS payload to provision a device as plug and play.
        /// </summary>
        /// <remarks>
        /// For more information on device provisioning service and plug and play compatibility,
        /// and PnP device certification, see <see href="https://docs.microsoft.com/azure/iot-pnp/howto-certify-device"/>.
        /// The DPS payload should be in the format:
        /// <c>
        /// {
        ///   "modelId": "dtmi:com:example:modelName;1"
        /// }
        /// </c>
        /// For information on DTDL, see <see href="https://github.com/Azure/opendigitaltwins-dtdl/blob/master/DTDL/v2/dtdlv2.md"/>
        /// </remarks>
        /// <param name="modelId">The Id of the model the device adheres to for properties, telemetry, and commands.</param>
        /// <returns>The DPS payload to provision a device as plug and play.</returns>
        /// <exception cref="ArgumentNullException">If modelId is <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        public static string CreateDpsPayload(string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                throw new ArgumentNullException();
            }
            
            return $"{{\"modelId\":\"{modelId}\"}}";
        }
    }
}
