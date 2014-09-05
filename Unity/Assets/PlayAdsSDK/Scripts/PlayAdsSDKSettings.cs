using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class PlayAdsSDKSettings : ScriptableObject 
{
	const string settingsAssetName = "PlayAdsSDKSettings";
	const string settingsPath = "PlayAdsSDK/Resources";
	const string settingsExtension = ".asset";
	
	private static PlayAdsSDKSettings instance;
	private static PlayAdsSDKSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load(settingsAssetName) as PlayAdsSDKSettings;
				if (instance == null)
				{
					// If not found, autocreate the asset object.
					instance = CreateInstance<PlayAdsSDKSettings>();

					#if UNITY_EDITOR
					string properPath = Path.Combine(Application.dataPath, settingsPath);
					if (!Directory.Exists(properPath))
					{
						AssetDatabase.CreateFolder(Application.dataPath, settingsPath);
					}
					string fullPath = Path.Combine(Path.Combine("Assets", settingsPath), settingsAssetName + settingsExtension);
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}
	
	#if UNITY_EDITOR
	[MenuItem("PlayAdsSDK/Configuration")]
	public static void Edit()
	{
		PlayAdsSDK.EnsureInstance();
		Selection.activeObject = Instance;
	}
	#endif
	
	#region App Settings

	[SerializeField]
	private string iOSAppID = "";
	public static string IOSAppID
	{
		get { return Instance.iOSAppID; }
		set
		{
			if (Instance.iOSAppID != value)
			{
				Instance.iOSAppID = value;
				DirtyEditor();
			}
		}
	}

	[SerializeField]
	private string iOSSecretToken = "";
	public static string IOSSecretToken
	{
		get { return Instance.iOSSecretToken; }
		set
		{
			if (Instance.iOSSecretToken != value)
			{
				Instance.iOSSecretToken = value;
				DirtyEditor();
			}
		}
	}

	[SerializeField]
	private string androidAppID = "";
	public static string AndroidAppID
	{
		get { return Instance.androidAppID; }
		set
		{
			if (Instance.androidAppID != value)
			{
				Instance.androidAppID = value;
				DirtyEditor();
			}
		}
	}

	[SerializeField]
	private string androidSecretToken = "";
	public static string AndroidSecretToken
	{
		get { return Instance.androidSecretToken; }
		set
		{
			if (Instance.androidSecretToken != value)
			{
				Instance.androidSecretToken = value;
				DirtyEditor();
			}
		}
	}

	private static void DirtyEditor()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty(Instance);
		#endif
	}
	
	#endregion
}
