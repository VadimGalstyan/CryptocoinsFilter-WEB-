using CryptocoinsFilter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CryptocoinsFilter.Background_Tasks;

namespace LearnCryptocoinsFilteringProject.Controllers
{
    public class FilterController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-EK1FURHH\\SQLEXPRESS;Database=CryptoExchanges;Trusted_Connection=True;Encrypt=False;";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Filter());
        }

        string chosen(bool include)
        {
            return include ? "" : "NOT ";
        }

        string mainExchange(Filter model)//returns tha exchange which is choosed by user
        {
            if (model.BinanceFutures == true)
            {
                return "Binance_Futures";
            }
            if (model.BinanceSpot == true)
            {
                return "Binance_Spot";
            }
            if (model.BybitFutures == true)
            {
                return "Bybit_Futures";
            }
            if (model.BybitSpot == true)
            {
                return "Bybit_Spot";
            }
            if (model.BitgetFutures == true)
            {
                return "Bitget_Futures";
            }
            if (model.BitgetSpot == true)
            {
                return "Bitget_Spot";
            }
            if (model.GateFutures == true)
            {
                return "Gate_Futures";
            }
            if (model.GateSpot == true)
            {
                return "Gate_Spot";
            }
            if (model.MexcFutures == true)
            {
                return "Mexc_Futures";
            }
            if (model.MexcSpot == true)
            {
                return "Mexc_Spot";
            }
            //adding asterdex
            if (model.AsterDexFutures == true)
            {
                return "AsterDex_Futures";
            }
            //adding kucoin
            if (model.KuCoinFutures == true)
            {
                return "KuCoin_Futures";
            }
            if (model.KuCoinSpot == true)
            {
                return "KuCoin_Spot";
            }
            //adding hyperliquid
            if (model.HyperliquidFutures == true)
            {
                return "Hyperliquid_Futures";
            }
            if (model.HyperliquidSpot == true)
            {
                return "Hyperliquid_Spot";
            }
            //adding  bingx
            if (model.BingxFutures == true)
            {
                return "Bingx_Futures";
            }
            if (model.BingxSpot == true)
            {
                return "Bingx_Spot";
            }
            //adding okx
            if (model.OkxFutures == true)
            {
                return "Okx_Futures";
            }
            if (model.OkxSpot == true)
            {
                return "Okx_Spot";
            }
            //adding  phemex
            if (model.PhemexFutures == true)
            {
                return "Phemex_Futures";
            }
            if (model.PhemexSpot == true)
            {
                return "Phemex_Spot";
            }
            //adding  weex
            if (model.WeexFutures == true)
            {
                return "Weex_Futures";
            }
            if (model.WeexSpot == true)
            {
                return "Weex_Spot";
            }
            return "NONE";            
        }

        string MainDisabledExchange(Filter model) //returns the exchange that user disabled.Calling when no  exchange are enabled
        {
            if (model.BinanceFutures == false)
            {
                return "Binance_Futures";
            }
            if (model.BinanceSpot == false)
            {
                return "Binance_Spot";
            }
            if (model.BybitFutures == false)
            {
                return "Bybit_Futures";
            }
            if (model.BybitSpot == false)
            {
                return "Bybit_Spot";
            }
            if (model.BitgetFutures == false)
            {
                return "Bitget_Futures";
            }
            if (model.BitgetSpot == false)
            {
                return "Bitget_Spot";
            }
            if (model.GateFutures == false)
            {
                return "Gate_Futures";
            }
            if (model.GateSpot == false)
            {
                return "Gate_Spot";
            }
            if (model.MexcFutures == false)
            {
                return "Mexc_Futures";
            }
            if (model.MexcSpot == false)
            {
                return "Mexc_Spot";
            }

            //adding asterdex
            if (model.AsterDexFutures == false)
            {
                return "AsterDex_Futures";
            }
            //adding kucoin
            if (model.KuCoinFutures == false)
            {
                return "KuCoin_Futures";
            }
            if (model.KuCoinSpot == false)
            {
                return "KuCoin_Spot";
            }
            //adding hyperliquid
            if (model.HyperliquidFutures == false)
            {
                return "Hyperliquid_Futures";
            }
            if (model.HyperliquidSpot == false)
            {
                return "Hyperliquid_Spot";
            }
            //adding Bingx
            if (model.BingxFutures == false)
            {
                return "Bingx_Futures";
            }
            if (model.BingxSpot == false)
            {
                return "Bingx_Spot";
            }
            //adding okx
            if (model.OkxFutures == false)
            {
                return "Okx_Futures";
            }
            if (model.OkxSpot == false)
            {
                return "Okx_Spot";
            }
            //adding phemex
            if (model.PhemexFutures == false)
            {
                return "Phemex_Futures";
            }
            if (model.PhemexSpot == false)
            {
                return "Phemex_Spot";
            }
            //adding weex
            if (model.WeexFutures == false)
            {
                return "Weex_Futures";
            }
            if (model.WeexSpot == false)
            {
                return "Weex_Spot";
            }
            return "NONE";
        }

        bool noOneChoosed(Filter model)
        {
            if (model.BinanceFutures == false && model.BinanceSpot == false && model.BybitFutures == false && model.BybitSpot == false
               && model.BitgetFutures == false && model.BitgetSpot == false && model.GateFutures == false && model.GateSpot == false
               && model.MexcFutures == false && model.MexcSpot == false      
               && model.AsterDexFutures == false && model.KuCoinFutures == false && model.KuCoinSpot == false 
               && model.HyperliquidFutures == false && model.HyperliquidSpot == false
               && model.BingxFutures == false && model.BingxSpot == false
               && model.OkxFutures == false && model.OkxSpot == false
               && model.PhemexFutures == false && model.PhemexSpot == false
               && model.WeexFutures == false && model.WeexSpot == false)
            {
                return true;
            }
            return false;
        }
        string AddAnd(string query)
        {
            if (query != "")
            {
                return " AND ";
            }
            return "";
        }

        string AddUnion(string query)
        {
            if (query != "SELECT s.Symbol, s.BaseAsset, s.QuoteAsset FROM(")
            {
                return @" 
                        Union 
                        ";
            }
            return "";
        }

        string AddAndForDisabled(string query)
        {
            if (query != " WHERE ")
            {
                return " AND ";
            }
            return "";
        }

        string ExchangesOneEnabled(Filter model) //This function constructing the query when at least one exchange is enabled
        {
            string query = "";
            if (model.BinanceFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Binance_Futures binanceF
                    WHERE binanceF.BaseAsset = mf.BaseAsset
                      AND binanceF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BinanceFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Binance_Futures binanceF
                    WHERE binanceF.BaseAsset = mf.BaseAsset
                      AND binanceF.QuoteAsset = mf.QuoteAsset
                )";
                }
            }

            if (model.BinanceSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Binance_Spot binanceS
                    WHERE binanceS.BaseAsset = mf.BaseAsset
                      AND binanceS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BinanceSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Binance_Spot binanceS
                    WHERE binanceS.BaseAsset = mf.BaseAsset
                      AND binanceS.QuoteAsset = mf.QuoteAsset
                )";
                }
            }


            if (model.BybitFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bybit_Futures bybitf
                    WHERE bybitf.BaseAsset = mf.BaseAsset
                      AND bybitf.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BybitFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bybit_Futures bybitf
                    WHERE bybitf.BaseAsset = mf.BaseAsset
                      AND bybitf.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.BybitSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bybit_Spot bybitS
                    WHERE bybitS.BaseAsset = mf.BaseAsset
                      AND bybitS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BybitSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bybit_Spot bybitS
                    WHERE bybitS.BaseAsset = mf.BaseAsset
                      AND bybitS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.BitgetFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bitget_Futures bitgetF
                    WHERE bitgetF.BaseAsset = mf.BaseAsset
                      AND bitgetF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BitgetFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bitget_Futures bitgetF
                    WHERE bitgetF.BaseAsset = mf.BaseAsset
                      AND bitgetF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.BitgetSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bitget_Spot bitgetS
                    WHERE bitgetS.BaseAsset = mf.BaseAsset
                      AND bitgetS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BitgetSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bitget_Spot bitgetS
                    WHERE bitgetS.BaseAsset = mf.BaseAsset
                      AND bitgetS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.GateFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Gate_Futures GateF
                    WHERE GateF.BaseAsset = mf.BaseAsset
                      AND GateF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.GateFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Gate_Futures GateF
                    WHERE GateF.BaseAsset = mf.BaseAsset
                      AND GateF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.GateSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Gate_Spot GateS
                    WHERE GateS.BaseAsset = mf.BaseAsset
                      AND GateS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.GateSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Gate_Spot GateS
                    WHERE GateS.BaseAsset = mf.BaseAsset
                      AND GateS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.MexcFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Mexc_Futures MexcF
                    WHERE MexcF.BaseAsset = mf.BaseAsset
                      AND MexcF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.MexcFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Mexc_Futures MexcF
                    WHERE MexcF.BaseAsset = mf.BaseAsset
                      AND MexcF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.MexcSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Mexc_Spot MexcS
                    WHERE MexcS.BaseAsset = mf.BaseAsset
                      AND MexcS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.MexcSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Mexc_Spot MexcS
                    WHERE MexcS.BaseAsset = mf.BaseAsset
                      AND MexcS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding asterdex
            if (model.AsterDexFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM AsterDex_Futures AsterdexF
                    WHERE AsterdexF.BaseAsset = mf.BaseAsset
                      AND AsterdexF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.AsterDexFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM AsterDex_Futures AsterdexF
                    WHERE AsterdexF.BaseAsset = mf.BaseAsset
                      AND AsterdexF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding kucoin
            if (model.KuCoinFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM KuCoin_Futures KuCoinF
                    WHERE KuCoinF.BaseAsset = mf.BaseAsset
                      AND KuCoinF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.KuCoinFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM KuCoin_Futures KuCoinF
                    WHERE KuCoinF.BaseAsset = mf.BaseAsset
                      AND KuCoinF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.KuCoinSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM KuCoin_Spot KuCoinS
                    WHERE KuCoinS.BaseAsset = mf.BaseAsset
                      AND KuCoinS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.KuCoinSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM KuCoin_Spot KuCoinS
                    WHERE KuCoinS.BaseAsset = mf.BaseAsset
                      AND KuCoinS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding hyperliquid
            if (model.HyperliquidFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Hyperliquid_Futures HyperliquidF
                    WHERE HyperliquidF.BaseAsset = mf.BaseAsset
                      AND HyperliquidF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.HyperliquidFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Hyperliquid_Futures HyperliquidF
                    WHERE HyperliquidF.BaseAsset = mf.BaseAsset
                      AND HyperliquidF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.HyperliquidSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Hyperliquid_Spot HyperliquidS
                    WHERE HyperliquidS.BaseAsset = mf.BaseAsset
                      AND HyperliquidS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.HyperliquidSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Hyperliquid_Spot HyperliquidS
                    WHERE HyperliquidS.BaseAsset = mf.BaseAsset
                      AND HyperliquidS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding Bingx
            if (model.BingxFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bingx_Futures BingxF
                    WHERE BingxF.BaseAsset = mf.BaseAsset
                      AND BingxF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BingxFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bingx_Futures BingxF
                    WHERE BingxF.BaseAsset = mf.BaseAsset
                      AND BingxF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.BingxSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Bingx_Spot BingxS
                    WHERE BingxS.BaseAsset = mf.BaseAsset
                      AND BingxS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.BingxSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Bingx_Spot BingxS
                    WHERE BingxS.BaseAsset = mf.BaseAsset
                      AND BingxS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding okx
            if (model.OkxFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Okx_Futures OkxF
                    WHERE OkxF.BaseAsset = mf.BaseAsset
                      AND OkxF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.OkxFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Okx_Futures OkxF
                    WHERE OkxF.BaseAsset = mf.BaseAsset
                      AND OkxF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.OkxSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Okx_Spot OkxS
                    WHERE OkxS.BaseAsset = mf.BaseAsset
                      AND OkxS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.OkxSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Okx_Spot OkxS
                    WHERE OkxS.BaseAsset = mf.BaseAsset
                      AND OkxS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            //adding Phemex
            if (model.PhemexFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Phemex_Futures PhemexF
                    WHERE PhemexF.BaseAsset = mf.BaseAsset
                      AND PhemexF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.PhemexFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Phemex_Futures PhemexF
                    WHERE PhemexF.BaseAsset = mf.BaseAsset
                      AND PhemexF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.PhemexSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Phemex_Spot PhemexS
                    WHERE PhemexS.BaseAsset = mf.BaseAsset
                      AND PhemexS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.PhemexSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Phemex_Spot PhemexS
                    WHERE PhemexS.BaseAsset = mf.BaseAsset
                      AND PhemexS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            // adding Weex
            if (model.WeexFutures == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Weex_Futures WeexF
                    WHERE WeexF.BaseAsset = mf.BaseAsset
                      AND WeexF.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.WeexFutures == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Weex_Futures WeexF
                    WHERE WeexF.BaseAsset = mf.BaseAsset
                      AND WeexF.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }

            if (model.WeexSpot == true)
            {
                query += AddAnd(query);
                query += @"EXISTS (
                    SELECT 1 FROM Weex_Spot WeexS
                    WHERE WeexS.BaseAsset = mf.BaseAsset
                      AND WeexS.QuoteAsset = mf.QuoteAsset
                )";
            }
            else
            {
                if (model.WeexSpot == false)
                {
                    query += AddAnd(query);
                    query += @"NOT EXISTS (
                    SELECT 1 FROM Weex_Spot WeexS
                    WHERE WeexS.BaseAsset = mf.BaseAsset
                      AND WeexS.QuoteAsset = mf.QuoteAsset
                    )";
                }
            }
            query += ';';

            return query;
        }

        string DisabledExchanges(Filter model)// This function construct the query when no one exchange is enabled and there are only disabled exchanges
        {
            string query = "SELECT s.Symbol, s.BaseAsset, s.QuoteAsset FROM(";//main query
            string queryTemp = " WHERE ";//the second part of query,in the past we gonna split 

            if (model.BinanceFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Binance_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                                SELECT 1
                                FROM Binance_Futures binF
                                WHERE binF.BaseAsset = s.BaseAsset
                                  AND binF.QuoteAsset = s.QuoteAsset
                                )";
            }

            if (model.BinanceSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Binance_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Binance_Spot binS
                            WHERE binS.BaseAsset = s.BaseAsset
                                AND binS.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.BybitFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bybit_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bybit_Futures bybF
                            WHERE bybF.BaseAsset = s.BaseAsset
                                AND bybF.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.BybitSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bybit_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bybit_Spot bybS
                            WHERE bybS.BaseAsset = s.BaseAsset
                                AND bybS.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.BitgetFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bitget_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bitget_Futures bitgF
                            WHERE bitgF.BaseAsset = s.BaseAsset
                                AND bitgF.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.BitgetSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bitget_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bitget_Spot bitgS
                            WHERE bitgS.BaseAsset = s.BaseAsset
                                AND bitgS.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.GateFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Gate_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Gate_Futures gateF
                            WHERE gateF.BaseAsset = s.BaseAsset
                                AND gateF.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.GateSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Gate_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Gate_Spot gateS
                            WHERE gateS.BaseAsset = s.BaseAsset
                                AND gateS.QuoteAsset = s.QuoteAsset
                            )";

            }


            if (model.MexcFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Mexc_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Mexc_Futures mexcF
                            WHERE mexcF.BaseAsset = s.BaseAsset
                                AND mexcF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.MexcSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Mexc_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Mexc_Spot mexcS
                            WHERE mexcS.BaseAsset = s.BaseAsset
                                AND mexcS.QuoteAsset = s.QuoteAsset
                            )";

            }

            //adding asterdex
            if (model.AsterDexFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM AsterDex_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM AsterDex_Futures asterdexF
                            WHERE asterdexF.BaseAsset = s.BaseAsset
                                AND asterdexF.QuoteAsset = s.QuoteAsset
                            )";

            }

            //adding kucoin
            if (model.KuCoinFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM KuCoin_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM KuCoin_Futures KuCoinF
                            WHERE KuCoinF.BaseAsset = s.BaseAsset
                                AND KuCoinF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.KuCoinSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM KuCoin_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM KuCoin_Spot KuCoinS
                            WHERE KuCoinS.BaseAsset = s.BaseAsset
                                AND KuCoinS.QuoteAsset = s.QuoteAsset
                            
                                 )";
            }

            //adding hyperliquid
            if (model.HyperliquidFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Hyperliquid_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Hyperliquid_Futures HyperliquidF
                            WHERE HyperliquidF.BaseAsset = s.BaseAsset
                                AND HyperliquidF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.HyperliquidSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Hyperliquid_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Hyperliquid_Spot HyperliquidS
                            WHERE HyperliquidS.BaseAsset = s.BaseAsset
                                AND HyperliquidS.QuoteAsset = s.QuoteAsset
                            
                                 )";
            }

            //adding Bingx
            if (model.BingxFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bingx_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bingx_Futures BingxF
                            WHERE BingxF.BaseAsset = s.BaseAsset
                                AND BingxF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.BingxSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Bingx_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Bingx_Spot BingxS
                            WHERE BingxS.BaseAsset = s.BaseAsset
                                AND BingxS.QuoteAsset = s.QuoteAsset
                            
                                 )";

            }

            //adding okx
            if (model.OkxFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Okx_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Okx_Futures OkxF
                            WHERE OkxF.BaseAsset = s.BaseAsset
                                AND OkxF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.OkxSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Okx_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Okx_Spot OkxS
                            WHERE OkxS.BaseAsset = s.BaseAsset
                                AND OkxS.QuoteAsset = s.QuoteAsset
                            
                                 )";

            }

            //adding Phemex
            if (model.PhemexFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Phemex_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Phemex_Futures PhemexF
                            WHERE PhemexF.BaseAsset = s.BaseAsset
                                AND PhemexF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.PhemexSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Phemex_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Phemex_Spot PhemexS
                            WHERE PhemexS.BaseAsset = s.BaseAsset
                                AND PhemexS.QuoteAsset = s.QuoteAsset
                            
                                 )";

            }

            //adding Weex
            if (model.WeexFutures != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Weex_Futures";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Weex_Futures WeexF
                            WHERE WeexF.BaseAsset = s.BaseAsset
                                AND WeexF.QuoteAsset = s.QuoteAsset
                            )";

            }

            if (model.WeexSpot != false)
            {
                query += AddUnion(query);
                query += "SELECT Symbol, BaseAsset, QuoteAsset FROM Weex_Spot";
            }
            else
            {
                queryTemp += AddAndForDisabled(queryTemp);
                queryTemp += @" NOT EXISTS(
                            SELECT 1
                            FROM Weex_Spot WeexS
                            WHERE WeexS.BaseAsset = s.BaseAsset
                                AND WeexS.QuoteAsset = s.QuoteAsset
                            
                                 )";

            }

            queryTemp += ";";
            query += ") AS s ";
            query += queryTemp;

            return query;

        }

        [HttpPost]
        public IActionResult Apply(Filter model)
        {
            if (UpdateStatus.IsUpdating)
            {

                ViewBag.Message = "Coin update in progress, please try again after 2 minutes.It is doing for adding new listings and remove delisted coins.";
                return View("Index", model);
            }

            if(noOneChoosed(model))
            {
                return View("Index", model);
            }

            string query = "";


            if (mainExchange(model) != "NONE")
            {
                //Console.WriteLine("hello");
                query =
                   @"SELECT mf.Symbol, mf.BaseAsset, mf.QuoteAsset
                    FROM " + mainExchange(model) + @" mf
                    WHERE ";


                query += ExchangesOneEnabled(model);


                //Console.WriteLine(query);
            }
            else
            {
                if (MainDisabledExchange(model) != "NONE")
                {

                    query += DisabledExchanges(model);
                    //Console.WriteLine("hello");
                    Console.WriteLine("here1" +  query);

                }
                else
                {
                    //add new exchange here
                    query = "SELECT s.Symbol, s.BaseAsset, s.QuoteAsset FROM(SELECT Symbol, BaseAsset, QuoteAsset FROM Binance_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Binance_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Bybit_Futures" +
                            " Union  SELECT Symbol, BaseAsset, QuoteAsset FROM Bybit_Spot " +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Bitget_Futures " +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Bitget_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Gate_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Gate_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Mexc_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Mexc_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM KuCoin_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM KuCoin_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM AsterDex_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Hyperliquid_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Hyperliquid_Spot " +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Bingx_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Bingx_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Okx_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Okx_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Phemex_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Phemex_Spot" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Weex_Futures" +
                            " Union SELECT Symbol, BaseAsset, QuoteAsset FROM Weex_Spot) AS s;";

                    //Console.WriteLine("here2" + query);

                }
            }

            List<string> coins = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    coins.Add($"{reader["Symbol"]} ({reader["BaseAsset"]}/{reader["QuoteAsset"]})");
                }
            }

            model.FilteredCoins = coins;

            return View("Index", model);
        }

    }
}
