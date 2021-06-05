// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Json;
using System;
using System.Collections;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Represents a collection of properties for <see cref="Twin"/>
    /// </summary>
    public class TwinCollection : IEnumerable
    {
        internal const string MetadataName = "$metadata";
        internal const string LastUpdatedName = "$lastUpdated";
        internal const string LastUpdatedVersionName = "$lastUpdatedVersion";
        internal const string VersionName = "$version";
        private readonly Hashtable _twin;

        public TwinCollection() : this(string.Empty)
        { }

        /// <summary>
        /// Creates a <see cref="TwinCollection"/> using the given JSON fragments for the body and metadata.
        /// </summary>
        /// <param name="twinJson">JSON fragment containing the twin data.</param>
        /// <param name="metadataJson">JSON fragment containing the metadata.</param>
        public TwinCollection(string twinJson)
        {
            _twin = (Hashtable)JsonConvert.DeserializeObject(twinJson, typeof(Hashtable));
        }

        /// <summary>
        /// Gets the version of the <see cref="TwinCollection"/>
        /// </summary>
        public long Version
        {
            get
            {
                try
                {
                    return (long)_twin[VersionName];
                }
                catch
                {
                    return default(long);
                }
            }
        }

        /// <summary>
        /// Gets the count of properties in the Collection.
        /// </summary>
        public int Count
        {
            get
            {
                int count = _twin.Count;
                if (count > 0)
                {
                    if (Contains(VersionName))
                    {
                        count--;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Property Indexer
        /// </summary>
        /// <param name="propertyName">Name of the property to get</param>
        /// <returns>Value for the given property name</returns>

        public object this[string propertyName]
        {
            get
            {
                try
                {
                    return _twin[propertyName];
                }
                catch
                {
                    return null;
                }
            }
            set => _twin[propertyName] = value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(_twin);
        }

        /// <summary>
        /// Gets the TwinProperties as a JSON string.
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(_twin);
        }

        /// <summary>
        /// Determines whether the specified property is present.
        /// </summary>
        /// <param name="propertyName">The property to locate</param>
        /// <returns>true if the specified property is present; otherwise, false</returns>
        public bool Contains(string propertyName)
        {
            try
            {
                var obj = _twin[propertyName];
                return true;
            }
            catch
            { // That means it doesn't exist
                return false;
            }
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException(); 
        }
    }
}
