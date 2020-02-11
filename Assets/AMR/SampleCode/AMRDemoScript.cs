using UnityEngine;
using System;

public class AMRDemoScript : MonoBehaviour
{
	AMR.AMRSdkConfig config;

	// Use this for initialization
	void Start()
	{
		config = new AMR.AMRSdkConfig();

		/* Set zone ids */
		config.ApplicationIdAndroid = "6cc8e89a-b52a-4e9a-bb8c-579f7ec538fe";
		config.ApplicationIdIOS = "15066ddc-9c18-492c-8185-bea7e4c7f88c";

		config.BannerIdAndroid = "86644357-21d0-45a4-906a-37262461df65";
		config.BannerIdIOS = "b4009772-de04-42c4-bbaa-c18da9e4a1ab";

		config.InterstitialIdAndroid = "f99e409b-f9ab-4a2e-aa9a-4d143e6809ae";
		config.InterstitialIdIOS = "39f74377-5682-436a-9338-9d1c4df410bd";

		config.RewardedVideoIdAndroid = "88cfcfd0-2f8c-4aba-9f36-cc0ac99ab140";
		config.RewardedVideoIdIOS = "2bdefd44-5269-4cbc-b93a-373b74a2f067";

		config.OfferWallIdAndroid = "fa1072e4-afcf-49b6-a919-1ab1ab1b0aa9";
		config.OfferWallIdIOS = "1cadca08-33f9-4da7-969e-ef116d4e7d0e";

		AMR.AMRSDK.startWithConfig(config);
//	    AMR.AMRSDK.setClientCampaignId("UnityCampaignTest-1.0");

		/* Banner Callbacks - Optional */
		AMR.AMRSDK.setOnBannerReady(OnBannerReady);
        AMR.AMRSDK.setOnBannerFail(OnBannerFail);
        AMR.AMRSDK.setOnBannerClick(OnBannerClick);

        /* Interstitial Callbacks - Optional */
        AMR.AMRSDK.setOnInterstitialReady(OnInterstitialReady);
		AMR.AMRSDK.setOnInterstitialFail(OnInterstitialFail);
		AMR.AMRSDK.setOnInterstitialShow(OnInterstitialShow);
		AMR.AMRSDK.setOnInterstitialFailToShow(OnInterstitialFailToShow);
        AMR.AMRSDK.setOnInterstitialClick(OnInterstitialClick);
        AMR.AMRSDK.setOnInterstitialDismiss(OnInterstitialDismiss);

		/* Rewarded Video Callbacks - Optional */
		AMR.AMRSDK.setOnRewardedVideoReady(OnVideoReady);
		AMR.AMRSDK.setOnRewardedVideoFail(OnVideoFail);
		AMR.AMRSDK.setOnRewardedVideoShow(OnVideoShow);
		AMR.AMRSDK.setOnRewardedVideoFailToShow(OnVideoFailToShow);
        AMR.AMRSDK.setOnRewardedVideoClick(OnVideoClick);
        AMR.AMRSDK.setOnRewardedVideoComplete(OnVideoComplete);
		AMR.AMRSDK.setOnRewardedVideoDismiss(OnVideoDismiss);

		/* OfferWall Callbacks - Optional */
		AMR.AMRSDK.setOnOfferWallReady(OnOfferWallReady);
		AMR.AMRSDK.setOnOfferWallFail(OnOfferWallFail);
		AMR.AMRSDK.setOnOfferWallDismiss(OnOfferWallDismiss);

		/* Virtual Currency Callback - Optional */
		AMR.AMRSDK.setOnDidSpendVirtualCurrency(OnVirtualCurrencyDidSpend);
		
		AMR.AMRSDK.setGDPRIsApplicable(isGDPRApplicable);
		AMR.AMRSDK.setTrackPurchaseOnResult(trackPurchaseOnResult);
	}

	void OnApplicationPause(Boolean paused)
	{
		// IMPORTANT FOR CHARTBOOST, DO NOT FORGET
		if (!AMR.AMRSDK.initialized())
			return;

		if (paused)
		{
			AMR.AMRSDK.onPause();
			AMR.AMRSDK.onStop();
		}
		else
		{
			AMR.AMRSDK.onStart();
			AMR.AMRSDK.onResume();
		}
	}

	void OnApplicationQuit()
	{
		AMR.AMRSDK.onDestroy();
	}

	#region AdMethods

	public void LoadBanner()
	{
		Debug.Log("<AMRSDK> Load Banner");
		AMR.AMRSDK.loadBanner(AMR.Enums.AMRSDKBannerPosition.BannerPositionBottom, true);
	}

	public void HideBanner()
	{
		Debug.Log("<AMRSDK> Hide Banner");
		AMR.AMRSDK.hideBanner();
	}

	public void LoadInterstitial()
	{
		Debug.Log("<AMRSDK> Load Interstitial");
		AMR.AMRSDK.loadInterstitial();
	}

	public void ShowInterstitial()
	{
		Debug.Log("<AMRSDK> Show Interstitial");

		if (AMR.AMRSDK.isInterstitialReady())
			AMR.AMRSDK.showInterstitial();
		else
			AMR.AMRSDK.loadInterstitial();
	}

	public void LoadRewardedVideo()
	{
		Debug.Log("<AMRSDK> Load RewardedVideo");
		AMR.AMRSDK.loadRewardedVideo();
	}

