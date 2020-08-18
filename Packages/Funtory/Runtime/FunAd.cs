using SeganX;
using UnityEngine;
using TapsellPlusSDK;

public class FunAd : MonoBehaviour
{
    public delegate bool OnPreRequestCallback(Segment segment);

    public abstract class Segment
    {
        public string ResponseZone { get; set; }
        public bool IsReady => ResponseZone.HasContent();
        public abstract string ZoneId { get; }
    }

    public class NativeData
    {
        public string title = null;
        public string description = null;
        public string callToActionText = null;
        public Sprite portraitBannerImage = null;
        public Sprite landscapeBannerImage = null;
        public Sprite iconImage = null;
        public System.Action onClick = null;
    }

    void Start()
    {
        instance = this;
        TapsellPlus.initialize(string.Empty);
    }



    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    public static OnPreRequestCallback OnPreRequest = segment => true;

    private static FunAd instance = null;

    public static class Banner
    {
        public static void Show(Segment segment)
        {
            if (OnPreRequest(segment) == false) return;

            var zone = segment.ZoneId;
            Debug.Log("Tapsell.ShowBanner: " + zone);

            if (zone.IsNullOrEmpty())
            {
                Hide();
                return;
            }

            TapsellPlus.showBannerAd(zone, BannerType.BANNER_320x50, Gravity.BOTTOM, Gravity.CENTER,
                zoneId =>
                {
                    TapsellPlus.displayBanner();
                },
                error =>
                {
                    Debug.Log("Tapsell.showBannerAd.Error: " + error.message);
                });
        }

        public static void Hide()
        {
            TapsellPlus.hideBanner();
        }
    }

    public static class Rewarded
    {
        public static void Request(Segment segment, System.Action<bool> callback)
        {
            var zone = segment.ZoneId;
            Debug.Log("Tapsell.RequestRewardedVideo: " + zone);

            if (zone.IsNullOrEmpty())
            {
                callback?.Invoke(false);
                return;
            }

            if (segment.IsReady)
            {
                callback?.Invoke(true);
                return;
            }

            TapsellPlus.requestRewardedVideo(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId;
                    callback?.Invoke(true);
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestRewardedVideo.Error: " + error.message);
                    callback?.Invoke(false);
                });
        }

        public static void Show(Segment segment, System.Action<bool> callback)
        {
            ShowAd(false, segment, callback);
        }
    }

    public static class Interstitial
    {
        public static void Request(Segment segment, System.Action<bool> callback = null)
        {
            if (OnPreRequest(segment) == false) return;

            var zone = segment.ZoneId;
            Debug.Log("Tapsell.RequestInterestetial: " + zone);

            if (zone.IsNullOrEmpty())
            {
                callback?.Invoke(false);
                return;
            }

            if (segment.IsReady)
            {
                callback?.Invoke(true);
                return;
            }

            TapsellPlus.requestInterstitial(zone,
                zoneId =>
                {
                    segment.ResponseZone = zoneId;
                    callback?.Invoke(true);
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestInterstitial.Error: " + error.message);
                    callback?.Invoke(false);
                });
        }

        public static void Show(Segment segment, System.Action<bool> callback = null)
        {
            ShowAd(true, segment, callback);
        }
    }

    public static class Native
    {
        public static void Request(Segment segment, System.Action<NativeData, string> callback)
        {
            var zone = segment.ZoneId;
            Debug.Log("Tapsell.DisplayNativeBanner: " + zone);

            if (zone.IsNullOrEmpty())
            {
                callback(null, null);
                return;
            }

            TapsellPlus.requestNativeBanner(instance, zone,
                result =>
                {
                    var res = new NativeData()
                    {
                        title = result.title,
                        description = result.description,
                        callToActionText = result.callToActionText,
                        portraitBannerImage = Sprite.Create(result.portraitBannerImage, new Rect(0, 0, result.portraitBannerImage.width, result.portraitBannerImage.height), Vector2.one * 0.5f),
                        landscapeBannerImage = Sprite.Create(result.landscapeBannerImage, new Rect(0, 0, result.landscapeBannerImage.width, result.landscapeBannerImage.height), Vector2.one * 0.5f),
                        iconImage = Sprite.Create(result.iconImage, new Rect(0, 0, result.iconImage.width, result.iconImage.height), Vector2.one * 0.5f),
                        onClick = result.clicked
                    };
                },
                error =>
                {
                    Debug.Log("Tapsell.RequestInterstitial.Error: " + error.message);
                    callback(null, error.message);
                });
        }
    }

    private static void ShowAd(bool interestitial, Segment segment, System.Action<bool> callback)
    {
        Debug.Log("Tapsell.ShowAd: " + segment.ResponseZone);

        if (segment.IsReady)
        {
            TapsellPlus.showAd(
                segment.ResponseZone,
                zid =>
                {
                    Debug.Log("Tapsell.ShowAd.OnOpenAd: " + zid);
                },
                zid =>
                {
                    Debug.Log("Tapsell.ShowAd.OnCloseAd: " + zid);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(interestitial);
                    }
                },
                zid =>
                {
                    Debug.Log("Tapsell.ShowAd.OnRewarded: " + zid);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(true);
                    }
                },
                error =>
                {
                    Debug.Log("Tapsell.ShowAd.Error: " + error.message);
                    if (segment.ResponseZone.HasContent())
                    {
                        segment.ResponseZone = null;
                        callback?.Invoke(false);
                    }
                });
        }
        else callback?.Invoke(false);
    }
}