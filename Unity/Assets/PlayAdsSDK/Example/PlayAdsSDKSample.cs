using UnityEngine;
using System.Collections;

public class PlayAdsSDKSample : MonoBehaviour 
{   
    void OnGUI ()
    {
        if(GUI.Button(new Rect(Screen.width * 0.1f, Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.1f), "Cache"))
        {
            this.PlayAds_Cache();
        }
        
        if(GUI.Button(new Rect(Screen.width * 0.1f, Screen.height * 0.5f, Screen.width * 0.8f, Screen.height * 0.1f), "Show"))
        {
            this.PlayAds_Show();
        }
    }
    
    private void Awake()
    {
        PlayAdsSDK.AdReady  += PlayAds_AdReady;
        PlayAdsSDK.AdShown  += PlayAds_AdShown;
        PlayAdsSDK.AdFailed += PlayAds_AdFailed;
        PlayAdsSDK.AdClosed += PlayAds_AdClosed;
    }
    
    private void Destroy()
    {
        PlayAdsSDK.AdReady  -= PlayAds_AdReady;
        PlayAdsSDK.AdShown  -= PlayAds_AdShown;
        PlayAdsSDK.AdFailed -= PlayAds_AdFailed;
        PlayAdsSDK.AdClosed -= PlayAds_AdClosed;
    }
    
    private void PlayAds_Show()
    {
        PlayAdsSDK.Show();
    }

    private void PlayAds_Cache()
    {
        PlayAdsSDK.Cache();
    }
    
    #region PlayAdsSDK Callbacks
    
    #region Ad callbacks

    private void PlayAds_AdReady()
    {
        // The ad was cached and it's ready to be shown
        Debug.Log("* PlayAdsSDK * - AdReady");
    }
    
    private void PlayAds_AdShown()
    {
        // The ad is in the screen
        Debug.Log("* PlayAdsSDK * - AdShown");
    }
    
    private void PlayAds_AdFailed(string errorMessage)
    {
        // Something bad happened with the interstitial
        // After this callback, the SDK will automatically raise Close event (if already shown)
        Debug.Log("* PlayAdsSDK * - AdFailed: " + errorMessage);
    }
    
    private void PlayAds_AdClosed()
    {
        //Do whatever is needed when the app is returned
        
        Debug.Log("* PlayAdsSDK * - AdClosed");
    }
    
    #endregion
    
    #endregion
}
