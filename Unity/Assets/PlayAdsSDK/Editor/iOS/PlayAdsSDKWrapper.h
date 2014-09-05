#include <iostream>

#ifdef __cplusplus
extern "C"
{
    void PlayAdsSDKStart(const char* appID, const char* secret, const char* instanceName);
	void PlayAdsSDKCache(const char* typeString);
	void PlayAdsSDKShow(const char* typeString, const bool showLoadingScreen);
    void PlayAdsSDKGetVersion();
}
#endif