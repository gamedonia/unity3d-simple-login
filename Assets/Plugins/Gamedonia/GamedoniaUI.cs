using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Gamedonia.Backend {
	public class GamedoniaUI 
	{

		#if UNITY_IPHONE
			[DllImport ("__Internal")]
			public static extern void showAlertDialog (String parameters);
		
		#elif UNITY_ANDROID
			public static void showAlertDialog (String parameters) { 
			
				AndroidJNI.AttachCurrentThread(); 
		
				AndroidJavaClass uiClass = new AndroidJavaClass("com.gamedonia.utilities.GamedoniaUI");						
				uiClass.CallStatic("showAlertDialog",new object [] {parameters});
		
			}	

		#else
			public static void showAlertDialog (String parameters) {}	
		#endif
		
		public GamedoniaUI () {}
		
		
	}
}
