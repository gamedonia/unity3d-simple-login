using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;

namespace Gamedonia.Backend
{
	
		public interface IGamedoniaDeviceInfo
		{
		
				string GetLanguageCode ();

				string GetCountryCode ();

				double GetTimeZoneGMTOffset ();

				string GetDevice ();

				string GetPlatform ();

				string GetBuildVersion ();

				string GetOS ();

				string GetOSVersion ();

				string GetIMEI ();

				string GetIDFA ();

				string GetIDFV ();

				string GetDeviceManufacturer ();

				string GetDeviceModel ();

				string GetDeviceHardware ();

				string GetDeviceSystem ();

				bool IsAdTrackingEnabled ();

				bool IsJailBroken ();
		}


		public class GamedoniaDeviceInfo
		{

				private static IGamedoniaDeviceInfo GetDeviceInfoImpl ()
				{

						IGamedoniaDeviceInfo deviceInfoImpl = null;
						if (Application.isEditor) {
								deviceInfoImpl = new GamedoniaDefaultDeviceInfoImpl ();
						} else {
								deviceInfoImpl = new GamedoniaDeviceInfoImpl ();
						}

						return deviceInfoImpl;

				}

		
				public static string GetLanguageCode ()
				{
						return GetDeviceInfoImpl ().GetLanguageCode ();
				}

				public static string GetCountryCode ()
				{		
						return GetDeviceInfoImpl ().GetCountryCode ();
				}

				public static double GetTimeZoneGMTOffset ()
				{										
						return GetDeviceInfoImpl ().GetTimeZoneGMTOffset ();
				}

		
				/*
				 * NEW FIELDS
				 */

				public static string GetDevice ()
				{						
						return GetDeviceInfoImpl ().GetDevice ();
				}

				public static string GetPlatform ()
				{						
						return GetDeviceInfoImpl ().GetPlatform ();			
				}

				public static string GetBuildVersion ()
				{						
						return GetDeviceInfoImpl ().GetBuildVersion ();		
				}

				public static string GetOS ()
				{
						return GetDeviceInfoImpl ().GetOS ();
				}

				public static string GetOSVersion ()
				{
						return GetDeviceInfoImpl ().GetOSVersion ();
				}

				public static string GetIMEI ()
				{
						return GetDeviceInfoImpl ().GetIMEI ();
				}

				public static string GetIDFA ()
				{
						return GetDeviceInfoImpl ().GetIDFA ();
				}

				public static string GetIDFV ()
				{
						return GetDeviceInfoImpl ().GetIDFV ();
				}

				public static string GetDeviceManufacturer ()
				{
						return GetDeviceInfoImpl ().GetDeviceManufacturer ();
				}

				public static string GetDeviceModel ()
				{
						return GetDeviceInfoImpl ().GetDeviceModel ();
				}

				public static string GetDeviceHardware ()
				{
						return GetDeviceInfoImpl ().GetDeviceHardware ();
				}

				public static string GetDeviceSystem ()
				{
						return GetDeviceInfoImpl ().GetDeviceSystem ();
				}

				public static bool IsAdTrackingEnabled ()
				{
						return GetDeviceInfoImpl ().IsAdTrackingEnabled ();
				}

				public static bool IsJailBroken ()
				{
						return GetDeviceInfoImpl ().IsJailBroken ();
				}

		}


		/*
	 * Default Impl used always on Editor
	 */
		public class GamedoniaDefaultDeviceInfoImpl: IGamedoniaDeviceInfo
		{
		
				private static Dictionary<string,string> platforms = new Dictionary<string,string> () {{ "OSXEditor","editor" },
						{ "WindowsEditor","editor" },		
						{ "IPhonePlayer","ios" },
						{ "Android","android" },
						{ "OSXPlayer", "mac"},
						{ "WindowsPlayer", "pc"},
						{ "OSXDashboardPlayer", "mac"},
						{ "WindowsWebPlayer", "pc"},
						{ "XBOX360", "xbox360"},
						{ "PS3", "ps3"},
						{ "LinuxPlayer", "linux"},
						{ "WebGLPlayer", "webgl"},
						{ "WSAPlayerX86", "windowsphone"},
						{ "WSAPlayerX64", "windowsphone"},
						{ "WSAPlayerARM", "windowsphone"},
						{ "WP8Player", "windowsphone"},
						{ "TizenPlayer", "tizen"},
						{ "PSP2", "psp2"},
						{ "PS4", "ps4"},
						{ "XboxOne", "xboxone"},
						{ "SamsungTVPlayer", "samsungtv"},
						{ "WiiU", "wiiu"},
						{ "tvOS", "tvos"}


				};

