using System;
namespace AMR
{
	public interface IAMRBanner
	{
		void loadBannerForZoneId(string zoneId, 
		                         AMR.Enums.AMRSDKBannerPosition position,
                                 int offset,
		                         AMRBannerViewDelegate delegateObject);
		void showBanner();
		void hideBanner();
	}
}

