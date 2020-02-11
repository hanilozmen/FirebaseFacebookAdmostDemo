namespace AMR
{
	public interface AMRRewardedVideoViewDelegate
	{
		void didReceiveRewardedVideo(string networkName, double ecpm);
		void didFailtoReceiveRewardedVideo(string error);
        void didShowRewardedVideo();
		void didFailtoShowRewardedVideo();
        void didClickRewardedVideo(string networkName);
        void didCompleteRewardedVideo();
		void didDismissRewardedVideo();
	}
}