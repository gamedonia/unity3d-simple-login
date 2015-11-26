using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend {
	public class GamedoniaStore 
	{	


		#if UNITY_WEBPLAYER
		public static bool CanMakePayments () { return true; }
		public static void RequestProducts (string [] productIdentifiers, int size) {
			string response = "[{\"identifier\":\"premium\", \"description\":\"Premium product\", \"priceLocale\":\"$ 0.89\"}]";
			GameObject.Find("Gamedonia").SendMessage("ProductsRequested", response);
		}	
		public static void BuyProduct (string productIdentifier) {
			string response = "{\"success\":true, \"status\": \"success\", \"identifier\":\"" + productIdentifier +"\", \"message\":\"\", \"transactionId\":\"\", \"receipt\":\"\"}";
			GameObject.Find("Gamedonia").SendMessage("ProductPurchased", response);
		}

		public static void SetNonConsumables(String [] productIdentifiers, int size) {}
		
		#elif UNITY_IPHONE
		[DllImport ("__Internal")]
		public static extern bool CanMakePayments ();
		[DllImport("__Internal")]
		public static extern void RequestProducts(String [] productIdentifiers, int size);	
		[DllImport ("__Internal")] 
		public static extern void BuyProduct (String productIdentifier);
		public static void SetNonConsumables(String [] productIdentifiers, int size) {}
		
		#elif UNITY_ANDROID
		private static bool started = false;
		public static bool CanMakePayments () { return true; }	//By iphone compatibility
		
		private static void _StartInAppBilling(string key) {
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.BillingPlugin");					
			billingClass.CallStatic("StartInAppBilling",new object [] {key});		
		}
		
		public static void StartInAppBilling(string key){
			if(!started){
				started = true;
				if( Application.platform == RuntimePlatform.Android )
					_StartInAppBilling(key);
			}
		}
		
		private static void _StopInAppBilling() {
			try {
				AndroidJNI.AttachCurrentThread(); 
			
				AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.BillingPlugin");					
				billingClass.CallStatic("StopInAppBilling",new object [] {});			
			} catch (Exception ex) {
					Debug.Log(ex.Message);
			}
		}	
		
		public static void StopInAppBilling(){
			if(started){
				started = false;
				if( Application.platform == RuntimePlatform.Android )
					_StopInAppBilling();
			}
		}	
		
		private static void _RequestProducts(string [] productIdentifiers) {
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.BillingPlugin");					
			billingClass.CallStatic("RequestProducts",new object [] {productIdentifiers});			
		}

		private static void _SetNonConsumables(string [] productIdentifiers) {
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.BillingPlugin");					
			billingClass.CallStatic("SetNoConsumableProducts",new object [] {productIdentifiers});			
		}
		
		// Request products
		public static void RequestProducts( string [] productIdentifiers, int size )
		{
			if( Application.platform == RuntimePlatform.Android )
				_RequestProducts(productIdentifiers);
		}	

		public static void SetNonConsumables( string [] productIdentifiers, int size )
		{
			if( Application.platform == RuntimePlatform.Android )
				_SetNonConsumables(productIdentifiers);
		}


		private static void _BuyProduct(string productIdentifier) {
			
			AndroidJNI.AttachCurrentThread(); 
			
			AndroidJavaClass billingClass = new AndroidJavaClass("com.gamedonia.inapppurchase.BillingPlugin");					
			billingClass.CallStatic("PurchaseProduct",new object [] {productIdentifier});			
		}

	    // Buy product
		public static void BuyProduct (string productIdentifier) {
			if( Application.platform == RuntimePlatform.Android )
				_BuyProduct(productIdentifier);
		}

		#else
		public static bool CanMakePayments () { return true; }
		public static void RequestProducts (string [] productIdentifiers, int size) {
			string response = "[{\"identifier\":\"premium\", \"description\":\"Premium product\", \"priceLocale\":\"$ 0.89\"}]";
			GameObject.Find("Gamedonia").SendMessage("ProductsRequested", response);
		}	
		public static void BuyProduct (string productIdentifier) {
			string response = "{\"success\":true, \"status\": \"success\", \"identifier\":\"" + productIdentifier +"\", \"message\":\"\", \"transactionId\":\"\", \"receipt\":\"\"}";
			GameObject.Find("Gamedonia").SendMessage("ProductPurchased", response);
		}	

		public static void SetNonConsumables(String [] productIdentifiers, int size) {}

		#endif
		
		private static PurchaseResponse _purchaseResponse;
		private static ProductsRequestResponse _productsRequestResponse;

		public GamedoniaStore () {}
		
		public static void ProductPurchased(string response) {
			_purchaseResponse = JsonMapper.ToObject<PurchaseResponse>((string) response);
		}
		
		public static PurchaseResponse purchaseResponse {
			get { return _purchaseResponse; }
		}
		
		public static void ProductsRequested(string response) {
			
			if (response != null &&  !response.Equals("[]") ) {

				IDictionary theProductsRequestResponse = Json.Deserialize((string) response) as IDictionary;

				bool success = (bool)theProductsRequestResponse["success"];
				String message = theProductsRequestResponse["message"] as String;
				IList products = theProductsRequestResponse["products"] as IList;

				IDictionary<string, Product> theProducts = new Dictionary<string,Product>();

				for(int i = 0; i < products.Count; i++) {
					IDictionary product = (IDictionary) products[i];
					theProducts[(string) product["identifier"]] = new Product((string) product["identifier"], 
					                                                                        (string) product["description"], 
					                                                                        (string) product["priceLocale"]);
				}

				_productsRequestResponse = new ProductsRequestResponse(success,message,theProducts);
			}
		
		}
		
		public static void VerifyPurchase(Dictionary<string,object> parameters, Action<bool> callback = null) {
			
			string json = JsonMapper.ToJson(parameters);
			
			GamedoniaBackend.RunCoroutine(
				GamedoniaRequest.post("/purchase/verify", json, null, GamedoniaUsers.GetSessionToken(), null,
					delegate (bool success, object data) {
						if (callback!=null) callback(success);
					}
			 	 )
			);
		}	
		
		public static ProductsRequestResponse productsRequestResponse {
			get { return _productsRequestResponse; }
		}
	}

	public class PurchaseResponse {

			public bool success;
			public string status;
			public string message;
			public string identifier;
			public string transactionId;
			public string receipt;
		
	}
		

	public class ProductsRequestResponse {

		public bool success;
		public string message;
		public IDictionary<string, Product>products;

		public ProductsRequestResponse (bool success, string message, IDictionary<string, Product> products) {
			this.success = success;
			this.message = message;
			this.products = products;
		}
	}

	public class Product {
		
			public string identifier;
			public string description;
			public string priceLocale;
		
			public Product (string identifier, string description, string priceLocale) {
				this.identifier = identifier;
				this.description = description;
				this.priceLocale = priceLocale;
			}
		
	}
}
