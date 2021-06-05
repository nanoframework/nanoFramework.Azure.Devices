// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.TestFramework;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Azure.Devices.Client.Test.ConnectionString
{
    [TestClass]
    public class IotHubConnectionStringBuilderTests
    {
        private const string HostName = "acme.azure-devices.net";
        private const string GatewayHostName = "gateway.acme.azure-devices.net";
        private const string TransparentGatewayHostName = "test";
        private const string DeviceId = "device1";
        private const string DeviceIdSplChar = "device1-.+%_#*?!(),=@;$'";
        private const string ModuleId = "moduleId";
        private const string SharedAccessKey = "dGVzdFN0cmluZzE=";
        private const string SharedAccessKeyName = "AllAccessKey";
        private const string CredentialScope = "Device";
        private const string CredentialType = "SharedAccessSignature";
        private const string SharedAccessSignature = "SharedAccessSignature sr=dh%3a%2f%2facme.azure-devices.net&sig=dGVzdFN0cmluZzU=&se=1654387200&skn=AllAccessKey";

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesHostName()
        {
            var connectionString = $"HostName={HostName};SharedAccessKeyName={SharedAccessKeyName};DeviceId={DeviceId};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            Assert.Equal(csBuilder.HostName, HostName);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesDeviceId()
        {
            var connectionString = $"HostName={HostName};SharedAccessKeyName={SharedAccessKeyName};DeviceId={DeviceId};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            Assert.Equal(csBuilder.DeviceId, DeviceId);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesModuleId()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};ModuleId={ModuleId};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            Assert.Equal(csBuilder.ModuleId, ModuleId);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesComplexDeviceId()
        {
            var connectionString = $"HostName={HostName};SharedAccessKeyName={SharedAccessKeyName};DeviceId={DeviceIdSplChar};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesSharedAccessKey()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);

            Assert.Equal(csBuilder.SharedAccessKey, SharedAccessKey);
            Assert.IsType(typeof(DeviceAuthenticationWithRegistrySymmetricKey), csBuilder.AuthenticationMethod);

            Assert.Null(csBuilder.SharedAccessSignature, "SharedAccessKey and SharedAccessSignature are mutually exclusive");
            Assert.False(csBuilder.UsingX509Cert, "SharedAccessKey and X509 are mutually exclusive");
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509False()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={SharedAccessKey};X509Cert=false";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);

            Assert.Equal(csBuilder.SharedAccessKey, SharedAccessKey);
            Assert.IsType(typeof(DeviceAuthenticationWithRegistrySymmetricKey), csBuilder.AuthenticationMethod);
            Assert.False(csBuilder.UsingX509Cert, "SharedAccessKey and X509 are mutually exclusive");
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesSharedAccessKeyName()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessKeyName={SharedAccessKeyName};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            Assert.Equal(csBuilder.SharedAccessKeyName, SharedAccessKeyName);
            Assert.IsType(typeof(DeviceAuthenticationWithSharedAccessPolicyKey), csBuilder.AuthenticationMethod);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesSharedAccessSignature()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessSignature={SharedAccessSignature}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);

            Assert.Equal(csBuilder.SharedAccessSignature, SharedAccessSignature);
            Assert.IsType(typeof(DeviceAuthenticationWithToken), csBuilder.AuthenticationMethod);

            Assert.Null(csBuilder.SharedAccessKey, "SharedAccessSignature and SharedAccessKey are mutually exclusive");
            Assert.False(csBuilder.UsingX509Cert, "SharedAccessSignature and X509 are mutually exclusive");
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_OverrideAuthMethodToken()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessSignature={SharedAccessSignature}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            csBuilder.AuthenticationMethod = new DeviceAuthenticationWithToken(DeviceId, SharedAccessSignature);

            Assert.Equal(csBuilder.SharedAccessSignature, SharedAccessSignature);
            Assert.IsType(typeof(DeviceAuthenticationWithToken), csBuilder.AuthenticationMethod);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_OverrideAuthMethodSapk()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessSignature={SharedAccessSignature}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            csBuilder.AuthenticationMethod = new DeviceAuthenticationWithSharedAccessPolicyKey(DeviceId, SharedAccessKeyName, SharedAccessKey);

            Assert.Equal(csBuilder.SharedAccessKey, SharedAccessKey);
            Assert.IsType(typeof(DeviceAuthenticationWithSharedAccessPolicyKey), csBuilder.AuthenticationMethod);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509()
        {
            IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509("true");
            IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509("True");
            IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509("TRUE");
        }

        //[DataRow("true")]
        //[DataRow("True")]
        //[DataRow("TRUE")]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesX509(string value)
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};X509Cert={value}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);

            Assert.True(csBuilder.UsingX509Cert);

            Assert.Null(csBuilder.SharedAccessKey);
            Assert.Null(csBuilder.SharedAccessKeyName);
            Assert.Null(csBuilder.SharedAccessSignature);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_ParsesGatewayHostName()
        {
            var connectionString = $"HostName={HostName};DeviceId={DeviceId};GatewayHostName={TransparentGatewayHostName};SharedAccessKey={SharedAccessKey}";
            var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            Assert.Equal(csBuilder.GatewayHostName, TransparentGatewayHostName);
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_MissingHostName_Throws()
        {
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                var connectionString = $"DeviceId={DeviceId};SharedAccessKey={SharedAccessKey}";
                var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            });
        }

        [TestMethod]

        public void IotHubConnectionStringBuilder_ParamConnectionString_MissingDeviceId_Throws()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                var connectionString = $"HostName={HostName};SharedAccessKey={SharedAccessKey}";
                var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            });
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_IncludesBothSharedAcccess_Throws()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                var connectionString = $"HostName={HostName};DeviceId={DeviceId};SharedAccessSignature={SharedAccessSignature};SharedAccessKey={SharedAccessKey}";
                var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            });
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamConnectionString_NoAuthSpecied_Throws()
        {
            Assert.Throws(typeof(ArgumentException), () =>
            {
                var connectionString = $"HostName={HostName};DeviceId={DeviceId}";
                var csBuilder = IotHubConnectionStringBuilder.Create(connectionString);
            });
        }

        [TestMethod]
        public void IotHubConnectionStringBuilder_ParamHostNameAuthMethod_SharedAccessKey()
        {
            var authMethod = new DeviceAuthenticationWithSharedAccessPolicyKey(DeviceId, SharedAccessKeyName, SharedAccessKey);
            var csBuilder = IotHubConnectionStringBuilder.Create(HostName, authMethod);
            Assert.Equal(csBuilder.HostName, HostName);
            Assert.Equal(csBuilder.DeviceId, DeviceId);
            Assert.Equal(csBuilder.SharedAccessKey, SharedAccessKey);
            Assert.IsType(typeof(DeviceAuthenticationWithSharedAccessPolicyKey), csBuilder.AuthenticationMethod);

            Assert.Null(csBuilder.SharedAccessSignature);
        }

        // Not sure we will implement this AuthenticationMethodFactory
        //[TestMethod]
        //public void IotHubConnectionStringBuilder_ParamHostNameAuthMethod_SharedAccessSignature()
        //{
        //    IAuthenticationMethod authMethod = AuthenticationMethodFactory.CreateAuthenticationWithToken(DeviceId, SharedAccessSignature);
        //    var csBuilder = IotHubConnectionStringBuilder.Create(HostName, authMethod);
        //    Assert.Equal(csBuilder.HostName, HostName);
        //    Assert.Equal(csBuilder.DeviceId, DeviceId);
        //    Assert.Equal(csBuilder.SharedAccessSignature, SharedAccessSignature);
        //    Assert.IsType(typeof(DeviceAuthenticationWithToken), csBuilder.AuthenticationMethod);

        //    Assert.Null(csBuilder.SharedAccessKey);
        //}

        // Not sure we will implement this DeviceAuthenticationWithRegistrySymmetricKey
        //[TestMethod]
        //public void IotHubConnectionStringBuilder_ParamHostNameAuthMethod_DeviceIdComplex()
        //{
        //    var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(DeviceIdSplChar, SharedAccessKey);
        //    var csBuilder = IotHubConnectionStringBuilder.Create(HostName, authMethod);
        //    Assert.Equal(csBuilder.DeviceId, DeviceIdSplChar);
        //}

        // Not sure we will implement this DeviceAuthenticationWithRegistrySymmetricKey
        //[TestMethod]
        //public void IotHubConnectionStringBuilder_ParamHostNameGatewayAuthMethod_Basic()
        //{
        //    IAuthenticationMethod authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, SharedAccessKey);
        //    var csBuilder = IotHubConnectionStringBuilder.Create(HostName, GatewayHostName, authMethod);
        //    Assert.Equal(csBuilder.HostName, HostName);
        //    Assert.Equal(csBuilder.DeviceId, DeviceId);
        //    Assert.Equal(csBuilder.GatewayHostName, GatewayHostName);
        //    Assert.Equal(csBuilder.SharedAccessKey, SharedAccessKey);
        //    Assert.IsType(typeof(DeviceAuthenticationWithRegistrySymmetricKey), csBuilder.AuthenticationMethod);

        //    Assert.Null(csBuilder.SharedAccessSignature);
        //}
    }
}
