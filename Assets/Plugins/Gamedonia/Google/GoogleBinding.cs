using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Gamedonia.Backend {

	public class GoogleBinding {


		#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern void _signIn();

		[DllImport ("__Internal")]
		private static extern void _initGoogle(string clientId);

		#elif UNITY_ANDROID
		private static void _initGoogle(string clientId) {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.google.GamedoniaSDKGoogle");					
			facebookClass.CallStatic("init",new object [] {clientId});
		}


		private static void _signIn() {
			AndroidJNI.AttachCurrentThread(); 			
			AndroidJavaClass facebookClass = new AndroidJavaClass("com.gamedonia.sdk.social.google.GamedoniaSDKGoogle");					
			facebookClass.CallStatic("signIn",new object [] {});
		}

		

		#else 

		private static void _initGoogle(string clientId) {
			Debug.Log ("Google + Sign In is not supported for platform " + Application.platform);
		}

		private static void _signIn() {

		}

		#endif

		public static void SignIn(Action<bool,bool,string> callback = null) {
			if (!Application.isEditor) {
				GoogleManager.Instance.openSessionCallback = callback;
				_signIn ();
			}
		}

		public static void Init(string clientId, Action<bool> callback = null) {
			if (!Application.isEditor) {
				_initGoogle (clientId);
			} else {
				Debug.Log("Google plugin doesn't work in Unity editor, test your Google Sign In in a device");
			}
		}


	}
}
