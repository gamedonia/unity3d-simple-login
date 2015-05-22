using UnityEngine;
using System.Collections.Generic;
using System;

public delegate void InAppEventHandler();

public class GamedoniaStoreInAppPurchases : MonoBehaviour {

	private static List<GDInAppService> requestServices = new List<GDInAppService>();
	private static List<GDInAppService> purchaseServices = new List<GDInAppService>();
	
	public string androidPublickey = "";

	public bool debug = true;
	
	public void ProductPurchased(string response) {
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] ProductPurchased: " + response);
		if (response != null) GamedoniaStore.ProductPurchased(response);
		
		if (GamedoniaStore.purchaseResponse != null && GamedoniaStore.purchaseResponse.success) {
			#if UNITY_EDITOR
				OnPurchaseResponse();
			#elif UNITY_IPHONE
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters["deviceId"] = GamedoniaDevices.device.deviceId;
				parameters["receipt"] = GamedoniaStore.purchaseResponse.receipt;			
				GamedoniaStore.VerifyPurchase(parameters, 
					delegate (bool success) {
						if (!success) { 
							GamedoniaStore.purchaseResponse.success = false;
							GamedoniaStore.purchaseResponse.status = "error";
							GamedoniaStore.purchaseResponse.message = "purchase verification failed";
						}
						OnPurchaseResponse();
					}
				);	
			#elif UNITY_ANDROID
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters["deviceId"] = GamedoniaDevices.device.deviceId;
				parameters["receipt"] = GamedoniaStore.purchaseResponse.receipt;			
				GamedoniaStore.VerifyPurchase(parameters, 
					delegate (bool success) {
						if (!success) { 
							GamedoniaStore.purchaseResponse.success = false;
							GamedoniaStore.purchaseResponse.status = "error";
							GamedoniaStore.purchaseResponse.message = "purchase verification failed";
						}
						OnPurchaseResponse();
					}
				);
			#endif
		} else {
			OnPurchaseResponse();
		}
		
	}

	
	public void ProductsRequested(string response) {
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] ProductsRequested: " + response);
		if (response != null) GamedoniaStore.ProductsRequested(response);

		OnRequestResponse();
	}
		

	private static void Profile (GDDeviceProfile device, Action<bool> callback) {
		callback (true);
	}

	void Awake() {

		GDService service = new GDService();
		service.ProfileEvent += new ProfilerEventHandler(Profile);
		GamedoniaDevices.services.Add(service);

		#if UNITY_EDITOR
		#elif UNITY_ANDROID

		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] StartInAppBilling");
		GamedoniaStore.StartInAppBilling(androidPublickey);

		#endif
	}

	#if UNITY_EDITOR
	#elif UNITY_ANDROID
	void OnDestroy(){
		if (debug) Debug.Log("[GamedoniaStoreInAppPurchases] StopInAppBilling");
		GamedoniaStore.StopInAppBilling();
	}
	#endif

	private static GamedoniaStoreInAppPurchases _instance;

	public static GamedoniaStoreInAppPurchases Instance
	{
		get {
			return _instance;
		}
	}

	public static void AddRequestService(GDInAppService service){
		requestServices.Add(service);
	}

	public static void AddPurchaseService(GDInAppService service){
		purchaseServices.Add(service);
	}
	
	void OnRequestResponse() {
		foreach(GDInAppService service in requestServices) {
			service.OnResponse();
		}
	}

	void OnPurchaseResponse() {
		foreach(GDInAppService service in purchaseServices) {
			service.OnResponse();
		}
	}

}

public class GDInAppService {
	public event InAppEventHandler RegisterEvent;
	
	public void OnResponse() {
		RegisterEvent();
	}
}
