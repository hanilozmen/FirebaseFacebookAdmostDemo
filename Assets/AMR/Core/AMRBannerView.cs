using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace AMR
{
	public class AMRBannerView: AMRAdView
	{
        private bool isConflicted;
        private static AMRBannerView instance;
        private BannerState state;

        private class BannerDelegate : AMR.AMRBannerViewDelegate
        {
            private AMRBannerView bannerView;
            public BannerDelegate(AMRBannerView bv)
            {
                bannerView = bv;
            }
            public void didFailtoReceiveBanner(string error)
            {
                bannerView.state = BannerState.New;
                if (bannerView.didFailToReceiveDelegate != null)
                    bannerView.didFailToReceiveDelegate(error);                
            }

			public void didReceiveBanner(string networkName, double ecpm)
            {
				bannerView.state = BannerState.Loaded;

                if (bannerView.autoShow)
                {
                    bannerView.showBanner();
                }

                if (bannerView.didReceiveDelegate != null)
                {
                    bannerView.didReceiveDelegate(networkName, ecpm);
                }
            }

            public void didClickBanner(string networkName)
            {
                if (bannerView.didClickDelegate != null)
                {
                    bannerView.didClickDelegate(networkName);
                }
            }
        }

        private IAMRBanner Banner;
        private EventDelegateReady didReceiveDelegate;
        private EventDelegateClick didClickDelegate;
        private EventDelegateFail didFailToReceiveDelegate;
        private string zoneIdiOS;
        private string zoneIdAndroid;
        private Enums.AMRSDKBannerPosition position;
        private int offset;
		private bool autoShow;

        private enum BannerState
        {
            New,
            Loading,
            Loaded
        }

        public static AMRBannerView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AMRBannerView();
                }
                return instance;
            }
        }

        public void loadBannerForZoneId(string zoneIdiOS,
										string zoneIdAndroid,
										AMR.Enums.AMRSDKBannerPosition position,
                                        int offset,
										bool autoShow)
		{
            if (state == BannerState.Loading)
            {
                return;
            }

            this.zoneIdiOS = zoneIdiOS;
            this.zoneIdAndroid = zoneIdAndroid;
            this.position = position;
            this.offset = offset;
			this.autoShow = autoShow;
            this.isConflicted = false;
            hideBanner();

            state = BannerState.Loading;
            BannerDelegate bDelegate = new BannerDelegate(this);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (zoneIdiOS != null)
                {
                    Banner = new AMR.iOS.AMRBanner();
                    Banner.loadBannerForZoneId(zoneIdiOS,
                                               position,
                                               offset,
                                               bDelegate);
                }
            }
            else if (Application.platform == RuntimePlatform.Android) 
            {
				if (zoneIdAndroid != null)
				{
					if (Banner == null)
					{
						Banner = new AMR.Android.AMRBanner();
					}

					Banner.loadBannerForZoneId(zoneIdAndroid,
											   position,
                                               offset,
											   bDelegate);
				}
            }
        }

		public void showBanner()
		{
            if (AMRSDK.isFullScreenAdShowing())
            {
                hideBanner();
                isConflicted = true;
            }
            else
            {
                if (state == BannerState.Loaded)
                {
                    Banner.showBanner();
                }
                else if (state == BannerState.New)
                {
                    loadBannerForZoneId(zoneIdiOS, zoneIdAndroid, position, offset, autoShow);
                }
            }
		}

        public void hideBanner()
		{
            if (state == BannerState.Loaded || state == BannerState.Loading)
            {
                if (Banner != null)
                    Banner.hideBanner();
            }
            
		}

        public void resolveConflict()
        {
            if (isConflicted)
            {
                isConflicted = false;
                showBanner();
            }		
        }

        /* Possible Callbacks */
        public void setDidReceiveBanner(EventDelegateReady didReceiveDelegate)
        {
            this.didReceiveDelegate = didReceiveDelegate;
        }

        public void setDidClickBanner(EventDelegateClick didClickDelegate)
        {
            this.didClickDelegate = didClickDelegate;
        }

        public void setDidFailToReceiveBanner(EventDelegateFail didFailToReceiveDelegate)
        {
            this.didFailToReceiveDelegate = didFailToReceiveDelegate;
        }
        
    }
}