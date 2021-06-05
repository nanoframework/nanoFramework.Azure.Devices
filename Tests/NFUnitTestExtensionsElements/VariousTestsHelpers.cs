using Microsoft.Azure.Devices.Client.Extensions;
using nanoFramework.TestFramework;
using System;
using System.Diagnostics;

namespace NFUnitTestExtensionsElements
{
    [TestClass]
    public class VariousTestsHelpers
    {
        private const char ValuePairDelimiter = ';';
        private const char ValuePairSeparator = '=';

        [TestMethod]
        public void TestToDictionnary()
        {
            string str = "HostName=EllerbachIOT.azure-devices.net;DeviceId=nanoDeepSleep;SharedAccessKey=xzYq/LTMgPIUuVYPgoEBowwNq2yicXzsa3L9ijJdGGc=";
            Debug.WriteLine("To Dictionnary");
            var dico = str.ToDictionary(ValuePairDelimiter, ValuePairSeparator);
            Assert.Equal("EllerbachIOT.azure-devices.net", (string)dico["HostName"]);
            Assert.Equal("nanoDeepSleep", (string)dico["DeviceId"]);
            Assert.Equal("xzYq/LTMgPIUuVYPgoEBowwNq2yicXzsa3L9ijJdGGc=", (string)dico["SharedAccessKey"]);
        }
    }
}
