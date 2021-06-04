// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices
{
    /// <summary>
    /// Specifies the different states of a device.
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// Indicates that a Device is enabled
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// Indicates that a Device is disabled
        /// </summary>
        Disabled,
    }
}