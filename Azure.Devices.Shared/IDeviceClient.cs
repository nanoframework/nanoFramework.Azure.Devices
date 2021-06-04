using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading;

namespace Microsoft.Azure.Devices.Client
{
    public interface IDeviceClient : IDisposable
    {
                /// <summary>
        /// Stores the timeout used in the operation retries. Note that this value is ignored for operations
        /// where a cancellation token is provided. For example, <see cref="SendEventAsync(Message)"/> will use this timeout, but
        /// <see cref="SendEventAsync(Message, CancellationToken)"/> will not. The latter operation will only be canceled by the
        /// provided cancellation token.
        /// </summary>
        public uint OperationTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// Sets the retry policy used in the operation retries.
        /// The change will take effect after any in-progress operations.
        /// </summary>
        /// <param name="retryPolicy">The retry policy. The default is
        /// <c>new ExponentialBackoff(int.MaxValue, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));</c></param>
        // Codes_SRS_DEVICECLIENT_28_001: [This property shall be defaulted to the exponential retry strategy with back-off
        // parameters for calculating delay in between retries.]
        public void SetRetryPolicy(IRetryPolicy retryPolicy);

        /// <summary>
        /// Open the DeviceClient instance.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void Open(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Close the DeviceClient instance.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void Close(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Receive a message from the device queue using the default timeout.
        /// After handling a received message, a client should call <see cref="CompleteAsync(Message)"/>,
        /// <see cref="AbandonAsync(Message)"/>, or <see cref="RejectAsync(Message)"/>, and then dispose the message.
        /// </summary>
        /// <remarks>
        /// You cannot Reject or Abandon messages over MQTT protocol.
        /// For more details, see https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d#the-cloud-to-device-message-life-cycle.
        /// </remarks>
        /// <returns>The receive message or null if there was no message until the default timeout</returns>
        public Message Receive(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a received message from the device queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The previously received message</returns>
        public void Complete(Message message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a received message from the device queue
        /// </summary>
        /// <param name="lockToken">The message lockToken.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The lock identifier for the previously received message</returns>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void Complete(string lockToken, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Puts a received message back onto the device queue
        /// </summary>
        /// <remarks>
        /// You cannot Abandon a message over MQTT protocol.
        /// For more details, see https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d#the-cloud-to-device-message-life-cycle.
        /// </remarks>
        /// <param name="lockToken">The message lockToken.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The previously received message</returns>
        public void Abandon(string lockToken, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Puts a received message back onto the device queue
        /// </summary>
        /// <remarks>
        /// You cannot Abandon a message over MQTT protocol.
        /// For more details, see https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d#the-cloud-to-device-message-life-cycle.
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The lock identifier for the previously received message</returns>
        public void Abandon(Message message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a received message from the device queue and indicates to the server that the message could not be processed.
        /// </summary>
        /// <remarks>
        /// You cannot Reject a message over MQTT protocol.
        /// For more details, see https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d#the-cloud-to-device-message-life-cycle.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="lockToken">The message lockToken.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The previously received message</returns>
        public void Reject(string lockToken, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a received message from the device queue and indicates to the server that the message could not be processed.
        /// </summary>
        /// <remarks>
        /// You cannot Reject a message over MQTT protocol.
        /// For more details, see https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d#the-cloud-to-device-message-life-cycle.
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The lock identifier for the previously received message</returns>
        public void Reject(Message message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sends an event to a hub
        /// </summary>
        /// <param name="message">The message to send. Should be disposed after sending.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The task to await</returns>
        public void SendEvent(Message message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets a new delegate for the named method. If a delegate is already associated with
        /// the named method, it will be replaced with the new delegate.
        /// A method handler can be unset by passing a null MethodCallback.
        /// <param name="methodName">The name of the method to associate with the delegate.</param>
        /// <param name="methodHandler">The delegate to be used when a method with the given name is called by the cloud service.</param>
        /// <param name="userContext">generic parameter to be interpreted by the client code.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// </summary>
        public void SetMethodHandler(string methodName, MethodCallback methodHandler, object userContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets a new delegate that is called for a method that doesn't have a delegate registered for its name.
        /// If a default delegate is already registered it will replace with the new delegate.
        /// A method handler can be unset by passing a null MethodCallback.
        /// </summary>
        /// <param name="methodHandler">The delegate to be used when a method is called by the cloud service and there is no delegate registered for that method name.</param>
        /// <param name="userContext">Generic parameter to be interpreted by the client code.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void SetMethodDefaultHandler(MethodCallback methodHandler, object userContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set a callback that will be called whenever the client receives a state update
        /// (desired or reported) from the service.
        /// Set callback value to null to clear.
        /// </summary>
        /// <remarks>
        /// This has the side-effect of subscribing to the PATCH topic on the service.
        /// </remarks>
        /// <param name="callback">Callback to call after the state update has been received and applied</param>
        /// <param name="userContext">Context object that will be passed into callback</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void SetDesiredPropertyUpdateCallback(DesiredPropertyUpdateCallback callback, object userContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieve the device twin properties for the current device.
        /// For the complete device twin object, use Microsoft.Azure.Devices.RegistryManager.GetTwinAsync(string deviceId).
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        /// <returns>The device twin object for the current device</returns>
        public Twin GetTwinAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Push reported property changes up to the service.
        /// </summary>
        /// <param name="reportedProperties">Reported properties to push</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="OperationCanceledException">Thrown when the operation has been canceled.</exception>
        public void UpdateReportedPropertiesAsync(TwinCollection reportedProperties, CancellationToken cancellationToken = default(CancellationToken));

        public void Dispose();        
    }
}
