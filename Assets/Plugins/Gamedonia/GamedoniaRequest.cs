using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using HTTP;
using System.Threading;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

namespace Gamedonia.Backend {

	public static class GamedoniaRequest
	{
		
		public const string GD_APIKEY 			= "X-Gamedonia-ApiKey";
		public const string GD_SIGNATURE 		= "X-Gamedonia-Signature";
		public const string GD_SESSION_TOKEN 	= "X-Gamedonia-SessionToken";
		public const string GD_AUTH 			= "Authorization";
		public const string GD_GAMEID			= "gameid";
		
		private static string _apiKey;
		private static string _secret;
		private static string _baseURL 			= "http://localhost:9000";
		private static string _version 			= "v1";
		private const string DATE_FORMAT 		= "{0:dd/MM/yyyy HH:mm:ss zzz}";
		private const string _content_type 		= "application/json";
		
		/*
		 * Gamedonia Request Interface
		 */ 		
		public static void initialize(string apiKey, string secret, string baseUrl, string apiVersion) {
			
			_apiKey = apiKey;
			_secret = secret;
			_baseURL = baseUrl;
			_version = apiVersion;
			
		}
		
		
		public static IEnumerator postWithAuth(string url, string auth, Action<bool,object> callback = null) {
			return post(url,null,auth,null,null,callback);			
		}
		
		public static IEnumerator post(string url, string content, Action<bool,object> callback = null) {
			return post(url,content,null,null,null,callback);			
		}
		
		
		public static IEnumerator post(string url, string content, string auth = null, string sessionToken = null, string gameid = null, Action<bool,object> callback = null) {
			
			if (GamedoniaBackend.INSTANCE.debug) {
				String debugMsg = "[Api Call] - " + url;
				if (!String.IsNullOrEmpty(content)) debugMsg += " " + content;
				Debug.Log(debugMsg);
			}
			
			string path = _baseURL + "/" + _version + url;
			
			Request request = new Request("POST",path);
			request.AddHeader("Content-Type",_content_type);
			request.AddHeader(GD_APIKEY, _apiKey);
			
			if (!string.IsNullOrEmpty(auth)) request.AddHeader(GD_AUTH, auth);
			if (!string.IsNullOrEmpty(sessionToken)) request.AddHeader(GD_SESSION_TOKEN, sessionToken);
			if (!string.IsNullOrEmpty(gameid)) request.AddHeader(GD_GAMEID, gameid);
			
			if (!string.IsNullOrEmpty(content)) {
				
				//Sign data to post
				string date = GetCurrentDate();
				string hmac = Sign(_apiKey, _secret, Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(content)), _content_type, date, "POST", request.uri.AbsolutePath);
				
				request.AddHeader(GD_SIGNATURE,hmac);
				request.AddHeader("Date",date);
				request.bytes = Encoding.UTF8.GetBytes(content);
			}
			
			request.Send();
			
			while (!request.isDone) {	yield return null; }
					
			
			if (request.response.status == 200) {
				if (GamedoniaBackend.INSTANCE.debug) Debug.Log("[Api Response][" + url + "] - " + request.response.status + " " + request.response.Text);
				callback(true, request.response.Text);
			}else {

				printResponseLog(url,request.response);
				GDError error = GDError.buildError(request.response);
				GamedoniaBackend.INSTANCE.lastError = error;
				callback(false, request.response.message);
			}
			
		}
		
