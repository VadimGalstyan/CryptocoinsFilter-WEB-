using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;


namespace CryptocoinsFilter.Background_Tasks
{


    class ProgramLogic
    {
        public async Task runTask()
        {
            string connectionString = "Server=LAPTOP-EK1FURHH\\SQLEXPRESS;Database=CryptoExchanges;Trusted_Connection=True;Encrypt=False;";

            var exchanges = new[]
            {
                new { Name="Binance", SpotApi="https://api.binance.com/api/v3/exchangeInfo", FuturesApi="https://fapi.binance.com/fapi/v1/exchangeInfo" },
                new { Name="Bybit", SpotApi="https://api.bybit.com/v5/market/instruments-info?category=spot", FuturesApi="https://api.bybit.com/v5/market/instruments-info?category=linear" },
                new { Name="Bitget", SpotApi="https://api.bitget.com/api/v2/spot/public/symbols", FuturesApi="https://api.bitget.com/api/v2/mix/market/contracts?productType=USDT-FUTURES" },
                new { Name="Gate", SpotApi="https://api.gateio.ws/api/v4/spot/currency_pairs", FuturesApi="https://api.gateio.ws/api/v4/futures/usdt/contracts" },

                new { Name="MEXC", SpotApi="https://api.mexc.com/api/v3/exchangeInfo", FuturesApi="https://contract.mexc.com/api/v1/contract/detail" },

                //adding asterdex
                new { Name="AsterDex", SpotApi="", FuturesApi="https://fapi.asterdex.com/fapi/v1/exchangeInfo" },
                //adding kucoin
                new {Name = "KuCoin",SpotApi = "https://api.kucoin.com/api/v2/symbols",FuturesApi = "https://api-futures.kucoin.com/api/v1/contracts/active"},
                //adding hyperliquid
                new {Name = "Hyperliquid",SpotApi = "https://api.hyperliquid.xyz/info",FuturesApi = "https://api.hyperliquid.xyz/info"},
                //adding Bingx
                new {Name="Bingx",SpotApi="https://open-api.bingx.com/openApi/spot/v1/common/symbols",FuturesApi="https://open-api.bingx.com/openApi/swap/v2/quote/contracts"},
                //adding okx
                new {Name="OKX",SpotApi="https://www.okx.com/api/v5/public/instruments?instType=SPOT",FuturesApi="https://www.okx.com/api/v5/public/instruments?instType=SWAP"},
                //adding Phemex
                new {Name="Phemex",SpotApi="https://api.phemex.com/public/products",FuturesApi="https://api.phemex.com/public/products"},
                //adding Weex
                new {Name="Weex",SpotApi="https://api-spot.weex.com/api/v3/exchangeInfo",FuturesApi="https://api-contract.weex.com/capi/v3/market/exchangeInfo"},

            };



            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            foreach (var ex in exchanges)
            {
                string spotTable = $"{ex.Name}_Spot";
                string futTable = $"{ex.Name}_Futures";

                await CreateTableIfNotExists(conn, spotTable);
                await CreateTableIfNotExists(conn, futTable);

                await ClearTable(conn, spotTable);
                await ClearTable(conn, futTable);

                // Spot
                try
                {
                    await ImportSymbols(ex.SpotApi, spotTable, conn, ex.Name);
                    Console.WriteLine($"{ex.Name} Spot Done");
                }
                catch (Exception exSpot)
                {
                    Console.WriteLine($"Error Spot {ex.Name}: {exSpot.Message}");
                }

                // Futures
                try
                {
                    await ImportFutures(ex.FuturesApi, futTable, conn, ex.Name);
                    Console.WriteLine($"{ex.Name} Futures Done");
                }
                catch (Exception exFut)
                {
                    Console.WriteLine($"Error Futures {ex.Name}: {exFut.Message}");
                }
            }

            Console.WriteLine("All imports are done!");
        }

