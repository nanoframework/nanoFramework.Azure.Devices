using System;

namespace nanoFramework.Azure.Devices
{
    public delegate void TwinUpdated(object sender, TwinUpdateEventArgs e);

    public class TwinUpdateEventArgs : EventArgs
    {
        public TwinUpdateEventArgs(TwinCollection twin)
        {
            Twin = twin;
        }

        public TwinCollection Twin { get; set; }
    }
}