		/*
		public static IEnumerator post(string url, string content, string auth = null, string sessionToken = null, string gameid = null, Action<bool,object> callback = null) {
			
			string path = _baseURL + "/" + _version + url;
			
			//Set headers
			Hashtable headers = new Hashtable();
			headers.Add("Content-Type",_content_type);
			headers.Add(GD_APIKEY, _apiKey);
			
			if (!string.IsNullOrEmpty(auth)) headers.Add(GD_AUTH, auth);
			if (!string.IsNullOrEmpty(sessionToken)) headers.Add(GD_SESSION_TOKEN, sessionToken);
			if (!string.IsNullOrEmpty(gameid)) headers.Add(GD_GAMEID, gameid);
			
			WWW www = null;
			
			if (!string.IsNullOrEmpty(content)) {
				
				//Sign data to post
				string hmac = Sign(_apiKey,_secret,content,_content_type,"POST","/" + _version + url);
				
				headers.Add(GD_SIGNATURE,hmac);
				headers.Add("Date",GetCurrentDate());
				
				www = new WWW(path, Encoding.UTF8.GetBytes(content), headers);
			}
			
			
			
			yield return www;
			
			if (www.error != null)
			{	
				callback(false,www.text);
			}else {
				callback(true,www.text);
			}
			
		}*/
			
		public static IEnumerator get(string url, string sessionToken, Action<bool,object> callback = null) {
			
			if (GamedoniaBackend.INSTANCE.debug) {
				String debugMsg = "[Api Call] - " + url;
				Debug.Log(debugMsg);
			}
			
			string path = _baseURL + "/" + _version + url;
			string date = GetCurrentDate();
			Request request = new Request("get",path);
			request.AddHeader(GD_SESSION_TOKEN,sessionToken);
			request.AddHeader(GD_APIKEY, _apiKey);
			request.AddHeader("Date", date);
			request.AddHeader(GD_SIGNATURE, Sign(_apiKey, _secret, date, "GET", request.uri.AbsolutePath));
					
			request.Send();
			
			while (!request.isDone)
	        {
	            yield return request;
	        }

			if (request.response.status == 200) {
				if (GamedoniaBackend.INSTANCE.debug) Debug.Log("[Api Response][" + url + "] - " + request.response.status + " " + request.response.Text);
				callback(true, request.response.Text);
			}else {

				printResponseLog(url,request.response);
				GDError error = GDError.buildError(request.response);
				GamedoniaBackend.INSTANCE.lastError = error;
				callback(false, request.response.message);
			}
			
		}
		
		
		public static IEnumerator put(string url, string content, string auth = null, string sessionToken = null, string gameid = null, Action<bool,object> callback = null) {
			
			if (GamedoniaBackend.INSTANCE.debug) {
				String debugMsg = "[Api Call] - " + url;
				if (!String.IsNullOrEmpty(content)) debugMsg += " " + content;
				Debug.Log(debugMsg);
			}
			
			string path = _baseURL + "/" + _version + url;
			
			Request request = new Request("PUT",path);
			request.AddHeader("Content-Type",_content_type);
			request.AddHeader(GD_APIKEY, _apiKey);
			
			if (!string.IsNullOrEmpty(auth)) request.AddHeader(GD_AUTH, auth);
			if (!string.IsNullOrEmpty(sessionToken)) request.AddHeader(GD_SESSION_TOKEN, sessionToken);
			if (!string.IsNullOrEmpty(gameid)) request.AddHeader(GD_GAMEID, gameid);
			
			if (!string.IsNullOrEmpty(content)) {
				
				//Sign data to post
				string date = GetCurrentDate();
				string hmac = Sign(_apiKey, _secret, Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(content)), _content_type, date, "PUT", request.uri.AbsolutePath);
				
				request.AddHeader(GD_SIGNATURE,hmac);
				request.AddHeader("Date", date);
				request.bytes = Encoding.UTF8.GetBytes(content);
			}
			
			request.Send();
			
			while (!request.isDone)
	        {
	            yield return null;
	        }
			
