using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;
using LitJson_Gamedonia;

public class FacebookBinding {

		private static bool _initialized = false;

		#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern void _init(string appID, string urlSchemeSuffix, bool frictionless);

		[DllImport ("__Internal")]
		private static extern bool _isSessionOpen();

		[DllImport ("__Internal")]
		private static extern void _getAccessToken(StringBuilder accessToken, int n);

		[DllImport ("__Internal")]
		private static extern void _openSessionWithReadPermissions(string [] permissions, bool allowUI);

		[DllImport ("__Internal")]
		private static extern void _openSessionWithPublishPermissions(string [] permissions);

		[DllImport ("__Internal")]
		private static extern void _openSessionWithPermissions(string [] permissions);

		[DllImport ("__Internal")]
		private static extern void _reauthorizeSessionWithReadPermissions(string [] permissions);

		[DllImport ("__Internal")]
		private static extern void _reauthorizeSessionWithPublishPermissions(string [] permissions);

		[DllImport ("__Internal")]
		private static extern void _closeSessionAndClearTokenInformation();

		[DllImport ("__Internal")]
		private static extern void _requestWithGraphPath(string graphPath, string parameters = null, string httpMethod = "GET", string callbackName = null);

		[DllImport ("__Internal")]
		private static extern void _dialog(string method, string parameters = null, bool allowNativeUI = true, string callbackName = null);

		[DllImport ("__Internal")]
		private static extern void _publishInstall(string appId);
		
		#elif UNITY_ANDROID