	public void ShowRewardedVideo()
	{
		Debug.Log("<AMRSDK> Show RewardedVideo");

		if (AMR.AMRSDK.isRewardedVideoReady())
			AMR.AMRSDK.showRewardedVideo();
		else
			AMR.AMRSDK.loadRewardedVideo();
	}

	public void LoadOfferWall()
	{
		Debug.Log("<AMRSDK> Load OfferWall");
		AMR.AMRSDK.loadOfferWall();
	}

	public void ShowOfferWall()
	{
		Debug.Log("<AMRSDK> Show OfferWall");

		if (AMR.AMRSDK.isOfferWallReady())
			AMR.AMRSDK.showOfferWall();
		else
			AMR.AMRSDK.loadOfferWall();
	}

	public void SpendVirtualCurrency()
	{
		Debug.Log("<AMRSDK> Spend Virtual Currency");
		AMR.AMRSDK.spendVirtualCurrency();
	}

	public void StartTestSuite()
	{
		Debug.Log("<AMRSDK> Start Test Suite");

#if UNITY_IPHONE
		AMR.AMRSDK.startTestSuite(
			new string[] {config.BannerIdIOS, config.InterstitialIdIOS, config.RewardedVideoIdIOS});
#endif

#if UNITY_ANDROID
			AMR.AMRSDK.startTestSuite(new string[] {config.BannerIdAndroid, config.InterstitialIdAndroid, config.RewardedVideoIdAndroid});
		#endif
	}

	#endregion

	#region Banner

	public void OnBannerReady(string networkName, double ecpm)
	{
		Debug.Log("<AMRSDK> OnBannerReady: " + networkName + " with ecpm: " + ecpm);
		AMR.AMRSDK.showBanner();
	}

    public void OnBannerFail(string error)
    {
        Debug.Log("<AMRSDK> OnBannerFail: " + error);
    }

    public void OnBannerClick(string networkName)
    {
        Debug.Log("<AMRSDK> OnBannerClick: " + networkName);
    }

    #endregion

    #region Interstitial

    public void OnInterstitialReady(string networkName, double ecpm)
    {
        Debug.Log("<AMRSDK> OnInterstitialReady: " + networkName + " with ecpm: " + ecpm);

        //if (AMR.AMRSDK.isInterstitialReady())
        //	AMR.AMRSDK.showInterstitial();
    }

    public void OnInterstitialFail(string error)
	{
		Debug.Log("<AMRSDK> OnInterstitialFail: " + error);
	}

	public void OnInterstitialShow()
	{
		Debug.Log("<AMRSDK> OnInterstitialShow");
	}

	public void OnInterstitialFailToShow()
	{
		Debug.Log("<AMRSDK> OnInterstitialFailToShow" );
	}

    public void OnInterstitialClick(string networkName)
    {
        Debug.Log("<AMRSDK> OnInterstitialClick: " + networkName);
    }

    public void OnInterstitialDismiss()
	{
		Debug.Log("<AMRSDK> OnInterstitialDismiss");
	}

	#endregion

	#region RewardedVideo

	public void OnVideoReady(string networkName, double ecpm)
	{
		Debug.Log("<AMRSDK> OnVideoReady: " + networkName + " with ecpm: " + ecpm);

		//if (AMR.AMRSDK.isRewardedVideoReady())
		//	AMR.AMRSDK.showRewardedVideo();
	}

	public void OnVideoFail(string errorMessage)
	{
		Debug.Log("<AMRSDK> OnVideoFail called and reason is: " + errorMessage);
	}

	public void OnVideoShow()
	{
		Debug.Log("<AMRSDK> OnVideoShow");
	}
	
	public void OnVideoFailToShow()
	{
		Debug.Log("<AMRSDK> OnVideoFailToShow");
	}

    public void OnVideoClick(string networkName)
    {
        Debug.Log("<AMRSDK> OnVideoClick: " + networkName);
    }

    public void OnVideoComplete()
	{
		Debug.Log("<AMRSDK> OnVideoComplete");
		// reward user for watching a video
	}

	public void OnVideoDismiss()
	{
		Debug.Log("<AMRSDK> OnVideoDismiss");
	}

	#endregion

	#region OfferWall

	public void OnOfferWallReady(string networkName, double ecpm)
	{
		Debug.Log("<AMRSDK> OnOfferWallReady: " + networkName + " with ecpm: " + ecpm);

		AMR.AMRSDK.showOfferWall();
	}

	public void OnOfferWallFail(string error)
	{
		Debug.Log("<AMRSDK> OnOfferWallFail: " + error);
	}

	public void OnOfferWallDismiss()
	{
		Debug.Log("<AMRSDK> OnOfferWallDismiss");
	}

	public void OnVirtualCurrencyDidSpend(string networkName, string currency, double amount)
	{
		Debug.Log("<AMRSDK> OnVirtualCurrencyDidSpend: " + networkName + " currency: " + currency + " amout: " +
		          amount.ToString());
	}

	#endregion
	
	public void trackPurchaseOnResult(string purchaseId, AMR.Enums.AMRSDKTrackPurchaseResult responseCode)
	{
		Debug.Log("<AMRSDK> trackPurchaseOnResult : " + purchaseId + " " + responseCode);
	}

	public void isGDPRApplicable(bool isGDPRApplicable)
	{
		Debug.Log("<AMRSDK> isGDPRApplicable : " + isGDPRApplicable);
	}
}