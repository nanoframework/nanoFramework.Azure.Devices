using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading;

namespace Microsoft.Azure.Devices.Client
{
    public class DeviceClient : IDeviceClient
    {
        /// <summary>
        /// Default operation timeout.
        /// </summary>
        public const uint DefaultOperationTimeoutInMilliseconds = 4 * 60 * 1000;
        protected IRetryPolicy _retryPolicy;

        protected DeviceClient(string connectionString)
        {

        }

        protected DeviceClient(string connectionString, TransportType transportType)
        {

        }

        protected static DeviceClient CreateFromConnectionString(string connectionString, TransportType transportType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the timeout used in the operation retries. Note that this value is ignored for operations
        /// where a cancellation token is provided. For example, <see cref="SendEventAsync(Message)"/> will use this timeout, but
        /// <see cref="SendEventAsync(Message, CancellationToken)"/> will not. The latter operation will only be canceled by the
        /// provided cancellation token.
        /// </summary>
        protected uint OperationTimeoutInMilliseconds { get; set; }
        uint IDeviceClient.OperationTimeoutInMilliseconds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Sets the retry policy used in the operation retries.
        /// The change will take effect after any in-progress operations.
        /// </summary>
        /// <param name="retryPolicy">The retry policy. The default is
        /// <c>new ExponentialBackoff(int.MaxValue, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));</c></param>
        // Codes_SRS_DEVICECLIENT_28_001: [This property shall be defaulted to the exponential retry strategy with back-off
        // parameters for calculating delay in between retries.]
        public void SetRetryPolicy(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Close(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Message Receive(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Complete(Message message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Complete(string lockToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Abandon(string lockToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Abandon(Message message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Reject(string lockToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Reject(Message message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SendEvent(Message message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetMethodHandler(string methodName, MethodCallback methodHandler, object userContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetMethodDefaultHandler(MethodCallback methodHandler, object userContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetDesiredPropertyUpdateCallback(DesiredPropertyUpdateCallback callback, object userContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Twin GetTwinAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateReportedPropertiesAsync(TwinCollection reportedProperties, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
