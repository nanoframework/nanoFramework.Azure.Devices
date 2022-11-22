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
        private static UserProperty _userProperty2 = new(_propertyTwoName, _propertyTwoValue.ToString());
        private static UserProperty _userProperty3 = new(_propertyThreeName, _propertyThreeValue);

        private static UserProperty _userPropertyBad1 = new(null, _propertyOneValue);
        private static UserProperty _userPropertyBad2 = new(_propertyTwoName, null);

        [TestMethod]
        public void EncodeUserPropertiesTest_00()
        {
            DeviceClient client = new();

            var encodedProperties = client.EncodeUserProperties(new ArrayList() { _userProperty1, _userProperty2, _userProperty3 });

            Assert.Equal(encodedProperties, "prop1=iAmValue1&prop2=33.44&prop3=string+with+%24%2F%23%25+chars");
        }

        [TestMethod]
        public void EncodeUserPropertiesTest_01()
        {
            DeviceClient client = new();

            Assert.Throws(typeof(ArgumentException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userProperty3, _userPropertyBad1 });
                },
                "Expecting ArgumentException with invalid user property 01."
            );

            Assert.Throws(typeof(ArgumentException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userPropertyBad2, _userProperty3 });
                },
                "Expecting ArgumentException with invalid user property 02."
            );

            Assert.Throws(typeof(InvalidCastException), () =>
                {
                    client.EncodeUserProperties(new ArrayList() { _userProperty1, "Invalid property" });
                },
                "Expecting ArgumentException with invalid user property 03."
            );

            Assert.Throws(typeof(InvalidCastException), () =>
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

            Assert.Equal(
                client.EncodeContentType(contentType),
                encodedContentType);
        }
    }
}
