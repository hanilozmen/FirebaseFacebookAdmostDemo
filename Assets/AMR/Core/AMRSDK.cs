using System;
using UnityEngine;
using AMR;

namespace AMR
{
	public class AMRSDK
	{
        public const string AMR_PLUGIN_VERSION = "1.5.4"; 
	    
	    public delegate void VirtualCurrencyDelegateDidSpend(string network, string currency, double amount);
        public delegate void TrackPurchaseOnResult(string purchaseId, AMR.Enums.AMRSDKTrackPurchaseResult responseCode);
        public delegate void GDPRIsApplicable(bool isGDPRApplicable);

        private class VirtualCurrencyDelegate : AMRVirtualCurrencyDelegate
	    {
	        private AMRSDK amrSdk;
	        public VirtualCurrencyDelegate(AMRSDK asdk)
	        {
	            amrSdk = asdk;
	        }

	        public void didSpendVirtualCurrency(string network, string currency, double amount)
	        {
	            if (amrSdk.onDidSpendDelegate != null) {
	                amrSdk.onDidSpendDelegate(network, currency, amount);
	            }
	        }
	    }

        private class TrackPurchaseDelegate : AMRTrackPurchaseDelegate
        {
            private AMRSDK amrSdk;
            public TrackPurchaseDelegate(AMRSDK asdk)
            {
                amrSdk = asdk;
            }

            public void onResult(string purchaseId, AMR.Enums.AMRSDKTrackPurchaseResult responseCode)
            {
                if (amrSdk.onTrackPurchaseOnResult != null)
                {
                    amrSdk.onTrackPurchaseOnResult(purchaseId, responseCode);
                }
            }
        }

        private class GDPRDelegate : AMRGDPRDelegate
        {
            private AMRSDK amrSdk;
            public GDPRDelegate(AMRSDK asdk)
            {
                amrSdk = asdk;
            }

            public void isGDPRApplicable(bool isGDPRApplicable)
            {
                if (amrSdk.isGDPRApplicable != null)
                {
                    amrSdk.isGDPRApplicable(isGDPRApplicable);
                }
            }
        }

