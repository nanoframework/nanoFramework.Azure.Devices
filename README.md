[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_Azure.Devices&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_Azure.Devices) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_Azure.Devices&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_Azure.Devices) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Azure.Devices.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Azure.Devices.Client/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET **nanoFramework** Azure.Devices.Client Library repository

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Azure.Devices.Client | [![Build Status](https://dev.azure.com/nanoframework/Azure.Devices/_apis/build/status/nanoFramework.Azure.Devices?repoName=nanoframework%2FnanoFramework.Azure.Devices&branchName=main)](https://dev.azure.com/nanoframework/Azure.Devices/_build/latest?definitionId=75&repoName=nanoframework%2FnanoFramework.Azure.Devices&branchName=main)| [![NuGet](https://img.shields.io/nuget/v/nanoFramework.Azure.Devices.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Azure.Devices.Client/) |

## See it in action

You can watch this video from the Microsoft [IoT Show](https://aka.ms/iotshow) featuring the Azure SDK and a real life example with .NET nanoFramework:

[![IoT Show](https://img.youtube.com/vi/TLYqRdmmj5k/0.jpg)](https://youtu.be/TLYqRdmmj5k)

## Usage

**Important**: You **must** be connected to a network with a valid IP address **and** a valid date. Please check the examples with the Network Helpers on how to help you making sure you have both.

This Azure IoT Hub SDK is using MQTT. So you need to ensure you can connect to port 8883 using TLS protocol. If you are connected to an enterprise network, this may be blocked. In most cases, this is not an issue.

The namespaces, the name of the classes and the methods try to get close to the .NET C# Azure IoT SDK. This should allow easier portability of the code between the full .Net framework and nanoFramework environments.

### Certificate

You have 2 options to provide the right Azure IoT TLS certificate:

- Parse it into the constructor
- Store it into the device

The [AzureCertificates](AzureCertificates) contains, for your convenience, the root certificate used to connect to Azure IoT. The current one,  a Baltimore Root CA is the one to use up to June 2022. Starting from June 2022, the Digicert Global Root 2 is the one to use. For more information, please read the following [blog](https://techcommunity.microsoft.com/t5/internet-of-things/azure-iot-tls-critical-changes-are-almost-here-and-why-you/ba-p/2393169).

#### Through the constructor

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

You can place your binary certificate in the program resources as well and just get the certificate from it:

```csharp
X509Certificate azureRootCACert = new X509Certificate(Resources.GetBytes(Resources.BinaryResources.AzureCAcertificate));
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey, azureCert: azureRootCACert);
```

Note: when the certificate expires, you will have to fully reflash the device with the new certificate.

#### Storing the certificate on the device

You can store the certificate on the device flash and not in the code, so if you have to change the certificate, you'll just have to clean the current store and upload the new one. Edit the network properties:

![edit device network](device-network.jpg)

Navigate to the `General` tab:

![device network certificate](device-network-certificate.jpg)

Browse to choose your certificate, it can be in a binary (crt, der) or string form (pem, txt) and select ok. The certificate to connect will be selected automatically during the connection.

### Creating a DeviceClient

You can connect to Azure IoT Hub using either a symmetric Key or a certificate. The following example shows how to use a symmetric key:

```csharp
const string DeviceID = "nanoEdgeTwin";
const string IotBrokerAddress = "youriothub.azure-devices.net";
const string SasKey = "yoursaskey";
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey);
```

Note: please see the previous section to understand how to better parse the certificate for your usage. The example shows the certificate uploaded into the device and not in the code.

### Azure IoT Plug&Play

Azure IoT Plug&Play is supported as well. You'll need to provide a model ID when creating the DeviceClient:

```csharp
DeviceClient azureIoT = new DeviceClient(IotBrokerAddress, DeviceID, SasKey, modelID:"dtmi:com:example:Thermostat;1");
```

Note: the model ID has to be passed at the creation of the DeviceClient, it is not possible to pass it later on.

#### Reporting properties

Reporting Plug & Play properties is supported. He is a comprehensive example and how you can check if you have received one property that you're interested in:

```csharp
const string TargetTemerature = "targetTemperature";
DeviceClient azureIoT = new DeviceClient(Secrets.IotHub, Secrets.DeviceName, Secrets.SasKey, azureCert: new X509Certificate(Resource.GetBytes(Resource.BinaryResources.AzureRoot)), modelId: "dtmi:com:example:Thermostat;1");
azureIoT.TwinUpdated += AzureTwinUpdated;
azureIoT.Open();

void AzureTwinUpdated(object sender, TwinUpdateEventArgs e)
{
    if (e.Twin.Contains(TargetTemerature))
    {
        // We got an update for the target temperature
        var target = e.Twin[TargetTemerature];
        Debug.WriteLine($"Target temperature updated: {target}");
        PropertyAcknowledge targetReport = new() { Version = (int)e.Twin.Version, Status = PropertyStatus.Completed, Description = "All perfect", Value = target };
        TwinCollection twin = new TwinCollection();
        twin.Add(TargetTemerature, targetReport.BuildAcknowledge());
        azureIoT.UpdateReportedProperties(twin);
    }
}
```

In this example, the property we are interested in to receive is called `targetTemperature`. To receive its update, we are subscribing to the twin update. And we can get the value thu the `e.Twin[TargetTemerature]` once we've checked that the property exist.

The patter to publish a writable property is then quite simple. it's about building a `PropertyAcknowledge`, creating a TwinCollection, adding it to it with the property name, here our `targetTemperature`. You can add more properties to report of course. Note that what you add to the TwinCollection is not directly the object but `BuildAcknowledge()`. One done, just ask the library to update the twin through the `UpdateReportedProperties` method.

#### Receiving commands

An IoT Plug & Play command is a method callback. See further in this document how you can use them. In our case, the method is called `getMaxMinReport`. The name of the method in C# **must** be the exact same as the name from the DTDL file.

```csharp
DeviceClient azureIoT = new DeviceClient(Secrets.IotHub, Secrets.DeviceName, Secrets.SasKey, azureCert: new X509Certificate(Resource.GetBytes(Resource.BinaryResources.AzureRoot)), modelId: "dtmi:com:example:Thermostat;1");
azureIoT.AddMethodCallback(getMaxMinReport);
azureIoT.Open();

string getMaxMinReport(int rid, string payload)
{
    TemperatureReporting reporting = new() { avgTemp = 20, maxTemp = 42, minTemp = 12.34, startTime = DateTime.UtcNow.AddDays(-10), endTime = DateTime.UtcNow };
    return JsonConvert.SerializeObject(reporting);
}
```

In this example, the expected result is an object. Just populate the object and serialize it as a json as the command expect and return it. If any parameter to this command, it will be in the payload.

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

Note: it's important to use a `CancellationToken` that will be cancelled after a certain amount of time. Otherwise, this will be blocking the thread up to the point the twin is received. 

Twins have properties, reported and desired. They are collection and you can get or try to get any element.

You can report your Twin as simple as this:

```csharp
TwinCollection reported = new TwinCollection();
reported.Add("firmware", "myNano");
reported.Add("sdk", 0.2);
azureIoT.UpdateReportedProperties(reported);
```

You also have the option to wait for the twin update confirmation, in this case use a `CancellationToken` that can be cancelled. Otherwise the check will be ignored.

Note: the function will return false if the twin reception confirmation is not checked or if it did not arrive on time.

You can also register for any twin update:

```csharp
azureIoT.TwinUpdated += TwinUpdatedEvent;

void TwinUpdatedEvent(object sender, TwinUpdateEventArgs e)
{
    Debug.WriteLine($"Twin update received: {e.Twin.Count}");
}
```

### Sending message

You have to use the `SendMessage` function to send any kind of message or telemetry to Azure IoT. As for the other function, you have the possibility to ensure delivery with a `CancellationToken` than can be cancelled. If one that can't be cancelled is used, the delivery insurance will be ignored and the function will return false.

```csharp
var isReceived = azureIoT.SendMessage($"{{\"Temperature\":42,\"Pressure\":1024}}", new CancellationTokenSource(5000).Token);
Debug.WriteLine($"Message received by IoT Hub: {isReceived}");
```

Note: The message will be sent with the default service quality of service you created the device with. You won't get any answer for the quality `0`. In this case, you can simplify it to:

```csharp
azureIoT.SendMessage($"{{\"Temperature\":42,\"Pressure\":1024}}");
```

### Cloud to device messages

You can register an event to receive Cloud to device messages:

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

Method callback is supported as well. You can register and unregister your methods. Here are a few examples:

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

**Important**: method names are case sensitive. So make sure you name your functions in C# use the same case.

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

### QoS Level

By default, the device SDKs connect to an IoT Hub use QoS 1 for message exchange with the IoT hub. You can change this by setting the `qosLevel` argument of the `DeviceClient` constructor.

Here are existing QoS levels that you can use:

* AtMostOnce: The broker/client will deliver the message once, with no confirmation.
* AtLeastOnce: The broker/client will deliver the message at least once, with confirmation required.
* ExactlyOnce: The broker/client will deliver the message exactly once by using a four step handshake.

While it's possible to configure QoS 0 (AtMostOnce) for faster message exchange, you should note that the delivery isn't guaranteed nor acknowledged. For this reason, QoS 0 is often referred as "fire and forget".

## Module support

Modules are supported, you will have to use the constructor to pass the module ID either with a SAS token, either with a certificate. The rest fully works like a normal device. Everything is fully supported including module direct method, telemetry and of course twins!

For example here with a SAS token. Note that the certificates are fully supported as well. And if you are not storing the Azure root certificate on the device, you'll need to pass it in the constructor.

```csharp
const string DeviceID = "nanoEdgeTwin";
const string ModuleID = "myModule";
const string IotBrokerAddress = "youriothub.azure-devices.net";
const string SasKey = "yoursaskey";
DeviceClient module = new DeviceClient(IotBrokerAddress, DeviceID, ModuleID, SasKey);
```

## Azure IoT Device Provisioning Service (DPS) support

This SDK also supports the Azure IoT Device Provisioning Service. Group and individual provisioning scenarios are supported either with a symmetric key either with certificates. To understand the mechanism behind DPS, it is recommended to read the [documentation](https://docs.microsoft.com/azure/iot-dps/).

### Provisioning using symmetric key

For symmetric key provisioning you only need the following elements:

- A registration ID
- The ID Scope
- The device name
- The key or the derived key for group provisioning

The code is then straight forward:

```csharp
const string RegistrationID = "nanoDPStTest";
const string DpsAddress = "global.azure-devices-provisioning.net";
const string IdScope = "0ne01234567";
const string SasKey = "alongkeyencodedbase64";

// See the previous sections in the SDK help, you either need to have the Azure certificate embedded
// Either passing it in the constructor
X509Certificate azureCA = new X509Certificate(DpsSampleApp.Resources.GetBytes(DpsSampleApp.Resources.BinaryResources.BaltimoreRootCA_crt));
var provisioning = ProvisioningDeviceClient.Create(DpsAddress, IdScope, RegistrationID, SasKey, azureCA);
var myDevice = provisioning.Register(new CancellationTokenSource(60000).Token);

if(myDevice.Status != ProvisioningRegistrationStatusType.Assigned)
{
    Debug.WriteLine($"Registration is not assigned: {myDevice.Status}, error message: {myDevice.ErrorMessage}");
    return;
}

// You can then create the device
var device = new DeviceClient(myDevice.AssignedHub, myDevice.DeviceId, SasKey, nanoFramework.M2Mqtt.Messages.MqttQoSLevel.AtLeastOnce, azureCA);
// Open it and continue like for the previous sections
var res = device.Open();
if(!res)
{
    Debug.WriteLine($"can't open the device");
    return;
}
```

In case a [DPS model](https://github.com/Azure/opendigitaltwins-dtdl/blob/master/DTDL/v2/dtdlv2.md) is going to be used, the ID of the model has to be passed to the ProvisioningDeviceClient and DeviceClient constructor.
The code above requires the following changes.

Add the model ID as a constant:

```csharp
public const string ModelId = "dtmi:orgpal:palthree:palthree_demo_0;1";

```

Create the additional payload information with the model ID to be sent along the registration with DPS and pass that to the call to `Register()`.

```csharp
var pnpPayload = new ProvisioningRegistrationAdditionalData
{
    JsonData = PnpConvention.CreateDpsPayload(ModelId),
};

var myDevice = provisioning.Register(pnpPayload, new CancellationTokenSource(60000).Token);

```

Create the device client passing the model ID to the respective parameter in the constructor.

```csharp
var device = new DeviceClient(myDevice.AssignedHub, myDevice.DeviceId, SasKey, nanoFramework.M2Mqtt.Messages.MqttQoSLevel.AtLeastOnce, azureCA, ModelId);
```

Note: like for the `DeviceClient` you need to make sure you are connected to a network properly and also have a proper data and time set on the device.

### Provisioning using certificates

For symmetric key provisioning you only need the following elements:

- A registration ID
- The ID Scope
- The device name
- The device certificate
- Make sure that your IoT Hub is as well aware of the root/intermediate certificate you are using otherwise you won't be able to connect to your IoT Hub once your device is provisioned

The code is then straight forward:

```csharp
const string RegistrationID = "nanoCertTest";
const string DpsAddress = "global.azure-devices-provisioning.net";
const string IdScope = "0ne0034F11A";

const string cert = @"
-----BEGIN CERTIFICATE-----
Your certificate
-----END CERTIFICATE-----
";

const string privateKey = @"
-----BEGIN ENCRYPTED PRIVATE KEY-----
the encrypted private key
-----END ENCRYPTED PRIVATE KEY-----
";

// See the previous sections in the SDK help, you either need to have the Azure certificate embedded
// Either passing it in the constructor
X509Certificate azureCA = new X509Certificate(DpsSampleApp.Resources.GetBytes(DpsSampleApp.Resources.BinaryResources.BaltimoreRootCA_crt));
// Note: if your private key is not protected with a password, you don't need to pass it
// You can as well store your certificate directly in the device certificate store
// And you can store it as a resource as well if needed
X509Certificate2 deviceCert = new X509Certificate2(cert, privateKey, "1234");

var provisioning = ProvisioningDeviceClient.Create(DpsAddress, IdScope, RegistrationID, deviceCert, azureCA);
var myDevice = provisioning.Register(new CancellationTokenSource(60000).Token);

if(myDevice.Status != ProvisioningRegistrationStatusType.Assigned)
{
    Debug.WriteLine($"Registration is not assigned: {myDevice.Status}, error message: {myDevice.ErrorMessage}");
    return;
}

// You can then create the device
var device = new DeviceClient(myDevice.AssignedHub, myDevice.DeviceId, deviceCert, nanoFramework.M2Mqtt.Messages.MqttQoSLevel.AtLeastOnce, azureCA);
// Open it and continue like for the previous sections
var res = device.Open();
if(!res)
{
    Debug.WriteLine($"can't open the device");
    return;
}
```

### Additional payload

Additional payload is supported as well. You can set it up as as json string in the `ProvisioningRegistrationAdditionalData` class when calling the `Register` function. When the device has been provisioned, you may have as well additional payload provided.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
