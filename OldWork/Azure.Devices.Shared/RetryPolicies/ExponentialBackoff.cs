//Copyright(c) Microsoft.All rights reserved.
//Microsoft would like to thank its contributors, a list
//of whom are at http://aka.ms/entlib-contributors

using System;

//Licensed under the Apache License, Version 2.0 (the "License"); you
//may not use this file except in compliance with the License. You may
//obtain a copy of the License at

//http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
//implied. See the License for the specific language governing permissions
//and limitations under the License.

// THIS FILE HAS BEEN MODIFIED FROM ITS ORIGINAL FORM.
// Change Log:
// 9/1/2017 jasminel Renamed namespace to Microsoft.Azure.Devices.Client.TransientFaultHandling and modified access modifier to internal.

namespace Microsoft.Azure.Devices.Client.TransientFaultHandling
{
    /// <summary>
    /// A retry strategy with back-off parameters for calculating the exponential delay between retries.
    /// </summary>
    internal class ExponentialBackoff : RetryStrategy
    {
        private readonly int _retryCount;

        private readonly TimeSpan _minBackoff;

        private readonly TimeSpan _maxBackoff;

        private readonly TimeSpan _deltaBackoff;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoff" /> class.
        /// </summary>
        public ExponentialBackoff() : this(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultClientBackoff)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoff" /> class with the specified retry settings.
        /// </summary>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum back-off time</param>
        /// <param name="maxBackoff">The maximum back-off time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        public ExponentialBackoff(int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) : this(null, retryCount, minBackoff, maxBackoff, deltaBackoff, RetryStrategy.DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoff" /> class with the specified name and retry settings.
        /// </summary>
        /// <param name="name">The name of the retry strategy.</param>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum back-off time</param>
        /// <param name="maxBackoff">The maximum back-off time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        public ExponentialBackoff(string name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff) : this(name, retryCount, minBackoff, maxBackoff, deltaBackoff, RetryStrategy.DefaultFirstFastRetry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialBackoff" /> class with the specified name, retry settings, and fast retry option.
        /// </summary>
        /// <param name="name">The name of the retry strategy.</param>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="minBackoff">The minimum back-off time</param>
        /// <param name="maxBackoff">The maximum back-off time.</param>
        /// <param name="deltaBackoff">The value that will be used to calculate a random delta in the exponential delay between retries.</param>
        /// <param name="firstFastRetry">true to immediately retry in the first attempt; otherwise, false. The subsequent retries will remain subject to the configured retry interval.</param>
        public ExponentialBackoff(string name, int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, bool firstFastRetry) : base(name, firstFastRetry)
        {
            if ((retryCount < 0) || (minBackoff.Ticks <0)||(maxBackoff.Ticks<0) || (deltaBackoff.Ticks<0) ||(minBackoff.TotalMilliseconds > maxBackoff.TotalMilliseconds))
            {
                throw new ArgumentException();
            }

            _retryCount = retryCount;
            _minBackoff = minBackoff;
            _maxBackoff = maxBackoff;
            _deltaBackoff = deltaBackoff;
        }

        /// <summary>
        /// Returns the corresponding ShouldRetry delegate.
        /// </summary>
        /// <returns>The ShouldRetry delegate.</returns>
        public override ShouldRetry GetShouldRetry()
        {
            return delegate (int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
            {
                if (currentRetryCount < _retryCount)
                {
                    Random random = new Random();

                    int min = (int)_deltaBackoff.TotalMilliseconds * 8 / 10;
                    int max = (int)_deltaBackoff.TotalMilliseconds * 12 / 10;
                    double exponentialInterval =
                        ((currentRetryCount* currentRetryCount) - 1.0)
                        * ( random.Next(max - min) + min)
                        + _minBackoff.TotalMilliseconds;

                    double maxInterval = _maxBackoff.TotalMilliseconds;
                    double num2 = exponentialInterval < maxInterval ? exponentialInterval: maxInterval;
                    retryInterval = TimeSpan.FromMilliseconds((long)num2);
                    return true;
                }

                retryInterval = TimeSpan.Zero;
                return false;
            };
        }
    }
}
