package com.applift.playads.unity;

import com.unity3d.player.UnityPlayer;
import com.applift.playads.PlayAds;
import com.applift.playads.api.PlayAdsListener;
import com.applift.playads.api.PlayAdsPromo;
import com.applift.playads.api.PlayAdsType;
import com.applift.playads.contract.Constants;

public class PlayAdsSDKWrapper 
{
    private static final String AD_TYPE_SCRATCH         = "Scratch";
    private static final String AD_TYPE_LIGHT           = "Interstitial";
    private static final String AD_TYPE_VIDEO           = "Video";
    private static final String AD_TYPE_SLOT_MACHINE    = "SlotMachine";
    private static final String AD_TYPE_MEMORY          = "Memory";
    private static final String AD_TYPE_COVER_FLOW      = "CoverFlow";
    private static final String AD_TYPE_GAMES_LIST      = "GameList";

    private static final String CALLBACK_SDK_START_END  = "SDKStartedCallback";
    private static final String CALLBACK_SDK_START_FAIL = "SDKStartFailedCallback";
    private static final String CALLBACK_SDK_VERSION    = "SDKVersionCallback";
    private static final String CALLBACK_AD_READY       = "AdReadyCallback";
    private static final String CALLBACK_AD_SHOWN       = "AdShownCallback";
    private static final String CALLBACK_AD_FAIL        = "AdFailedCallback";
    private static final String CALLBACK_AD_CLOSED      = "AdClosedCallback";

    public static String InstanceName = "PlayAdsSDKInstance";

    public static void PlayAdsSDKStart(final String appId, final String appSecret, final String instanceName)
    {
        Runnable r = new Runnable()
        {
            @Override
            public void run() 
            {
                InstanceName = instanceName;
                int parsedAppId;
                
                try
                {
                    parsedAppId = Integer.parseInt(appId);
                }
                catch (Exception e)
                {
                    parsedAppId = -1; //If something fails, 
                }
                
                PlayAds.init(UnityPlayer.currentActivity, parsedAppId, appSecret);
                PlayAds.addListener(UNITY_LISTENER);
                callback(CALLBACK_SDK_START_END);
            }
        };
        runOnUIThread(r);
    }

    public static void PlayAdsSDKCache(final String typeString) 
    {
        Runnable r = new Runnable() 
        {
            @Override
            public void run() 
            {
                PlayAdsType type = getType(typeString);
                if (type == null) 
                {
                    PlayAds.cache();
                }
                else 
                {
                    PlayAds.cache(type);
                }
            }
        };
        runOnUIThread(r);
    }

    public static void PlayAdsSDKShow(final String typeString, final boolean cacheIfNeeded) 
    {
        Runnable r = new Runnable() 
        {
            @Override
            public void run() 
            {
                PlayAdsType type = getType(typeString);
                if (type == null) 
                {
                    PlayAds.show(UnityPlayer.currentActivity, cacheIfNeeded) ;
                }
                else 
                {
                    PlayAds.show(UnityPlayer.currentActivity, cacheIfNeeded, type);
                }
            }
        };
        runOnUIThread(r);
    }

    public static void PlayAdsSDKGetVersion() 
    {
        Runnable r = new Runnable() 
        {
            @Override
            public void run() 
            {
                callback(CALLBACK_SDK_VERSION, Constants.VERSION);
            }
        };
        runOnUIThread(r);
    }

    public static final PlayAdsListener UNITY_LISTENER = new PlayAdsListener() 
    {
        @Override
        public void onCached(PlayAdsType type) 
        {
            callback(CALLBACK_AD_READY);
        }

        @Override
        public void onShown(PlayAdsType type) 
        {
            callback(CALLBACK_AD_SHOWN);
        }

        @Override
        public void onTapped(PlayAdsPromo promo) 
        {
            // Do nothing
        }

        @Override
        public void onClosed(PlayAdsType type) 
        {
            callback(CALLBACK_AD_CLOSED);
        }

        @Override
        public void onError(Exception ex) 
        {
            callback(CALLBACK_AD_FAIL, ex.getMessage());
        }

    };

    // HELPERS

    private static PlayAdsType getType(String typeString) 
    {
        PlayAdsType result = null;

        if (AD_TYPE_SCRATCH.equals(typeString)) 
        {
            result = PlayAdsType.SCRATCH_SCREEN;
        }
        else if (AD_TYPE_LIGHT.equals(typeString)) 
        {
            result = PlayAdsType.INTERSTITIAL;
        } 
        else if (AD_TYPE_MEMORY.equals(typeString)) 
        {
            result = PlayAdsType.MEMORY_GAME;
        } 
        else if (AD_TYPE_SLOT_MACHINE.equals(typeString)) 
        {
            result = PlayAdsType.SLOT_MACHINE;
        }
        else if (AD_TYPE_COVER_FLOW.equals(typeString)) 
        {
            result = PlayAdsType.COVER_FLOW;
        }
        else if (AD_TYPE_VIDEO.equals(typeString)) 
        {
            result = PlayAdsType.VIDEO;
        }
        else if (AD_TYPE_GAMES_LIST.equals(typeString)) 
        {
            result = PlayAdsType.GAME_LIST;
        }

        return result;
    }

    public static void callback(final String method) 
    {
        callback(method, "");
    }

    public static void callback(final String method, String value) 
    {
        UnityPlayer.UnitySendMessage(InstanceName, method, value);
    }

    private static void runOnUIThread(Runnable r) 
    {
        UnityPlayer.currentActivity.runOnUiThread(r);
    }
}