				public string GetLanguageCode ()
				{
						SystemLanguage lang = Application.systemLanguage;
						string res = "en";
						switch (lang) {
						case SystemLanguage.Afrikaans:
								res = "af";
								break;
						case SystemLanguage.Arabic:
								res = "ar";
								break;
						case SystemLanguage.Basque:
								res = "eu";
								break;
						case SystemLanguage.Belarusian:
								res = "by";
								break;
						case SystemLanguage.Bulgarian:
								res = "bg";
								break;
						case SystemLanguage.Catalan:
								res = "ca";
								break;
						case SystemLanguage.Chinese:
								res = "zh";
								break;
						case SystemLanguage.Czech:
								res = "cs";
								break;
						case SystemLanguage.Danish:
								res = "da";
								break;
						case SystemLanguage.Dutch:
								res = "nl";
								break;
						case SystemLanguage.English:
								res = "en";
								break;
						case SystemLanguage.Estonian:
								res = "et";
								break;
						case SystemLanguage.Faroese:
								res = "fo";
								break;
						case SystemLanguage.Finnish:
								res = "fi";
								break;
						case SystemLanguage.French:
								res = "fr";
								break;
						case SystemLanguage.German:
								res = "de";
								break;
						case SystemLanguage.Greek:
								res = "el";
								break;
						case SystemLanguage.Hebrew:
								res = "iw";
								break;
						case SystemLanguage.Hungarian:
								res = "hu";
								break;
						case SystemLanguage.Icelandic:
								res = "is";
								break;
						case SystemLanguage.Indonesian:
								res = "in";
								break;
						case SystemLanguage.Italian:
								res = "it";
								break;
						case SystemLanguage.Japanese:
								res = "ja";
								break;
						case SystemLanguage.Korean:
								res = "ko";
								break;
						case SystemLanguage.Latvian:
								res = "lv";
								break;
						case SystemLanguage.Lithuanian:
								res = "lt";
								break;
						case SystemLanguage.Norwegian:
								res = "no";
								break;
						case SystemLanguage.Polish:
								res = "pl";
								break;
						case SystemLanguage.Portuguese:
								res = "pt";
								break;
						case SystemLanguage.Romanian:
								res = "ro";
								break;
						case SystemLanguage.Russian:
								res = "ru";
								break;
						case SystemLanguage.SerboCroatian:
								res = "sh";
								break;
						case SystemLanguage.Slovak:
								res = "sk";
								break;
						case SystemLanguage.Slovenian:
								res = "sl";
								break;
						case SystemLanguage.Spanish:
								res = "es";
								break;
						case SystemLanguage.Swedish:
								res = "sv";
								break;
						case SystemLanguage.Thai:
								res = "th";
								break;
						case SystemLanguage.Turkish:
								res = "tr";
								break;
						case SystemLanguage.Ukrainian:
								res = "uk";
								break;
						case SystemLanguage.Unknown:
								res = "en";
								break;
						case SystemLanguage.Vietnamese:
								res = "vi";
								break;
						}
			
						return res;
				}

				public string GetCountryCode ()
				{

						//string name = CultureInfo.CurrentCulture.Name;

						CultureInfo cInfo = CultureInfo.CurrentCulture;
						if (cInfo == CultureInfo.InvariantCulture) {
								return "Invariant";
						} else {
								
								var r = new RegionInfo(cInfo.LCID);
								return r.TwoLetterISORegionName;
						}
								
				}

				public double GetTimeZoneGMTOffset ()
				{
						TimeZone localZone = TimeZone.CurrentTimeZone;
						TimeSpan currentOffset = localZone.GetUtcOffset (System.DateTime.Now);
						return currentOffset.TotalHours;
				}

				public string GetDevice ()
				{
						return platforms.ContainsKey (Application.platform.ToString ()) ? platforms [Application.platform.ToString ()] : "";
				}

				public string GetPlatform ()
				{
						return platforms.ContainsKey (Application.platform.ToString ()) ? platforms [Application.platform.ToString ()] : "";
				}

				public string GetBuildVersion ()
				{
						return "";
				}

				public string GetOS ()
				{
						return SystemInfo.operatingSystem;
				}

				public string GetOSVersion ()
				{
						return SystemInfo.operatingSystem;
				}

