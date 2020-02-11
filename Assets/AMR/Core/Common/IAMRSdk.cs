using System;

namespace AMR
{
	public interface IAMRSdk
	{
		void startWithAppId(string appId, bool isUserChild);
        void startWithAppIdConsent(string appId, string subjectToGDPR, string userConsent, bool isUserChild);
        void pause();
        void resume();
        void start();
        void stop();
        void destroy();
		void startTestSuite(string[] zoneIds);
        void setUserId(string userId);
        void setAdjustUserId(string adjustUserId);
        void setClientCampaignId(string campaignId);
        string trackPurchase(string uniqueID, double localizedPrice, string isoCurrencyCode);
        string trackPurchaseForAmazon(string userId, string receiptId, double localizedPrice, string marketPlace, string isoCurrencyCode);
        void spendVirtualCurrency();
        void setVirtualCurrencyDelegate(AMRVirtualCurrencyDelegate delegateObject);
        void setTrackPurchaseDelegate(AMRTrackPurchaseDelegate delegateObject);
        void setGDPRDelegate(AMRGDPRDelegate delegateObject);
    }
}

