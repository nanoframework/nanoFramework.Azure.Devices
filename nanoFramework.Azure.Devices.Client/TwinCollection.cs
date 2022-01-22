// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using nanoFramework.Json;
using System.Collections;

namespace nanoFramework.Azure.Devices.Shared
{
    /// <summary>
    /// Represents a collection of properties for <see cref="Twin"/>.
    /// </summary>
    public class TwinCollection : IEnumerable
    {
        internal const string VersionName = "$version";
        private readonly Hashtable _twin;

        /// <summary>
        /// Creates an empty <see cref="TwinCollection"/>.
        /// </summary>
        public TwinCollection() : this(string.Empty)
        { }

        /// <summary>
        /// Creates a <see cref="TwinCollection"/> using the given JSON fragments for the body.
        /// </summary>
        /// <param name="twinJson">JSON fragment containing the twin data.</param>        
        public TwinCollection(string twinJson)
        {
            _twin = string.IsNullOrEmpty(twinJson) ? new() : (Hashtable)JsonConvert.DeserializeObject(twinJson, typeof(Hashtable));
        }

        /// <summary>
        /// Creates a <see cref="TwinCollection"/> using the given JSON fragments for the body.
        /// </summary>
        /// <param name="twin">The JSON hashtable.</param>
        public TwinCollection(Hashtable twin)
        {
            _twin = twin ?? new();
        }

        /// <summary>
        /// Gets the version of the <see cref="TwinCollection"/>.
        /// </summary>
        public long Version
        {
            get
            {
                try
                {
                    int ver = (int)_twin[VersionName];
                    return ver;
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
                if ((count > 0) && Contains(VersionName))
                {
                    count--;
                }

                return count;
            }
        }

        /// <summary>
        /// Property Indexer.
        /// </summary>
        /// <param name="propertyName">Name of the property to get.</param>
        /// <returns>Value for the given property name.</returns>

        public object this[string propertyName]
        {
            get => _twin[propertyName];

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
        public string ToJson() => ToString();

        /// <summary>
        /// Add a property.
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <param name="value">The value of the property.</param>
        public void Add(string property, object value)
        {
            _twin.Add(property, value);
        }

        /// <summary>
        /// Determines whether the specified property is present.
        /// </summary>
        /// <param name="propertyName">The property to locate.</param>
        /// <returns>True if the specified property is present; otherwise, false.</returns>
        public bool Contains(string propertyName)
        {
            foreach (string key in _twin.Keys)
            {
                if (key == propertyName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator() => _twin.GetEnumerator();
    }
}
