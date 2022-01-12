// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Status code for the Azure Plug and Play reported values.
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>Completed</summary>
        Completed = 200,

        /// <summary>InProgress</summary>
        InProgress = 202,

        /// <summary>NotFound</summary>
        NotFound = 404,

        /// <summary>BadRequest</summary>
        BadRequest = 400,

        /// <summary>BadRequest</summary>
        InternalError = 500
    }
}