		private static void _init(string appID, string urlSchemeSuffix, bool frictionless) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("init",new object [] {appID,urlSchemeSuffix});		
		}

		private static bool _isSessionOpen() {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			return facebookClass.CallStatic<bool>("isSessionOpen",new object [] {});	
		}

		private static string _getAccessToken() {	
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			return facebookClass.CallStatic<string>("getAccessToken",new object [] {});	
		}

		private static void _openSessionWithReadPermissions(string [] permissions, bool allowUI) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("openSessionWithPermissions",new object [] {permissions,"read"});
		}

		private static void _openSessionWithPublishPermissions(string [] permissions) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("openSessionWithPermissions",new object [] {permissions,"publish"});
		}

		private static void _openSessionWithPermissions(string [] permissions) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("openSessionWithPermissions",new object [] {permissions,"readAndPublish"});
		}

		private static void _reauthorizeSessionWithReadPermissions(string [] permissions) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("reauthorizeSessionWithPermissions",new object [] {permissions,"read"});
		}

		private static void _reauthorizeSessionWithPublishPermissions(string [] permissions) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("reauthorizeSessionWithPermissions",new object [] {permissions,"publish"});
		}

		private static void _closeSessionAndClearTokenInformation() {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("closeSessionAndClearTokenInformation",new object [] {});
		}

		private static void _requestWithGraphPath(string graphPath, string parameters = null, string httpMethod = "GET", string callbackName = null) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("requestWithGraphPath",new object [] {graphPath,parameters,httpMethod,callbackName});
		}

		private static void _dialog(string method, string parameters = null, bool allowNativeUI = true, string callbackName = null) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.GamedoniaSDKFacebook");					
			facebookClass.CallStatic("dialog",new object [] {method,parameters,callbackName});
		}

		#else
			
		
		private static void _init(string appID, string urlSchemeSuffix, bool frictionless) {
			//Empty
		}
		
		
		private static bool _isSessionOpen() {
			return false;
		}
		
		
		private static void _getAccessToken(StringBuilder accessToken, int n) {
			//Empty
		}
		
		
		private static void _openSessionWithReadPermissions(string [] permissions, bool allowUI) {
			//Empty
		}
		
		
		private static void _openSessionWithPublishPermissions(string [] permissions) {
			//Empty
		}
		
		
		private static void _openSessionWithPermissions(string [] permissions) {
			//Empty
		}
		
		
		private static void _reauthorizeSessionWithReadPermissions(string [] permissions) {
			//Empty
		}
		
		
		private static void _reauthorizeSessionWithPublishPermissions(string [] permissions) {
			//Empty
		}
		
		
		private static void _closeSessionAndClearTokenInformation() {
			//Empty
		}
		
		
		private static void _requestWithGraphPath(string graphPath, string parameters = null, string httpMethod = "GET", string callbackName = null) {
			//Empty
		}
		
		
		private static void _dialog(string method, string parameters = null, bool allowNativeUI = true, string callbackName = null) {
			//Empty
		}
		

		private static void _publishInstall(string appId) {
			//Empty
		}


		#endif

		public static void Init(string appID, string urlSchemeSuffix = "", bool frictionaless = true) {
			if (!Application.isEditor) {
					if (!_initialized) {
							_init (appID, urlSchemeSuffix, frictionaless);
							_initialized = true;
					}
			} else {
				Debug.Log("Facebook plugin doesn't work in Unity editor, test your Facebook in a device");
			}
		}

		public static bool IsSessionOpen() {
			if (!Application.isEditor) {
				return _isSessionOpen ();
			} else {
				return false;
			}
		}

		public static string GetAccessToken() {
			if (!Application.isEditor) {
				#if UNITY_IOS
				StringBuilder sb = new StringBuilder (512);
				_getAccessToken (sb, sb.Capacity);
				return sb.ToString ();
				#elif UNITY_ANDROID
				return _getAccessToken();
				#else
				return "";
				#endif
			} else {
				return "";
			}
		}

		public static void OpenSessionWithReadPermissions(string [] permissions, Action<bool,bool,string> callback = null, bool allowUI = true) {
			if (!Application.isEditor) {
					FacebookManager.Instance.openSessionCallback = callback;
					_openSessionWithReadPermissions (permissions, allowUI);
			}
		}

		public static void OpenSessionWithPublishPermissions(string [] permissions, Action<bool,bool,string> callback = null) {
			if (!Application.isEditor) {	
					FacebookManager.Instance.openSessionCallback = callback;
					_openSessionWithPublishPermissions (permissions);
			}
		}

		public static void OpenSessionWithPermissions(string [] permissions, Action<bool,bool,string> callback = null) {
			if (!Application.isEditor) {
				FacebookManager.Instance.openSessionCallback = callback;
				_openSessionWithPermissions (permissions);
			}
		}

		public static void ReauthorizeSessionWithReadPermissions(string [] permissions, Action<bool,bool,string> callback = null) {
			if (!Application.isEditor) {
						FacebookManager.Instance.reauthorizeSessionCallback = callback;
					_reauthorizeSessionWithReadPermissions (permissions);
			}
		}

		public static void ReauthorizeSessionWithPublishPermissions(string [] permissions, Action<bool,bool,string> callback = null) {
			if (!Application.isEditor) {	
					FacebookManager.Instance.reauthorizeSessionCallback = callback;
					_reauthorizeSessionWithPublishPermissions (permissions);
			}
		}

		public static void CloseSessionAndClearTokenInformation() {		
			if (!Application.isEditor) {
					_closeSessionAndClearTokenInformation ();
			}
		}

		public static void RequestWithGraphPath(string graphPath, IDictionary parameters = null, string httpMethod = "GET", Action<IDictionary> callback = null) {			

			if (!Application.isEditor) {
						// Verify the HTTP method
						if (!httpMethod.Equals ("GET") && !httpMethod.Equals ("POST") && !httpMethod.Equals ("DELETE")) {
								Debug.Log ("ERROR - Invalid HTTP method: " + httpMethod + " (must be GET, POST or DELETE)");
								return;
						}
			
						// Register the callback
						string callbackName = getNewCallbackName (callback);


						string json = null;

						try {
								if (parameters != null) {
										json = JsonMapper.ToJson (parameters);
								}

								_requestWithGraphPath (graphPath, json, httpMethod, callbackName);
						} catch (JsonException ex) {
								Debug.Log ("Request Input parameters in wrong format " + ex.Message);
						}
				}

			
			
		}

		public static void Dialog(string method, IDictionary parameters = null, bool allowNativeUI = true, Action<IDictionary> callback = null) {
			
			if (!Application.isEditor) {
						// Register the callback
						string callbackName = getNewCallbackName (callback);
			
			
						string json = null;
			
						try {
								if (parameters != null) {
										json = JsonMapper.ToJson (parameters);
								}
				
								_dialog (method, json, allowNativeUI, callbackName);
						} catch (JsonException ex) {
								Debug.Log ("Request Input paramters in wrong format " + ex.Message);
						}
				}
		}


		private static string getNewCallbackName(Action<IDictionary> callback) {
			
			// Generate callback name based on current time
			string callbackName = DateTime.Now.ToString();
					
			if (FacebookManager.Instance.requestCallbacks.ContainsKey (callbackName)) {
				FacebookManager.Instance.requestCallbacks.Remove(callbackName);
			}

			FacebookManager.Instance.requestCallbacks.Add (callbackName, callback);			
			return callbackName;
			
		}
}
