// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Azure.Devices.Shared;
using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;

namespace TwinTests
{
    [TestClass]
    public class TwinTest
    {
        [TestMethod]
        public void TestContains()
        {
            Hashtable twinHash = new()
            {
                { "something", "22" },
                { "else", 42 },
                { "null", null }
            };
            TwinCollection twin = new(twinHash);

            Assert.IsTrue(twin.Contains("something"), "something");
            Assert.AreEqual("22", (string)twin["something"]);
            Assert.IsTrue(twin.Contains("else"), "else");
            Assert.AreEqual(42, (int)twin["else"]);
            Assert.IsTrue(twin.Contains("null"), "null");
            var nullresult = twin["null"];
            Assert.IsNull(nullresult);

            Assert.IsFalse(twin.Contains("nothing"), "nothing");
            Assert.IsNull(twin["nothing"]);
        }

        [TestMethod]
        public void TestEnumeration()
        {
            Hashtable twinHash = new()
            {
                { "something", "22" },
                { "else", 42 },
                { "null", null }
            };
            TwinCollection twin = new(twinHash);

            int count = 0;
            foreach (DictionaryEntry coll in twin)
            {
                if (coll.Key.ToString() == "something")
                {
                    Assert.AreEqual("22", coll.Value.ToString());
                }

                if (coll.Key.ToString() == "else")
                {
                    Assert.AreEqual(42, (int)coll.Value);
                }

                if (coll.Key.ToString() == "null")
                {
                    Assert.IsNull(coll.Value);
                }

                count++;
            }

            Assert.AreEqual(3, count);
        }
    }
}
