// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace nanoFramework.Azure.Devices.Provisioning
{
    /// <summary>
    /// Used for the deserialization of the registration message
    /// </summary>
    internal class RegistrationState
    {
        public string registrationId { get; set; }

        public DateTime createdDateTimeUtc { get; set; }

        public string assignedHub { get; set; }

        public string deviceId { get; set; }

        public string status { get; set; }

        public string substatus { get; set; }

        public DateTime lastUpdatedDateTimeUtc { get; set; }

        public string etag { get; set; }

        public int errorCode { get; set; }

        public string errorMessage { get; set; }

        public string payload { get; set; }
    }

}
