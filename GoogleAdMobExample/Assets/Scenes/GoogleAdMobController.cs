using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using System;

/// <summary>
///
///     Code for this example is taken from the Google Ad Mob documentation.
///     They also have a sample project here: https://github.com/googleads/googleads-mobile-unity/tree/main/samples
///     but, have more examples is potentially beneficial to you.
///
///     Google Ad Mob Documentation:https://developers.google.com/admob/unity/quick-start
/// 
/// </summary>

public class GoogleAdMobController : MonoBehaviour
{

    private bool _isInitialized = false;
    private List<string> _deviceIds;

    // These ad units are configured to serve test ads.
#if UNITY_ANDROID
    private string _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";


#elif UNITY_IPHONE
    private string _bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    private string _interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
    private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";

#else
    private string _bannerAdUnitId = "unused";
    private string _interstitialAdUnitId = "unused";
    private string _rewardedAdUnitId = "unused";
#endif



    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    // Start is called before the first frame update
    void Start()
    {
        // Test Ad Tutorial: https://developers.google.com/admob/unity/test-ads#ios_1
        // Demonstrates how to configure Google Mobile Ads.
        // Google Mobile Ads needs to be run only once and before loading any ads.
        if (_isInitialized) return;

        // Setting up a test device lets you safely test production ads and
        // verify your implementation code without violating AdMob’s
        // invalid traffic policy.
        // How to setup Test IDs: https://support.google.com/admob/answer/9691433?hl=en
        _deviceIds = new List<string>();
        _deviceIds.Add(
            ""
            );

        RequestConfiguration requestConfig = new RequestConfiguration
                                                .Builder()
                                                .SetTestDeviceIds(_deviceIds)
                                                .build();

        MobileAds.SetRequestConfiguration(requestConfig);
#if UNITY_IPHONE
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif

        // Initialize the Google Mobile Ads SDK
        // If using mediation, we may want to wait until the callback
        // occurs before loaindg ads (e.g. obtain consent from users
        // in the European Economic Area EEA).
        MobileAds.Initialize(initStatus =>
        {
            _isInitialized = true;
            Debug.Log("[AdMob Controller] Google Mobile Ads Initialized");

            // After Mobile ads have been initialized load ads
            LoadRewardedAd();
        });
    }

    #region Banner Ads
    private void CreateBannerView()
    {
        Debug.Log("[AdMob Controller] Creating banner view");

        if(_bannerView != null)
        {
            DestroyBannerView();
        }

        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        ListenToBannerAdEvents();
    }

    public void ShowBannerAd()
    {
        DestroyBannerView();
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        Debug.Log("[AdMob Controller] Loading ad");
        _bannerView.LoadAd(adRequest);
    }

    private void ListenToBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("[AdMob Controller] Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("[AdMob Controller] Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("[AdMob Controller] Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("[AdMob Controller] Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("[AdMob Controller] Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("[AdMob Controller] Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMob Controller] Banner view full screen content closed.");
        };
    }

    private void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("[AdMob Controller] Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    #endregion

    #region Interstitial Ads
    public void ShowInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("[AdMob Controller] Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_interstitialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("[AdMob Controller] interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("[AdMob Controller] Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterInterstitialEventHandlers(_interstitialAd);
                _interstitialAd.Show();
            });
    }

    private void RegisterInterstitialEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("[AdMob Controller] Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("[AdMob Controller] Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("[AdMob Controller] Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("[AdMob Controller] Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMob Controller] Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("[AdMob Controller] Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    #endregion

    #region Rewarded Ads
    private void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("[AdMob Controller] Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_rewardedAdUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("[AdMob Controller] Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("[AdMob Controller] Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "[AdMob Controller] Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("[AdMob Controller] Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("[AdMob Controller] Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("[AdMob Controller] Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("[AdMob Controller] Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMob Controller] Rewarded ad full screen content closed.");
            _rewardedAd.Destroy();
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("[AdMob Controller] Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            _rewardedAd.Destroy();
            LoadRewardedAd();
        };
    }
    #endregion
}
