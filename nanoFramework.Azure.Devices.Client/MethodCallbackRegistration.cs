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
        private const int StatusOk = 200;
        private const int StatusGatewayTimeout = 504;

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
        /// Invokes the callback and builds the response to return to the caller.
        /// </summary>
        /// <param name="rid">The request ID.</param>
        /// <param name="payload">The request payload.</param>
        /// <returns>The response to return. 200 with an empty payload if the callback returns <see langword="null"/>, 504 if the callback throws an exception.</returns>
        internal MethodResponse Invoke(int rid, string payload)
        {
            try
            {
                if (Callback is MethodResponseCallback responseCallback)
                {
                    return responseCallback.Invoke(rid, payload) ?? new MethodResponse(StatusOk);
                }

                return new MethodResponse(
                    StatusOk,
                    ((MethodCallback)Callback).Invoke(rid, payload));
            }
            catch (Exception ex)
            {
                return new MethodResponse(
                    StatusGatewayTimeout,
                    $"{{\"Exception:\":\"{ex}\"}}");
            }
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
