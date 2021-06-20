using System;
using System.Text;

namespace nanoFramework.Azure.Devices
{
    public enum Status
    {
        Connected,
        Disconnected,
        TwinUpdated,
        TwinUpdateError,
        TwinReceived,
        TwinUpdateReceived,
        IoTHubError,
        IoTHubWarning,
        IoTHubInformation,
        IoTHubHighlightInformation,
        InternalError,
        MessageReceived,
        DirectMethodCalled,
    }
}
