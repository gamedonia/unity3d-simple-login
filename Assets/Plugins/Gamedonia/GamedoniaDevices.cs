using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

public delegate void ProfilerEventHandler(GDDeviceProfile device, Action<bool> callback);

public class GamedoniaDevices {
		
	public static GDDeviceProfile device = new GDDeviceProfile();	
	public static List<GDService> services = new List<GDService>();
	
	
	private static Dictionary<string,string> platforms = new Dictionary<string,string>(){{"OSXEditor","editor"},
		                 																 {"WindowsEditor","editor"},		
		                 																 {"IPhonePlayer","ios"},
																						 {"Android","android"}};
	private static int registeredServices = 0;
	
	public GamedoniaDevices () {
	}
	
	public static void GetProfile(Action<bool, GDDeviceProfile> callback) {
		
		bool result = true;
	
		device.deviceId = OpenUDIDPlugin.GetOpenUDID();
		device.deviceType = platforms.ContainsKey(Application.platform.ToString()) ? platforms[Application.platform.ToString()] : "";

		registeredServices = 0;

		if (services.Count == 0) {
			callback (true, device);
		} else {
			foreach(GDService service in services) {
				service.GetProfile(device, 
				                   delegate (bool success) {
					if (!success)
						result = false;
					
					registeredServices ++;
					if ( registeredServices == services.Count )
						if (callback != null) callback(result, device);
				}
				);
			}
		}




	}
	
	public static void Register(GDDeviceProfile device, Action<bool> callback = null) {
		
		string json = JsonMapper.ToJson(device);		
				
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/device/register",json,
				delegate (bool success, object data) {
					if (callback!=null) callback(success);				
				}
		 	 )
		);
		
	}
	
}

public class GDService {
	public event ProfilerEventHandler ProfileEvent;
	
	public void GetProfile(GDDeviceProfile device, Action<bool> callback) {
		ProfileEvent(device, callback);
	}
}

public class GDDeviceProfile {
	
	public string deviceId;
	public string deviceToken;
	public string deviceType;
	public string uid;
	
}
