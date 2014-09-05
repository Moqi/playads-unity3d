using UnityEngine;
using System;
using System.Collections;

public class PlayAdsSDK : MonoBehaviour
{
	public enum AdType
	{
		Scratch,
		Interstitial,
		Video,
		SlotMachine,
		Memory,
		CoverFlow,
		GameList,
		Smart
	}

	private const string INSTANCE_NAME = "PlayAdsSDKInstance";
	private const string WARNING_MESSAGE = "PlayAdsSDK - Error ocurred while initializing the SDK";
	private static bool SDKReady = false;
	private static bool IsAdReady = false;
	private static bool showLoadingScreen = false;
	private static AdType currentType = AdType.Smart;

	private static bool initializing = false;
	private static AdType waitingType;

	private static string waitingAction = "";
	private const string ACTION_CACHE	= "Cache";
	private const string ACTION_SHOW 	= "Show";

	#region --- PUBLIC ---
	
	#region -- CALLBACKS --

	public static event Action 			AdReady;
	public static event Action 			AdShown;
	public static event Action<String> 	AdFailed;
	public static event Action 			AdClosed;
	
	#endregion

	#region -- SINGLETON INSTANCE --
	private static PlayAdsSDK instance;
	public static void EnsureInstance()
	{
		if(instance == null)
		{
			instance = FindObjectOfType(typeof(PlayAdsSDK) ) as PlayAdsSDK;
			if(instance == null)
			{
				instance = new GameObject(INSTANCE_NAME).AddComponent<PlayAdsSDK>();
			}
		}
	}
	private void Awake()
	{
		name = INSTANCE_NAME;
		DontDestroyOnLoad(transform.gameObject);
	}

	#endregion

	#region -- METHODS --

	private static void Start(string action, AdType type)
	{
		PlayAdsSDK.EnsureInstance();
		if(!PlayAdsSDK.initializing)
		{
			PlayAdsSDK.initializing = true;
			PlayAdsSDK.waitingAction = action;
			PlayAdsSDK.waitingType = type;

			string appID = "";
			string secretToken = "";
			
			#if UNITY_IPHONE
			appID = PlayAdsSDKSettings.IOSAppID;
			secretToken = PlayAdsSDKSettings.IOSSecretToken;
			#elif UNITY_ANDROID
			appID = PlayAdsSDKSettings.AndroidAppID;
			secretToken = PlayAdsSDKSettings.AndroidSecretToken;
			#endif
			
			PlayAdsSDK.PlayAdsSDKStart(appID, secretToken, INSTANCE_NAME);
		}
	}

	public static void Cache()
	{
		PlayAdsSDK.Cache(AdType.Smart);
    }
    public static void Cache(AdType adType)
	{
		PlayAdsSDK.EnsureInstance();
		PlayAdsSDK.currentType = adType;

		if(!PlayAdsSDK.SDKReady)
		{
			PlayAdsSDK.Start(ACTION_CACHE, adType);
			return;
		}
		PlayAdsSDK.PlayAdsSDKCache(PlayAdsSDK.GetTypeString(adType));
    }

    public static bool Ready()
	{
		return PlayAdsSDK.Ready(PlayAdsSDK.currentType);
    }

    public static bool Ready(AdType adType)
	{
		return (PlayAdsSDK.currentType == adType) && PlayAdsSDK.IsAdReady;
    }

    public static void ShowLoadingScreen(bool showLoadingScreen)
    {
    	PlayAdsSDK.showLoadingScreen = showLoadingScreen;
    }
	
	public static void Show()
	{
		PlayAdsSDK.Show(AdType.Smart);
    }

    public static void Show(AdType adType)
    {
		PlayAdsSDK.EnsureInstance();
		PlayAdsSDK.currentType = adType;
		
		if(!PlayAdsSDK.SDKReady)
		{
			PlayAdsSDK.Start(ACTION_SHOW, adType);
			return;
		}
		PlayAdsSDK.PlayAdsSDKShow(PlayAdsSDK.GetTypeString(adType), PlayAdsSDK.showLoadingScreen);
    }

	#endregion
	
	#endregion
	
	#region --- PRIVATE ---

	#region -- BRIDGES --
	
#if UNITY_EDITOR
	private static void PlayAdsSDKStart(string appId, string secret, string instanceName)
	{
		instance.SDKStartedCallback("");
	}
	
	private static void PlayAdsSDKCache(string typeString)
	{
		instance.AdReadyCallback("");
	}