        static async Task CreateTableIfNotExists(SqlConnection conn, string tableName)
        {
            string query = "";
            if (tableName.EndsWith("_Spot"))
            {
                query = $@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                        CREATE TABLE {tableName} (
                            Id INT IDENTITY PRIMARY KEY,
                            Symbol NVARCHAR(50),
                            BaseAsset NVARCHAR(50),
                            QuoteAsset NVARCHAR(50),
                            Status NVARCHAR(50)
                        );";


            }
            else if (tableName.EndsWith("_Futures"))
            {

                query = $@"
                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                            CREATE TABLE {tableName} (
                                Id INT IDENTITY PRIMARY KEY,
                                Symbol NVARCHAR(50),
                                BaseAsset NVARCHAR(50),
                                QuoteAsset NVARCHAR(50),
                                Status NVARCHAR(50),
                                MaxLeverage SMALLINT
                            );";

            }


            if (!string.IsNullOrEmpty(query))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        static async Task ClearTable(SqlConnection conn, string tableName)
        {
            var cmd = new SqlCommand($"DELETE FROM {tableName};", conn);
            await cmd.ExecuteNonQueryAsync();
        }

        static async Task ImportSymbols(string apiUrl, string tableName, SqlConnection conn, string exchangeName)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("csharp-exchange-client/1.0");
            http.Timeout = TimeSpan.FromSeconds(30);

            string resp = "";

            if (exchangeName == "Hyperliquid")
            {
                resp = ""; //for hyperliquid we are using POST method
            }
            else
            {
                resp = await http.GetStringAsync(apiUrl); //for others GET method
            }

            var symbols = new List<(string Symbol, string BaseAsset, string QuoteAsset, string Status)>();

            try
            {
                switch (exchangeName)
                {
                    case "Binance":
                        var binanceJson = JObject.Parse(resp);
                        var binanceSymbols = binanceJson["symbols"] as JArray;
                        if (binanceSymbols != null)
                        {
                            foreach (var s in binanceSymbols)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "TRADING"
                                ));
                            }
                        }
                        break;

                    case "Bybit":
                        var bybitJson = JObject.Parse(resp);
                        var bybitResult = bybitJson["result"];
                        if (bybitResult != null)
                        {
                            var bybitList = bybitResult["list"] as JArray;
                            if (bybitList != null)
                            {
                                foreach (var s in bybitList)
                                {
                                    symbols.Add((
                                        s["symbol"]?.ToString() ?? "",
                                        s["baseCoin"]?.ToString() ?? "",
                                        s["quoteCoin"]?.ToString() ?? "",
                                        s["status"]?.ToString() ?? "Trading"
                                    ));
                                }
                            }
                        }
                        break;

