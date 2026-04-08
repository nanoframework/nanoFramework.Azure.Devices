# Copilot Instructions for nanoFramework.Azure.Devices

## Repository Overview

This repository implements an **Azure IoT Hub / Device Provisioning Service (DPS) client SDK** for [.NET nanoFramework](https://www.nanoframework.net/) — a .NET implementation for constrained embedded/IoT devices (microcontrollers). The library communicates with Azure IoT Hub via **MQTT over TLS** on port 8883.

There are **two NuGet packages** produced from the same source files:

| Package | Project file | Notes |
|---|---|---|
| `nanoFramework.Azure.Devices.Client` | `nanoFramework.Azure.Devices.Client/Azure.Devices.Client.nfproj` | Uses native `System.Net`, `System.Security.Cryptography.X509Certificates`, and `nanoFramework.M2Mqtt` |
| `nanoFramework.Azure.Devices.Client.FullyManaged` | `nanoFramework.Azure.Devices.Client.FullyManaged/Azure.Devices.Client.FullyManaged.nfproj` | Hardware-independent; uses `IMqttClient` abstraction, `byte[]` for certs, no native networking required. Compiled with `#define FULLYMANAGED` |

The `FullyManaged` project is a **linked-file project**: it references the exact same `.cs` source files from the main project via `<Link>` items in the `.nfproj`. Do **not** add separate source files to the FullyManaged folder — link them instead.

## Language and Framework Constraints

- **C# targeting .NET nanoFramework** (`TargetFrameworkVersion v1.0`). This is NOT standard .NET; many APIs available in full .NET do NOT exist here.
- **No LINQ, no `dynamic`, no `async`/`await`** in library code — nanoFramework does not support these.
- **Use `Hashtable` (from `System.Collections`) instead of `Dictionary<K,V>`**; generics support is limited.
- **Use `ArrayList` instead of `List<T>`** for collections.
- **JSON handling**: use `nanoFramework.Json.JsonConvert.SerializeObject` / `DeserializeObject`. Deserialize to `typeof(Hashtable)` for unknown structures.
- **Threading**: use `System.Threading.Thread`, `System.Threading.Timer`, `System.Threading.CancellationToken`, and `System.Threading.CancellationTokenSource`. No `Task` or `async`.
- **Conditional compilation** separates the two variants:
  - `#if FULLYMANAGED` — code path for the FullyManaged package (uses `IMqttClient`, `byte[]` certs).
  - `#if !FULLYMANAGED` — code path for the native package (uses `MqttClient`, `X509Certificate`/`X509Certificate2`).

## Project Structure

```
nanoFramework.Azure.Devices.Client/        # Main source files (shared by both packages)
  DeviceClient.cs                           # Core Azure IoT Hub client
  ProvisioningDeviceClient.cs               # DPS client
  Twin.cs / TwinCollection.cs / TwinProperties.cs / TwinUpdateEventArgs.cs
  PlugAndPlay/PnpConvention.cs              # IoT Plug & Play helpers
  HMACSHA256.cs / SHA256.cs                 # Crypto (custom implementations for nanoFramework)
  Helper.cs / HttpUtility.cs                # Utility classes
  CloudToDeviceMessage.cs                   # Cloud-to-device messaging
  MethodCallback.cs                         # Direct method callbacks
  PropertyAcknowledge.cs / PropertyStatus.cs # PnP property reporting
  Status.cs / IoTHubStatus.cs / StatusUpdatedEventArgs.cs

nanoFramework.Azure.Devices.Client.FullyManaged/   # Links to source files above
  Azure.Devices.Client.FullyManaged.nfproj          # Compiled with FULLYMANAGED define

Tests/
  CryptoTests/          # Unit tests for SHA256 / HMACSHA256
  DeviceClientTests/    # Unit tests for DeviceClient encoding logic
  TwinTests/            # Unit tests for Twin/TwinCollection

AzureCertificates/      # DigiCert Global Root G2 and Baltimore Root CA cert files
```

## Key Classes

### `DeviceClient` (namespace `nanoFramework.Azure.Devices.Client`)
- Connects to Azure IoT Hub via MQTT (port 8883, TLS 1.2).
- Authentication: **SAS key** or **X.509 client certificate**.
- Supports optional **Module ID** (for Azure IoT Edge modules).
- Supports **Azure IoT Plug & Play** via `modelId` parameter.
- Key methods: `Open()`, `Close()`, `SendMessage()`, `GetTwin()`, `UpdateReportedProperties()`, `AddMethodCallback()`, `RemoveMethodCallback()`.
- Key events: `TwinUpdated`, `StatusUpdated`, `CloudToDeviceMessage`.
- Uses `CancellationToken` for blocking calls (`GetTwin`, `SendMessage` with confirmation).

### `ProvisioningDeviceClient` (namespace `nanoFramework.Azure.Devices.Provisioning.Client`)
- Connects to Azure DPS (`global.azure-devices-provisioning.net`).
- Authentication: symmetric key or X.509 certificate.
- Entry point: static `Create(...)` factory method; then call `Register(cancellationToken)`.

### `TwinCollection` (namespace `nanoFramework.Azure.Devices.Shared`)
- Backed by `Hashtable`. Key `"$version"` is reserved for the version number.
- `Contains(string)`, `Add(string, object)`, indexer, `Count` (excludes `$version`).

## Namespaces

- `nanoFramework.Azure.Devices.Client` — DeviceClient and supporting types
- `nanoFramework.Azure.Devices.Provisioning.Client` — ProvisioningDeviceClient
- `nanoFramework.Azure.Devices.Shared` — Twin, TwinCollection, TwinProperties

## Build System

- **Project files**: `.nfproj` (MSBuild-based, nanoFramework project system).
- **Solution**: `nanoFramework.Azure.Devices.Client.sln`.
- **Build**: Uses Azure Pipelines (`azure-pipelines.yml`) with nanoframework/nf-tools templates. Builds on `windows-latest`.
- **Versioning**: [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) via `version.json`.
- **Assembly signing**: `key.snk` (strong-name signing, delay-sign is false).
- **NuGet packages**: NuGet restore uses `packages.lock.json` (locked mode in CI via `RestoreLockedMode`).
- **PR checks** (GitHub Actions, `.github/workflows/pr-checks.yml`): Verify package lock file consistency and that packages are up to date.

## Testing

- Test framework: `nanoFramework.TestFramework` with `[TestClass]` / `[TestMethod]` / `[DataRow]` attributes.
- Tests run with `.runsettings` files (`nano.runsettings`).
- Tests are desktop-runnable (the nanoFramework test runner executes on the host).
- **Run tests**: The CI pipeline uses the `runUnitTests: true` parameter against `Tests/CryptoTests/nano.runsettings`. Local execution requires the nanoFramework test adapter.
- Tests live in `Tests/CryptoTests/`, `Tests/DeviceClientTests/`, `Tests/TwinTests/`.

## Important Patterns and Conventions

### FULLYMANAGED conditional compilation
When modifying `DeviceClient.cs` or `ProvisioningDeviceClient.cs`, wrap platform-specific code with:
```csharp
#if FULLYMANAGED
    // Uses byte[] for certs, IMqttClient interface
#else
    // Uses X509Certificate, MqttClient concrete class
#endif
```

### Certificate handling
- Native package: `X509Certificate` (root CA), `X509Certificate2` (client cert with private key).
- FullyManaged package: `byte[]` for both (may be PEM or DER depending on the modem/broker).
- The DigiCert Global Root G2 is required as of October 2023. It is in `AzureCertificates/`.

### SAS Token
SAS tokens are generated by the library from the SAS key. They expire and are automatically renewed via an internal `Timer`. The token expiry is managed in `DeviceClient`.

### MQTT Topics
Key MQTT topic patterns used internally:
- Telemetry: `devices/{deviceId}/messages/events/`
- Cloud-to-device: `devices/{deviceId}/messages/devicebound/`
- Twin GET: `$iothub/twin/GET/`
- Twin PATCH: `$iothub/twin/PATCH/properties/reported/`
- Direct methods: `$iothub/methods/POST/`
- DPS: `$dps/registrations/res/#`

### Method callbacks
Method names in C# must **exactly match** the method name in the Azure IoT Hub or DTDL definition (case-sensitive). Register with `AddMethodCallback(methodName)`.

### QoS Levels
Default QoS is `MqttQoSLevel.AtLeastOnce`. QoS 0 (`AtMostOnce`) is fire-and-forget with no delivery confirmation.

### CancellationToken usage
Blocking operations (`GetTwin`, `SendMessage` with confirmation) require a `CancellationToken` that will cancel after a timeout. If a non-cancellable token is passed, confirmation is skipped and the method returns false.

## Adding Dependencies

- Update `packages.config` in the relevant `.nfproj` project.
- Run NuGet restore (the project uses `packages.lock.json` — update lock file locally, not just `packages.config`).
- The FullyManaged project has its own separate `packages.config` and `packages.lock.json`.

## Common Errors and Workarounds

- **`RestoreLockedMode` failure in CI**: The `packages.lock.json` must be up-to-date and committed. Update it locally by running NuGet restore without locked mode before committing.
- **`FULLYMANAGED` define missing**: The FullyManaged `.nfproj` sets `<DefineConstants>FULLYMANAGED</DefineConstants>`. If conditionally compiled code is not behaving as expected, verify which project is being built.
- **nanoFramework API not available**: Remember this targets constrained devices. Avoid using standard .NET APIs that do not exist in nanoFramework (e.g., `Task`, `async/await`, `Dictionary<K,V>`, `List<T>`, `StringBuilder` in some contexts, `Encoding.UTF8.GetString` may need a workaround).
- **Certificate expiry**: The Azure DigiCert Global Root G2 is valid until 2038. Do not use the old Baltimore Root CA (expired June 2022).
