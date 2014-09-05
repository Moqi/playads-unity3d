using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PlayAdsSDK;
using System.IO;

public static class PlayAdsPostProcess
{
	[PostProcessBuild]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		if (target == BuildTarget.iPhone) 
		{
			if(string.IsNullOrEmpty(PlayAdsSDKSettings.IOSAppID) || 
			   string.IsNullOrEmpty(PlayAdsSDKSettings.IOSSecretToken))
			{
				Debug.LogError("PlayAdsSDK - AppID or Secret not specified for iOS ");
				return;
			}

			string projModPath = System.IO.Path.Combine(Application.dataPath, "PlayAdsSDK/Editor/iOS");
			string[] files = Directory.GetFiles( projModPath, "*.projmods", SearchOption.AllDirectories );
			UnityEditor.PlayAdsSDK.XCProject project = new UnityEditor.PlayAdsSDK.XCProject( path );
			foreach( string file in files )
			{
				project.ApplyMod(file);
			}
			project.Save();
		}
		else if (target == BuildTarget.Android)
		{
			if(string.IsNullOrEmpty(PlayAdsSDKSettings.AndroidAppID) || 
			   string.IsNullOrEmpty(PlayAdsSDKSettings.AndroidSecretToken))
			{
				Debug.LogError("PlayAdsSDK - AppID or Secret not specified for Android ");
				return;
			}
		}
		else
		{
			Debug.LogWarning("PlayAdsSDK - Sorry, but PlayAdsSDK is only supported for iOS or Android builds, PlayAds will remain inactive in this platform");
		}
	}
}