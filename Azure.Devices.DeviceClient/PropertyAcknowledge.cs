// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Property 
    /// </summary>
    public class PropertyAcknowledge
    {
        private const string TwinVersion = "av";
        private const string TwinDescription = "ad";
        private const string TwinStatus = "ac";
        private const string TwinValue = "value";

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status;
        /// </summary>
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// The value to report.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Bluid the acknowledge to return to IoT plug and play.
        /// </summary>
        /// <returns>A hashtable that will be serialized.</returns>
        public Hashtable BuildAcknowledge()
        {
            Hashtable toReport = new();
            toReport.Add(TwinVersion, Version);
            toReport.Add(TwinDescription, Description);
            toReport.Add(TwinStatus, Status);
            toReport.Add(TwinValue, Value);
            return toReport;
        }
    }
}