	private static void PlayAdsSDKShow(string typeString, bool showLoadingScreen)
	{
		instance.AdShownCallback("");
		instance.AdClosedCallback("");
	}
	
	private static void PlayAdsSDKGetVersion ()
    {
		instance.SDKVersionCallback("");
    }
	
#elif UNITY_IPHONE
	
	[System.Runtime.InteropServices.DllImport("__Internal")]
   	private static extern void PlayAdsSDKStart(string appId, string secret, string instanceName);
   	[System.Runtime.InteropServices.DllImport("__Internal")]
   	private static extern void PlayAdsSDKCache(string typeString);
	[System.Runtime.InteropServices.DllImport("__Internal")]
   	private static extern void PlayAdsSDKShow(string typeString, bool showLoadingScreen);
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void PlayAdsSDKGetVersion();
	
#elif UNITY_ANDROID

	private static void PlayAdsSDKStart(string appId, string secret, string instanceName)
	{
		PlayAdsSDK.CallAndroidSDK("PlayAdsSDKStart", appId, secret, instanceName);
	}

   	private static void PlayAdsSDKCache(string typeString)
	{
		PlayAdsSDK.CallAndroidSDK("PlayAdsSDKCache", typeString);
	}

   	private static void PlayAdsSDKShow(string typeString, bool showLoadingScreen)
	{
		PlayAdsSDK.CallAndroidSDK("PlayAdsSDKShow", typeString, showLoadingScreen);
	}

	private static void PlayAdsSDKGetVersion()
	{
		PlayAdsSDK.CallAndroidSDK("PlayAdsSDKGetVersion");
	}

	private static AndroidJavaClass AndroidSDK;
	private static void CallAndroidSDK(string methodName, params object[] args)
	{
		if(AndroidSDK == null)
		{
			AndroidSDK = new AndroidJavaClass("com.applift.playads.unity.PlayAdsSDKWrapper");
		}
		AndroidSDK.CallStatic(methodName, args);
	}


#endif
	#endregion
	
	#region -- CALLBACKS --
	
	private void SDKStartedCallback(string message)
	{
		PlayAdsSDK.SDKReady = true;
		PlayAdsSDK.initializing = false;

		if(!string.IsNullOrEmpty(PlayAdsSDK.waitingAction))
		{
			if(ACTION_CACHE.Equals(PlayAdsSDK.waitingAction))
			{
				PlayAdsSDK.Cache(PlayAdsSDK.waitingType);
			}
			else if(ACTION_SHOW.Equals(PlayAdsSDK.waitingAction))
			{
				PlayAdsSDK.Show(PlayAdsSDK.waitingType);
			}
			PlayAdsSDK.waitingAction = null;
		}
	}

	private void SDKStartFailedCallback(string error)
	{
		PlayAdsSDK.SDKReady = false;
		PlayAdsSDK.initializing = false;
		Debug.Log(WARNING_MESSAGE);
	}
	
	private void AdReadyCallback(string message)
	{
		PlayAdsSDK.IsAdReady = true;

		if(AdReady != null)
		{
			AdReady();
		}
	}
	
	private void AdShownCallback(string message)
	{
		if(AdShown != null)
		{
			AdShown();
		}
	}
	
	private void AdFailedCallback(string error)
	{
		PlayAdsSDK.IsAdReady = false;

		if(AdFailed != null)
		{
			AdFailed(error);
		}
	}
	
	private void AdClosedCallback(string message)
	{
		PlayAdsSDK.IsAdReady = false;

		if(AdClosed != null)
		{
			AdClosed();
		}
	}

	private void SDKVersionCallback(string version)
	{
		Debug.Log("PlayAdsSDK - Native version: " + version);
	}
	
	#endregion
	
	#region -- HELPERS --

	private static String GetTypeString(AdType type)
	{
		String result = null;

		switch(type)
		{
			case AdType.Scratch:	 	result = "Scratch";			break;
			case AdType.Interstitial:	result = "Interstitial";	break;
			case AdType.Video: 			result = "Video";			break;
			case AdType.SlotMachine: 	result = "SlotMachine";		break;
			case AdType.Memory: 		result = "Memory";			break;
			case AdType.CoverFlow: 		result = "CoverFlow";		break;
			case AdType.GameList: 		result = "GameList";		break;
			case AdType.Smart: 			result = "Smart";			break;
		}

		return result;
	}

	#endregion

	#endregion
}
