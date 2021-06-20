using System;
using System.Text;

namespace nanoFramework.Azure.Devices
{
    internal class ConfirmationStatus
    {
        public ConfirmationStatus(ushort rid)
        {
            ResponseId = rid;
            Received = false;
        }

        public ushort ResponseId{ get; set;}
        public bool Received { get; set; }

    }
}
