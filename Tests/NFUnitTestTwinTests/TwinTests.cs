using Microsoft.Azure.Devices.Shared;
using nanoFramework.Json;
using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace NFUnitTestTwinTests
{
    [TestClass]
    public class TwinTests
    {
        static string FullTwin = "{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}";
        static string CollectionProperties = "{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}";

        [TestMethod]
        public void DeserializeTwin()
        {
            TwinCollection twin = new(CollectionProperties);
            Assert.Equal(94, twin.Version);
        }

        [TestMethod]
        public void TestTest()
        {
            Hashtable array = (Hashtable)JsonConvert.DeserializeObject(CollectionProperties, typeof(Hashtable));
            foreach(var arr in array)
            {
                Debug.WriteLine(arr.ToString());
            }
        }
    }
}
