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
        public void DeserializeTwinCollection()
        {
            TwinCollection twin = new(CollectionProperties);
            Assert.Equal(94, twin.Version);
            Assert.True(twin.Contains("$version"));
            Assert.Equal(2, (int)twin["TimeToSleep"]);
            // $version doesn't count as a property
            Assert.Equal(2, twin.Count);
        }

        [TestMethod]
        public void RawTestDirectlyWithJson()
        {
            Hashtable array = (Hashtable)JsonConvert.DeserializeObject(CollectionProperties, typeof(Hashtable));
            Assert.Equal("nanoFramework",array["Firmware"].ToString());
            Assert.Equal(2,(int)array["TimeToSleep"]);
            Assert.IsType(typeof(int), array["TimeToSleep"].GetType());
            Assert.Equal(94,(int)array["$version"]);
            Assert.IsType(typeof(int), array["$version"].GetType());
        }

        [TestMethod]
        public void DeserializeTwinProperties()
        {
            TwinProperties properties = new TwinProperties();
            Hashtable props = (Hashtable)JsonConvert.DeserializeObject("{\"desired\":{\"TimeToSleep\":5,\"$version\":2},\"reported\":{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":2,\"$version\":94}}", typeof(Hashtable));
            foreach (string str in props)
            {
                Debug.WriteLine(str);
            }
            //Debug.WriteLine(props["desired"].ToString());
            //properties.Desired = new(JsonConvert.SerializeObject(props["desired"]));
            //properties.Reported = new(JsonConvert.SerializeObject(props["reported"]));
            //Assert.Equal(2, properties.Desired.Version);
            //Assert.Equal(5, (int)properties.Desired["TimeToSleep"]);
            //Assert.Equal(2, (int)properties.Reported["TimeToSleep"]);
        }
    }
}