				public string GetIMEI ()
				{
						return null;						
				}

				public string GetIDFA ()
				{
						return null;						
				}

				public string GetIDFV ()
				{
						return null;						
				}

				public string GetDeviceManufacturer ()
				{
						return null;
				}

				public string GetDeviceModel ()
				{
						return null;
				}

				public string GetDeviceHardware ()
				{
						return null;
				}

				public string GetDeviceSystem ()
				{
						return null;
				}

				public bool IsAdTrackingEnabled ()
				{
						return false;
				}

				public bool IsJailBroken ()
				{
						return false;
				}

		}


		#if UNITY_IPHONE
		
		public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo
		{

				[DllImport ("__Internal")]
				public static extern string GetDeviceLanguageCode ();

				public string GetLanguageCode ()
				{
			
						return GamedoniaDeviceInfoImpl.GetDeviceLanguageCode ();
			
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceCountryCode ();

				public string GetCountryCode ()
				{
			
						return GamedoniaDeviceInfoImpl.GetDeviceCountryCode ();
			
				}

				[DllImport ("__Internal")]
				public static extern double GetDeviceTimeZoneGMTOffset ();

				public double GetTimeZoneGMTOffset ()
				{
			
						return GamedoniaDeviceInfoImpl.GetDeviceTimeZoneGMTOffset ();
			
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceDevice ();
				public string GetDevice ()
				{
						return GamedoniaDeviceInfoImpl.GetDeviceDevice();	
				}

				[DllImport ("__Internal")]
				public static extern string GetDevicePlatform ();
				public string GetPlatform ()
				{
						return GamedoniaDeviceInfoImpl.GetDevicePlatform ();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceBuildVersion ();
				public string GetBuildVersion() {
						return GamedoniaDeviceInfoImpl.GetDeviceBuildVersion ();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceOS ();
				public string GetOS () {
						return GamedoniaDeviceInfoImpl.GetDeviceOS();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceOSVersion ();
				public string GetOSVersion () {
						return GamedoniaDeviceInfoImpl.GetDeviceOSVersion();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceIMEI ();
				public string GetIMEI () {
						return null;				
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceIDFA ();
				public string GetIDFA () {
						return GamedoniaDeviceInfoImpl.GetDeviceIDFA();						
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceIDFV ();
				public string GetIDFV () {
						return GamedoniaDeviceInfoImpl.GetDeviceIDFV();						
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceDeviceManufacturer ();
				public string GetDeviceManufacturer () {
						return GamedoniaDeviceInfoImpl.GetDeviceDeviceManufacturer ();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceDeviceModel ();
				public string GetDeviceModel () {
						return GamedoniaDeviceInfoImpl.GetDeviceDeviceModel ();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceDeviceHardware ();
				public string GetDeviceHardware () {
						return GamedoniaDeviceInfoImpl.GetDeviceDeviceHardware ();
				}

				[DllImport ("__Internal")]
				public static extern string GetDeviceDeviceSystem ();
				public string GetDeviceSystem () {
						return null;
				}

				[DllImport ("__Internal")]
				public static extern bool isDeviceAdTrackingEnabled ();
				public bool IsAdTrackingEnabled () {
						return GamedoniaDeviceInfoImpl.isDeviceAdTrackingEnabled ();
				}

				[DllImport ("__Internal")]
				public static extern bool isDeviceJailBroken ();
				public bool IsJailBroken () {
						return GamedoniaDeviceInfoImpl.isDeviceJailBroken ();
				}

		}
	
		


#elif UNITY_ANDROID
		
		public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo
		{
		
				private static string _GetLanguageCode ()
				{
						AndroidJNI.AttachCurrentThread ();
			
						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getLanguageCode", new object [] { });
				}

				public string GetLanguageCode ()
				{
						return GamedoniaDeviceInfoImpl._GetLanguageCode ();
				}


				private static string _GetCountryCode ()
				{
						AndroidJNI.AttachCurrentThread ();
			
						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getCountryCode", new object [] { });
				}

				public string GetCountryCode ()
				{
						return GamedoniaDeviceInfoImpl._GetCountryCode ();
				}


				private static double _GetTimeZoneGMTOffset ()
				{
						AndroidJNI.AttachCurrentThread ();
			
						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<double> ("getTimeZoneGMTOffset", new object [] { });
				}

				public double GetTimeZoneGMTOffset ()
				{
						return GamedoniaDeviceInfoImpl._GetTimeZoneGMTOffset ();
				}


				private static string _GetDevice () {
						
						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getDevice", new object [] { });
				}

				public string GetDevice ()
				{
						return GamedoniaDeviceInfoImpl._GetDevice();	
				}

				private static string _GetPlatform () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getPlatform", new object [] { });

				}

				public string GetPlatform ()
				{
						return GamedoniaDeviceInfoImpl._GetPlatform();	
				}


				private static string _GetBuildVersion () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getBuildVersion", new object [] { });

				}
				public string GetBuildVersion() {
						return GamedoniaDeviceInfoImpl._GetBuildVersion();
				}

				private static string _GetOS () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getOS", new object [] { });

				}
				public string GetOS() {
						return GamedoniaDeviceInfoImpl._GetOS();
				}


				private static string _GetOSVersion () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getOSVersion", new object [] { });

				}
				public string GetOSVersion() {
						return GamedoniaDeviceInfoImpl._GetOSVersion();
				}

				private static string _GetIMEI () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getIMEI", new object [] { });

				}
				public string GetIMEI() {
						return GamedoniaDeviceInfoImpl._GetIMEI();
				}

				private static string _GetIDFA () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getIDFA", new object [] { });

				}
				public string GetIDFA() {
						return GamedoniaDeviceInfoImpl._GetIDFA();
				}

				private static string _GetIDFV () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getIDFV", new object [] { });

				}
				public string GetIDFV() {
						return GamedoniaDeviceInfoImpl._GetIDFV();
				}

				private static string _GetDeviceManufacturer () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getDeviceManufacturer", new object [] { });

				}

				public string GetDeviceManufacturer() {
						return GamedoniaDeviceInfoImpl._GetDeviceManufacturer();
				}

				private static string _GetDeviceModel () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getDeviceModel", new object [] { });

				}
				public string GetDeviceModel() {
						return GamedoniaDeviceInfoImpl._GetDeviceModel();
				}

				private static string _GetDeviceHardware () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getDeviceHardware", new object [] { });

				}
				public string GetDeviceHardware() {
						return GamedoniaDeviceInfoImpl._GetDeviceHardware();
				}


				private static string _GetDeviceSystem () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<string> ("getDeviceSystem", new object [] { });

				}
				public string GetDeviceSystem() {
						return GamedoniaDeviceInfoImpl._GetDeviceSystem();
				}

				private static bool _IsAdTrackingEnabled () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<bool> ("isAdTrackingEnabled", new object [] { });

				}
				public bool IsAdTrackingEnabled() {
						return GamedoniaDeviceInfoImpl._IsAdTrackingEnabled();
				}

