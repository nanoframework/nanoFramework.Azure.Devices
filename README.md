[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_Azure.Devices&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_Azure.Devices) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_Azure.Devices&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_Azure.Devices) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Azure.Devices.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Azure.Devices.Client/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://github.com/nanoframework/Home/blob/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the **nanoFramework** Azure.Devices.Client Library repository!

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Azure.Devices.Client | [![Build Status](https://dev.azure.com/nanoframework/Azure.Devices/_apis/build/status/nanoframework.nanoFramework.Azure.Devices?branchName=main)](https://dev.azure.com/nanoframework/Azure.Devices/_build/latest?definitionId=75&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.Azure.Devices.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Azure.Devices.Client/) |
| nanoFramework.Azure.Devices.Client (preview) | [![Build Status](https://dev.azure.com/nanoframework/Azure.Devices/_apis/build/status/nanoframework.nanoFramework.Azure.Devices?branchName=develop)](https://dev.azure.com/nanoframework/Azure.Devices/_build/latest?definitionId=75&branchName=develop) | [![NuGet](https://img.shields.io/nuget/vpre/nanoFramework.Azure.Devices.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Azure.Devices.Client/) |

## Usage

**Importnat**: You **must** be connected to a network with a valid IP address **and** a valid date. Please check the examples with the Network Helpers on how to help you making sure you have both.

This Azure IoT Hub SDK is using MQTT. So you need to ensure you can connect to port 8883 using TLS protocol. If you are in an enterprise network, this may be blocked. In most cases, this is not an issue.

The namespaces, the name of the classes and the methods try to get close to the .NET C# Azure IoT SDK. This should allow an easier portability of the code between both environment.

### Certificate

You have 2 options to provide the right Azure IoT TLS certificate:

- Pass it in the constructor
- Store it into the device

The [AzureCertificate](AzureCertificate) contains, for your convenience, the root certificated used to connect to Azure IoT. The current one, Baltimore Root CA is the one to use up to June 2022. Starting in June 2022, the Digicert Global Root 2 is the one to use. For more information, please read the following [blog](https://techcommunity.microsoft.com/t5/internet-of-things/azure-iot-tls-critical-changes-are-almost-here-and-why-you/ba-p/2393169).

#### Thru the constructor

You will have to embed the certificate into your code:

```csharp
const string AzureRootCA = @"-----BEGIN CERTIFICATE-----
MIIDdzCCAl+gAwIBAgIEAgAAuTANBgkqhkiG9w0BAQUFADBaMQswCQYDVQQGEwJJ
RTESMBAGA1UEChMJQmFsdGltb3JlMRMwEQYDVQQLEwpDeWJlclRydXN0MSIwIAYD
VQQDExlCYWx0aW1vcmUgQ3liZXJUcnVzdCBSb290MB4XDTAwMDUxMjE4NDYwMFoX
DTI1MDUxMjIzNTkwMFowWjELMAkGA1UEBhMCSUUxEjAQBgNVBAoTCUJhbHRpbW9y
ZTETMBEGA1UECxMKQ3liZXJUcnVzdDEiMCAGA1UEAxMZQmFsdGltb3JlIEN5YmVy
VHJ1c3QgUm9vdDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKMEuyKr
mD1X6CZymrV51Cni4eiVgLGw41uOKymaZN+hXe2wCQVt2yguzmKiYv60iNoS6zjr
IZ3AQSsBUnuId9Mcj8e6uYi1agnnc+gRQKfRzMpijS3ljwumUNKoUMMo6vWrJYeK
mpYcqWe4PwzV9/lSEy/CG9VwcPCPwBLKBsua4dnKM3p31vjsufFoREJIE9LAwqSu
XmD+tqYF/LTdB1kC1FkYmGP1pWPgkAx9XbIGevOF6uvUA65ehD5f/xXtabz5OTZy
dc93Uk3zyZAsuT3lySNTPx8kmCFcB5kpvcY67Oduhjprl3RjM71oGDHweI12v/ye
jl0qhqdNkNwnGjkCAwEAAaNFMEMwHQYDVR0OBBYEFOWdWTCCR1jMrPoIVDaGezq1
BE3wMBIGA1UdEwEB/wQIMAYBAf8CAQMwDgYDVR0PAQH/BAQDAgEGMA0GCSqGSIb3
DQEBBQUAA4IBAQCFDF2O5G9RaEIFoN27TyclhAO992T9Ldcw46QQF+vaKSm2eT92
9hkTI7gQCvlYpNRhcL0EYWoSihfVCr3FvDB81ukMJY2GQE/szKN+OMY3EU/t3Wgx
jkzSswF07r51XgdIGn9w/xZchMB5hbgF/X++ZRGjD8ACtPhSNzkE1akxehi/oCr0
Epn3o0WC4zxe9Z2etciefC7IpJ5OCBRLbf1wbWsaY71k5h+3zvDyny67G7fyUIhz
ksLi4xaNmjICq44Y3ekQEe5+NauQrz4wlHrQMz2nZQ/1/I6eYs9HRCwBXbsdtTLS
R9I4LtD+gdwyah617jzV/OeBHRnDJELqYzmp
-----END CERTIFICATE-----
";
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey, azureCert: new X509Certificate(AzureRootCA));
```

You can place your binary certificate in the resources as well and just get the certificate from it:

```csharp
X509Certificate azureRootCACert = new X509Certificate(Resources.GetBytes(Resources.BinaryResources.AzureCAcertificate));
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey, azureCert: azureRootCACert);
```

Note: when the certificate will expire, you will have to reflash fully the device with the new certificate.

#### Storing the certificate into the device

You can store the certificate into the device and not in the code, so if you have to change, the certificate, you'll just have to clean the current store and upload the new one. Edit the network properties:

![edit device network](device-network.jpg)

Navigate to the `General` tab:

![device network certificate](device-network-certificate.jpg)

Brose to choose your certificate, it can be in a binary (crt, der) or string form (pem, txt) and select ok. The certificate to connect will be selected automatically during the connection.

### Creating a DeviceClient

You can connect to Azure IoT Hub using either a symmetric Key, either a certificate. The following example shows how to use a symmetric key:

```csharp
const string DeviceID = "nanoEdgeTwin";
const string IotBrokerAddress = "youriothub.azure-devices.net";
const string SasKey = "yoursaskey";
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey);
```

Note: please see the previous section to understand how to better pass the certificate for your usage. The example shows the certificate uploaded into the device and not in the code.

### Getting and updating Twin

You can request your Azure IoT Twin simply by calling the `GetTwin` function.

```csharp
var twin = azureIoT.GetTwin(new CancellationTokenSource(20000).Token);
if (twin == null)
{
    Debug.WriteLine($"Can't get the twins");
    azureIoT.Close();
    return;
}

Debug.WriteLine($"Twin DeviceID: {twin.DeviceId}, #desired: {twin.Properties.Desired.Count}, #reported: {twin.Properties.Reported.Count}");
```

Note: it's important to use a `CancellationToken` that be cancelled after a certain amount of time. Otherwise, this will be blocking the thread up to the point the twin will be received. 

Twins have properties, reported and desired. They are collection and you can get or try to get any element.

You can report your Twin as simple as this:

```csharp
TwinCollection reported = new TwinCollection();
reported.Add("firmware", "myNano");
reported.Add("sdk", 0.2);
azureIoT.UpdateReportedProperties(reported);
```

You have as well the option to wait for th twin update confirmation, in this case use a `CancellationToken` than can be cancelled. Otherwise the check will be ignore.

Note: the function will return false if the twin reception confirmation is not checked or if it did not arrive on time.

You can register as well for any twin update:

```csharp
azureIoT.TwinUpated += TwinUpdatedEvent;

void TwinUpdatedEvent(object sender, TwinUpdateEventArgs e)
{
    Debug.WriteLine($"Twin update received:  {e.Twin.Count}");
}
```

### Sending message

You have to use the `SendMessage` function to send any kind of message or telemetry to Azure IoT. As for the other function, you have the possibility to ensure delivery with a `CancellationToken` than can be cancelled. If one that can't be cancelled is used, the delivery insurance will be ignored and the function will return false.

```csharp
var isReceived = azureIoT.SendMessage($"{{\"Temperature\":42,\"Pressure\":1024}}", new CancellationTokenSource(5000).Token);
Debug.WriteLine($"Message received by IoT Hub: {isReceived}");
```

Note: The message will be send with the default service quality you created the device. You won't get any answer for the quality 0. In this case, you can simplify it to:

```csharp
azureIoT.SendMessage($"{{\"Temperature\":42,\"Pressure\":1024}}");
```

### Cloud to device messages

You can register an event to received the Cloud to device messages:

```csharp
azureIoT.CloudToDeviceMessage += CloudToDeviceMessageEvent;

// The following example shows how to display all keys in debug
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

    // e.Message contains the message itself
    if(e.Message == "stop")
    {
        ShoudIStop = true;
    }
}
```

Note: the `sender` is a `DeviceClient` class, you can then send a message back, a confirmation or any logic you've put in place.

### Method callback

Method callback is supported as well. You can register and unregister your methods. Here are few examples:

```csharp
azureIoT.AddMethodCallback(MethodCallbackTest);
azureIoT.AddMethodCallback(MakeAddition);
azureIoT.AddMethodCallback(RaiseExceptionCallbackTest);

string MethodCallbackTest(int rid, string payload)
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
    // This will properly return as well the exception error
    throw new Exception("I got you, it's to test the 504");
}
```

**Important**: method names are case sensitive. So make sure you name your functions in C# the same way.

### Status update event

A status update event is available:

```csharp
azureIoT.StatusUpdated += StatusUpdatedEvent;

void StatusUpdatedEvent(object sender, StatusUpdatedEventArgs e)
{
    Debug.WriteLine($"Status changed: {e.IoTHubStatus.Status}, {e.IoTHubStatus.Message}");
    // You may want to reconnect or use a similar retry mechanism
    ////if (e.IoTHubStatus.Status == Status.Disconnected)
    ////{
    ////    mqtt.Open();
    ////}
}
```

Note that those are status change based, so once the connect or disconnect event arrives, they'll be replaced by other events as soon as something else happened like receiving a twin.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