        private IAMRSdk AMRSdk;
        private AMRSdkConfig Config;
	    private VirtualCurrencyDelegateDidSpend onDidSpendDelegate;
        private TrackPurchaseOnResult onTrackPurchaseOnResult;
        private GDPRIsApplicable isGDPRApplicable;
        private bool isInitialized = false;
        private static AMRSDK instance;
        private static AMRSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AMRSDK();
                }
                return instance;
            }
        }

        private void create()
        {
            if (AMRSdk != null)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AMRSdk = new IOS.AMRInitialize();
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                AMRSdk = new Android.AMRInitialize();
            }
            else
            {
                AMRSdk = null;
            }
        }

        private void startWithAppId(string appIdiOS, string appIdAndroid, bool isUserChild = false)
		{
            create();

            if (Application.platform == RuntimePlatform.IPhonePlayer) 
            {
				AMRSdk.startWithAppId(appIdiOS, isUserChild);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
				AMRSdk.startWithAppId(appIdAndroid,isUserChild);
            }
		}

        private void startWithAppIdConsent(string appIdiOS, string appIdAndroid, string subjectToGDPR, string userConsent, bool isUserChild = false)
        {
            create();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AMRSdk.startWithAppIdConsent(appIdiOS, subjectToGDPR, userConsent, isUserChild);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                AMRSdk.startWithAppIdConsent(appIdAndroid, subjectToGDPR, userConsent,isUserChild);
            }
        }

        public static void startWithConfig(AMRSdkConfig config)
        {
            AMRUtil.Log("AMR Plugin Version: [" + AMR_PLUGIN_VERSION + "]");

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                Instance.Config = config;
                if (config.SubjectToGDPR != null || config.UserConsent != null)
                {
                    Instance.startWithAppIdConsent(config.ApplicationIdIOS, config.ApplicationIdAndroid, config.SubjectToGDPR, config.UserConsent, config.IsUserChild);
                } else { 
                    Instance.startWithAppId(config.ApplicationIdIOS, config.ApplicationIdAndroid, config.IsUserChild);
                }
                Instance.isInitialized = true;
            } else {
                AMRUtil.Log("AMRSDK only supports Android and iOS platforms.");
            }
        }

		public static bool initialized() {
			return Instance.isInitialized;
		}

		public static void startTestSuite(string[] zoneIds)
		{
            if (initialized()) {
                Instance.AMRSdk.startTestSuite(zoneIds);   
            } else {
                //AMRUtil.Log("AMRSDK has not been initialized.");
            }
		}

        public static void setGDPRIsApplicable(GDPRIsApplicable isGDPRApplicable)
        {
            if (!initialized())
            {
                Instance.create();
            }

            if (Instance.AMRSdk == null)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android)
            {
                Instance.isGDPRApplicable = isGDPRApplicable;
                GDPRDelegate gdprDelegate = new GDPRDelegate(Instance);
                Instance.AMRSdk.setGDPRDelegate(gdprDelegate);
            }
        }

        public static string trackPurchaseForAndroid(string receipt, decimal localizedPrice, string isoCurrencyCode)
        {
			if (receipt == null || 
                receipt.Equals ("") ||
			    isoCurrencyCode == null || 
                isoCurrencyCode.Equals ("")) {
                //AMRUtil.Log("Track Purchase faild: null or empty parameters");
				return "";
			}

            return Instance.AMRSdk.trackPurchase(receipt, Convert.ToDouble(localizedPrice), isoCurrencyCode);
        }

        public static string trackPurchaseForAmazon(string userId, string receiptId, decimal localizedPrice, string marketplace, string isoCurrencyCode)
        {
            if (receiptId == null || receiptId.Equals("") || 
                userId == null || userId.Equals("") ||
                (
                (marketplace == null || marketplace.Equals("")) && 
                (isoCurrencyCode == null || isoCurrencyCode.Equals(""))
                ))
            {
                //AMRUtil.Log("Track Purchase faild: null or empty parameters");
                return "";
            }

            return Instance.AMRSdk.trackPurchaseForAmazon(userId, receiptId, Convert.ToDouble(localizedPrice), marketplace, isoCurrencyCode);
        }

        public static string trackPurchaseForIOS(string transactionID, decimal localizedPrice, string isoCurrencyCode)
        {
            if (transactionID == null ||
                transactionID.Equals("") ||
                isoCurrencyCode == null ||
                isoCurrencyCode.Equals(""))
            {
                //AMRUtil.Log("Track Purchase faild: null or empty parameters");
                return "";
            }

            return Instance.AMRSdk.trackPurchase(transactionID, Convert.ToDouble(localizedPrice), isoCurrencyCode);
        }

        public static void setTrackPurchaseOnResult(TrackPurchaseOnResult onResultDelegate)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android)
            {
                Instance.onTrackPurchaseOnResult = onResultDelegate;
                TrackPurchaseDelegate trackPurchaseDelegate = new TrackPurchaseDelegate(Instance);
                Instance.AMRSdk.setTrackPurchaseDelegate(trackPurchaseDelegate);
            }
        }

        public static void setUserId(string userId)
        {
            if (initialized())
            {
                Instance.AMRSdk.setUserId(userId);
            }
            else
            {
                AMRUtil.Log("AMRSDK has not been initialized.");
            }
        }

        public static void setAdjustUserId(string adjustUserId)
        {
            if (initialized())
            {
                Instance.AMRSdk.setAdjustUserId(adjustUserId);
            }
            else
            {
                AMRUtil.Log("AMRSDK has not been initialized.");
            }
        }

        public static void setClientCampaignId(string campaignId)
		{
			if (initialized())
			{
				if (!String.IsNullOrEmpty(campaignId))
				{
					Instance.AMRSdk.setClientCampaignId(campaignId);
				}
				else
				{
					//AMRUtil.Log("campaignId is null or empty!");
				}
			}
			else
			{
				AMRUtil.Log("AMRSDK has not been initialized.");
			}
			
		}

		public static void spendVirtualCurrency()
	    {
	        if (initialized())
	        {
	            Instance.AMRSdk.spendVirtualCurrency();
	        }
	        else
	        {
	            AMRUtil.Log("AMRSDK has not been initialized.");
	        }
	    }

        public static void onPause()
        {
	        if (!initialized()) return;
	        Instance.AMRSdk.pause();
        }

        public static void onResume()
        {
            if (!initialized()) return;
	        Instance.AMRSdk.resume();
        }

        public static void onStop()
        {
            if (!initialized()) return;
            Instance.AMRSdk.stop();
        }

        public static void onStart()
        {
            if (!initialized()) return;
            Instance.AMRSdk.start();
        }

        public static void onDestroy()
        {
            if (!initialized()) return;
            Instance.AMRSdk.destroy();
        }

        #region Banner
		public static void loadBanner(AMR.Enums.AMRSDKBannerPosition position, bool autoShow) 
		{
		    if (!initialized()) return;
		    AMRBannerView.Instance.loadBannerForZoneId(Instance.Config.BannerIdIOS,
		                                                Instance.Config.BannerIdAndroid,
		                                                position,
                                                        0,
		                                                autoShow);
            
        }

        public static void loadBanner(AMR.Enums.AMRSDKBannerPosition position, int offset, bool autoShow)
        {
            if (!initialized()) return;
            AMRBannerView.Instance.loadBannerForZoneId(Instance.Config.BannerIdIOS,
                                                        Instance.Config.BannerIdAndroid,
                                                        position,
                                                        offset,
                                                        autoShow);

        }

        public static void showBanner()
		{
			AMRBannerView.Instance.showBanner();
		}
        public static void hideBanner()
        {
            AMRBannerView.Instance.hideBanner();
        }
        public static void resolveBannerConflict()
        {
            AMRBannerView.Instance.resolveConflict();
        }
        public static void setOnBannerReady(AMRAdView.EventDelegateReady onReadyDelegate)
        {
            AMRBannerView.Instance.setDidReceiveBanner(onReadyDelegate);
        }
        public static void setOnBannerFail(AMRAdView.EventDelegateFail onFailDelegate)
        {
            AMRBannerView.Instance.setDidFailToReceiveBanner(onFailDelegate);
        }

        public static void setOnBannerClick(AMRAdView.EventDelegateClick onClickDelegate)
        {
            AMRBannerView.Instance.setDidClickBanner(onClickDelegate);
        }

        #endregion

        #region Interstitial

        public static void loadInterstitial()
        {
            if (!Instance.isInitialized)
            {
                return;
            }
            AMRInterstitialView.Instance.loadInterstitialForZoneId(Instance.Config.InterstitialIdIOS,
                                       Instance.Config.InterstitialIdAndroid,false);
        }
		
		public static void showInterstitial()
		{
			if (isInterstitialReady())
			{
				AMRInterstitialView.Instance.showInterstitial();
			}
		}

        public static void showInterstitial(String tag)
        {
            if (isInterstitialReady())
            {
                AMRInterstitialView.Instance.showInterstitial(tag);
            }
        }


        public static void refreshInterstitial()
        {
            if (!Instance.isInitialized)
            {
                return;
            }
            AMRInterstitialView.Instance.loadInterstitialForZoneId(Instance.Config.InterstitialIdIOS,
                                       Instance.Config.InterstitialIdAndroid, true);
        }
		
		public static Boolean isInterstitialReady()
		{
			return AMRInterstitialView.Instance.isReady();
		}
		
		public static Boolean isInterstitialShowing()
		{
			return AMRInterstitialView.Instance.isShowing();
		}

        public static void setOnInterstitialReady(AMRAdView.EventDelegateReady onReadyDelegate)
        {
            AMRInterstitialView.Instance.setOnReady(onReadyDelegate);
        }

        public static void setOnInterstitialFail(AMRAdView.EventDelegateFail onFailDelegate)
        {
            AMRInterstitialView.Instance.setOnFail(onFailDelegate);
        }

        public static void setOnInterstitialShow(AMRAdView.EventDelegateShow onShowDelegate)
        {
            AMRInterstitialView.Instance.setOnShow(onShowDelegate);
        }

        public static void setOnInterstitialFailToShow(AMRAdView.EventDelegateFailToShow onFailToShowDelegate)
        {
            AMRInterstitialView.Instance.setOnFailToShow(onFailToShowDelegate);
        }

        public static void setOnInterstitialClick(AMRAdView.EventDelegateClick onClickDelegate)
        {
            AMRInterstitialView.Instance.setOnClick(onClickDelegate);
        }

        public static void setOnInterstitialDismiss(AMRAdView.EventDelegateDismiss onDismissDelegate)
        {
            AMRInterstitialView.Instance.setOnDismiss(onDismissDelegate);
        }

        #endregion

        #region RewardedVideo

        public static void loadRewardedVideo()
        {
	        if (!initialized()) return;
            AMRRewardedVideoView.Instance.loadRewardedVideoForZoneId(Instance.Config.RewardedVideoIdIOS,
                                       Instance.Config.RewardedVideoIdAndroid, false);
        }
		
		public static void showRewardedVideo()
		{
			if (isRewardedVideoReady())
			{
				AMRRewardedVideoView.Instance.showRewardedVideo();
			}
		}
        
        public static void showRewardedVideo(String tag)
        {
            if (isRewardedVideoReady())
            {
                AMRRewardedVideoView.Instance.showRewardedVideo(tag);
            }
        }

        public static void refreshRewardedVideo()
        {
	        if (!initialized()) return; 
            AMRRewardedVideoView.Instance.loadRewardedVideoForZoneId(Instance.Config.RewardedVideoIdIOS,
                                       Instance.Config.RewardedVideoIdAndroid, true);
        }
		
		public static Boolean isRewardedVideoReady()
		{
			return AMRRewardedVideoView.Instance.isReady();
		}
		
		public static Boolean isRewardedVideoShowing()
		{
			return AMRRewardedVideoView.Instance.isPlaying();
		}

        public static void setOnRewardedVideoReady(AMRAdView.EventDelegateReady onReadyDelegate)
        {
            AMRRewardedVideoView.Instance.setOnReady(onReadyDelegate);
        }

        public static void setOnRewardedVideoFail(AMRAdView.EventDelegateFail onFailDelegate)
        {
            AMRRewardedVideoView.Instance.setOnFail(onFailDelegate);
        }

        public static void setOnRewardedVideoShow(AMRAdView.EventDelegateShow onShowDelegate)
        {
            AMRRewardedVideoView.Instance.setOnShow(onShowDelegate);
        }

		public static void setOnRewardedVideoFailToShow(AMRAdView.EventDelegateFailToShow onFailToShowDelegate)
		{
			AMRRewardedVideoView.Instance.setOnFailToShow(onFailToShowDelegate);
		}

        public static void setOnRewardedVideoClick(AMRAdView.EventDelegateClick onClickDelegate)
        {
            AMRRewardedVideoView.Instance.setOnClick(onClickDelegate);
        }

        public static void setOnRewardedVideoComplete(AMRAdView.EventDelegateComplete onCompleteDelegate)
        {
            AMRRewardedVideoView.Instance.setOnComplete(onCompleteDelegate);
        }

        public static void setOnRewardedVideoDismiss(AMRAdView.EventDelegateDismiss onDismissDelegate)
        {
            AMRRewardedVideoView.Instance.setOnDismiss(onDismissDelegate);
        }

        #endregion

        #region OfferWall

        public static void loadOfferWall()
	    {
		    if (!initialized()) return;
	        
	        AMROfferWallView.Instance.loadOfferWallForZoneId(instance.Config.OfferWallIdIOS, instance.Config.OfferWallIdAndroid, false);
	    }
	    
	    public static void showOfferWall()
	    {
		    if (!initialized()) return;
	        
	        AMROfferWallView.Instance.showOfferWall();
	    }

        public static void showOfferWall(String tag)
        {
            if (!initialized()) return;

            AMROfferWallView.Instance.showOfferWall(tag);
        }

        public static void refreshOfferWall()
	    {
		    if (!initialized()) return;
	        
	        AMROfferWallView.Instance.loadOfferWallForZoneId(instance.Config.OfferWallIdIOS, instance.Config.OfferWallIdAndroid, true);
	    }
		
		public static Boolean isOfferWallReady()
		{
			return AMROfferWallView.Instance.isReady();
		}
		
		public static Boolean isOfferWallShowing()
		{
			return AMROfferWallView.Instance.isShowing();
		}

	    public static void setOnOfferWallReady(AMRAdView.EventDelegateReady onReadyDelegate)
	    {
	        AMROfferWallView.Instance.setOnReady(onReadyDelegate);
	    }
	    
	    public static void setOnOfferWallFail(AMRAdView.EventDelegateFail onFailDelegate)
	    {
	        AMROfferWallView.Instance.setOnFail(onFailDelegate);
	    }

	    public static void setOnOfferWallDismiss(AMRAdView.EventDelegateDismiss onDismissDelegate)
	    {
	        AMROfferWallView.Instance.setOnDismiss(onDismissDelegate);
	    }
	    
	    public static void setOnDidSpendVirtualCurrency(VirtualCurrencyDelegateDidSpend onDidSpendDelegate) {
		    if (Application.platform == RuntimePlatform.IPhonePlayer || 
		        Application.platform == RuntimePlatform.Android) 
		    {
			    Instance.onDidSpendDelegate = onDidSpendDelegate;
			    VirtualCurrencyDelegate virtualCurrencyDelegate = new VirtualCurrencyDelegate(Instance);
			    Instance.AMRSdk.setVirtualCurrencyDelegate(virtualCurrencyDelegate);
		    }
        }
        #endregion

        #region
        public static bool isFullScreenAdShowing()
        {
            return (isInterstitialShowing() || isRewardedVideoShowing() || isOfferWallShowing());
        }

        #endregion
    }

}
