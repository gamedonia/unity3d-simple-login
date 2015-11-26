using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend {

	public delegate void RegisterEventHandler(Dictionary<string,object> notification);

	public class GamedoniaPushNotifications : MonoBehaviour {

		private static List<GDPushService> services = new List<GDPushService>();
		
		public string androidSenderId = "";

		#if UNITY_WEBPLAYER

		private static List<Dictionary<string,object>> notifsWebPlayer = new List<Dictionary<string,object>>();

		public static void AddNotification(string json) {
			if (Instance.debug) Debug.Log("AddNotification: " + json);
			Dictionary<string,object> notification = new Dictionary<string,object>();
			
			IDictionary data = Json.Deserialize(json) as IDictionary;
			if (data.Contains("message")) notification["message"] = data["message"];
			if (data.Contains("payload")) {
				
				Hashtable payload = new Hashtable();
				IDictionary detail = (IDictionary) data["payload"];
				
				foreach(string key in detail.Keys) {
					payload.Add(key, detail[key]);
				}
				
				notification["payload"] = payload;
			}
			notifsWebPlayer.Add(notification);
		}
		private static void RegisterForRemoteNotifications() {}
		private static string GetDeviceTokenForRemoteNotifications() {
			return "20000000";	
		}	
		private static List<Dictionary<string,object>> GetRemoteNotifications() {
			List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
			
			if (notifsWebPlayer.Count > 0) {
				for (int r = 0; r < notifsWebPlayer.Count; r++) {
					Dictionary<string,object> notification = notifsWebPlayer[r];
					result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
						{"payload", notification["payload"]}});
				}
				notifsWebPlayer.Clear();
			}		
			
			return result;
		}	

		private static string GetErrorRemoteNotifications() {
			return null;
		}

		#elif UNITY_FLASH

		private static List<Dictionary<string,object>> notifsFlash = new List<Dictionary<string,object>>();

		public static void AddNotification(string json) {
			if (Instance.debug) Debug.Log("AddNotification: " + json);
			Dictionary<string,object> notification = new Dictionary<string,object>();
			
			IDictionary data = Json.Deserialize(json) as IDictionary;
			if (data.Contains("message")) notification["message"] = data["message"];
			if (data.Contains("payload")) {
				
				Hashtable payload = new Hashtable();
				IDictionary detail = (IDictionary) data["payload"];
				
				foreach(string key in detail.Keys) {
					payload.Add(key, detail[key]);
				}
				
				notification["payload"] = payload;
			}
			notifsFlash.Add(notification);
		}
		private static void RegisterForRemoteNotifications() {}
		private static string GetDeviceTokenForRemoteNotifications() {
			return "30000000";	
		}	
		private static List<Dictionary<string,object>> GetRemoteNotifications() {
			List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
			
			if (notifsFlash.Count > 0) {
				for (int r = 0; r < notifsFlash.Count; r++) {
					Dictionary<string,object> notification = notifsFlash[r];
					result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
						{"payload", notification["payload"]}});
				}
				notifsFlash.Clear();
			}		
			
			return result;
		}

		private static string GetErrorRemoteNotifications() {		
			return null;
		}

		#elif UNITY_IOS

			#if UNITY_5

			//public static RemoteNotificationType notificationType = RemoteNotificationType.Alert | RemoteNotificationType.Badge | RemoteNotificationType.Sound;
			public static UnityEngine.iOS.NotificationType notificationType = 0;

			[DllImport("__Internal")]
			private static extern void _RegisterForRemoteNotifications(int types);

			private static void RegisterForRemoteNotifications () {
				//NotificationServices.RegisterForRemoteNotificationTypes(GamedoniaPushNotifications.notificationType);

					if (GamedoniaPushNotifications.Instance.enableBadge) notificationType |= UnityEngine.iOS.NotificationType.Badge;
					if (GamedoniaPushNotifications.Instance.enableAlert) notificationType |= UnityEngine.iOS.NotificationType.Alert;
					if (GamedoniaPushNotifications.Instance.enableSound) notificationType |= UnityEngine.iOS.NotificationType.Sound;

					string sysInfo = SystemInfo.operatingSystem;
					sysInfo = sysInfo.Replace("iPhone OS ", "");
					string[] chunks = sysInfo.Split('.');
					int majorVersion = int.Parse(chunks[0]);
					if (majorVersion >= 8) {
						if (Instance.debug) Debug.Log("[Register Notification] major Version > 8");
						_RegisterForRemoteNotifications ((int)GamedoniaPushNotifications.notificationType);
					} else {
						if (Instance.debug) Debug.Log("[Register Notification] Unity Standard registration process");
						UnityEngine.iOS.NotificationServices.RegisterForNotifications (GamedoniaPushNotifications.notificationType);
					}


			}
			private static string GetDeviceTokenForRemoteNotifications () {
				return UnityEngine.iOS.NotificationServices.deviceToken != null ? System.BitConverter.ToString(UnityEngine.iOS.NotificationServices.deviceToken).ToLower().Replace("-", "") : null;
			}		
			private static List<Dictionary<string,object>> GetRemoteNotifications() {
				List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();		
					
				if (UnityEngine.iOS.NotificationServices.remoteNotificationCount > 0) {
					for (int r = 0; r < UnityEngine.iOS.NotificationServices.remoteNotificationCount; r++) {
						UnityEngine.iOS.RemoteNotification remoteNotification = UnityEngine.iOS.NotificationServices.GetRemoteNotification(r);
						result.Add(new Dictionary<string,object>() {{"message", remoteNotification.alertBody},
																	{"payload", remoteNotification.userInfo}});
					}
					UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
					if (GamedoniaPushNotifications.notificationType == UnityEngine.iOS.NotificationType.Badge) ClearBadge();
				}
				
				return result;
			}
			[DllImport("__Internal")]
			private static extern void ClearBadge ();

			private static string GetErrorRemoteNotifications() {
				return UnityEngine.iOS.NotificationServices.registrationError;
			}


			#else

			//public static RemoteNotificationType notificationType = RemoteNotificationType.Alert | RemoteNotificationType.Badge | RemoteNotificationType.Sound;
			public static RemoteNotificationType notificationType = 0;
			
			[DllImport("__Internal")]
			private static extern void _RegisterForRemoteNotifications(int types);
			
			private static void RegisterForRemoteNotifications () {
				//NotificationServices.RegisterForRemoteNotificationTypes(GamedoniaPushNotifications.notificationType);
				
				if (GamedoniaPushNotifications.Instance.enableBadge) notificationType |= RemoteNotificationType.Badge;
				if (GamedoniaPushNotifications.Instance.enableAlert) notificationType |= RemoteNotificationType.Alert;
				if (GamedoniaPushNotifications.Instance.enableSound) notificationType |= RemoteNotificationType.Sound;
				
				string sysInfo = SystemInfo.operatingSystem;
				sysInfo = sysInfo.Replace("iPhone OS ", "");
				string[] chunks = sysInfo.Split('.');
				int majorVersion = int.Parse(chunks[0]);
				if (majorVersion >= 8) {
					if (Instance.debug) Debug.Log("[Register Notification] major Version > 8");
					_RegisterForRemoteNotifications ((int)GamedoniaPushNotifications.notificationType);
				} else {
					if (Instance.debug) Debug.Log("[Register Notification] Unity Standard registration process");
					NotificationServices.RegisterForRemoteNotificationTypes (GamedoniaPushNotifications.notificationType);
				}
				
				
			}
			private static string GetDeviceTokenForRemoteNotifications () {
				return NotificationServices.deviceToken != null ? System.BitConverter.ToString(NotificationServices.deviceToken).ToLower().Replace("-", "") : null;
			}		
			private static List<Dictionary<string,object>> GetRemoteNotifications() {
				List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();		
				
				if (NotificationServices.remoteNotificationCount > 0) {
					for (int r = 0; r < NotificationServices.remoteNotificationCount; r++) {
						RemoteNotification remoteNotification = NotificationServices.GetRemoteNotification(r);
						result.Add(new Dictionary<string,object>() {{"message", remoteNotification.alertBody},
							{"payload", remoteNotification.userInfo}});
					}
					NotificationServices.ClearRemoteNotifications();
					if (GamedoniaPushNotifications.notificationType == RemoteNotificationType.Badge) ClearBadge();
				}
				
				return result;
			}
			[DllImport("__Internal")]
			private static extern void ClearBadge ();
			
			private static string GetErrorRemoteNotifications() {
				return NotificationServices.registrationError;
			}
			#endif


		#elif UNITY_ANDROID

		private static string registrationId = null;
		private static string errorId = null;
		private static List<Dictionary<string,object>> notifsAndroid = new List<Dictionary<string,object>>();

		public static void AddNotification(string json) {
			if (Instance.debug) Debug.Log("AddNotification: " + json);
			Dictionary<string,object> notification = new Dictionary<string,object>();
			
			IDictionary data = Json.Deserialize(json) as IDictionary;
			if (data.Contains("message")) notification["message"] = data["message"];
			if (data.Contains("payload")) {
				
				Hashtable payload = new Hashtable();
				IDictionary detail = (IDictionary) data["payload"];
				
				foreach(string key in detail.Keys) {
					payload.Add(key, detail[key]);
				}
				
				notification["payload"] = payload;
			}
			notifsAndroid.Add(notification);		
		}	
		
		public void SetErrorForRemoteNotifications(string errId) {
			errorId = errId;
		}
		
		public void SetDeviceTokenForRemoteNotifications(string regId) {
			registrationId = regId;

			Debug.Log ("Device Token Received: " + regId);
			GamedoniaDevices.device.deviceToken = regId;
			GamedoniaDevices.GetProfile(OnGetProfile);
		}

		public void SetRemoteNotificationsRegistrationError(string error) {
			errorId = error;
		}
		
		private static void _RegisterForRemoteNotifications(string senderId) {
			
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
			pushClass.CallStatic("registerForRemoteNotifications",new object [] {senderId});	
		}
		
		// Starts up the SDK
		private static void RegisterForRemoteNotifications()
		{
			if( Application.platform == RuntimePlatform.Android )
				_RegisterForRemoteNotifications(GamedoniaPushNotifications.Instance.androidSenderId);
		}	
		
		private static void _Pause() {
			
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
			pushClass.CallStatic("pause",new object [] {});	
		}
		
		// On Pause
		private static void Pause()
		{
			if( Application.platform == RuntimePlatform.Android )
				_Pause();
		}	
		
		private static void _Resume() {
			
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
			pushClass.CallStatic("resume",new object [] {});	
		}
		
		// On Resume
		private static void Resume()
		{
			if( Application.platform == RuntimePlatform.Android )
				_Resume();
		}	
		
		private static void _ClearNotifications() {
			
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass pushClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");						
			pushClass.CallStatic("clearNotifications",new object [] {});	
		}
		
		// Clear Notifications
		private static void ClearNotifications()
		{
			if( Application.platform == RuntimePlatform.Android )
				_ClearNotifications();
		}	
		
		private static string GetErrorRemoteNotifications() {
			return errorId;
		}	

		private static string GetDeviceTokenForRemoteNotifications() {
			return registrationId;
		}	

		private static void _GetRemoteNotifications() {
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass notifClass = new AndroidJavaClass("com.gamedonia.pushnotifications.PushNotifications");
			string[] notifications = notifClass.CallStatic<string[]>("getRemoteNotifications",new object [] {});

			foreach (string notification in notifications) {
				AddNotification(notification);
			}

		}
		
		private static List<Dictionary<string,object>> GetRemoteNotifications() {
			if( Application.platform == RuntimePlatform.Android ) {
				_GetRemoteNotifications();
			}
			
			List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
			
			if (notifsAndroid.Count > 0) {
				for (int r = 0; r < notifsAndroid.Count; r++) {
					Dictionary<string,object> notification = notifsAndroid[r];
					result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
																{"payload", notification["payload"]}});
				}
				notifsAndroid.Clear();
			}		
			
			return result;
		}	

		private static string GetRemoteNotificationError() {
			return errorId;
		}

		#else

		private static List<Dictionary<string,object>> notifsEditor = new List<Dictionary<string,object>>();
		
		public static void AddNotification(string json) {
			if (Instance.debug) Debug.Log("AddNotification: " + json);
			Dictionary<string,object> notification = new Dictionary<string,object>();
			
			IDictionary data = Json.Deserialize(json) as IDictionary;
			if (data.Contains("message")) notification["message"] = data["message"];
			if (data.Contains("payload")) {
				
				Hashtable payload = new Hashtable();
				IDictionary detail = (IDictionary) data["payload"];
				
				foreach(string key in detail.Keys) {
					payload.Add(key, detail[key]);
				}
				
				notification["payload"] = payload;
			}
			notifsEditor.Add(notification);
		}
		private static void RegisterForRemoteNotifications() {}
		private static string GetDeviceTokenForRemoteNotifications() {
			return "10000000";	
		}	
		private static List<Dictionary<string,object>> GetRemoteNotifications() {
			List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
			
			if (notifsEditor.Count > 0) {
				for (int r = 0; r < notifsEditor.Count; r++) {
					Dictionary<string,object> notification = notifsEditor[r];
					result.Add(new Dictionary<string,object>() {{"message", notification["message"]},
						{"payload", notification["payload"]}});
				}
				notifsEditor.Clear();
			}		
			
			return result;
		}
		
		private static string GetErrorRemoteNotifications() {		
			return null;
		}


		#endif	

		public bool clearBadgeOnActivate = true;
		public bool enableBadge = true;
		public bool enableSound = true;
		public bool enableAlert = true;
		public bool debug = true;
		
		public static string deviceToken;
		
		private static GamedoniaPushNotifications _instance;
		private static List<Dictionary<string,object>> notifications;
		private static int count;
		
		public static GamedoniaPushNotifications Instance
		{
			get {
				return _instance;
			}
		}

		public static void AddService(GDPushService service){
			services.Add(service);
		}
		
		private static void Profile (GDDeviceProfile device, Action<bool> callback) {
			GamedoniaBackend.RunCoroutine(
				ProfilePushNotification(device,
					delegate(bool success) {
						callback(success);
					}
				)
			);	
		}
		
		
		private static IEnumerator ProfilePushNotification (GDDeviceProfile device, Action<bool> callback) {
			
			if (Instance.debug) Debug.Log("[Register Notification] RegisterForRemoteNotifications");

			if (!Application.isEditor) {
					RegisterForRemoteNotifications ();

					while (GetDeviceTokenForRemoteNotifications() == null && GetErrorRemoteNotifications() == null) {
							yield return null;
					}

					if (GetErrorRemoteNotifications () == null) {
							deviceToken = GetDeviceTokenForRemoteNotifications ();

							if (Instance.debug)
									Debug.Log ("[Register Notification] token:" + deviceToken);
							device.deviceToken = deviceToken;
							callback (true);
					} else {
							Debug.LogWarning ("[Register Notification] failed to obtain Push Device Token: " + GetErrorRemoteNotifications ());
							callback (false);
					}
			} else {
				callback(true);
			}
			
		}	
			
		void Awake () {
			
			//make sure we only have one object with this Gamedonia script at any time
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}
			
			_instance = this;
			notifications = new List<Dictionary<string,object>>();
			DontDestroyOnLoad(this);
			
			#if UNITY_EDITOR 
			#elif UNITY_IOS
				#if UNITY_5 
				if ((notificationType & UnityEngine.iOS.NotificationType.Badge) != 0 && clearBadgeOnActivate) {
					ClearBadge();
				}
				#else			
				if ((notificationType & RemoteNotificationType.Badge) != 0 && clearBadgeOnActivate) {
					ClearBadge();
				}
				#endif				
			#endif
			
			/*
			GDService service = new GDService();
			service.ProfileEvent += new ProfilerEventHandler(Profile);
			GamedoniaDevices.services.Add(service);
			*/


			if (!Application.isEditor) {
				RegisterForRemoteNotifications ();
			}

		}	

		private void OnGetProfile(bool success, GDDeviceProfile device) {
			if (success) {

				Debug.Log ("OnGetProfile deviceType: " + device.deviceType);
				//label.text += "\n Register device with id => " + device.deviceId;
				//label.text += "\n Register device with platform => " + device.deviceType;
				//label.text += "\n Register device for remote notification with token => " + device.deviceToken;
				
				switch(device.deviceType) {
				case "ios":
				case "android":	
					if (GamedoniaUsers.me != null) GamedoniaDevices.device.uid = GamedoniaUsers.me._id;
					GamedoniaDevices.Register(device);
					break;
				}
			}
		}

		void FixedUpdate () {
			
			count++;
			if ((count % Application.targetFrameRate) == 0) {
				count = 0;
				
				notifications = GetRemoteNotifications();
				if (notifications.Count > 0) {
					if (debug) Debug.Log("[Remote Notification] count: " + notifications.Count);
					foreach(Dictionary<string,object> notification in notifications){
						if (debug) { 
							Debug.Log("[Remote Notification] message: " + notification["message"]);
						}
						if (debug) {
							foreach (DictionaryEntry entry in (Hashtable) notification["payload"]) {
								Debug.Log("[Remote Notification] payload: " + entry.Key + " => " + entry.Value);
							}
						}

						if (debug) Debug.Log("[Remote Notification] SendMessage: " + notification);
						OnNotification(notification);
									
					}
					notifications.Clear();				
				}
			}

		}



		void OnNotification(Dictionary<string,object> notification) {
			foreach(GDPushService service in services) {
				service.OnNotification(notification);
			}
		}
		
		void OnApplicationPause (bool pause) {
			if (debug) Debug.Log("[Remote Notification] OnApplicationPause : " + pause);
			#if UNITY_EDITOR
			#elif UNITY_IOS
				if (!pause) { 		
					#if UNITY_5
					if ((notificationType & UnityEngine.iOS.NotificationType.Badge) != 0 && clearBadgeOnActivate) {
						ClearBadge();
					}				
					#else
					if ((notificationType & RemoteNotificationType.Badge) != 0 && clearBadgeOnActivate) {
						ClearBadge();
					}
					#endif					 
				}
			#elif UNITY_ANDROID
				if (!pause) {
					Resume();
					if (clearBadgeOnActivate) ClearNotifications();
				}else {
					Pause();
				}
			#endif		
		}
		
		void OnEnable () {
			if (debug) Debug.Log("[Remote Notification] OnEnable");
		    #if UNITY_EDITOR		
			#elif UNITY_ANDROID
					Resume();
					if (clearBadgeOnActivate) ClearNotifications();
			#endif
		}
		
		void OnApplicationQuit () {
			if (debug) Debug.Log("[Remote Notification] OnApplicationQuit");
		    #if UNITY_EDITOR		
			#elif UNITY_ANDROID
					Pause();
			#endif
		}	

		void DidRegisterForRemoteNotifications(string data) {

			Debug.Log ("Device Token Received: " + data);
			GamedoniaDevices.device.deviceToken = data;
			GamedoniaDevices.GetProfile(OnGetProfile);
		}

		void DidFailToRegisterForRemoteNotificationsWithError(string error) {

			Debug.Log ("Remote notifications registration failed with error: " + error);

		}
	}

	public class GDPushService {
		public event RegisterEventHandler RegisterEvent;
		
		public void OnNotification(Dictionary<string,object> notification) {
			RegisterEvent(notification);
		}
	}
}
