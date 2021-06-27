// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Azure IoT Hub status.
    /// </summary>
    public class IoTHubStatus
    {
        internal IoTHubStatus(IoTHubStatus status)
        {
            Status = status.Status;
            Message = status.Message;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IoTHubStatus()
        { }

        /// <summary>
        /// The status.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// The associated message if any.
        /// </summary>
        public string Message { get; set; }
    }
}
