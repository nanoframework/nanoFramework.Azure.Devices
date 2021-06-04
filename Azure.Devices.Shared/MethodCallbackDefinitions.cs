using Microsoft.Azure.Devices.Shared;
using System;
using System.Text;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Delegate for method call. This will be called every time we receive a method call that was registered.
    /// </summary>
    /// <remarks>
    /// This can be set for both <see cref="DeviceClient"/> and <see cref="ModuleClient"/>.
    /// </remarks>
    /// <param name="methodRequest">Class with details about method.</param>
    /// <param name="userContext">Context object passed in when the callback was registered.</param>
    /// <returns>MethodResponse</returns>
    public delegate MethodResponse MethodCallback(MethodRequest methodRequest, object userContext);

    /// <summary>
    /// Delegate for connection status changed.
    /// </summary>
    /// <remarks>
    /// This can be set for both <see cref="DeviceClient"/> and <see cref="ModuleClient"/>.
    /// </remarks>
    /// <param name="status">The updated connection status</param>
    /// <param name="reason">The reason for the connection status change</param>
    public delegate void ConnectionStatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason);

    /// <summary>
    /// Delegate for desired property update callbacks. This will be called
    /// every time we receive a PATCH from the service.
    /// </summary>
    /// <remarks>
    /// This can be set for both <see cref="DeviceClient"/> and <see cref="ModuleClient"/>.
    /// </remarks>
    /// <param name="desiredProperties">Properties that were contained in the update that was received from the service</param>
    /// <param name="userContext">Context object passed in when the callback was registered</param>
    public delegate void DesiredPropertyUpdateCallback(TwinCollection desiredProperties, object userContext);
}
