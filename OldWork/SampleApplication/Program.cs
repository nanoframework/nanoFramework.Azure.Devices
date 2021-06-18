using Microsoft.Azure.Devices.Shared;
using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;


Debug.WriteLine("Hello from nanoFramework!");

Twin twin = new("nanoDeepSleep", "{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}");

Thread.Sleep(Timeout.Infinite);

