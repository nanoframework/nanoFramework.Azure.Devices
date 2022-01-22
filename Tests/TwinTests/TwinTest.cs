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

            Assert.True(twin.Contains("something"), "something");
            Assert.Equal("22", (string)twin["something"]);
            Assert.True(twin.Contains("else"), "else");
            Assert.Equal(42, (int)twin["else"]);
            Assert.True(twin.Contains("null"), "null");
            var nullresult = twin["null"];
            Assert.Null(nullresult);

            Assert.False(twin.Contains("nothing"), "nothing");
            Assert.Null(twin["nothing"]);
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
                    Assert.Equal("22", coll.Value.ToString());
                }

                if (coll.Key.ToString() == "else")
                {
                    Assert.Equal(42, (int)coll.Value);
                }

                if (coll.Key.ToString() == "null")
                {
                    Assert.Null(coll.Value);
                }

                count++;
            }

            Assert.Equal(3, count);
        }
    }
}
