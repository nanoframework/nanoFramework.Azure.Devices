using System;
using System.Text;

namespace nanoFramework.Azure.Devices
{
    public class IoTHubStatus
    {
        internal IoTHubStatus(IoTHubStatus status)
        {
            Status = status.Status;
            Message = status.Message;
        }

        public IoTHubStatus()
        {

        }

        public Status Status { get; set; }

        public string Message { get; set; }
    }
}
