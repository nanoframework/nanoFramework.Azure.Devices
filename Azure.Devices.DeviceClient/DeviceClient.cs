// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Azure.Devices.Shared;
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

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Azure IoT Client SDK for .NET nanoFramework using MQTT
    /// </summary>
    public class DeviceClient

    {
        const string TwinReportedPropertiesTopic = "$iothub/twin/PATCH/properties/reported/";
        const string TwinDesiredPropertiesTopic = "$iothub/twin/GET/";
        const string DirectMethodTopic = "$iothub/methods/POST/";

        private readonly string _iotHubName;
        private readonly string _deviceId;
        private readonly string _sasKey;
        private readonly string _telemetryTopic;
        private readonly X509Certificate2 _clientCert;
        private readonly string _deviceMessageTopic;
        private readonly string _privateKey;
        private Twin _twin;
        private bool _twinReceived;
        private MqttClient _mqttc;
        private readonly IoTHubStatus _ioTHubStatus = new IoTHubStatus();
        private readonly ArrayList _methodCallback = new ArrayList();
        private readonly ArrayList _waitForConfirmation = new ArrayList();
        private readonly object _lock = new object();

        /// <summary>
        /// Device twin updated event.
        /// </summary>
        public event TwinUpdated TwinUpated;

        /// <summary>
        /// Status change event.
        /// </summary>
        public event StatusUpdated StatusUpdated;

        /// <summary>
        /// Cloud to device message received event.
        /// </summary>
        public event CloudToDeviceMessage CloudToDeviceMessage;

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">The Azure IoT name fully qualified (ex: youriothub.azure-devices.net)</param>
        /// <param name="deviceId">The device ID which is the name of your device.</param>
        /// <param name="sasKey">One of the SAS Key either primary, either secondary.</param>
        /// <param name="qosLevel">The default quality level delivery for the MQTT messages</param>
        public DeviceClient(string iotHubName, string deviceId, string sasKey, byte qosLevel = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE)
        {
            _clientCert = null;
            _privateKey = null;
            _iotHubName = iotHubName;
            _deviceId = deviceId;
            _sasKey = sasKey;
            _telemetryTopic = $"devices/{_deviceId}/messages/events/";
            _ioTHubStatus.Status = Status.Disconnected;
            _ioTHubStatus.Message = string.Empty;
            _deviceMessageTopic = $"devices/{_deviceId}/messages/devicebound/";
            QosLevel = qosLevel;
        }

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">The Azure IoT name fully qualified (ex: youriothub.azure-devices.net)</param>
        /// <param name="deviceId">The device ID which is the name of your device.</param>
        /// <param name="clientCert">The certificate to connect the device containing both public and private key.</param>
        /// <param name="qosLevel">The default quality level delivery for the MQTT messages</param>
        public DeviceClient(string iotHubName, string deviceId, X509Certificate2 clientCert, byte qosLevel = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE)
        {
            _clientCert = clientCert;
            _privateKey = Convert.ToBase64String(clientCert.PrivateKey);
            _iotHubName = iotHubName;
            _deviceId = deviceId;
            _sasKey = null;
            _telemetryTopic = $"devices/{_deviceId}/messages/events/";
            _ioTHubStatus.Status = Status.Disconnected;
            _ioTHubStatus.Message = string.Empty;
            _deviceMessageTopic = $"devices/{_deviceId}/messages/devicebound/";
            QosLevel = qosLevel;
        }

        /// <summary>
        /// The latest Twin received.
        /// </summary>
        public Twin LastTwin => _twin;

        /// <summary>
        /// The latests status.
        /// </summary>
        public IoTHubStatus IoTHubStatus => new IoTHubStatus(_ioTHubStatus);

        /// <summary>
        /// The default level quality.
        /// </summary>
        public byte QosLevel { get; set; }

        /// <summary>
        /// True if the device connected
        /// </summary>
        public bool IsConnected => _mqttc.IsConnected;

        /// <summary>
        /// Open the connection with Azure IoT. This will connected to Azure IoT Hub the device.
        /// </summary>
        /// <returns></returns>
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
                _ioTHubStatus.Status = Status.Connected;
                _ioTHubStatus.Message = string.Empty;
                StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
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

        /// <summary>
        /// Close the connection with Azure IoT and disconnect the device.
        /// </summary>
        public void Close()
        {
            if (_mqttc.IsConnected)
            {
                _mqttc.Unsubscribe(new[] {
                $"devices/{_deviceId}/messages/devicebound/#",
                "$iothub/twin/#",
                "$iothub/methods/POST/#"
                    });
                _mqttc.Disconnect();
            }
        }

        /// <summary>
        /// Gets the twin.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The twin.</returns>
        /// <remarks>It is strongly recommended to use a cancellation token that can be canceled and manage this on the 
        /// caller code level. A reasonable time of few seconds is recommended with a retry mechanism.</remarks>
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

        /// <summary>
        /// Update the twin reported properties.
        /// </summary>
        /// <param name="reported">The reported properties.</param>
        /// <param name="cancellationToken">A cancellation token. If you use the default one, the confirmation of delivery will not be awaited.</param>
        /// <returns>True for successful message delivery.</returns>
        public bool UpdateReportedProperties(TwinCollection reported, CancellationToken cancellationToken = default)
        {
            string twin = reported.ToJson();
            Debug.WriteLine($"update twin: {twin}");
            var rid = _mqttc.Publish($"{TwinReportedPropertiesTopic}?$rid={Guid.NewGuid()}", Encoding.UTF8.GetBytes(twin), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
            _ioTHubStatus.Status = Status.TwinUpdated;
            _ioTHubStatus.Message = string.Empty;
            StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));

            if (cancellationToken.CanBeCanceled)
            {
                ConfirmationStatus conf = new(rid);
                _waitForConfirmation.Add(conf);
                while (!conf.Received && !cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(200, true);
                }

                _waitForConfirmation.Remove(conf);
                return conf.Received;
            }

            return false;
        }

        /// <summary>
        /// Add a callback method.
        /// </summary>
        /// <param name="methodCalback">The callback method to add.</param>
        public void AddMethodCallback(MethodCalback methodCalback)
        {
            _methodCallback.Add(methodCalback);
        }

        /// <summary>
        /// Remove a callback method.
        /// </summary>
        /// <param name="methodCalback">The callback method to remove.</param>
        public void RemoveMethodCallback(MethodCalback methodCalback)
        {
            _methodCallback.Remove(methodCalback);
        }

        /// <summary>
        /// Send a message to Azure IoT.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="cancellationToken">A cancellation token. If you use the default one, the confirmation of delivery will not be awaited.</param>
        /// <returns>True for successful message delivery.</returns>
        public bool SendMessage(string message, CancellationToken cancellationToken = default)
        {

            var rid = _mqttc.Publish(_telemetryTopic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

            if (cancellationToken.CanBeCanceled)
            {
                ConfirmationStatus conf = new(rid);
                _waitForConfirmation.Add(conf);
                while (!conf.Received && !cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(200, true);
                }

                _waitForConfirmation.Remove(conf);
                return conf.Received;
            }

            return false;
        }

        private void ClientMqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);

                if (e.Topic.StartsWith("$iothub/twin/res/204"))
                {
                    _ioTHubStatus.Status = Status.TwinUpdateReceived;
                    _ioTHubStatus.Message = string.Empty;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
                else if (e.Topic.StartsWith("$iothub/twin/"))
                {
                    if (e.Topic.IndexOf("res/400/") > 0 || e.Topic.IndexOf("res/404/") > 0 || e.Topic.IndexOf("res/500/") > 0)
                    {
                        _ioTHubStatus.Status = Status.TwinUpdateError;
                        _ioTHubStatus.Message = string.Empty;
                        StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    }
                    else if (e.Topic.StartsWith("$iothub/twin/PATCH/properties/desired/"))
                    {
                        TwinUpated?.Invoke(this, new TwinUpdateEventArgs(new TwinCollection(message)));
                        _ioTHubStatus.Status = Status.TwinUpdateReceived;
                        _ioTHubStatus.Message = string.Empty;
                        StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    }
                    else
                    {
                        if (message.Length > 0)
                        {
                            // skip if already received in this session
                            if (!_twinReceived)
                            {
                                try
                                {
                                    _twin = new Twin(_deviceId, message);
                                    _twinReceived = true;
                                    _ioTHubStatus.Status = Status.TwinReceived;
                                    _ioTHubStatus.Message = message;
                                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Exception receiving the twins: {ex}");
                                    _ioTHubStatus.Status = Status.InternalError;
                                    _ioTHubStatus.Message = ex.ToString();
                                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                                }
                            }
                        }
                    }
                }
                else if (e.Topic.StartsWith(DirectMethodTopic))
                {
                    const string C9PatternMainStyle = "<<Main>$>g__";
                    string method = e.Topic.Substring(DirectMethodTopic.Length);
                    string methodName = method.Substring(0, method.IndexOf('/'));
                    int rid = Convert.ToInt32(method.Substring(method.IndexOf('=') + 1));
                    _ioTHubStatus.Status = Status.DirectMethodCalled;
                    _ioTHubStatus.Message = $"{method}/{message}";
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    foreach (MethodCalback mt in _methodCallback)
                    {
                        string mtName = mt.Method.Name;
                        if (mtName.Contains(C9PatternMainStyle))
                        {
                            mtName = mtName.Substring(C9PatternMainStyle.Length);
                            mtName = mtName.Substring(0, mtName.IndexOf('|'));
                        }
                        if (mtName == methodName)
                        {
                            try
                            {
                                var res = mt.Invoke(rid, message);
                                _mqttc.Publish($"$iothub/methods/res/200/?$rid={rid}", Encoding.UTF8.GetBytes(res), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            }
                            catch (Exception ex)
                            {
                                _mqttc.Publish($"$iothub/methods/res/504/?$rid={rid}", Encoding.UTF8.GetBytes($"{{\"Exception:\":\"{ex}\"}}"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                            }
                        }
                    }
                }
                else if (e.Topic.StartsWith(_deviceMessageTopic))
                {
                    string messageTopic = e.Topic.Substring(_deviceMessageTopic.Length);
                    _ioTHubStatus.Status = Status.MessageReceived;
                    _ioTHubStatus.Message = $"{messageTopic}/{message}";
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    CloudToDeviceMessage?.Invoke(this, new CloudToDeviceMessageEventArgs(message, messageTopic));
                }
                else if (e.Topic.StartsWith("$iothub/clientproxy/"))
                {
                    _ioTHubStatus.Status = Status.Disconnected;
                    _ioTHubStatus.Message = message;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Info"))
                {
                    _ioTHubStatus.Status = Status.IoTHubInformation;
                    _ioTHubStatus.Message = message;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/HighlightInfo"))
                {
                    _ioTHubStatus.Status = Status.IoTHubHighlightInformation;
                    _ioTHubStatus.Message = message;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Error"))
                {
                    _ioTHubStatus.Status = Status.IoTHubError;
                    _ioTHubStatus.Message = message;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
                else if (e.Topic.StartsWith("$iothub/logmessage/Warning"))
                {
                    _ioTHubStatus.Status = Status.IoTHubWarning;
                    _ioTHubStatus.Message = message;
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in event: {ex}");
                _ioTHubStatus.Status = Status.InternalError;
                _ioTHubStatus.Message = ex.ToString();
                StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
            }
        }

        private void ClientMqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (_waitForConfirmation.Count == 0)
            {
                return;
            }

            // Making sure the object will not be added or removed in this loop
            lock (_lock)
            {
                foreach (ConfirmationStatus status in _waitForConfirmation)
                {
                    if (status.ResponseId == e.MessageId)
                    {
                        status.Received = true;
                        // messages are unique
                        return;
                    }
                }
            }
        }

        private void ClientConnectionClosed(object sender, EventArgs e)
        {
            _ioTHubStatus.Status = Status.Disconnected;
            _ioTHubStatus.Message = string.Empty;
            StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
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
