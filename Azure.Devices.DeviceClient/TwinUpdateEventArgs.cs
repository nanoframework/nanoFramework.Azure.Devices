// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Azure.Devices.Client;
using System;

namespace nanoFramework.Azure.Devices.Shared
{
    /// <summary>
    /// Delegate for Twin updated.
    /// </summary>
    /// <param name="sender">The <see cref="DeviceClient"/> sender.</param>
    /// <param name="e">The Twin updated event arguments.</param>
    public delegate void TwinUpdated(object sender, TwinUpdateEventArgs e);

    /// <summary>
    /// Twin updated event arguments.
    /// </summary>
    public class TwinUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for Twin updated event arguments.
        /// </summary>
        /// <param name="twin">The twin collection.</param>
        public TwinUpdateEventArgs(TwinCollection twin)
        {
            Twin = twin;
        }

        /// <summary>
        /// Twin collection.
        /// </summary>
        public TwinCollection Twin { get; set; }
    }
}
