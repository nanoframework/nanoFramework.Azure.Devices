//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Azure.Devices.Client
{
    /// <summary>
    /// A registered direct method callback, optionally bound to a DTDL component.
    /// </summary>
    internal class MethodCallbackRegistration
    {
        private const string C9PatternMainStyle = "<<Main>$>g__";

        /// <summary>
        /// The callback delegate.
        /// </summary>
        public Delegate Callback { get; }

        /// <summary>
        /// The DTDL component name the callback is bound to, or <see langword="null"/> for the default (root) component.
        /// </summary>
        public string DtdlComponentName { get; }

        /// <summary>
        /// The method name used for matching.
        /// </summary>
        public string MethodName { get; }

        public MethodCallbackRegistration(Delegate callback, string dtdlComponentName)
        {
            Callback = callback;
            DtdlComponentName = dtdlComponentName;
            MethodName = GetCallbackName(callback.Method.Name);
        }

        /// <summary>
        /// Gets the method name to match, removing the decoration the compiler adds to local functions declared in top-level statements.
        /// </summary>
        /// <param name="rawName">The method name as reported by reflection.</param>
        /// <returns>The method name to match.</returns>
        internal static string GetCallbackName(string rawName)
        {
            if (rawName.Contains(C9PatternMainStyle))
            {
                string name = rawName.Substring(C9PatternMainStyle.Length);
                return name.Substring(0, name.IndexOf('|'));
            }

            return rawName;
        }

        /// <summary>
        /// Checks if a direct method request matches a callback.
        /// </summary>
        /// <param name="requestedName">The method name in the request topic. Methods on a DTDL component use the format <c>componentName*methodName</c>.</param>
        /// <param name="methodName">The name of the callback method.</param>
        /// <param name="dtdlComponentName">The DTDL component the callback is bound to, or <see langword="null"/> for the default (root) component.</param>
        /// <returns><see langword="true"/> if the request matches the callback.</returns>
        internal static bool IsMethodMatch(
            string requestedName,
            string methodName,
            string dtdlComponentName)
        {
            if (string.IsNullOrEmpty(dtdlComponentName))
            {
                return requestedName == methodName;
            }

            return requestedName == $"{dtdlComponentName}*{methodName}";
        }
    }
}
