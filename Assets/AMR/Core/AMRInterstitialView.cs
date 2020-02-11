using System;
using UnityEngine;
namespace AMR
{
    public class AMRInterstitialView: AMRAdView
    {
        private class InterstitialDelegate : AMRInterstitialViewDelegate
        {
            private AMRInterstitialView interstitialView;
            
            public InterstitialDelegate(AMRInterstitialView iv)
            {
                interstitialView = iv;
            }

            public void didReceiveInterstitial(string networkName, double ecpm)
            {
                interstitialView.state = InterstitialState.Loaded;
                interstitialView.failCount = 0;
                if (interstitialView.onReadyDelegate != null)
                {
                    interstitialView.onReadyDelegate(networkName, ecpm);
                }
            }

            public void didFailtoReceiveInterstitial(string error)
            {
                interstitialView.failCount++;
                interstitialView.state = InterstitialState.New;
                if (interstitialView.failCount <= 1)
                { // Try once again when not loaded 
                    interstitialView.loadInterstitialForZoneId(interstitialView.iosZoneId, interstitialView.androidZoneId, interstitialView.isRefresh);
                }
                else
                {
                    interstitialView.failCount = 0;
                    if (interstitialView.onFailDelegate != null)
                    {
                        interstitialView.onFailDelegate(error);
                    }
                }
            }

            public void didShowInterstitial()
            {
                if (interstitialView.onShowDelegate != null)
                {
                    interstitialView.onShowDelegate();
                }
            }

            public void didFailtoShowInterstitial(string errorCode)
            {
                // Tag passive and have to be set to ready state again for another tag request
                interstitialView.state = InterstitialState.Loaded;

                if (interstitialView.onFailToShowDelegate != null)
                {
                    interstitialView.onFailToShowDelegate();
                }
            }

            public void didClickInterstitial(string networkName)
            {
                if (interstitialView.onClickDelegate != null)
                {
                    interstitialView.onClickDelegate(networkName);
                }
            }

            public void didDismissInterstitial()
            {
                interstitialView.state = InterstitialState.New;
                AMRSDK.resolveBannerConflict();
                if (interstitialView.onDismissDelegate != null)
                {
                    interstitialView.onDismissDelegate();
                }
            }
        }

        private static AMRInterstitialView instance;
        private IAMRInterstitial interstitial;
        private InterstitialState state;

        private EventDelegateReady onReadyDelegate;
        private EventDelegateFail onFailDelegate;
        private EventDelegateShow onShowDelegate;
        private EventDelegateFailToShow onFailToShowDelegate;
        private EventDelegateClick onClickDelegate;
        private EventDelegateDismiss onDismissDelegate;

        private int failCount = 0;
        private string androidZoneId, iosZoneId;
        private bool isRefresh;

        private enum InterstitialState
        {
            New,
            Loading,
            Loaded,
            Showing
        }

        public static AMRInterstitialView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AMRInterstitialView();
                }
                return instance;
            }
        }

        public void loadInterstitialForZoneId(string zoneIdiOS,
                                                string zoneIdAndroid,
                                                bool refresh)
        {
            if (refresh)
            {
                if (state != InterstitialState.New && state != InterstitialState.Loaded)
                {
                    return;
                }
            } 
            else if (state != InterstitialState.New)
            {
                return;
            }

            androidZoneId = zoneIdAndroid;
            iosZoneId = zoneIdiOS;
            isRefresh = refresh;
            state = InterstitialState.Loading;
            InterstitialDelegate interstitialDelegate = new InterstitialDelegate(this);
            if (Application.platform == RuntimePlatform.IPhonePlayer) 
            {
				if (zoneIdiOS != null)
				{
					interstitial = new AMR.iOS.AMRInterstitial();
					interstitial.loadInterstitialForZoneId(zoneIdiOS, interstitialDelegate);
				}
            }
            else if (Application.platform == RuntimePlatform.Android) {
				if (zoneIdAndroid != null)
				{
					if (interstitial != null)
					{
						interstitial.destroyInterstitial();
					}
					interstitial = new AMR.Android.AMRInterstitial();
					interstitial.loadInterstitialForZoneId(zoneIdAndroid, interstitialDelegate);
				}
            }

        }

        public void showInterstitial()
        {
            state = InterstitialState.Showing;
            interstitial.showInterstitial();
        }

        public void showInterstitial(string tag)
        {
            state = InterstitialState.Showing;
            interstitial.showInterstitial(tag);
        }

        /* States */
        public Boolean isReady()
        {
            return state == InterstitialState.Loaded;
        }
        public Boolean isShowing()
        {
            return state == InterstitialState.Showing;
        }

        /* Possible Callbacks */
        public void setOnReady(EventDelegateReady onReadyDelegate)
        {
            this.onReadyDelegate = onReadyDelegate;
        }

        public void setOnFail(EventDelegateFail onFailDelegate)
        {
            this.onFailDelegate = onFailDelegate;
        }

        public void setOnShow(EventDelegateShow onShowDelegate)
        {
            this.onShowDelegate = onShowDelegate;
        }

        public void setOnFailToShow(EventDelegateFailToShow onFailToShowDelegate)
        {
            this.onFailToShowDelegate = onFailToShowDelegate;
        }

        public void setOnDismiss(EventDelegateDismiss onDismissDelegate)
        {
            this.onDismissDelegate = onDismissDelegate;
        }

        public void setOnClick(EventDelegateClick onClickDelegate)
        {
            this.onClickDelegate = onClickDelegate;
        }
    }
}

