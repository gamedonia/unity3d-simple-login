using UnityEngine;
using System.Runtime.InteropServices;

namespace Gamedonia.Backend {
	public class OpenUDIDPlugin {

		#if UNITY_EDITOR
			//Returns a fixed value for testing purposes inside the editor
			public static string GetOpenUDID () { 
				return "10000";	
			}

		#elif UNITY_WEBPLAYER
			//Returns a fixed value for testing purposes inside the editor
			public static string GetOpenUDID () { 
				return "10000";	
			}
		
		#elif UNITY_IPHONE
			[DllImport ("__Internal")]
			public static extern string GetOpenUDID ();
		
		#elif UNITY_ANDROID
			public static string GetOpenUDID () { 
				
				AndroidJNI.AttachCurrentThread(); 
			
				AndroidJavaClass oudidManagerClass = new AndroidJavaClass("com.gamedonia.openudid.OpenUDIDPlugin");						
				return oudidManagerClass.CallStatic<string>("getOpenUDID",new object [] {});
			
			}
		#else
			public static string GetOpenUDID () { 
				return "10000";	
			}
		#endif
		
		
	}
}


