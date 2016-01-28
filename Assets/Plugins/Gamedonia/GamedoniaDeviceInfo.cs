using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Gamedonia.Backend {
	
	public interface IGamedoniaDeviceInfo {
		
		string GetLanguageCode();
		string GetCountryCode();
		double GetTimeZoneGMTOffset();

	}


	public class GamedoniaDeviceInfo {
		
		public static string GetLanguageCode() {
			
			IGamedoniaDeviceInfo deviceInfoImpl = null;
			if (Application.isEditor) {
				deviceInfoImpl = new GamedoniaDefaultDeviceInfoImpl();
			} else {
				deviceInfoImpl = new GamedoniaDeviceInfoImpl();
			}
			
			return deviceInfoImpl.GetLanguageCode ();
		}

		public static string GetCountryCode() {
			
			IGamedoniaDeviceInfo deviceInfoImpl = null;
			if (Application.isEditor) {
				deviceInfoImpl = new GamedoniaDefaultDeviceInfoImpl();
			} else {
				deviceInfoImpl = new GamedoniaDeviceInfoImpl();
			}
			
			return deviceInfoImpl.GetCountryCode ();
		}

		public static double GetTimeZoneGMTOffset() {
			
			IGamedoniaDeviceInfo deviceInfoImpl = null;
			if (Application.isEditor) {
				deviceInfoImpl = new GamedoniaDefaultDeviceInfoImpl();
			} else {
				deviceInfoImpl = new GamedoniaDeviceInfoImpl();
			}
			
			return deviceInfoImpl.GetTimeZoneGMTOffset ();
		}

	}


	/*
	 * Default Impl used always on Editor
	 */ 
	public class GamedoniaDefaultDeviceInfoImpl: IGamedoniaDeviceInfo {
		
		public string GetLanguageCode() {
			SystemLanguage lang = Application.systemLanguage;
			string res = "EN";
			switch (lang) {
				case SystemLanguage.Afrikaans: res = "AF"; break;
				case SystemLanguage.Arabic: res = "AR"; break;
				case SystemLanguage.Basque: res = "EU"; break;
				case SystemLanguage.Belarusian: res = "BY"; break;
				case SystemLanguage.Bulgarian: res = "BG"; break;
				case SystemLanguage.Catalan: res = "CA"; break;
				case SystemLanguage.Chinese: res = "ZH"; break;
				case SystemLanguage.Czech: res = "CS"; break;
				case SystemLanguage.Danish: res = "DA"; break;
				case SystemLanguage.Dutch: res = "NL"; break;
				case SystemLanguage.English: res = "EN"; break;
				case SystemLanguage.Estonian: res = "ET"; break;
				case SystemLanguage.Faroese: res = "FO"; break;
				case SystemLanguage.Finnish: res = "FI"; break;
				case SystemLanguage.French: res = "FR"; break;
				case SystemLanguage.German: res = "DE"; break;
				case SystemLanguage.Greek: res = "EL"; break;
				case SystemLanguage.Hebrew: res = "IW"; break;
				case SystemLanguage.Hungarian: res = "HU"; break;
				case SystemLanguage.Icelandic: res = "IS"; break;
				case SystemLanguage.Indonesian: res = "IN"; break;
				case SystemLanguage.Italian: res = "IT"; break;
				case SystemLanguage.Japanese: res = "JA"; break;
				case SystemLanguage.Korean: res = "KO"; break;
				case SystemLanguage.Latvian: res = "LV"; break;
				case SystemLanguage.Lithuanian: res = "LT"; break;
				case SystemLanguage.Norwegian: res = "NO"; break;
				case SystemLanguage.Polish: res = "PL"; break;
				case SystemLanguage.Portuguese: res = "PT"; break;
				case SystemLanguage.Romanian: res = "RO"; break;
				case SystemLanguage.Russian: res = "RU"; break;
				case SystemLanguage.SerboCroatian: res = "SH"; break;
				case SystemLanguage.Slovak: res = "SK"; break;
				case SystemLanguage.Slovenian: res = "SL"; break;
				case SystemLanguage.Spanish: res = "ES"; break;
				case SystemLanguage.Swedish: res = "SV"; break;
				case SystemLanguage.Thai: res = "TH"; break;
				case SystemLanguage.Turkish: res = "TR"; break;
				case SystemLanguage.Ukrainian: res = "UK"; break;
				case SystemLanguage.Unknown: res = "EN"; break;
				case SystemLanguage.Vietnamese: res = "VI"; break;
			}
			
			return res;
		}

		public string GetCountryCode() {
			string res ="US";
			//TODO get country
			return res;
		}

		public double GetTimeZoneGMTOffset() {
			TimeZone localZone = TimeZone.CurrentTimeZone;
			TimeSpan currentOffset = localZone.GetUtcOffset(System.DateTime.Now);
			return currentOffset.TotalHours;
		}
	}


	#if UNITY_IPHONE 

	public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo {

		[DllImport ("__Internal")]
		public static extern string GetDeviceLanguageCode ();
		
		public string GetLanguageCode() {
			
			return GamedoniaDeviceInfoImpl.GetDeviceLanguageCode();
			
		}

		[DllImport ("__Internal")]
		public static extern string GetDeviceCountryCode ();
		
		public string GetCountryCode() {
			
			return GamedoniaDeviceInfoImpl.GetDeviceCountryCode();
			
		}

		[DllImport ("__Internal")]
		public static extern double GetDeviceTimeZoneGMTOffset ();
		
		public double GetTimeZoneGMTOffset() {
			
			return GamedoniaDeviceInfoImpl.GetDeviceTimeZoneGMTOffset();
			
		}

	}
	
	#elif UNITY_ANDROID

	public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo {
		
		private static string _GetLanguageCode () {
			AndroidJNI.AttachCurrentThread();
			
			AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass("com.gamedonia.device.DeviceInfo");						
			return deviceInfoManagerClass.CallStatic<string>("getLanguageCode",new object [] {});
		}
		
		public string GetLanguageCode() {
			return GamedoniaDeviceInfoImpl._GetLanguageCode();
		}


		private static string _GetCountryCode () {
			AndroidJNI.AttachCurrentThread();
			
			AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass("com.gamedonia.device.DeviceInfo");						
			return deviceInfoManagerClass.CallStatic<string>("getCountryCode",new object [] {});
		}
		
		public string GetCountryCode() {
			return GamedoniaDeviceInfoImpl._GetCountryCode ();
		}


		private static double _GetTimeZoneGMTOffset () {
			AndroidJNI.AttachCurrentThread();
			
			AndroidJavaClass deviceInfoManagerClass = new AndroidJavaClass("com.gamedonia.device.DeviceInfo");						
			return deviceInfoManagerClass.CallStatic<double>("getTimeZoneGMTOffset",new object [] {});
		}

		public double GetTimeZoneGMTOffset() {
			return GamedoniaDeviceInfoImpl._GetTimeZoneGMTOffset ();
		}
		
	}

	#else
	
	public class GamedoniaDeviceInfoImpl: IGamedoniaDeviceInfo {
		
		public string GetLanguageCode() {
			return new GamedoniaDefaultDeviceInfoImpl ().GetLanguageCode ();	
		}

		public string GetCountryCode() {
			return new GamedoniaDefaultDeviceInfoImpl ().GetCountryCode ();
		}

		public double GetTimeZoneGMTOffset() {
			return new GamedoniaDefaultDeviceInfoImpl ().GetTimeZoneGMTOffset ();
		}
		
	}
	
	#endif
}	