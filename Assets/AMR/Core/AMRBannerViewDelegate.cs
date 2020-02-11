namespace AMR
{
	public interface AMRBannerViewDelegate
	{
		void didReceiveBanner(string networkName, double ecpm);
        void didClickBanner(string networkName);
        void didFailtoReceiveBanner(string error);
	}
}