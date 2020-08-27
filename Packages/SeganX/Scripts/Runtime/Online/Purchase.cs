using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
    {
        public static class Purchase
        {
            public enum Provider { Market, Gateway }
#if SX_ONLINE

            [System.Serializable]
            private class StartPost
            {
                public int game_id = 0;
                public string provider = string.Empty;
            }

            [System.Serializable]
            private class EndPost
            {
                public int version = 0;
                public string token = string.Empty;
                public string sku = string.Empty;
                public string payload = string.Empty;
            }

            [System.Serializable]
            private class BazaarValidation
            {
                public string developerPayload = string.Empty;
                public long purchaseTime = 0;
            }

            private static string market_access_token
            {
                get { return PlayerPrefsEx.GetString("Online.Purchase.market_access_token", string.Empty); }
                set { PlayerPrefsEx.SetString("Online.Purchase.market_access_token", value); }
            }
#endif
            public static void Start(Provider provider, System.Action callback)
            {

                if (provider == Provider.Market)
                {
#if SX_ONLINE
                    var post = new StartPost();
                    post.game_id = Core.GameId;

#if BAZAAR                        
                    post.provider = "bazaar";
#elif MYKET                        
                    post.provider = "myket";
#endif

                    DownloadData<string>("purchase-start.php", post, (succeed, token) =>
                    {
                        if (succeed) market_access_token = token;
                        callback();
                    }, 0, 3);
#else
                    callback();
#endif
                }
                else if (provider == Provider.Gateway)
                {
                    callback();
                }
            }

            public static void End(Provider provider, int version, string sku, string token, System.Action<bool, string> callback)
            {
                Verify(provider, sku, token, (success, payload) =>
                {
                    callback(success, payload);

                    if (provider == Provider.Market)
                    {
#if SX_ONLINE
                        var post = new EndPost();
                        post.version = version;
                        post.sku = sku;
                        post.token = token;
                        post.payload = payload;
                        DownloadData<string>("purchase-end.php", post, (done, msg) => { }, 0, 3);
#endif
                    }
                    else if (provider == Provider.Gateway)
                    {

                    }
                });
            }

            public static void Verify(Provider provider, string sku, string token, System.Action<bool, string> callback)
            {
                if (provider == Provider.Market)
                {
#if SX_ONLINE

                    if (market_access_token.HasContent())
                    {
                        var tmp = new Dictionary<string, string>();
#if BAZAAR
                        var url = "https://pardakht.cafebazaar.ir/devapi/v2/api/validate/" + Application.identifier + "/inapp/" + sku + "/purchases/" + token + "/";
                        tmp.Add("Authorization", market_access_token);
#elif MYKET
                        var url = "https://developer.myket.ir/api/applications/" + Application.identifier + "/purchases/products/" + sku + "/tokens/" + token + "/";
                        tmp.Add("X-Access-Token", market_access_token);
#else
                        var url = string.Empty;
#endif


                        Http.DownloadText(url, null, tmp, resjson =>
                        {
                            var res = JsonUtility.FromJson<BazaarValidation>(resjson);
                            if (res != null)
                                callback(res.purchaseTime > 0, res.developerPayload);
                            else
                                callback(false, "Market not respond!");
                        },
                        null, 0, 3);
                    }
                    else
                    {
                        callback(true, Core.Salt);
                    }
#else
                        callback(true, Core.Salt);
#endif

                }
                else if (provider == Provider.Gateway)
                {
                    callback(true, Core.Salt);
                }
            }

        }
    }
}