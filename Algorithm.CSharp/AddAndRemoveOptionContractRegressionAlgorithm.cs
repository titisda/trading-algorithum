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

using System;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Regression algorithm reproducing GH issue #5748 where in some cases an option underlying symbol was not being
    /// removed from all universes it was hold
    /// </summary>
    public class AddAndRemoveOptionContractRegressionAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Symbol _contract;
        private bool _hasRemoved;

        public override void Initialize()
        {
            SetStartDate(2014, 06, 06);
            SetEndDate(2014, 06, 09);

            UniverseSettings.DataNormalizationMode = DataNormalizationMode.Raw;
            UniverseSettings.MinimumTimeInUniverse = TimeSpan.Zero;

            var aapl = QuantConnect.Symbol.Create("AAPL", SecurityType.Equity, Market.USA);

            _contract = OptionChainProvider.GetOptionContractList(aapl, Time)
                .OrderBy(symbol => symbol.ID.Symbol)
                .FirstOrDefault(optionContract => optionContract.ID.OptionRight == OptionRight.Call
                    && optionContract.ID.OptionStyle == OptionStyle.American);
            AddOptionContract(_contract);
        }

        public override void OnData(Slice slice)
        {
            if (slice.HasData)
            {
                if (!_hasRemoved)
                {
                    RemoveOptionContract(_contract);
                    RemoveSecurity(_contract.Underlying);
                    _hasRemoved = true;
                }
                else
                {
                    throw new Exception("Expect a single call to OnData where we removed the option and underlying");
                }
            }
        }

        public override void OnEndOfAlgorithm()
        {
            if (!_hasRemoved)
            {
                throw new Exception("Expect a single call to OnData where we removed the option and underlying");
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp };

        /// <summary>
        /// Data Points count of all timeslices of algorithm
        /// </summary>
        public long DataPoints => 24;

        /// <summary>
        /// Data Points count of the algorithm history
        /// </summary>
        public int AlgorithmHistoryDataPoints => 0;

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "0"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "0%"},
            {"Drawdown", "0%"},
            {"Expectancy", "0"},
            {"Net Profit", "0%"},
            {"Sharpe Ratio", "0"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0"},
            {"Beta", "0"},
            {"Annual Standard Deviation", "0"},
            {"Annual Variance", "0"},
            {"Information Ratio", "-9.486"},
            {"Tracking Error", "0.008"},
            {"Treynor Ratio", "0"},
            {"Total Fees", "$0.00"},
            {"Estimated Strategy Capacity", "$0"},
            {"Lowest Capacity Asset", ""},
            {"Return Over Maximum Drawdown", "79228162514264337593543950335"},
            {"Portfolio Turnover", "0"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"OrderListHash", "d41d8cd98f00b204e9800998ecf8427e"}
        };
    }
}