				private static bool _IsJailBroken () {

						AndroidJNI.AttachCurrentThread ();

						AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass ("com.gamedonia.device.DeviceInfo");						
						return deviceInfoManagerClass.CallStatic<bool> ("isJailBroken", new object [] { });

				}
				public bool IsJailBroken() {
						return GamedoniaDeviceInfoImpl._IsJailBroken();
				}

		}

	
		

#else
	
		public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo
		{
		
				public string GetLanguageCode ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetLanguageCode ();	
				}

				public string GetCountryCode ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetCountryCode ();
				}

				public double GetTimeZoneGMTOffset ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetTimeZoneGMTOffset ();
				}

				public string GetDevice ()
				{						
						return new GamedoniaDefaultDeviceInfoImpl ().GetDevice ();
				}

				public string GetPlatform ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetPlatform ();
				}

				public string GetBuildVersion ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetBuildVersion ();
				}

				public string GetOS ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetOS ();
				}

				public string GetOSVersion ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetOSVersion ();
				}

				public string GetIMEI ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetIMEI ();						
				}

				public string GetIDFA ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetIDFA ();					
				}

				public string GetIDFV ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetIDFV ();						
				}

				public string GetDeviceManufacturer ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetDeviceManufacturer ();
				}

				public string GetDeviceModel ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetDeviceModel ();
				}

				public string GetDeviceHardware ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetDeviceHardware ();
				}

				public string GetDeviceSystem ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().GetDeviceSystem ();
				}

				public bool IsAdTrackingEnabled ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().IsAdTrackingEnabled ();
				}

				public bool IsJailBroken ()
				{
						return new GamedoniaDefaultDeviceInfoImpl ().IsJailBroken ();
				}
		
		}
	
		#endif
}	