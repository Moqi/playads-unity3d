using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

namespace UnityEditor.PlayAdsSDK
{
    public class ManifestModificator
    {
        public static void GenerateManifest()
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                File.Copy(inputFile, outputFile);
            }
            UpdateManifest(outputFile);
        }

        private static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlElement FindChildElement(string name, XmlNode parent)
        {
            XmlElement result = null;

            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr is XmlElement)
                {
                    result = curr as XmlElement;
                    break;
                }
                curr = curr.NextSibling;
            }

            return result;
        }

        private static XmlElement FindMainActivityNode(XmlNode parent)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals("activity") && curr.FirstChild != null && curr.FirstChild.Name.Equals("intent-filter"))
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlElement FindElementWithAndroidName(string name, string androidName, string ns, string value, XmlNode parent)
        {
            XmlElement result = null;
            
            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr is XmlElement && ((XmlElement)curr).GetAttribute(androidName, ns) == value)
                {
                    result = curr as XmlElement;
                    break;
                }
                curr = curr.NextSibling;
            }
            
            return result;
        }


        public static void UpdateManifest(string fullPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);
            if (doc == null)
            {
                Debug.LogError("Couldn't load " + fullPath);
                return;
            }

            XmlNode manifest = FindChildNode(doc, "manifest");
            XmlNode application = FindChildNode(manifest, "application");
            string applicationNamespace = application.GetNamespaceOfPrefix("android");

            string hardwareAcceleratedString = "hardwareAccelerated";

            XmlAttribute hardwareAcceleratedAttribute = application.Attributes[hardwareAcceleratedString, applicationNamespace];
            if(hardwareAcceleratedAttribute == null)
            {
                hardwareAcceleratedAttribute = doc.CreateAttribute(hardwareAcceleratedString, applicationNamespace);
                hardwareAcceleratedAttribute.Value = "true";
                application.Attributes.Append(hardwareAcceleratedAttribute);
            }
            else
            {
                hardwareAcceleratedAttribute.Value = "true";
            }

            if (application == null)
            {
                Debug.LogError("Error parsing " + fullPath);
                return;
            }

            //1.- Playads Activity
            //====================================================================================================
            //   <activity  android:name="com.applift.playads.PlayAdsActivity" 
            //              android:configChanges="keyboardHidden|orientation|screenSize" 
            //              android:taskAffinity="com.applift.playads"
            //              android:theme="@style/Theme.PlayAds" />

            XmlElement playadsActivity = FindElementWithAndroidName("activity", "name", applicationNamespace, "com.applift.playads.PlayAdsActivity", application);
            if(playadsActivity != null)
            {
                application.RemoveChild(playadsActivity);
            }

            playadsActivity = doc.CreateElement("activity");
            playadsActivity.SetAttribute("name", applicationNamespace, "com.applift.playads.PlayAdsActivity");
            playadsActivity.SetAttribute("configChanges", applicationNamespace, "keyboardHidden|orientation|screenSize");
            playadsActivity.SetAttribute("taskAffinity", applicationNamespace, "com.applift.playads");
            playadsActivity.SetAttribute("theme", applicationNamespace, "@style/Theme.PlayAds");
            playadsActivity.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            application.AppendChild(playadsActivity);
            //====================================================================================================

            //2.- Permissions
            //====================================================================================================
            ManifestModificator.AddPermission(doc, applicationNamespace, manifest, "android.permission.READ_PHONE_STATE");
            ManifestModificator.AddPermission(doc, applicationNamespace, manifest, "android.permission.ACCESS_WIFI_STATE");
            ManifestModificator.AddPermission(doc, applicationNamespace, manifest, "android.permission.ACCESS_NETWORK_STATE");
            ManifestModificator.AddPermission(doc, applicationNamespace, manifest, "android.permission.INTERNET");
            //====================================================================================================
            
            //3.- Uses SDK
            //====================================================================================================
            //<uses-sdk android:minSdkVersion="8" android:targetSdkVersion="17" />

            XmlElement usesSdk  = FindChildElement("uses-sdk", manifest);

            string minSDKVersionKey = "minSdkVersion";
            string targetSDKVersionKey = "targetSdkVersion";
    
            string minSDKVersion = "8";
            string targetSDKVersion = "17";

            if(usesSdk != null)
            {
                bool reset = false;
                string prevMinSDKVersion = usesSdk.GetAttribute(minSDKVersionKey, applicationNamespace);
                string prevTargetSDKVersion = usesSdk.GetAttribute(targetSDKVersionKey, applicationNamespace);

                if(int.Parse(minSDKVersion) < int.Parse(prevMinSDKVersion))
                {
                    minSDKVersion = prevMinSDKVersion;
                }
                else
                {
                    reset = true;
                }

                if(int.Parse(targetSDKVersion) < int.Parse(prevTargetSDKVersion))
                {
                    targetSDKVersion = prevTargetSDKVersion;
                    
                }
                else
                {
                    reset = true;
                }

                if(reset)
                {
                    manifest.RemoveChild(usesSdk);
                    usesSdk = null;
                }
            }

            if(usesSdk == null)
            {
                usesSdk = doc.CreateElement("uses-sdk");
                usesSdk.SetAttribute(minSDKVersionKey, applicationNamespace, minSDKVersion);
                usesSdk.SetAttribute(targetSDKVersionKey, applicationNamespace, targetSDKVersion);
                manifest.AppendChild(usesSdk);
            }

            doc.Save(fullPath);
        }

        public static void AddPermission(XmlDocument doc, string applicationNamespace, XmlNode manifest, string permission)
        {
            //<uses-permission android:name="android.permission.permission.<PERMISSION>" />
            XmlElement element  = FindElementWithAndroidName("uses-permission", "name", applicationNamespace, permission, manifest);
            if (element == null)
            {
                element = doc.CreateElement("uses-permission");
                element.SetAttribute("name", applicationNamespace, permission);
                manifest.AppendChild(element);
            }
        }
    }
}
