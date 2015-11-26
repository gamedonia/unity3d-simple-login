using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend {

	public class GoogleManager : MonoBehaviour {

		public string clientId;

		// Use this for initialization
		public Action<bool,bool,string> openSessionCallback = null;
		public Action<bool,bool,string> reauthorizeSessionCallback = null;
		//public Dictionary<string,Action<IDictionary>> requestCallbacks = null;
		
		
		public static GoogleManager Instance
		{
			get {
				return _instance;
			}
		}
		
		private static GoogleManager _instance;
		
		
		void Awake() {
			
			if (_instance == null) {
				//requestCallbacks = new Dictionary<string,Action<IDictionary>> ();
				GoogleBinding.Init(clientId);
				_instance = this;
			}
			
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		
		void OnGoogleStatus(string response) {
			
			Action<bool,bool,string> callback = null;
			
			if (GamedoniaBackend.INSTANCE.debug) Debug.Log("Google Response: " + response);
			
			IDictionary ret = Json.Deserialize((string) response) as IDictionary;
			
			string eventName = ret["eventName"] as string;
			string message = ret["message"] as string;

			Debug.Log (">>>>> Event Name: " + eventName);
			Debug.Log (">>>>> Message: " + message);

			if (eventName.IndexOf ("SESSION") != -1) {
				
				bool success = (eventName.IndexOf ("SUCCESS") != -1);
				bool userCancelled = (eventName.IndexOf ("CANCEL") != -1);
				//string error = (eventName.IndexOf ("ERROR") != -1) ? message : null;
				string error = message;
			
				if (eventName.IndexOf ("OPEN") != -1)
					callback = this.openSessionCallback;
				//else if (eventName.IndexOf ("REAUTHORIZE") != -1)
				//	callback = reauthorizeSessionCallback;
				
				this.openSessionCallback = null;
				this.reauthorizeSessionCallback = null;
				
				if (callback != null)
					callback (success, userCancelled, error);
			} else if (eventName.Equals ("LOGGING")) { // Simple log message
				Debug.Log (message);
			} else {

				/*
				//TODO Tratamiento de callbacks para dialogs y requests
				
				Action<IDictionary> requestCallback = null;								
				
				if (this.requestCallbacks.ContainsKey(eventName)) {	
					
					requestCallback = this.requestCallbacks[eventName];											
					
					IDictionary data = null;	
					
					if (message.Length > 0) {
						data = Json.Deserialize(message) as IDictionary;
					}else {
						data = new Dictionary<string, object>();
					}
					
					
					requestCallback(data);
					
					requestCallbacks.Remove(eventName);
					
				}
				*/
			}
			
		}
	}
}
