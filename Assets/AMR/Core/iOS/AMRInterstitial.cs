using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace AMR.iOS
{
	public class AMRInterstitial : IAMRInterstitial
	{
#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _setInterstitialSuccessCallback(InterstitialSuccessCallback cb);

		[DllImport("__Internal")]
		private static extern void _setInterstitialFailCallback(InterstitialFailCallback cb);

		[DllImport("__Internal")]
		private static extern void _setInterstitialShowCallback(InterstitialShowCallback cb);

        [DllImport("__Internal")]
		private static extern void _setInterstitialFailToShowCallback(InterstitialFailToShowCallback cb);

        [DllImport("__Internal")]
		private static extern void _setInterstitialClickCallback(InterstitialClickCallback cb);

		[DllImport("__Internal")]
		private static extern void _setInterstitialDismissCallback(InterstitialDismissCallback cb);

		[DllImport("__Internal")]
		private static extern IntPtr _loadInterstitialForZoneId(string zoneId, IntPtr interstitialHandle);

		[DllImport("__Internal")]
		private static extern void _showInterstitial(IntPtr interstitialPtr);
	
		[DllImport("__Internal")]
		private static extern void _showInterstitialWithTag(string tag, IntPtr interstitialPtr);
#endif

        private delegate void InterstitialSuccessCallback(IntPtr interstitialHandlePtr, string networkName, double ecpm);
		private delegate void InterstitialFailCallback(IntPtr interstitialHandlePtr, string error);
        private delegate void InterstitialShowCallback(IntPtr interstitialHandlePtr);
        private delegate void InterstitialFailToShowCallback(IntPtr interstitialHandlePtr, string errorCode);
        private delegate void InterstitialClickCallback(IntPtr interstitialHandlePtr, string networkName);
        private delegate void InterstitialDismissCallback(IntPtr interstitialHandlePtr);

        private IntPtr interstitialPtr;

		[MonoPInvokeCallback(typeof(InterstitialSuccessCallback))]
		private static void interstitialSuccessCallback(IntPtr interstitialHandlePtr, string networkName, double ecpm)
		{
			GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
			AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
			delegateObject.didReceiveInterstitial(networkName, ecpm);
		}

		[MonoPInvokeCallback(typeof(InterstitialFailCallback))]
		private static void interstitialFailCallback(IntPtr interstitialHandlePtr, string error)
		{
			GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
			AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
			delegateObject.didFailtoReceiveInterstitial(error);
		}

		[MonoPInvokeCallback(typeof(InterstitialShowCallback))]
		private static void interstitialShowCallback(IntPtr interstitialHandlePtr)
		{
			GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
			AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
			delegateObject.didShowInterstitial();
		}

        [MonoPInvokeCallback(typeof(InterstitialFailToShowCallback))]
        private static void interstitialFailToShowCallback(IntPtr interstitialHandlePtr, string errorCode)
        {
            GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
            AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
            if (errorCode.Equals("1081"))
            {
                delegateObject.didFailtoShowInterstitial(errorCode);
            } else {
                delegateObject.didFailtoReceiveInterstitial(errorCode);
            }
        }

        [MonoPInvokeCallback(typeof(InterstitialClickCallback))]
        private static void interstitialClickCallback(IntPtr interstitialHandlePtr, string networkName)
        {
            GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
            AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
            delegateObject.didClickInterstitial(networkName);
        }

        [MonoPInvokeCallback(typeof(InterstitialDismissCallback))]
        private static void interstitialDismissCallback(IntPtr interstitialHandlePtr)
        {
            GCHandle interstitialHandle = (GCHandle)interstitialHandlePtr;
            AMRInterstitialViewDelegate delegateObject = interstitialHandle.Target as AMRInterstitialViewDelegate;
            delegateObject.didDismissInterstitial();
        }

        #region - IAMRInterstitial

        public void loadInterstitialForZoneId(string zoneId, AMRInterstitialViewDelegate delegateObject)
		{
#if UNITY_IOS
				_setInterstitialSuccessCallback(interstitialSuccessCallback);
				_setInterstitialFailCallback(interstitialFailCallback);
                _setInterstitialShowCallback(interstitialShowCallback);
				_setInterstitialFailToShowCallback(interstitialFailToShowCallback);
                _setInterstitialClickCallback(interstitialClickCallback);
				_setInterstitialDismissCallback(interstitialDismissCallback);

				GCHandle handle = GCHandle.Alloc(delegateObject);
				IntPtr parameter = (IntPtr)handle;

				interstitialPtr = _loadInterstitialForZoneId(zoneId, parameter);
#endif
        }

        public void showInterstitial()
		{
#if UNITY_IOS
				_showInterstitial(interstitialPtr);
#endif
		}

        public void showInterstitial(string tag)
        {
#if UNITY_IOS
				_showInterstitialWithTag(tag, interstitialPtr);
#endif
        }


        public void destroyInterstitial()
        {
	        
        }
		
#endregion
    }
}

