using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend
{

		public delegate void ProfileEventHandler (GDDeviceProfile device,Action<bool> callback);

		public class GamedoniaDevices
		{
			
				public static GDDeviceProfile device = new GDDeviceProfile ();
				public static List<GDService> services = new List<GDService> ();

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
				private static int registeredServices = 0;

				public GamedoniaDevices ()
				{
				}

				public static void RegisterProfileBuilder(Action<GDDeviceProfile,Action<bool>> handler) {

						GDService service = new GDService ();
						service.ProfileEvent += new ProfileEventHandler (handler);
						GamedoniaDevices.services.Add (service);

				}

				public static void GetProfile (Action<bool, GDDeviceProfile> callback)
				{
			
						bool result = true;
		
						device.deviceId = GamedoniaSilent.GetSilentId ();
						device.deviceType = platforms.ContainsKey (Application.platform.ToString ()) ? platforms [Application.platform.ToString ()] : "";
						device.timeZoneGMTOffset = GamedoniaDeviceInfo.GetTimeZoneGMTOffset ();
						device.language = GamedoniaDeviceInfo.GetLanguageCode ();
						device.country = GamedoniaDeviceInfo.GetCountryCode ();
						device.device = GamedoniaDeviceInfo.GetDevice ();
						device.platform = GamedoniaDeviceInfo.GetPlatform ();
						device.buildVersion = GamedoniaDeviceInfo.GetBuildVersion ();
						device.os = GamedoniaDeviceInfo.GetOS ();
						device.osVersion = GamedoniaDeviceInfo.GetOSVersion ();
						device.imei = GamedoniaDeviceInfo.GetIMEI ();
						device.idfa = GamedoniaDeviceInfo.GetIDFA ();
						device.idfv = GamedoniaDeviceInfo.GetIDFV ();
						device.manufacturer = GamedoniaDeviceInfo.GetDeviceManufacturer ();
						device.model = GamedoniaDeviceInfo.GetDeviceModel ();
						device.hardware = GamedoniaDeviceInfo.GetDeviceHardware ();
						device.adTrackingEnabled = GamedoniaDeviceInfo.IsAdTrackingEnabled ();
						device.jailbroken = GamedoniaDeviceInfo.IsJailBroken ();





						registeredServices = 0;

						if (services.Count == 0) {
								callback (true, device);
						} else {
								foreach (GDService service in services) {
										service.GetProfile (device, 
												delegate (bool success) {
														if (!success)
																result = false;
						
														registeredServices++;
														if (registeredServices == services.Count)
														if (callback != null)
																callback (result, device);
												}
										);
								}
						}


						//Callback to intercept creation

				}

				public static void Register (GDDeviceProfile device, Action<bool> callback = null)
				{
			
						string json = JsonMapper.ToJson (device);		
					
						GamedoniaBackend.RunCoroutine (
								GamedoniaRequest.post ("/device/register", json,
										delegate (bool success, object data) {
												if (callback != null)
														callback (success);				
										}
								)
						);
			
				}
		
		}

		public class GDService
		{
				public event ProfileEventHandler ProfileEvent;

				public void GetProfile (GDDeviceProfile device, Action<bool> callback)
				{
						ProfileEvent (device, callback);
				}
		}

		public class GDDeviceProfile
		{
		
				public string deviceId;
				public string deviceToken;
				public string deviceType;
				public string uid;
				public string language;
				public string country;
				public double timeZoneGMTOffset;

				//New Fields
				public string device;
				public string platform;
				public string buildVersion;
				public string os;
				public string osVersion;
				public string imei;
				public string idfa;
				public string idfv;
				public string manufacturer;
				public string model;
				public string hardware;
				public bool adTrackingEnabled;
				public bool jailbroken;
				public Dictionary<string,object> custom = new Dictionary<string,object> ();

		}
}