			if (request.response.status == 200) {
				if (GamedoniaBackend.INSTANCE.debug) Debug.Log("[Api Response][" + url + "] - " + request.response.status + " " + request.response.Text);
				callback(true, request.response.Text);
			}else {

				printResponseLog(url,request.response);
				GDError error = GDError.buildError(request.response);
				GamedoniaBackend.INSTANCE.lastError = error;
				callback(false, request.response.message);
			}
			
			
		}
		
		public static IEnumerator delete(string url, string sessionToken, Action<bool,object> callback = null) {
			
			if (GamedoniaBackend.INSTANCE.debug) {
				String debugMsg = "[Api Call] - " + url;
				Debug.Log(debugMsg);
			}
			
			string path = _baseURL + "/" + _version + url;
			string date = GetCurrentDate();
			Request request = new Request("delete",path);
			if (!string.IsNullOrEmpty(sessionToken)) request.AddHeader(GD_SESSION_TOKEN,sessionToken);
			request.AddHeader(GD_APIKEY, _apiKey);
			request.AddHeader("Date", date);
			request.AddHeader(GD_SIGNATURE, Sign(_apiKey, _secret, date, "DELETE", request.uri.AbsolutePath));
			
			request.Send();
			
			while (!request.isDone) {	yield return null; }
					
			if (request.response.status == 200) {
				if (GamedoniaBackend.INSTANCE.debug) Debug.Log("[Api Response][" + url + "] - " + request.response.status + " " + request.response.Text);
				callback(true, request.response.Text);
			}else {

				printResponseLog(url, request.response);
				GDError error = GDError.buildError(request.response);
				GamedoniaBackend.INSTANCE.lastError = error;
				callback(false, request.response.message + ' ' + request.response.Text);
			}	
		}
			
		private static void printResponseLog(string url, Response response) {

			if (GamedoniaBackend.INSTANCE.debug) {
				String debugMsg = "[Api Response][" + url + "] - " + response.status;
				if (!String.IsNullOrEmpty(response.message)) debugMsg += " " + response.message;
				if (!String.IsNullOrEmpty(response.Text)) debugMsg += " " + response.Text;	
				Debug.Log(debugMsg);
			}

		}
		
		/*
		 * Security Support Methods
		 * 
		 */
				
		private static string Sign(String apiKey, String secret, String data, String contentType, String date, String requestMethod, String path) {					
			
			string contentMd5 = Md5(data);

			string toSign = requestMethod + "\n" + contentMd5 + "\n" + contentType + "\n" + date + "\n" + path;	
			
			string signature = HMACSHA1(secret, toSign);
			
			if (GamedoniaBackend.INSTANCE.debug) {
				Debug.Log("hmac:" + signature);
				Debug.Log("Md5: " + contentMd5);
			}
			return signature;
						
		}
		
		private static string Sign(String apiKey, String secret,String date, String requestMethod, String path) {					
			
			string toSign = requestMethod + "\n" + date + "\n" + path;	
			
			string signature = HMACSHA1(secret, toSign);
			
			if (GamedoniaBackend.INSTANCE.debug) {
				Debug.Log("hmac:" + signature);
			}
			return signature;
						
		}

		
		/**
		 * Reformating date to adjust to standard timezone formatting in other languages
		 */ 
		private static string GetCurrentDate() {
			
			string currentDate = String.Format(DATE_FORMAT,DateTime.Now);
			return currentDate.Remove(currentDate.Length-3,1);
		}
		
		private static string Md5(string input)
		{
			System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
			
			byte[] dataMd5 = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < dataMd5.Length; i++)
			{
				sBuilder.Append(dataMd5[i].ToString("x2"));
			}
			
			return sBuilder.ToString();
		}
		
		private static string HMACSHA1(string secret,string data)
		{
			System.Security.Cryptography.HMACSHA1 hmacsha1 = new System.Security.Cryptography.HMACSHA1(Encoding.Default.GetBytes(secret));
			
			byte [] dataHMAC = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(data));
			
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < dataHMAC.Length; i++)
			{
				sBuilder.Append(dataHMAC[i].ToString("x2"));
			}
			
			return sBuilder.ToString ();
		}

		public static string baseURL {
			
			get {
				return _baseURL;
			}
		}

		public static string version {

			get {
				return _version;
			}
		}
	}
}