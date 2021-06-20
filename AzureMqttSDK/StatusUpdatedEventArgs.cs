using System;

namespace nanoFramework.Azure.Devices
{
    public delegate void StatusUpdated(object sender, StatusUpdatedEventArgs e);

    public class StatusUpdatedEventArgs : EventArgs
    {
        public StatusUpdatedEventArgs(IoTHubStatus status)
        {
            IoTHubStatus = new IoTHubStatus(status);
        }

        public IoTHubStatus IoTHubStatus { get; set; }
    }
}
