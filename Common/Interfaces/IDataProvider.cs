/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.IO;
using System.ComponentModel.Composition;
using System;

namespace QuantConnect.Interfaces
{
    /// <summary>
    /// Fetches a remote file for a security.
    /// Must save the file to Globals.DataFolder.
    /// </summary>
    [InheritedExport(typeof(IDataProvider))]
    public interface IDataProvider
    {
        /// <summary>
        /// Event raised each time data fetch is finished (successfully or not)
        /// </summary>
        event EventHandler<DataProviderNewDataRequestEventArgs> NewDataRequest;

        /// <summary>
        /// Retrieves data to be used in an algorithm
        /// </summary>
        /// <param name="key">A string representing where the data is stored</param>
        /// <returns>A <see cref="Stream"/> of the data requested</returns>
        Stream Fetch(string key);
    }
}
