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

const string DeviceID = "devicename";
const string IotBrokerAddress = "youriot.azure-devices.net";
const string SasKey = "thelongsastokenprimaryorsecondarykey";
const string Ssid = "yourwifi";
const string Password = "yourwifipassword";

// One minute unit
DateTime allupOperation = DateTime.UtcNow;
int sleepTimeMinutes = 60000;

Mqtt mqtt = new Mqtt(IotBrokerAddress, DeviceID, SasKey);

try
{
    ConnectToWifi();

    mqtt.TwinUpated += TwinUpdatedEvent;
    mqtt.Open();
    var twin = mqtt.GetTwin(new CancellationTokenSource(20000).Token);
    if (twin == null)
        return;

    Debug.WriteLine($"Twin DeviceID: {twin.DeviceId}, #desired: {twin.Properties.Desired.Count}, #reported: {twin.Properties.Reported.Count}");

    TwinCollection reported = new TwinCollection();
    reported.Add("firmware", "myNano");
    reported.Add("sdk", 2.2);
    mqtt.UpdateReportedProperties(reported);
    // Just to make sure all is going out
    //Thread.Sleep(10000);
    //mqtt.Close();
}
catch (Exception ex)
{
    // We won't do anything
    // This global try catch is to make sure whatever happen, we will safely be able to go
    // To sleep
    Debug.WriteLine(ex.ToString());
}

Thread.Sleep(Timeout.InfiniteTimeSpan);

void ConnectToWifi()
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

        // GoToSleep();
        return;
    }

    // Reset the time counter if the previous date was not valid
    if (allupOperation.Year < 2018)
    {
        allupOperation = DateTime.UtcNow;
    }

    Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");
}

void TwinUpdatedEvent(object sender, TwinUpdateEventArgs e)
{
    Debug.WriteLine($"Twin update received:  {e.Twin.Count}");
}