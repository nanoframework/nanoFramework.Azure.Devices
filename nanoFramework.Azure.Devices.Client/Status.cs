// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Azure IoT Hub status.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Connection happened.
        /// </summary>
        Connected,

        /// <summary>
        /// Disconnection happened.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Twin has been updated.
        /// </summary>
        TwinUpdated,

        /// <summary>
        /// Error updating the twins.
        /// </summary>
        TwinUpdateError,

        /// <summary>
        /// Twin received.
        /// </summary>
        TwinReceived,

        /// <summary>
        /// Twin update received.
        /// </summary>
        TwinUpdateReceived,

        /// <summary>
        /// IoT Hub Error.
        /// </summary>
        IoTHubError,

        /// <summary>
        /// IoT Hub Warning.
        /// </summary>
        IoTHubWarning,

        /// <summary>
        /// IoT Hub Information.
        /// </summary>
        IoTHubInformation,

        /// <summary>
        /// IoT Hub Highlight Information.
        /// </summary>
        IoTHubHighlightInformation,

        /// <summary>
        /// Internal SDK error.
        /// </summary>
        InternalError,

        /// <summary>
        /// Message received.
        /// </summary>
        MessageReceived,

        /// <summary>
        /// A direct method has been called.
        /// </summary>
        DirectMethodCalled,
    }
}
