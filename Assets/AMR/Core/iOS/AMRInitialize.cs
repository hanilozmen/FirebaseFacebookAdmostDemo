using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace AMR.IOS
{
    public class AMRInitialize : IAMRSdk
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _startWithAppId(string appID);
        
        [DllImport("__Internal")]
        private static extern void _startTestSuite(string zoneIds);

        [DllImport("__Internal")]
        private static extern void _trackPurchase(string identifier, string currencyCode, double amount);

        [DllImport("__Internal")]
        private static extern void _setUserId(string userId);

        [DllImport("__Internal")]
        private static extern void _setAdjustUserId(string adjustUserId);

        [DllImport("__Internal")]
        private static extern void _setClientCampaignId(string campaignId);
        
        [DllImport("__Internal")]
        private static extern void _setUserConsent(bool consent);
        
        [DllImport("__Internal")]
        private static extern void _subjectToGDPR(bool subject);
        
        [DllImport("__Internal")]
        private static extern void _spendVirtualCurrency();
        
        [DllImport("__Internal")]
        private static extern void _setVirtualCurrencyDidSpendCallback(VirtualCurrencyDidSpendCallback cb, IntPtr virtualCurrencyHandle);

        [DllImport("__Internal")]
        private static extern void _setTrackPurchaseResponseCallback(TrackPurchaseResponseCallback cb, IntPtr trackPurchaseHandle);

        [DllImport("__Internal")]
        private static extern void _setIsGDPRApplicableCallback(IsGDPRApplicableCallback cb, IntPtr gdprHandle);

#endif

        private delegate void VirtualCurrencyDidSpendCallback(IntPtr virtualCurrencyHandlePtr, string networkName, string currency, double amount);
        private delegate void TrackPurchaseResponseCallback(IntPtr trackPurchaseHandlePtr, string uniqueID, int status);
        private delegate void IsGDPRApplicableCallback(IntPtr gdprHandlePtr, bool isApplicable);

        public void start() {}
        public void stop() {}
        public void resume() {}
        public void pause() {}
        public void destroy() {}
        
        public void startWithAppId(string appId, bool isUserChild)
        {
#if UNITY_IOS
            _startWithAppId(appId);
#endif
        }

        public void startWithAppIdConsent(string appId, string subjectToGDPR, string userConsent, bool isUserChild)
        {
#if UNITY_IOS
            if (!string.IsNullOrEmpty(userConsent))
            {
                _setUserConsent(userConsent == "1");
            }
                
            if (!string.IsNullOrEmpty(subjectToGDPR))
            {
                _subjectToGDPR(subjectToGDPR == "1");
            }
                
            _startWithAppId(appId);
#endif
        }

		public void startTestSuite(string[] zoneIds)
		{
#if UNITY_IOS
			string zones = "";
			for (int i = 0; i < zoneIds.Length; i++) {
				zones = zones + "," + zoneIds [i];
			}

			_startTestSuite(zones);
#endif
		}

        public string trackPurchase(string uniqueID, double localizedPrice, string isoCurrencyCode)
        {
#if UNITY_IOS
            /* uniqueID = transactionID for ios */
            _trackPurchase(uniqueID, isoCurrencyCode, localizedPrice);
#endif
            return uniqueID;
        }

        public string trackPurchaseForAmazon(string userId, string receiptId, double localizedPrice, string marketPlace, string isoCurrencyCode)
        {
            return "";
        }

        public void setUserId(string userId)
        {
#if UNITY_IOS
            _setUserId(userId);
#endif
        }

        public void setAdjustUserId(string adjustUserId)
        {
#if UNITY_IOS
            _setAdjustUserId(adjustUserId);
#endif
        }

        public void setClientCampaignId(string campaignId)
        {
#if UNITY_IOS
            _setClientCampaignId(campaignId);
#endif
        }

        public void setUserConsent(bool consent)
        {
#if UNITY_IOS
            _setUserConsent(consent);
#endif
        }
        
        public void subjectToGDPR(bool subject)
        {
#if UNITY_IOS
            _subjectToGDPR(subject);
#endif
        }

        public void spendVirtualCurrency()
        {
#if UNITY_IOS
            _spendVirtualCurrency();
#endif
        }
        
        [MonoPInvokeCallback(typeof(VirtualCurrencyDidSpendCallback))]
        private static void virtualCurrencyDidSpendCallback(IntPtr virtualCurrencyHandlePtr, string networkName, string currency, double amount)
        {
            GCHandle virtualCurrencyVideoHandle = (GCHandle)virtualCurrencyHandlePtr;
            AMRVirtualCurrencyDelegate delegateObject = virtualCurrencyVideoHandle.Target as AMRVirtualCurrencyDelegate;
            delegateObject.didSpendVirtualCurrency(networkName, currency, amount);
        }

        public void setVirtualCurrencyDelegate(AMRVirtualCurrencyDelegate delegateObject)
        {
#if UNITY_IOS
            GCHandle handle = GCHandle.Alloc(delegateObject);
            IntPtr parameter = (IntPtr)handle;
            
            _setVirtualCurrencyDidSpendCallback(virtualCurrencyDidSpendCallback, parameter);
#endif
        }

        [MonoPInvokeCallback(typeof(TrackPurchaseResponseCallback))]
        private static void trackPurchaseResponseCallback(IntPtr trackPurchaseHandlePtr, string uniqueID, int status)
        {
            GCHandle trackPurchaseHandle = (GCHandle)trackPurchaseHandlePtr;
            AMRTrackPurchaseDelegate delegateObject = trackPurchaseHandle.Target as AMRTrackPurchaseDelegate;
            delegateObject.onResult(uniqueID, (AMR.Enums.AMRSDKTrackPurchaseResult)status);
        }


        public void setTrackPurchaseDelegate(AMRTrackPurchaseDelegate delegateObject)
        {
#if UNITY_IOS
            GCHandle handle = GCHandle.Alloc(delegateObject);
            IntPtr parameter = (IntPtr)handle;
            
            _setTrackPurchaseResponseCallback(trackPurchaseResponseCallback, parameter);

#endif
        }
        
        [MonoPInvokeCallback(typeof(IsGDPRApplicableCallback))]
        private static void isGDPRApplicableCallback(IntPtr gdprHandlePtr, bool isApplicable)
        {
            GCHandle gdprHandle = (GCHandle)gdprHandlePtr;
            AMRGDPRDelegate delegateObject = gdprHandle.Target as AMRGDPRDelegate;
            delegateObject.isGDPRApplicable(isApplicable);
        }

        
        public void setGDPRDelegate(AMRGDPRDelegate delegateObject)
        {
#if UNITY_IOS
            GCHandle handle = GCHandle.Alloc(delegateObject);
            IntPtr parameter = (IntPtr)handle;
            
            _setIsGDPRApplicableCallback(isGDPRApplicableCallback, parameter);

#endif

        }
    }

}
