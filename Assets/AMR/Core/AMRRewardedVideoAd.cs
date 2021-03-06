﻿using System;
using AMR.iOS;
using UnityEngine;

namespace AMR
{
    public class AMRRewardedVideoAd: AMRAdView
    {

        public string AndroidZoneId;
        public string iOSZoneId;
        public string ZoneId {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return iOSZoneId;
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    return AndroidZoneId;
                } else
                {
                    return null;
                }
            }
        }

        private Android.AMRRewardedVideo rewardedVideoAndroid;

        public AdStatus Status;
        private int failCount;

        private AdDelegateReady onVideoReadyDelegate;
        private AdDelegateFail onVideoFailDelegate;
        private AdDelegateShow onVideoShowDelegate;
        private AdDelegateFailToShow onVideoFailToShowDelegate;
        private AdDelegateClick onVideoClickDelegate;
        private AdDelegateComplete onVideoCompleteDelegate;
        private AdDelegateDismiss onVideoDismissDelegate;

        #region AMRRewardedVideoAd

        public void LoadRewardedVideo()
        {
            if (Status == AdStatus.Loading)
            {
                return;
            }

            Status = AdStatus.Loading;
            RewardedVideoAdDelegate videoDelegate = new RewardedVideoAdDelegate(this);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AMRRewardedVideoManager.LoadRewardedVideo(ZoneId, videoDelegate);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                if (rewardedVideoAndroid != null)
                {
                    rewardedVideoAndroid.destroyRewardedVideo();
                }
                rewardedVideoAndroid = new Android.AMRRewardedVideo();
                rewardedVideoAndroid.loadRewardedVideoForZoneId(ZoneId, videoDelegate);
            }
        }

        public void ShowRewardedVideo(string tag = null)
        {
            Status = AdStatus.Playing;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AMRRewardedVideoManager.ShowRewardedVideo(ZoneId, tag);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                if (tag == null)
                {
                    rewardedVideoAndroid.showRewardedVideo();
                } else
                {
                    rewardedVideoAndroid.showRewardedVideo(tag);
                }
            }
        }

        /* Rewarded Video Callbacks */

        public void SetOnVideoReady(AdDelegateReady onVideoReadyDelegate)
        {
            this.onVideoReadyDelegate = onVideoReadyDelegate;
        }

        public void SetOnVideoFail(AdDelegateFail onVideoFailDelegate)
        {
            this.onVideoFailDelegate = onVideoFailDelegate;
        }

        public void SetOnVideoShow(AdDelegateShow onVideoShowDelegate)
        {
            this.onVideoShowDelegate = onVideoShowDelegate;
        }

        public void SetOnVideoFailToShow(AdDelegateFailToShow onVideoFailToShowDelegate)
        {
            this.onVideoFailToShowDelegate = onVideoFailToShowDelegate;
        }

        public void SetOnVideoClick(AdDelegateClick onVideoClickDelegate)
        {
            this.onVideoClickDelegate = onVideoClickDelegate;
        }

        public void SetOnVideoComplete(AdDelegateComplete onVideoCompleteDelegate)
        {
            this.onVideoCompleteDelegate = onVideoCompleteDelegate;
        }

        public void SetOnVideoDismiss(AdDelegateDismiss onVideoDismissDelegate)
        {
            this.onVideoDismissDelegate = onVideoDismissDelegate;
        }

        #endregion


        #region Other

        public enum AdStatus
        {
            New,
            Loading,
            Loaded,
            Playing
        }
        private class RewardedVideoAdDelegate : AMRRewardedVideoViewDelegate
        {
            private readonly AMRRewardedVideoAd videoAd;
            public RewardedVideoAdDelegate(AMRRewardedVideoAd va)
            {
                videoAd = va;
            }

            public void didReceiveRewardedVideo(string networkName, double ecpm)
            {
                videoAd.Status = AdStatus.Loaded;
                videoAd.failCount = 0;

                if (videoAd.onVideoReadyDelegate != null)
                {
                    videoAd.onVideoReadyDelegate(videoAd.ZoneId, networkName, ecpm);
                }   
            }

            public void didFailtoReceiveRewardedVideo(string error)
            {
                videoAd.failCount++;
                videoAd.Status = AdStatus.New;
                if (videoAd.failCount <= 0)
                { // Try once again when not loaded 
                    videoAd.LoadRewardedVideo();
                }
                else
                {
                    videoAd.failCount = 0;

                    if (videoAd.onVideoFailDelegate != null)
                    {
                        videoAd.onVideoFailDelegate(videoAd.ZoneId, error);
                    }
                }
            }

            public void didShowRewardedVideo()
            {
                if (videoAd.onVideoShowDelegate != null)
                {
                    videoAd.onVideoShowDelegate(videoAd.ZoneId);
                }
            }

            public void didFailtoShowRewardedVideo()
            {
                videoAd.Status = AdStatus.Loaded;

                if (videoAd.onVideoFailToShowDelegate != null)
                {
                    videoAd.onVideoFailToShowDelegate(videoAd.ZoneId);
                }
            }

            public void didClickRewardedVideo(string networkName)
            {
                if (videoAd.onVideoClickDelegate != null)
                {
                    videoAd.onVideoClickDelegate(videoAd.ZoneId, networkName);
                }
            }

            public void didCompleteRewardedVideo()
            {
                if (videoAd.onVideoCompleteDelegate != null)
                {
                    videoAd.onVideoCompleteDelegate(videoAd.ZoneId);
                }
            }

            public void didDismissRewardedVideo()
            {
                videoAd.Status = AdStatus.New;

                AMRSDK.resolveBannerConflict();

                if (videoAd.onVideoDismissDelegate != null)
                {
                    videoAd.onVideoDismissDelegate(videoAd.ZoneId);
                }
            }
        }
        #endregion
    }
}


