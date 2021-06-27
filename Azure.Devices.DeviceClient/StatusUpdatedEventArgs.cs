// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Delegate method for status update.
    /// </summary>
    /// <param name="sender">The <see cref="DeviceClient"/> sender.</param>
    /// <param name="e">The status updated arguments.</param>
    public delegate void StatusUpdated(object sender, StatusUpdatedEventArgs e);

    /// <summary>
    /// Status updated arguments.
    /// </summary>
    public class StatusUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for status updated arguments.
        /// </summary>
        /// <param name="status">The status.</param>
        public StatusUpdatedEventArgs(IoTHubStatus status)
        {
            IoTHubStatus = new IoTHubStatus(status);
        }

        /// <summary>
        /// The IoT Hub status.
        /// </summary>
        public IoTHubStatus IoTHubStatus { get; set; }
    }
}
