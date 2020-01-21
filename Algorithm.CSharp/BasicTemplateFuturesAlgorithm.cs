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
 *
*/

using System;
using System.Collections.Generic;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Securities;
using QuantConnect.Securities.Future;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This example demonstrates how to add futures for a given underlying asset.
    /// It also shows how you can prefilter contracts easily based on expirations, and how you
    /// can inspect the futures chain to pick a specific contract to trade.
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="benchmarks" />
    /// <meta name="tag" content="futures" />
    public class BasicTemplateFuturesAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Symbol _contractSymbol;

        // S&P 500 EMini futures
        private const string RootSP500 = Futures.Indices.SP500EMini;
        public Symbol SP500 = QuantConnect.Symbol.Create(RootSP500, SecurityType.Future, Market.USA);

        // Gold futures
        private const string RootGold = Futures.Metals.Gold;
        public Symbol Gold = QuantConnect.Symbol.Create(RootGold, SecurityType.Future, Market.USA);

        /// <summary>
        /// Initialize your algorithm and add desired assets.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2013, 10, 08);
            SetEndDate(2013, 10, 10);
            SetCash(1000000);

            var futureSP500 = AddFuture(RootSP500);
            var futureGold = AddFuture(RootGold);

            // set our expiry filter for this futures chain
            futureSP500.SetFilter(TimeSpan.Zero, TimeSpan.FromDays(182));
            futureGold.SetFilter(TimeSpan.Zero, TimeSpan.FromDays(182));

            var benchmark = AddEquity("SPY");
            SetBenchmark(benchmark.Symbol);
        }

        /// <summary>
        /// Event - v3.0 DATA EVENT HANDLER: (Pattern) Basic template for user to override for receiving all subscription data in a single event
        /// </summary>
        /// <param name="slice">The current slice of data keyed by symbol string</param>
        public override void OnData(Slice slice)
        {
            if (!Portfolio.Invested)
            {
                foreach(var chain in slice.FutureChains)
                {
                    // find the front contract expiring no earlier than in 90 days
                    var contract = (
                        from futuresContract in chain.Value.OrderBy(x => x.Expiry)
                        where futuresContract.Expiry > Time.Date.AddDays(90)
                        select futuresContract
                    ).FirstOrDefault();

                    // if found, trade it
                    if (contract != null)
                    {
                        _contractSymbol = contract.Symbol;
                        MarketOrder(_contractSymbol, 1);
                    }
                }
            }
            else
            {
                Liquidate();
            }
        }

        public override void OnEndOfAlgorithm()
        {
            // Get the margin requirements
            var buyingPowerModel = Securities[_contractSymbol].BuyingPowerModel;
            var futureMarginModel = buyingPowerModel as FutureMarginModel;
            if (buyingPowerModel == null)
            {
                throw new Exception($"Invalid buying power model. Found: {buyingPowerModel.GetType().Name}. Expected: {nameof(FutureMarginModel)}");
            }
            var initialOvernight = futureMarginModel?.InitialMarginRequirement;
            var maintenanceOvernight = futureMarginModel?.MaintenanceMarginRequirement;
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "8220"},
            {"Average Win", "0.00%"},
            {"Average Loss", "0.00%"},
            {"Compounding Annual Return", "-100.000%"},
            {"Drawdown", "13.500%"},
            {"Expectancy", "-0.818"},
            {"Net Profit", "-13.517%"},
            {"Sharpe Ratio", "-23.826"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "89%"},
            {"Win Rate", "11%"},
            {"Profit-Loss Ratio", "0.69"},
            {"Alpha", "-7.042"},
            {"Beta", "-0.992"},
            {"Annual Standard Deviation", "0.373"},
            {"Annual Variance", "0.139"},
            {"Information Ratio", "-21.379"},
            {"Tracking Error", "0.503"},
            {"Treynor Ratio", "8.964"},
            {"Total Fees", "$15207.00"},
            {"Fitness Score", "0.033"},
            {"Kelly Criterion Estimate", "-36.472"},
            {"Kelly Criterion Probability Value", "0.82"},
            {"Sortino Ratio", "-8.62"},
            {"Return Over Maximum Drawdown", "-7.81"},
            {"Portfolio Turnover", "302.321"},
            {"Total Insights Generated", "8220"},
            {"Total Insights Closed", "8218"},
            {"Total Insights Analysis Completed", "8218"},
            {"Long Insight Count", "4110"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$79.81579"},
            {"Total Accumulated Estimated Alpha Value", "$8.425"},
            {"Mean Population Estimated Insight Value", "$0.001025189"},
            {"Mean Population Direction", "38.7618%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "34.5315%"},
            {"Rolling Averaged Population Magnitude", "0%"}
        };
    }
}
