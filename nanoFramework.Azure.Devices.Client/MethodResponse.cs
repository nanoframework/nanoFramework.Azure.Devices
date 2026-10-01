//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// The response to a direct method call.
    /// </summary>
    public class MethodResponse
    {
        /// <summary>
        /// Gets the status code returned to the caller (ex: 200 for OK, 202 for accepted).
        /// </summary>
        public int Status { get; }

        /// <summary>
        /// Gets the payload returned to the caller. It must be valid JSON. <see langword="null"/> returns an empty payload.
        /// </summary>
        public string Payload { get; }

        /// <summary>
        /// Creates a <see cref="MethodResponse"/>.
        /// </summary>
        /// <param name="status">The status code returned to the caller (ex: 200 for OK, 202 for accepted).</param>
        /// <param name="payload">The payload returned to the caller. It must be valid JSON. <see langword="null"/> returns an empty payload.</param>
        public MethodResponse(int status, string payload = null)
        {
            Status = status;
            Payload = payload;
        }
    }
}
