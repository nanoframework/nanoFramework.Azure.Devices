using System;
using System.Collections;
using System.Text;
using System.Web;

namespace nanoFramework.Azure.Devices
{
    public delegate void CloudToDeviceMessage(object sender, CloudToDeviceMessageEventArgs e);

    public class CloudToDeviceMessageEventArgs : EventArgs
    {
        public CloudToDeviceMessageEventArgs(string message, string properties)
        {
            Message = message;
            Properties = new Hashtable();
            string decoded = HttpUtility.UrlDecode(properties);
            // The properties look like that encoded and decoded:
            // %24.cid=aaaaa&%24.mid=idgenrated&%24.to=%2Fdevices%2FnanoEdgeTwin%2Fmessages%2Fdevicebound&%24.ct=aaaaa&%24.ce=utf-8&iothub-ack=positive&prop2=%20&prop3=42&prop4=something
            // $.cid=aaaaa&$.mid=idgenrated&$.to=/devices/nanoEdgeTwin/messages/devicebound&$.ct=aaaaa&$.ce=utf-8&iothub-ack=positive&prop2= &prop3=42&prop4=something
            if (!string.IsNullOrEmpty(decoded))
            {
                var props = decoded.Split('&');
                foreach (string prop in props)
                {
                    var items = prop.Split('=');
                    if (items.Length == 1)
                    {
                        Properties.Add(items[0], null);
                    }
                    else
                    {
                        Properties.Add(items[0], items[1]);
                    }
                }
            }
        }

        public Hashtable Properties { get; set; }

        public string Message { get; set; }

    }
}
