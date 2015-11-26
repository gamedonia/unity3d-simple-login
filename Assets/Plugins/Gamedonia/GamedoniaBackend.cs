using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using HTTP;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;
	
namespace Gamedonia.Backend {

	public class GamedoniaBackend: MonoBehaviour {
		
		public enum CredentialsType {SILENT, MAIL, FACEBOOK, TWITTER, GAMECENTER, GOOGLE};
		public enum ApiVersionNum {v1};
		public bool paused;


		public static GamedoniaBackend INSTANCE
		{
			get {
				return _instance;
			}
		}
		
		private static GamedoniaBackend _instance;	
		
		public string ApiServerUrl = "";
		public string ApiKey = "";
		public string Secret = "";	
		public ApiVersionNum ApiVersion = ApiVersionNum.v1;
		public CredentialsType AuthenticationMode = CredentialsType.GAMECENTER; 
		public bool debug = true;
		public GDError lastError;
		
		public void Awake() {
			
			//make sure we only have one object with this Gamedonia script at any time
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}
			
			if (ApiKey.Equals("") || Secret.Equals(""))
			{
				Debug.LogError("Gamedonia Error: Missing value for ApiKey / Secret Gamedonia will not work!");
				Destroy(gameObject);
				return;
			}
			
			_instance = this;
			DontDestroyOnLoad(this);
			GamedoniaRequest.initialize(ApiKey,Secret, ApiServerUrl, ApiVersion.ToString());
			Debug.Log ("Gamedonia initialized successfully");
		}
		


		public static Coroutine RunCoroutine(IEnumerator routine)
		{
			if (_instance == null)
			{
				Debug.LogError("Gamedonia Error: No Gamedonia instance exists. Drag the Gamedonia game object into your scene.");
				return null;
			}
			
			return _instance.StartCoroutine(routine);
		}
		
		public static GDError getLastError() {		
			return INSTANCE.lastError;		
		}

		void OnApplicationPause(bool pauseStatus) {
			paused = pauseStatus;
		}

		public bool IsDeviceRegisterNeeded() {

			GamedoniaPushNotifications pushScript = this.GetComponent<GamedoniaPushNotifications>();
			GamedoniaStoreInAppPurchases inappScript = this.GetComponent<GamedoniaStoreInAppPurchases>();

			return (pushScript != null || inappScript != null);

		}

		public static void isInternetConnectionAvailable(Action<bool> callback) {

			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.get("/ping","",delegate (bool success, object data) {
					if (callback!=null) callback(success);
				})
			);
		
		}
	}
		

	public class GDError {
		
		public int httpErrorCode;
		public string httpErrorMessage;
		public int code;
		public string message;
		
		public string ToString() {		
			return code.ToString() + " - " + message;		
		}

		public static GDError buildError(Response response) {

			GDError error = null;
			if (!String.IsNullOrEmpty (response.Text)) {
				error = JsonMapper.ToObject<GDError>(response.Text);
				error.httpErrorCode = response.status;
				if (!String.IsNullOrEmpty(response.message)) error.httpErrorMessage = response.message;

			} else {
				error = new GDError();
				error.httpErrorCode = response.status;
			}

			return error;
		}
	}
}
