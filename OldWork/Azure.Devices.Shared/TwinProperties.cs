// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Represents <see cref="Twin"/> properties
    /// </summary>
    public class TwinProperties
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TwinProperties"/>
        /// </summary>
        public TwinProperties()
        {
            Desired = new TwinCollection();
            Reported = new TwinCollection();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TwinProperties"/>
        /// </summary>
        /// <param name="desired">Hashtable for the desired properties</param>
        /// <param name="reported">Hashtable for the reported properties</param>
        public TwinProperties(Hashtable desired, Hashtable reported)
        {
            Desired = new TwinCollection(desired);
            Reported = new TwinCollection(reported);
        }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> desired properties.
        /// </summary>
        public TwinCollection Desired { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> reported properties.
        /// </summary>
        public TwinCollection Reported { get; set; }
    }
}

