using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.PlayAdsSDK;

[CustomEditor(typeof(PlayAdsSDKSettings))]
public class PlayAdsSDKSettingsEditor : Editor
{
	public void OnEnable()
	{
		ManifestModificator.GenerateManifest();
		PlayAdsSDK.EnsureInstance();
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("iOS - Configuration", GUI.skin.box, GUILayout.ExpandWidth(true));
		PlayAdsSDKSettings.IOSAppID			= EditorGUILayout.TextField("AppID", PlayAdsSDKSettings.IOSAppID);
		PlayAdsSDKSettings.IOSSecretToken 	= EditorGUILayout.TextField("Secret Token", PlayAdsSDKSettings.IOSSecretToken);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Android - Configuration", GUI.skin.box, GUILayout.ExpandWidth(true));
		PlayAdsSDKSettings.AndroidAppID			= EditorGUILayout.TextField("AppID", PlayAdsSDKSettings.AndroidAppID);
		PlayAdsSDKSettings.AndroidSecretToken	= EditorGUILayout.TextField("Secret Token", PlayAdsSDKSettings.AndroidSecretToken);
	}
}