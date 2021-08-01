// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace nanoFramework.Azure.Devices.Provisioning
{
    /// <summary>
    /// Registration operation status.
    /// </summary>
    internal class RegistrationOperationStatus
    {
        public const string Assigned = "assigned";
        public const string Assigning = "assigning";
        public const string Unassigned = "unassigned";
        public const string Disabled = "disabled";
        public const string Failed = "failed";

        public RegistrationOperationStatus()
        { }

        /// <summary>
        /// Gets or sets operation Id.
        /// </summary>
        public string operationId { get; set; }

        /// <summary>
        /// Gets or sets device enrollment status. Possible values include:
        /// 'unassigned', 'assigning', 'assigned', 'failed', 'disabled'
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The registration state
        /// </summary>
        public RegistrationState registrationState { get; set; }
    }

    internal class RegistrationOperationStatusSimple
    {
        public RegistrationOperationStatusSimple()
        { }

        /// <summary>
        /// Gets or sets operation Id.
        /// </summary>
        public string operationId { get; set; }

        /// <summary>
        /// Gets or sets device enrollment status. Possible values include:
        /// 'unassigned', 'assigning', 'assigned', 'failed', 'disabled'
        /// </summary>
        public string status { get; set; }
    }
}
