// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Azure.Devices.Shared;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// Azure IoT Client SDK for .NET nanoFramework using MQTT.
    /// </summary>
    public class DeviceClient : IDisposable
    {
        private const string TwinReportedPropertiesTopic = "$iothub/twin/PATCH/properties/reported/";
        private const string TwinDesiredPropertiesTopic = "$iothub/twin/GET/";
        private const string DirectMethodTopic = "$iothub/methods/POST/";
        private const string ApiVersion = "2021-04-12";

        private readonly string _iotHubName;
        private readonly string _deviceId;
        private readonly string _sasKey;
        private readonly string _telemetryTopic;
        private readonly X509Certificate2 _clientCert;
        private readonly string _deviceMessageTopic;
        private Twin _twin;
        private bool _twinReceived;
        private MqttClient _mqttc = null;
        private readonly IoTHubStatus _ioTHubStatus = new IoTHubStatus();
        private readonly ArrayList _methodCallback = new ArrayList();
        private readonly ArrayList _waitForConfirmation = new ArrayList();
        private readonly object _lock = new object();
        private Timer _timerTokenRenew;
        private readonly X509Certificate _azureRootCACert;
        private bool _hasClientCertificate;

        /// <summary>
        /// Device twin updated event.
        /// </summary>
        public event TwinUpdated TwinUpdated;

        /// <summary>
        /// Status change event.
        /// </summary>
        public event StatusUpdated StatusUpdated;

        /// <summary>
        /// Cloud to device message received event.
        /// </summary>
        public event CloudToDeviceMessage CloudToDeviceMessage;

        internal DeviceClient()
        {
            // required for Unit Tests
        }

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">The Azure IoT name fully qualified (ex: youriothub.azure-devices.net).</param>
        /// <param name="deviceId">The device ID which is the name of your device.</param>
        /// <param name="moduleId">The module ID which is attached to the device ID.</param>
        /// <param name="sasKey">One of the SAS Key either primary, either secondary.</param>
        /// <param name="qosLevel">The default quality level delivery for the MQTT messages, default to the lower quality.</param>
        /// <param name="azureCert">Azure certificate for the connection to Azure IoT Hub.</param>
        /// <param name="modelId">Azure Plug and Play model ID.</param>
        public DeviceClient(string iotHubName, string deviceId, string moduleId, string sasKey, MqttQoSLevel qosLevel = MqttQoSLevel.AtLeastOnce, X509Certificate azureCert = null, string modelId = null)
        {
            _clientCert = null;
            _iotHubName = iotHubName;
            ModelId = modelId;
            ModuleId = moduleId;
            _sasKey = sasKey;
            QosLevel = qosLevel;
            _azureRootCACert = azureCert;

            if (string.IsNullOrEmpty(moduleId))
            {
                _telemetryTopic = $"devices/{deviceId}/messages/events/";
                _deviceId = deviceId;
            }
            else
            {
                _telemetryTopic = $"devices/{deviceId}/modules/{ModuleId}/messages/events/";
                _deviceId = deviceId + "/" + moduleId;
            }

            _deviceMessageTopic = $"devices/{_deviceId}/messages/devicebound/";

            _ioTHubStatus.Status = Status.Disconnected;
            _ioTHubStatus.Message = string.Empty;
        }

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">Your Azure IoT Hub fully qualified domain name (example: youriothub.azure-devices.net).</param>
        /// <param name="deviceId">The device ID (name of your device).</param>
        /// /// <param name="moduleId">The module ID which is attached to the device ID.</param>
        /// <param name="clientCert">The certificate to connect the device (containing both public and private keys). Pass null if you are using the certificate store on the device.</param>
        /// <param name="qosLevel">The default quality of assurance level for delivery for the MQTT messages (defaults to the lowest quality).</param>
        /// /// <param name="azureCert">Azure certificate for the connection to Azure IoT Hub.</param>
        /// /// <param name="modelId">Azure Plug and Play model ID.</param>
        public DeviceClient(string iotHubName, string deviceId, string moduleId, X509Certificate2 clientCert, MqttQoSLevel qosLevel = MqttQoSLevel.AtMostOnce, X509Certificate azureCert = null, string modelId = null)
        {
            _hasClientCertificate = true;
            _clientCert = clientCert;
            _iotHubName = iotHubName;
            ModelId = modelId;
            ModuleId = moduleId;
            QosLevel = qosLevel;
            _azureRootCACert = azureCert;

            if (string.IsNullOrEmpty(moduleId))
            {
                _telemetryTopic = $"devices/{deviceId}/messages/events/";
                _deviceId = deviceId;
            }
            else
            {
                _telemetryTopic = $"devices/{deviceId}/modules/{ModuleId}/messages/events/";
                _deviceId = deviceId + "/" + moduleId;
            }

            _deviceMessageTopic = $"devices/{_deviceId}/messages/devicebound/";

            _ioTHubStatus.Status = Status.Disconnected;
            _ioTHubStatus.Message = string.Empty;
        }

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">The Azure IoT name fully qualified (ex: youriothub.azure-devices.net).</param>
        /// <param name="deviceId">The device ID which is the name of your device.</param>
        /// <param name="sasKey">One of the SAS Key either primary, either secondary.</param>
        /// <param name="qosLevel">The default quality level delivery for the MQTT messages, default to the lower quality.</param>
        /// <param name="azureCert">Azure certificate for the connection to Azure IoT Hub.</param>
        /// <param name="modelId">Azure Plug and Play model ID.</param>
        public DeviceClient(string iotHubName, string deviceId, string sasKey, MqttQoSLevel qosLevel = MqttQoSLevel.AtLeastOnce, X509Certificate azureCert = null, string modelId = null)
            : this(iotHubName, deviceId, string.Empty, sasKey, qosLevel, azureCert, modelId)
        {
        }

        /// <summary>
        /// Creates an <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="iotHubName">Your Azure IoT Hub fully qualified domain name (example: youriothub.azure-devices.net).</param>
        /// <param name="deviceId">The device ID (name of your device).</param>
        /// <param name="clientCert">The certificate to connect the device (containing both public and private keys). Pass null if you are using the certificate store on the device.</param>
        /// <param name="qosLevel">The default quality of assurance level for delivery for the MQTT messages (defaults to the lowest quality).</param>
        /// /// <param name="azureCert">Azure certificate for the connection to Azure IoT Hub.</param>
        /// /// <param name="modelId">Azure Plug and Play model ID.</param>
        public DeviceClient(string iotHubName, string deviceId, X509Certificate2 clientCert, MqttQoSLevel qosLevel = MqttQoSLevel.AtMostOnce, X509Certificate azureCert = null, string modelId = null)
             : this(iotHubName, deviceId, string.Empty, clientCert, qosLevel, azureCert, modelId)
        {
        }

        /// <summary>
        /// Azure Plug and Play model ID.
        /// </summary>
        public string ModelId { get; internal set; }

        /// <summary>
        /// The module ID attached to the device ID.
        /// </summary>
        public string ModuleId { get; internal set; }

        /// <summary>
        /// The latest Twin received.
        /// </summary>
        public Twin LastTwin => _twin;

        /// <summary>
        /// The latest status.
        /// </summary>
        public IoTHubStatus IoTHubStatus => new IoTHubStatus(_ioTHubStatus);

        /// <summary>
        /// The default level quality.
        /// </summary>
        public MqttQoSLevel QosLevel { get; set; }

        /// <summary>
        /// True if the device connected.
        /// </summary>
        public bool IsConnected => (_mqttc != null) && _mqttc.IsConnected;

        /// <summary>
        /// Open the connection with Azure IoT. This will initiate a connection from the device to the Azure IoT Hub instance.
        /// </summary>
        /// <returns>True if open.</returns>
        public bool Open()
        {
            // Creates MQTT Client usinf the default port of 8883 and the TLS 1.2 protocol
            _mqttc = new MqttClient(
                _iotHubName,
                8883,
                true,
                _azureRootCACert,
                _clientCert,
                MqttSslProtocols.TLSv1_2);

            // Handler for received messages on the subscribed topics
            _mqttc.MqttMsgPublishReceived += ClientMqttMsgReceived;
            // Handler for publisher
            _mqttc.MqttMsgPublished += ClientMqttMsgPublished;
            // event when connection has been dropped
            _mqttc.ConnectionClosed += ClientConnectionClosed;

            string userName = $"{_iotHubName}/{_deviceId}/?api-version={ApiVersion}";
            if (!string.IsNullOrEmpty(ModelId))
            {
                userName += $"&model-id={HttpUtility.UrlEncode(ModelId)}";
            }

            Helper.ComposeTelemetryInformation(ref userName);

            // need to compute SHA if not using client certificate
            string key = _hasClientCertificate ? null : Helper.GetSharedAccessSignature(null, _sasKey, $"{_iotHubName}/devices/{_deviceId}", new TimeSpan(24, 0, 0));

            // Now connect the device
            _mqttc.Connect(
                _deviceId,
                userName,
                key,
                false,
                MqttQoSLevel.ExactlyOnce,
                false, "$iothub/twin/GET/?$rid=999",
                "Disconnected",
                true,
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
                        MqttQoSLevel.AtLeastOnce,
                        MqttQoSLevel.AtLeastOnce,
                        MqttQoSLevel.AtLeastOnce
                    }
                );

                _ioTHubStatus.Status = Status.Connected;
                _ioTHubStatus.Message = string.Empty;
                StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                // We will renew 10 minutes before just in case
                _timerTokenRenew = new Timer(TimerCallbackReconnect, null, new TimeSpan(23, 50, 0), TimeSpan.MaxValue);
            }

            return _mqttc.IsConnected;
        }

        /// <summary>
        /// Reconnect to Azure Iot Hub.
        /// </summary>
        public void Reconnect()
        {
            Close();
            Open();
        }

        private void TimerCallbackReconnect(object state)
        {
            _timerTokenRenew.Dispose();
            Reconnect();
        }

        /// <summary>
        /// Close the connection with Azure IoT and disconnect the device.
        /// </summary>
        public void Close()
        {
            if (_mqttc != null)
            {
                if (_mqttc.IsConnected)
                {
                    _mqttc.Unsubscribe(new[] {
                    $"devices/{_deviceId}/messages/devicebound/#",
                    "$iothub/twin/#",
                    "$iothub/methods/POST/#"
                    });
                }

                _mqttc.Disconnect();
                _mqttc.Close();
                // Make sure all get disconnected, cleared 
                Thread.Sleep(1000);
            }

            _timerTokenRenew?.Dispose();
        }

        /// <summary>
        /// Gets the twin.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The twin.</returns>
        /// <remarks>It is strongly recommended to use a cancellation token that can be canceled and manage this on the 
        /// caller code level. A reasonable time of few seconds is recommended with a retry mechanism.</remarks>
        public Twin GetTwin(CancellationToken cancellationToken = default)
        {
            _twinReceived = false;
            _mqttc.Publish(
                $"{TwinDesiredPropertiesTopic}?$rid={Guid.NewGuid()}",
                Encoding.UTF8.GetBytes(""),
                null,
                new ArrayList(),
                MqttQoSLevel.AtLeastOnce,
                false);

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
            var rid = _mqttc.Publish(
                $"{TwinReportedPropertiesTopic}?$rid={Guid.NewGuid()}",
                Encoding.UTF8.GetBytes(twin),
                null,
                new ArrayList(),
                MqttQoSLevel.AtLeastOnce,
                false);
            ConfirmationStatus conf = new(rid);
            _waitForConfirmation.Add(conf);
            _ioTHubStatus.Status = Status.TwinUpdated;
            _ioTHubStatus.Message = string.Empty;
            StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));

            if (cancellationToken.CanBeCanceled)
            {
                while (!conf.Received && !cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(200, true);
                }
            }

            _waitForConfirmation.Remove(conf);
            return conf.Received;
        }

        /// <summary>
        /// Add a callback method.
        /// </summary>
        /// <param name="methodCallback">The callback method to add.</param>
        public void AddMethodCallback(MethodCallback methodCallback)
        {
            _methodCallback.Add(methodCallback);
        }

        /// <summary>
        /// Remove a callback method.
        /// </summary>
        /// <param name="methodCallback">The callback method to remove.</param>
        public void RemoveMethodCallback(MethodCallback methodCallback)
        {
            _methodCallback.Remove(methodCallback);
        }

        /// <summary>
        /// Send a message to Azure IoT.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="contentType">Content of the application message.</param>
        /// <param name="cancellationToken">A cancellation token. If you use the default one, the confirmation of delivery will not be awaited.</param>
        /// <returns>True for successful message delivery.</returns>
        public bool SendMessage(
            string message,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            return SendMessage(
                message,
                contentType,
                new ArrayList(),
                cancellationToken,
                null);
        }

        /// <summary>
        /// Send a message to Azure IoT.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="cancellationToken">A cancellation token. If you use the default one, the confirmation of delivery will not be awaited.</param>
        /// <param name="dtdlComponentname">The DTDL component name.</param>
        /// <returns>True for successful message delivery.</returns>
        public bool SendMessage(
            string message,
            CancellationToken cancellationToken = default,
            string dtdlComponentname = "")
        {
            return SendMessage(
                message,
                null,
                new ArrayList(),
                cancellationToken,
                null);
        }

        /// <summary>
        /// Send a message to Azure IoT.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="contentType">Content of the application message.</param>
        /// <param name="userProperties">User properties to add to the application message.</param>
        /// <param name="cancellationToken">A cancellation token. If you use the default one, the confirmation of delivery will not be awaited.</param>
        /// <param name="dtdlComponentname">The DTDL component name.</param>
        /// <returns>True for successful message delivery.</returns>
        public bool SendMessage(
            string message,
            string contentType,
            ArrayList userProperties,
            CancellationToken cancellationToken = default,
            string dtdlComponentname = "")
        {
            StringBuilder topic = new(_telemetryTopic);

            // add content type to property bag, if there is one
            if (!string.IsNullOrEmpty(contentType))
            {
                topic.Append(EncodeContentType(contentType));
                topic.Append("&");
            }

            // add user properties to property bag, if they exist
            if (userProperties != null
                && userProperties.Count > 0)
            {
                topic.Append(EncodeUserProperties(userProperties));
                topic.Append("&");
            }

            if (!string.IsNullOrEmpty(dtdlComponentname))
            {
                topic.Append($"$.sub={HttpUtility.UrlEncode(dtdlComponentname)}");
            }

            var topicName = topic.ToString();

            // remove trailing '&' if there is one
            if (topicName.EndsWith("&"))
            {
                topicName = topicName.Substring(0, topicName.Length - 1);
            }

            var rid = _mqttc.Publish(
                topicName,
                Encoding.UTF8.GetBytes(message),
                null,
                new ArrayList(),
                QosLevel,
                false);

            ConfirmationStatus conf = new(rid);
            _waitForConfirmation.Add(conf);

            if (cancellationToken.CanBeCanceled)
            {
                while (!conf.Received && !cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(200, true);
                }
            }

            _waitForConfirmation.Remove(conf);

            return conf.Received;
        }

        internal string EncodeContentType(string contentType)
        {
            return $"$.ct={HttpUtility.UrlEncode(contentType)}&$.ce=utf-8";
        }

        internal string EncodeUserProperties(ArrayList userProperties)
        {
            StringBuilder encodedUserProperties = new();

            foreach (UserProperty property in userProperties)
            {
                if (!property.GetType().Equals(typeof(UserProperty)))
                {
#pragma warning disable S3928 // OK to use this in .NET nanoFramework without the argument name
                    throw new ArgumentException();
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                }

                if (string.IsNullOrEmpty(property.Name) || string.IsNullOrEmpty(property.Value))
                {
#pragma warning disable S3928 // OK to use this in .NET nanoFramework without the argument name
                    throw new ArgumentException();
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                }
                else
                {
                    encodedUserProperties.Append(HttpUtility.UrlEncode(property.Name));
                    encodedUserProperties.Append("=");
                    encodedUserProperties.Append(HttpUtility.UrlEncode(property.Value));
                    encodedUserProperties.Append("&");
                }
            }

            // remove trailing '&'
            return encodedUserProperties.ToString(
                0,
                encodedUserProperties.Length - 1);
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
                        TwinUpdated?.Invoke(this, new TwinUpdateEventArgs(new TwinCollection(message)));
                        _ioTHubStatus.Status = Status.TwinUpdateReceived;
                        _ioTHubStatus.Message = string.Empty;
                        StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    }
                    else
                    {
                        if ((message.Length > 0) && !_twinReceived)
                        {
                            // skip if already received in this session                         
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
                else if (e.Topic.StartsWith(DirectMethodTopic))
                {
                    const string C9PatternMainStyle = "<<Main>$>g__";
                    string method = e.Topic.Substring(DirectMethodTopic.Length);
                    string methodName = method.Substring(0, method.IndexOf('/'));
                    int rid = Convert.ToInt32(method.Substring(method.IndexOf('=') + 1), 16);
                    _ioTHubStatus.Status = Status.DirectMethodCalled;
                    _ioTHubStatus.Message = $"{method}/{message}";
                    StatusUpdated?.Invoke(this, new StatusUpdatedEventArgs(_ioTHubStatus));
                    foreach (MethodCallback mt in _methodCallback)
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
                                _mqttc.Publish(
                                    $"$iothub/methods/res/200/?$rid={rid:X}",
                                    Encoding.UTF8.GetBytes(res),
                                    null,
                                    new ArrayList(),
                                    MqttQoSLevel.AtLeastOnce,
                                    false);
                            }
                            catch (Exception ex)
                            {
                                _mqttc.Publish(
                                    $"$iothub/methods/res/504/?$rid={rid:X}",
                                    Encoding.UTF8.GetBytes($"{{\"Exception:\":\"{ex}\"}}"),
                                    null,
                                    new ArrayList(),
                                    MqttQoSLevel.AtLeastOnce,
                                    false);
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

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_mqttc != null)
            {
                // Making sure we unregister the registered events
                _mqttc.MqttMsgPublishReceived -= ClientMqttMsgReceived;
                _mqttc.MqttMsgPublished -= ClientMqttMsgPublished;
                _mqttc.ConnectionClosed -= ClientConnectionClosed;
                // Closing and waiting for the connection to be properly closed
                Close();
                while (_mqttc.IsConnected)
                {
                    Thread.Sleep(100);

                }

                // Cleaning
                GC.SuppressFinalize(_mqttc);
                _mqttc = null;
            }
        }
    }
}
