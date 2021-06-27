// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Method call back delegate.
    /// </summary>
    /// <param name="rid">The request ID.</param>
    /// <param name="payload">The payload.</param>
    /// <returns></returns>
    public delegate string MethodCalback(int rid, string payload);
}