                    case "Bitget":
                        var bitgetJson = JObject.Parse(resp);
                        var bitgetData = bitgetJson["data"] as JArray;
                        if (bitgetData != null)
                        {
                            foreach (var s in bitgetData)
                            {
                                symbols.Add((
                                    s["symbolName"]?.ToString() ?? s["symbol"]?.ToString() ?? "",
                                    s["baseCoin"]?.ToString() ?? "",
                                    s["quoteCoin"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "online"
                                ));
                            }
                        }
                        break;

                    case "Gate":
                        var gateArray = JArray.Parse(resp);
                        foreach (var s in gateArray)
                        {
                            symbols.Add((
                                s["id"]?.ToString() ?? "",
                                s["base"]?.ToString() ?? "",
                                s["quote"]?.ToString() ?? "",
                                s["trade_status"]?.ToString() == "tradable" ? "TRADING" : "DELISTED"
                            ));
                        }
                        break;

                    case "MEXC":
                        var mexcJson = JObject.Parse(resp);
                        var mexcSymbols = mexcJson["symbols"] as JArray;
                        if (mexcSymbols != null)
                        {
                            foreach (var s in mexcSymbols)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "TRADING"
                                ));
                            }
                        }
                        break;

                    //asterdex has no spot

                    //adding kucoin spot
                    case "KuCoin":
                        var kucoinJson = JObject.Parse(resp);
                        var kucoinData = kucoinJson["data"] as JArray;

                        if (kucoinData != null)
                        {
                            foreach (var s in kucoinData)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseCurrency"]?.ToString() ?? "",
                                    s["quoteCurrency"]?.ToString() ?? "",
                                    s["enableTrading"]?.ToObject<bool>() == true ? "TRADING" : "DELISTED"
                                ));
                            }
                        }
                        break;

                    //adding hyperliquid spot
                    case "Hyperliquid":
                        var hlSpotContent = new StringContent("{\"type\": \"spotMeta\"}", System.Text.Encoding.UTF8, "application/json");
                        var hlSpotResponse = await http.PostAsync(apiUrl, hlSpotContent);
                        hlSpotResponse.EnsureSuccessStatusCode();

                        var hlSpotData = await hlSpotResponse.Content.ReadAsStringAsync();
                        var hlSpotJson = JObject.Parse(hlSpotData);

                        var tokenMap = new Dictionary<int, string>();
                        var tokensArray = hlSpotJson["tokens"] as JArray;
                        if (tokensArray != null)
                        {
                            for (int i = 0; i < tokensArray.Count; i++)
                            {
                                tokenMap[i] = tokensArray[i]["name"]?.ToString() ?? i.ToString();
                            }
                        }

                        var spotUniverse = hlSpotJson["universe"] as JArray;
                        if (spotUniverse != null)
                        {
                            foreach (var s in spotUniverse)
                            {
                                var indices = s["tokens"] as JArray;
                                string name = s["name"]?.ToString() ?? "";

                                string baseAsset = "Unknown";
                                string quoteAsset = "USDC";

                                if (indices != null && indices.Count >= 1)
                                {
                                    int baseIdx = indices[0].Value<int>();
                                    if (tokenMap.ContainsKey(baseIdx))
                                    {
                                        baseAsset = tokenMap[baseIdx];
                                    }
                                }

                                string dbSymbol = $"{baseAsset}/{quoteAsset}";

                                symbols.Add((
                                    dbSymbol,
                                    baseAsset,
                                    quoteAsset,
                                    "TRADING"
                                ));
                            }
                        }
                        break;

                    case "Bingx":
                        var bingxSpotRoot = JObject.Parse(resp);
                        var bxSpotSymbols = bingxSpotRoot["data"]?["symbols"] as JArray;
                        if (bxSpotSymbols != null)
                        {
                            foreach (var s in bxSpotSymbols)
                            {
                                string symbol = s["symbol"]?.ToString() ?? "";
                                // BingX spot has this formate BTC-USDT
                                var parts = symbol.Split('-');
                                symbols.Add((
                                    symbol,
                                    parts.Length > 0 ? parts[0] : symbol,
                                    parts.Length > 1 ? parts[1] : "USDT",
                                    s["status"]?.ToString() == "1" ? "TRADING" : "BREAK"
                                ));
                            }
                        }
                        break;

                    case "OKX":
                        var okxSpotJson = JObject.Parse(resp);
                        var okxSpotData = okxSpotJson["data"] as JArray;
                        if (okxSpotData != null)
                        {
                            foreach (var s in okxSpotData)
                            {
                                symbols.Add((
                                    s["instId"]?.ToString() ?? "",
                                    s["baseCcy"]?.ToString() ?? "",
                                    s["quoteCcy"]?.ToString() ?? "",
                                    s["state"]?.ToString() == "live" ? "TRADING" : "DELISTED"
                                ));
                            }
                        }
                        break;

                    case "Phemex":
                        var phemexSpotJson = JObject.Parse(resp);
                        var phemexSpotData = phemexSpotJson["data"]?["products"] as JArray;
                        if (phemexSpotData != null)
                        {
                            string cleanSymbol = ""; //from phemex spot symbols come like "sBTCUSDT",s shall be removed
                            foreach (var s in phemexSpotData)
                            {
                                if (s["type"]?.ToString() == "Spot")
                                {
                                    cleanSymbol = s["symbol"]?.ToString().Remove(0, 1) ?? "";

                                    symbols.Add((
                                        cleanSymbol,
                                        s["baseCurrency"]?.ToString() ?? "",
                                        s["quoteCurrency"]?.ToString() ?? "",
                                        s["status"]?.ToString() == "Listed" ? "Trading" : "Break"
                                    ));

                                }
                            }
                        }
                        break;

                    case "Weex":
                        var weexSpotJson = JObject.Parse(resp);
                        var weexSpotData = weexSpotJson["symbols"] as JArray;
                        if (weexSpotData != null)
                        {
                            foreach (var s in weexSpotData)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "TRADING"
                                ));
                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spot parsing error for {exchangeName}: {ex.Message}");
                return;
            }

            foreach (var (symbol, baseAsset, quoteAsset, status) in symbols)
            {
                try
                {
                    var cmd = new SqlCommand(
                        $"INSERT INTO {tableName} (Symbol, BaseAsset, QuoteAsset, Status) VALUES (@Symbol,@BaseAsset,@QuoteAsset,@Status)",
                        conn
                    );

                    cmd.Parameters.AddWithValue("@Symbol", symbol);
                    cmd.Parameters.AddWithValue("@BaseAsset", baseAsset);
                    cmd.Parameters.AddWithValue("@QuoteAsset", quoteAsset);
                    cmd.Parameters.AddWithValue("@Status", status);

                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Spot symbol adding error {symbol}: {ex.Message}");
                }
            }
        }

        static async Task ImportFutures(string apiUrl, string tableName, SqlConnection conn, string exchangeName)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("csharp-exchange-client/1.0");
            http.Timeout = TimeSpan.FromSeconds(30);

            string resp = "";
            if (exchangeName == "Hyperliquid")
            {
                resp = ""; //for hyperliquid we use POST method
            }
            else
            {
                resp = await http.GetStringAsync(apiUrl); //for others GET
            }

            var symbols = new List<(string Symbol, string BaseAsset, string QuoteAsset, string Status, short MaxLeverage)>();

            try
            {
                switch (exchangeName)
                {
                    case "Binance":
                        var binanceJson = JObject.Parse(resp);
                        var binanceSymbols = binanceJson["symbols"] as JArray;
                        if (binanceSymbols != null)
                        {
                            short leverage = 0;  //leverage = 100/(requiredMaginPercent)

                            foreach (var s in binanceSymbols)
                            {
                                //leverage = -1;
                                //var requiredMaginPercent = s["requiredMarginPercent"].ToString();

                                //if (decimal.TryParse(requiredMaginPercent, System.Globalization.CultureInfo.InvariantCulture, out decimal marginPercent) && marginPercent > 0)
                                //{
                                //    leverage = (short)(100 / marginPercent);
                                //    Console.WriteLine($"MarginePercent = {marginPercent} / Leverage = {leverage}");
                                //}

                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "TRADING",
                                    leverage
                                ));
                            }
                        }
                        break;

                    case "Bybit":
                        var bybitJson = JObject.Parse(resp);
                        var bybitResult = bybitJson["result"];
                        if (bybitResult != null)
                        {
                            var bybitList = bybitResult["list"] as JArray;
                            if (bybitList != null)
                            {
                                foreach (var s in bybitList)
                                {
                                    symbols.Add((
                                        s["symbol"]?.ToString() ?? "",
                                        s["baseCoin"]?.ToString() ?? "",
                                        s["quoteCoin"]?.ToString() ?? "",
                                        s["status"]?.ToString() ?? "Trading",
                                        0
                                    ));
                                }
                            }
                        }
                        break;

                    case "Bitget":
                        var bitgetJson = JObject.Parse(resp);
                        var bitgetData = bitgetJson["data"] as JArray;
                        if (bitgetData != null)
                        {
                            foreach (var s in bitgetData)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseCoin"]?.ToString() ?? "",
                                    s["quoteCoin"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "normal",
                                    0
                                ));
                            }
                        }
                        break;


                    case "Gate":
                        JArray gateArray;

                        if (resp.TrimStart().StartsWith("["))
                            gateArray = JArray.Parse(resp);
                        else
                            gateArray = JArray.Parse(JObject.Parse(resp)["data"]!.ToString());

                        foreach (var s in gateArray)
                        {
                            string name = s["name"]?.ToString() ?? "";

                            string baseAsset = "";
                            string quoteAsset = "";

                            if (name.Contains("_"))
                            {
                                var parts = name.Split("_");
                                baseAsset = parts[0];
                                quoteAsset = parts[1];
                            }

                            string statusRaw = s["status"]?.ToString()?.ToLower() ?? "trading";
                            string status = statusRaw == "trading" ? "TRADING" : "DELISTED";

                            symbols.Add((name, baseAsset, quoteAsset, status, 0));
                        }
                        break;

                    case "MEXC":
                        var mexcJson = JObject.Parse(resp);
                        var mexcDataArray = mexcJson["data"] as JArray;
                        if (mexcDataArray != null)
                        {
                            foreach (var s in mexcDataArray)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseCoin"]?.ToString() ?? "",
                                    s["quoteCoin"]?.ToString() ?? "USDT",
                                    s["state"]?.ToString() == "1" ? "TRADING" : "DELISTED",
                                    0
                                ));
                            }
                        }
                        break;

                    //adding asterdex
                    case "AsterDex":
                        var asterJson = JObject.Parse(resp);
                        var asterSymbols = asterJson["symbols"] as JArray;

                        if (asterSymbols != null)
                        {
                            foreach (var s in asterSymbols)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "TRADING",
                                    0
                                ));
                            }
                        }
                        break;

                    //adding kucoin
                    case "KuCoin":
                        var kucoinJson = JObject.Parse(resp);
                        var kucoinData = kucoinJson["data"] as JArray;

                        if (kucoinData != null)
                        {
                            foreach (var s in kucoinData)
                            {
                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseCurrency"]?.ToString() ?? "",
                                    s["quoteCurrency"]?.ToString() ?? "USDT",
                                    s["status"]?.ToString() == "Open" ? "TRADING" : "DELISTED",
                                    0
                                ));
                            }
                        }
                        break;

                    //adding hyperliquid
                    case "Hyperliquid":
                        var hlFutContent = new StringContent("{\"type\": \"meta\"}", System.Text.Encoding.UTF8, "application/json");
                        var hlFutResponse = await http.PostAsync(apiUrl, hlFutContent);
                        hlFutResponse.EnsureSuccessStatusCode();

                        var hlFutData = await hlFutResponse.Content.ReadAsStringAsync();
                        var hlFutJson = JObject.Parse(hlFutData);
                        var universeFut = hlFutJson["universe"] as JArray;

                        if (universeFut != null)
                        {
                            foreach (var s in universeFut)
                            {
                                string name = s["name"]?.ToString() ?? "";
                                symbols.Add((
                                    name,
                                    name,
                                    "USDC",
                                    "TRADING",
                                    0
                                ));
                            }
                        }
                        break;

                    case "Bingx":
                        var bingxFutRoot = JObject.Parse(resp);
                        var bxFutSymbols = bingxFutRoot["data"] as JArray;
                        if (bxFutSymbols != null)
                        {
                            foreach (var s in bxFutSymbols)
                            {
                                string symbol = s["symbol"]?.ToString() ?? "";
                                symbols.Add((
                                    symbol,
                                    s["asset"]?.ToString() ?? symbol.Split('-')[0],
                                    s["currency"]?.ToString() ?? "USDT",
                                    s["status"]?.ToString() == "1" ? "TRADING" : "BREAK",
                                    0
                                ));
                            }
                        }
                        break;

                    case "OKX":
                        var okxFutJson = JObject.Parse(resp);
                        var okxFutData = okxFutJson["data"] as JArray;
                        if (okxFutData != null)
                        {
                            foreach (var s in okxFutData)
                            {
                                symbols.Add((
                                    s["instId"]?.ToString() ?? "",
                                    s["ctValCcy"]?.ToString() ?? "",
                                    s["settleCcy"]?.ToString() ?? "USDT",
                                    s["state"]?.ToString() == "live" ? "TRADING" : "DELISTED",
                                    0
                                ));
                            }
                        }
                        break;

                    case "Phemex":
                        var phemexFutJson = JObject.Parse(resp);
                        var phemexFutData = phemexFutJson["data"]?["perpProductsV2"] as JArray;
                        if (phemexFutData != null)
                        {
                            short leverage = 0;
                            foreach (var s in phemexFutData)
                            {

                                short.TryParse(s["maxOpenPosLeverage"]?.ToString(), out leverage);

                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["contractUnderlyingAssets"]?.ToString() ?? "",
                                    s["quoteCurrency"]?.ToString() ?? "",
                                    s["status"]?.ToString() == "Listed" ? "Trading" : "Break",
                                    leverage

                                ));

                            }
                        }
                        break;

                    case "Weex":
                        var weexFutJson = JObject.Parse(resp);
                        var weexFutData = weexFutJson["symbols"] as JArray;
                        if (weexFutData != null)
                        {
                            short leverage = 0;
                            foreach (var s in weexFutData)
                            {

                                short.TryParse(s["maxLeverage"]?.ToString(), out leverage);

                                symbols.Add((
                                    s["symbol"]?.ToString() ?? "",
                                    s["baseAsset"]?.ToString() ?? "",
                                    s["quoteAsset"]?.ToString() ?? "",
                                    s["status"]?.ToString() ?? "Trading",
                                    leverage

                                ));

                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Futures parsing error for {exchangeName}: {ex.Message}");
                return;
            }


            /////////////????????????????????????????????????????????????????????
            foreach (var (symbol, baseAsset, quoteAsset, status, leverage) in symbols)
            {
                try
                {
                    var cmd = new SqlCommand(
                        $"INSERT INTO {tableName} (Symbol, BaseAsset, QuoteAsset, Status, MaxLeverage) VALUES (@Symbol,@BaseAsset,@QuoteAsset,@Status,@MaxLeverage)",
                        conn
                    );

                    cmd.Parameters.AddWithValue("@Symbol", symbol);
                    cmd.Parameters.AddWithValue("@BaseAsset", baseAsset);
                    cmd.Parameters.AddWithValue("@QuoteAsset", quoteAsset);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@MaxLeverage", leverage);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Futures symbol adding error {symbol}: {ex.Message}");
                }
            }
        }
    }

}