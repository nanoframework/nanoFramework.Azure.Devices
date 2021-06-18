using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace nanoFramework.Azure.Devices
{
    public class Mqtt
    {
        const string TwinReportedPropertiesTopic = "$iothub/twin/PATCH/properties/reported/";
        const string TwinDesiredPropertiesTopic = "$iothub/twin/GET/";

        private readonly string _iotHubName;
        private readonly string _deviceId;
        private readonly string _sasKey;
        private readonly string _telemetryTopic;
        private readonly X509Certificate2 _clientCert;

        private string _privateKey;
        private bool _twinReceived;
        private Twin _twin;
        private MqttClient _mqttc;

        public event TwinUpdated TwinUpated;

        public Mqtt(string iotHubName, string deviceId, string sasKey)
        {
            _clientCert = null;
            _privateKey = null;
            _iotHubName = iotHubName;
            _deviceId = deviceId;
            _sasKey = sasKey;
            _telemetryTopic = $"devices/{_deviceId}/messages/events/";
        }

        public Mqtt(string iotHubName, string deviceId, X509Certificate2 clientCert, string privateKey)
        {
            _clientCert = clientCert;
            _privateKey = privateKey;
            _iotHubName = iotHubName;
            _deviceId = deviceId;
            _sasKey = null;
            _telemetryTopic = $"devices/{_deviceId}/messages/events/";
        }

        public bool Open()
        {
            // nanoFramework socket implementation requires a valid root CA to authenticate with.
            // This can be supplied to the caller (as it's doing on the code bellow) or the Root CA has to be stored in the certificate store
            // Root CA for Azure from here: https://github.com/Azure/azure-iot-sdk-c/blob/master/certs/certs.c
            // We are storing this certificate in the resources
            X509Certificate azureRootCACert = new X509Certificate(Resources.GetBytes(Resources.BinaryResources.AzureRoot));

            // Creates MQTT Client with default port 8883 using TLS protocol
            _mqttc = new MqttClient(
                _iotHubName,
                8883,
                true,
                azureRootCACert,
                _clientCert,
                MqttSslProtocols.TLSv1_2);

            // Handler for received messages on the subscribed topics
            _mqttc.MqttMsgPublishReceived += ClientMqttMsgReceived;
            // Handler for publisher
            _mqttc.MqttMsgPublished += ClientMqttMsgPublished;
            // event when connection has been dropped
            _mqttc.ConnectionClosed += ClientConnectionClosed;
            // handler for subscriber 
            _mqttc.MqttMsgSubscribed += ClientMqttMsgSubscribed;
            // handler for unsubscriber
            _mqttc.MqttMsgUnsubscribed += clientMqttMsgUnsubscribed;

            // Now connect the device
            byte code = _mqttc.Connect(
                _deviceId,
                $"{_iotHubName}/{_deviceId}/api-version=2020-09-30",
                _clientCert == null ? GetSharedAccessSignature(null, _sasKey, $"{_iotHubName}/devices/{_deviceId}", new TimeSpan(24, 0, 0)) : _privateKey,
                false,
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false, "$iothub/twin/GET/?$rid=999",
                "Disconnected",
                false,
                60
                );

            if (_mqttc.IsConnected)
            {
                _mqttc.Subscribe(
                    new[] {
                $"devices/{_deviceId}/messages/devicebound/#",
                "$iothub/twin/#",
                "$iothub/methods/POST/#"
                    },
                    new[] {
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE
                    }
                );
            }

            return _mqttc.IsConnected;
        }

        public void Close()
        {
            _mqttc.Disconnect();
        }

        public Twin GetTwin(CancellationToken cancellationToken = default)
        {
            _twinReceived = false;
            _mqttc.Publish($"{TwinDesiredPropertiesTopic}?$rid={Guid.NewGuid()}", Encoding.UTF8.GetBytes(""), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

            while (!_twinReceived && !cancellationToken.IsCancellationRequested)
            {
                cancellationToken.WaitHandle.WaitOne(200, true);
            }

            return _twinReceived ? _twin : null;
        }

        public void UpdateReportedProperties(TwinCollection reported)
        {
            string twin = reported.ToJson();
            Debug.WriteLine($"update twin: {twin}");
            _mqttc.Publish($"{TwinReportedPropertiesTopic}?$rid={Guid.NewGuid()}", Encoding.UTF8.GetBytes(twin), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        void ClientMqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                //Trace($"Message received on topic: {e.Topic}");
                string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                //Trace($"and message length: {message.Length}");

                if (e.Topic.StartsWith("$iothub/twin/res/204"))
                {
                    //Trace("and received confirmation for desired properties.");
                }
                else if (e.Topic.StartsWith("$iothub/twin/"))
                {
                    if (e.Topic.IndexOf("res/400/") > 0 || e.Topic.IndexOf("res/404/") > 0 || e.Topic.IndexOf("res/500/") > 0)
                    {
                        //Trace("and was in the error queue.");
                    }
                    else if (e.Topic.StartsWith("$iothub/twin/PATCH/properties/desired/"))
                    {
                        TwinUpated?.Invoke(this, new TwinUpdateEventArgs(new TwinCollection(message)));
                    }
                    else
                    {
                        //Trace("and was in the success queue.");
                        if (message.Length > 0)
                        {
                            // skip if already received in this session
                            if (!_twinReceived)
                            {
                                try
                                {
                                    _twin = new Twin(_deviceId, message);
                                    _twinReceived = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Exception receiving the twins: {ex}");
                                }
                            }
                        }
                    }
                }
                else if (e.Topic.StartsWith("$iothub/methods/POST/"))
                {
                    //Trace("and was a method.");
                }
                else if (e.Topic.StartsWith($"devices/{_deviceId}/messages/devicebound/"))
                {
                    //Trace("and was a message for the device.");
                }
                else if (e.Topic.StartsWith("$iothub/clientproxy/"))
                {
                    //Trace("and the device has been disconnected.");
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Info"))
                {
                    //Trace("and was in the log message queue.");
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/HighlightInfo"))
                {
                    //Trace("and was in the Highlight info queue.");
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Error"))
                {
                    //Trace("and was in the logmessage error queue.");
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Warning"))
                {
                    //Trace("and was in the logmessage warning queue.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in event: {ex}");
            }
        }

        void ClientMqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //Trace($"Response from publish with message id: {e.MessageId}");
            //if (e.MessageId == messageID)
            //{
            //    messageReceived = true;
            //}
        }

        void ClientMqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            //Trace(String.Format("Response from subscribe with message id: {0}", e.MessageId.ToString()));
        }

        void clientMqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            //Trace(String.Format("Response from unsubscribe with message id: {0}", e.MessageId.ToString()));
        }

        void ClientConnectionClosed(object sender, EventArgs e)
        {
            //Trace("Connection closed");
        }

        private string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            // http://msdn.microsoft.com/en-us/library/azure/dn170477.aspx
            // the canonical Uri scheme is http because the token is not amqp specific
            // signature is computed from joined encoded request Uri string and expiry string

            var exp = DateTime.UtcNow.ToUnixTimeSeconds() + (long)tokenTimeToLive.TotalSeconds;

            string expiry = exp.ToString();
            string encodedUri = HttpUtility.UrlEncode(resource);

            var hmacsha256 = new HMACSHA256(Convert.FromBase64String(sharedAccessKey));
            byte[] hmac = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(encodedUri + "\n" + expiry));
            string sig = Convert.ToBase64String(hmac);

            if (keyName != null)
            {
                return String.Format(
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                encodedUri,
                HttpUtility.UrlEncode(sig),
                HttpUtility.UrlEncode(expiry),
                HttpUtility.UrlEncode(keyName));
            }
            else
            {
                return String.Format(
                    "SharedAccessSignature sr={0}&sig={1}&se={2}",
                    encodedUri,
                    HttpUtility.UrlEncode(sig),
                    HttpUtility.UrlEncode(expiry));
            }
        }
    }
}
