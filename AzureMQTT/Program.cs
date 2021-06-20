//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

///
/// NOTE: this demo uses the information outlined in https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-mqtt-support
///

using nanoFramework.Azure.Devices;
using nanoFramework.Networking;
using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using nanoFramework.Json;
using Bmp280Measurement;

const string DeviceID = "nanoEdgeTwin";
const string IotBrokerAddress = "youriothub.azure-devices.net";
const string SasKey = "yoursaskey";
const string Ssid = "your wifi";
const string Password = "your wifi password";

// One minute unit
int sleepTimeMinutes = 60000;
Bmp280M bmp280 = new();
bool ShoudIStop = false;

Mqtt mqtt = new Mqtt(IotBrokerAddress, DeviceID, SasKey);

try
{
    if (!ConnectToWifi()) return;

    mqtt.TwinUpated += TwinUpdatedEvent;
    mqtt.StatusUpdated += StatusUpdatedEvent;
    mqtt.CloudToDeviceMessage += CloudToDeviceMessageEvent;
    mqtt.AddMethodCallback(MethodCalbackTest);
    mqtt.AddMethodCallback(MakeAddition);
    mqtt.AddMethodCallback(RaiseExceptionCallbackTest);
    var isOpen = mqtt.Open();
    Debug.WriteLine($"Connection is open: {isOpen}");

    var twin = mqtt.GetTwin(new CancellationTokenSource(20000).Token);
    if (twin == null)
    {
        Debug.WriteLine($"Can't get the twins");
        mqtt.Close();
        return;
    }

    Debug.WriteLine($"Twin DeviceID: {twin.DeviceId}, #desired: {twin.Properties.Desired.Count}, #reported: {twin.Properties.Reported.Count}");

    TwinCollection reported = new TwinCollection();
    reported.Add("firmware", "myNano");
    reported.Add("sdk", 0.2);
    mqtt.UpdateReportedProperties(reported);

    while (!ShoudIStop)
    {
        var values = bmp280.Read();
        var isReceived = mqtt.SendMessage($"{{\"Temperature\":{values.Temperature.DegreesCelsius},\"Pressure\":{values.Pressure.Hectopascals}}}", new CancellationTokenSource(5000).Token);
        Debug.WriteLine($"Message received by IoT Hub: {isReceived}");
        Thread.Sleep(20000);
    }
    
}
catch (Exception ex)
{
    // We won't do anything
    // This global try catch is to make sure whatever happen, we will safely be able to go
    // To sleep
    Debug.WriteLine(ex.ToString());
}

Thread.Sleep(Timeout.InfiniteTimeSpan);

bool ConnectToWifi()
{
    Debug.WriteLine("Program Started, connecting to WiFi.");

    // As we are using TLS, we need a valid date & time
    // We will wait maximum 1 minute to get connected and have a valid date
    var success = NetworkHelper.ConnectWifiDhcp(Ssid, Password, setDateTime: true, token: new CancellationTokenSource(sleepTimeMinutes).Token);
    if (!success)
    {
        Debug.WriteLine($"Can't connect to wifi: {NetworkHelper.ConnectionError.Error}");
        if (NetworkHelper.ConnectionError.Exception != null)
        {
            Debug.WriteLine($"NetworkHelper.ConnectionError.Exception");
        }
    }

    Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");
    return success;
}

void TwinUpdatedEvent(object sender, TwinUpdateEventArgs e)
{
    Debug.WriteLine($"Twin update received:  {e.Twin.Count}");
}

void StatusUpdatedEvent(object sender, StatusUpdatedEventArgs e)
{
    Debug.WriteLine($"Status changed: {e.IoTHubStatus.Status}, {e.IoTHubStatus.Message}");
    //if (e.IoTHubStatus.Status == Status.Disconnected)
    //{
    //    mqtt.Open();
    //}
}

string MethodCalbackTest(int rid, string payload)
{
    Debug.WriteLine($"Call back called :-) rid={rid}, payload={payload}");
    return "{\"Yes\":\"baby\",\"itisworking\":42}";
}

string MakeAddition(int rid, string payload)
{
    Hashtable variables = (Hashtable)JsonConvert.DeserializeObject(payload, typeof(Hashtable));
    int arg1 = (int)variables["arg1"];
    int arg2 = (int)variables["arg2"];
    return $"{{\"result\":{arg1 + arg2}}}";
}

string RaiseExceptionCallbackTest(int rid, string payload)
{
    throw new Exception("I got you, it's to test the 504");
}

void CloudToDeviceMessageEvent(object sender, CloudToDeviceMessageEventArgs e)
{
    Debug.WriteLine($"Message arrived: {e.Message}");
    foreach (string key in e.Properties.Keys)
    {
        Debug.Write($"  Key: {key} = ");
        if (e.Properties[key] == null)
        {
            Debug.WriteLine("null");
        }
        else
        {
            Debug.WriteLine((string)e.Properties[key]);
        }
    }

    if(e.Message == "stop")
    {
        ShoudIStop = true;
    }
}
