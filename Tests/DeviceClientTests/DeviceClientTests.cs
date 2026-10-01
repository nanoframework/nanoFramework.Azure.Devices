//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Azure.Devices.Client;
using nanoFramework.M2Mqtt.Messages;
using nanoFramework.TestFramework;
using System;
using System.Collections;

namespace DeviceClientTests
{
    [TestClass]
    public class DeviceClientTests
    {
        private static string _propertyOneName = "prop1";
        private static string _propertyTwoName = "prop2";
        private static string _propertyThreeName = "prop3";
        private static string _propertyOneValue = "iAmValue1";
        private static float _propertyTwoValue = 33.44f;
        private static string _propertyThreeValue = "string with $/#% chars";

        private static UserProperty _userProperty1 = new(_propertyOneName, _propertyOneValue);
        private static UserProperty _userProperty2 = new(_propertyTwoName, _propertyTwoValue.ToString("N2"));
        private static UserProperty _userProperty3 = new(_propertyThreeName, _propertyThreeValue);

        private static UserProperty _userPropertyBad1 = new(null, _propertyOneValue);
        private static UserProperty _userPropertyBad2 = new(_propertyTwoName, null);

        [TestMethod]
        public void EncodeUserPropertiesTest_00()
        {
            DeviceClient client = new();

            var encodedProperties = client.EncodeUserProperties(new ArrayList() { _userProperty1, _userProperty2, _userProperty3 });

            Assert.AreEqual(encodedProperties, "prop1=iAmValue1&prop2=33.44&prop3=string+with+%24%2F%23%25+chars");
        }

        [TestMethod]
        public void EncodeUserPropertiesTest_01()
        {
            DeviceClient client = new();

            Assert.ThrowsException(typeof(ArgumentException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userProperty3, _userPropertyBad1 });
                },
                "Expecting ArgumentException with invalid user property 01."
            );

            Assert.ThrowsException(typeof(ArgumentException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userPropertyBad2, _userProperty3 });
                },
                "Expecting ArgumentException with invalid user property 02."
            );

            Assert.ThrowsException(typeof(InvalidCastException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userProperty1, "Invalid property" });
                },
                "Expecting ArgumentException with invalid user property 03."
            );

            Assert.ThrowsException(typeof(InvalidCastException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { 8888888, "Invalid property" });
                },
                "Expecting ArgumentException with invalid user property 04."
            );
        }

        [DataRow("application/json", "$.ct=application%2Fjson&$.ce=utf-8")]
        [DataRow("application/mime", "$.ct=application%2Fmime&$.ce=utf-8")]
        [TestMethod]
        public void EncodeContentType_00(string contentType, string encodedContentType)
        {
            DeviceClient client = new();

            Assert.AreEqual(
                client.EncodeContentType(contentType),
                encodedContentType);
        }

        [DataRow("getMaxMinReport", "getMaxMinReport", "", true)]
        [DataRow("thermostat1*getMaxMinReport", "getMaxMinReport", "thermostat1", true)]
        [DataRow("getMaxMinReport", "getMaxMinReport", "thermostat1", false)]
        [DataRow("thermostat1*getMaxMinReport", "getMaxMinReport", "", false)]
        [DataRow("thermostat2*getMaxMinReport", "getMaxMinReport", "thermostat1", false)]
        [DataRow("getmaxminreport", "getMaxMinReport", "", false)]
        [DataRow("Thermostat1*getMaxMinReport", "getMaxMinReport", "thermostat1", false)]
        [TestMethod]
        public void IsMethodMatch_00(
            string requestedName,
            string methodName,
            string dtdlComponentName,
            bool expected)
        {
            Assert.AreEqual(
                expected,
                MethodCallbackRegistration.IsMethodMatch(
                    requestedName,
                    methodName,
                    dtdlComponentName));
        }

        [TestMethod]
        public void IsMethodMatch_01()
        {
            // null component is the default (root) component
            Assert.IsTrue(MethodCallbackRegistration.IsMethodMatch("getMaxMinReport", "getMaxMinReport", null));
            Assert.IsFalse(MethodCallbackRegistration.IsMethodMatch("thermostat1*getMaxMinReport", "getMaxMinReport", null));
        }

        [DataRow("getMaxMinReport", "getMaxMinReport")]
        [DataRow("<<Main>$>g__getMaxMinReport|0_1", "getMaxMinReport")]
        [TestMethod]
        public void GetCallbackName_00(string rawName, string expected)
        {
            Assert.AreEqual(
                expected,
                MethodCallbackRegistration.GetCallbackName(rawName));
        }

        [TestMethod]
        public void AddRemoveMethodCallback_00()
        {
            DeviceClient client = new();

            client.AddMethodCallback(getMaxMinReport);
            client.AddMethodCallback(getMaxMinReport, "thermostat1");
            client.AddMethodCallback(getMaxMinReport, "thermostat2");

            Assert.AreEqual(3, client.MethodCallbacks.Count);

            foreach (MethodCallbackRegistration registration in client.MethodCallbacks)
            {
                Assert.AreEqual("getMaxMinReport", registration.MethodName);
            }

            client.RemoveMethodCallback(getMaxMinReport);

            Assert.AreEqual(0, client.MethodCallbacks.Count);
        }

        private static string getMaxMinReport(int rid, string payload) => "{}";
    }
}
