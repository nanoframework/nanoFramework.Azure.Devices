// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        private readonly string _twin;
        private readonly string _metadata;

        public TwinCollection()
        { }

        /// <summary>
        /// Creates a <see cref="TwinCollection"/> using the given JSON fragments for the body and metadata.
        /// </summary>
        /// <param name="twinJson">JSON fragment containing the twin data.</param>
        /// <param name="metadataJson">JSON fragment containing the metadata.</param>
        public TwinCollection(string twinJson, string metadataJson = null)            
        {
            // TODO: as soon as the new JSON is here, this will have to be implemented here for dynamic JObject equivalent
            _twin = twinJson;
            _metadata = metadataJson;
        }

        /// <summary>
        /// Gets the version of the <see cref="TwinCollection"/>
        /// </summary>
        public long Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the count of properties in the Collection.
        /// </summary>
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Property Indexer
        /// </summary>
        /// <param name="propertyName">Name of the property to get</param>
        /// <returns>Value for the given property name</returns>

        public string this[string propertyName]
        {
            get
            {
                throw new NotImplementedException();
            }
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the Metadata for this property.
        /// </summary>
        /// <returns>Metadata instance representing the metadata for this property</returns>
        public Metadata GetMetadata()
        {
            return new Metadata(GetLastUpdated(), GetLastUpdatedVersion());
        }

        /// <summary>
        /// Gets the LastUpdated time for this property
        /// </summary>
        /// <returns>DateTime instance representing the LastUpdated time for this property</returns>
        /// <exception cref="System.NullReferenceException">Thrown when the TwinCollection metadata is null.
        /// An example would be when the TwinCollection class is created with the default constructor</exception>
        public DateTime GetLastUpdated()
        {
            // return (DateTime)_metadata[LastUpdatedName];
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the LastUpdatedVersion for this property.
        /// </summary>
        /// <returns>LastUpdatdVersion if present, null otherwise</returns>
        public long GetLastUpdatedVersion()
        {
            //return (long?)_metadata?[LastUpdatedVersionName];
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the TwinProperties as a JSON string.
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            //return JsonConvert.SerializeObject(JObject, formatting);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified property is present.
        /// </summary>
        /// <param name="propertyName">The property to locate</param>
        /// <returns>true if the specified property is present; otherwise, false</returns>
        public bool Contains(string propertyName)
        {
            //return JObject.TryGetValue(propertyName, out JToken ignored);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }        

        /// <summary>
        /// Clears metadata out of the twin collection.
        /// </summary>
        /// <remarks>
        /// This will only clear the metadata from the twin collection but will not change the base metadata object. This allows you to still use methods such as <see cref="GetMetadata"/>. If you need to remove all metadata, please use <see cref="ClearAllMetadata"/>.
        /// </remarks>
        public void ClearMetadata()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all metadata out of the twin collection as well as the base metadata object.
        /// </summary>
        /// <remarks>
        /// This will remove all metadata from the base metadata object as well as the metadata for the twin collection. The difference from the <see cref="ClearMetadata"/> method is this will also clear the underlying metadata object which will affect methods such as <see cref="GetMetadata"/> and <see cref="GetLastUpdatedVersion"/>.
        /// This method would be useful if you are performing any operations that require <see cref="TryGetMemberInternal(string, out object)"/> to return a <see cref="JToken"/> regardless of the client you are using.
        /// </remarks>
        public void ClearAllMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
