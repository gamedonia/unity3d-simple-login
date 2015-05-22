using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;


public class GamedoniaData  {
	
	public GamedoniaData () {
		
	}
	
	public static void Create(string collection, Dictionary<string,object> entity, Action<bool, IDictionary> callback = null) {
		
		string json = JsonMapper.ToJson(entity);
				
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/data/" + collection + "/create",json, null, GamedoniaUsers.GetSessionToken(), null,
				delegate (bool success, object data) {
					if (callback!=null) {
						if (success) {
							callback(success,Json.Deserialize((string)data) as IDictionary);
						}else {
							callback(success,null);
						}
						
					}					
				}
		 	 )
		);
	}
	
	public static void Delete(string collection, string entityId, Action<bool> callback = null) {
		Gamedonia.RunCoroutine(
			GamedoniaRequest.delete("/data/" + collection + "/delete/" + entityId, GamedoniaUsers.GetSessionToken(),
				delegate (bool success, object data) {					
					callback(success);
				}
		 	 )
		);
	}

	public static void Delete(string collection, List<string> entities, bool all, Action<bool,int> callback = null) {

		string sessionToken = null;
		if (GamedoniaUsers.isLoggedIn ())
						sessionToken = GamedoniaUsers.GetSessionToken ();
		//string sessionToken = 

		if (all) {

			Gamedonia.RunCoroutine(
				GamedoniaRequest.delete("/data/" + collection + "/delete?all=true", sessionToken,
			        delegate (bool success, object data) {	
						IDictionary response = Json.Deserialize((string)data) as IDictionary;		
						if (success) {
						callback(success, int.Parse(response["deleted"].ToString()));
						} else {
							callback(success, 0);
						}
					}
				)
			);

		}else {

			Gamedonia.RunCoroutine(
				GamedoniaRequest.delete("/data/" + collection + "/delete?keys=" + String.Join(",",entities.ToArray()), sessionToken,
			        delegate (bool success, object data) {
						IDictionary response = Json.Deserialize((string)data) as IDictionary;
						if (success) {
							callback(success, int.Parse(response["deleted"].ToString()));
						} else {
							callback(success, 0);
						}
					}
				)
			);

		}
	}
	
	public static void Update (string collection, Dictionary<string,object> entity, Action<bool, IDictionary> callback = null, bool overwrite = false) {
		
		string json = JsonMapper.ToJson(entity);
		
		if (!overwrite) {
			Gamedonia.RunCoroutine(
					GamedoniaRequest.post("/data/" + collection + "/update", json , null, GamedoniaUsers.GetSessionToken(), null,
						delegate (bool success, object data) {												
							if (callback!=null) {
								if (success) {
									callback(success,Json.Deserialize((string)data) as IDictionary);
								}else {
									callback(success,null);
								}							
							}	
						}
				 	 )
			);
		}else {
			Gamedonia.RunCoroutine(
					GamedoniaRequest.put("/data/" + collection + "/update", json , null, GamedoniaUsers.GetSessionToken(), null,
						delegate (bool success, object data) {					
							if (callback!=null) {
								if (success) {
									callback(success,Json.Deserialize((string)data) as IDictionary);
								}else {
									callback(success,null);
								}							
							}				
						}
				 	 )
			);	
		}
	}	
	
	public static void Get (string collection, string entityId, Action<bool, IDictionary> callback = null) {
		
		Gamedonia.RunCoroutine(
				GamedoniaRequest.get("/data/" + collection + "/get/" + entityId, GamedoniaUsers.GetSessionToken(),
					delegate (bool success, object data) {					
						if (callback!=null) {
							if (success) {
								callback(success,Json.Deserialize((string)data) as IDictionary);
							}else {
								callback(success,null);
							}							
						}
					}
			 	 )
			);		
	}
	
	public static void Search(string collection, string query, Action<bool, IList> callback = null) {
		 Search(collection,query,0,null,0,callback);
	}
	
	public static void Search(string collection, string query, int limit, Action<bool, IList> callback = null) {
		Search(collection,query,limit,null,0,callback);
	}
	
	public static void Search(string collection, string query, int limit, string sort, Action<bool, IList> callback = null) {
		Search(collection,query,limit,sort,0,callback);
	}
		
	public static void Search(string collection, string query, int limit=0, string sort=null, int skip=0, Action<bool, IList> callback = null) {
		
		query= Uri.EscapeDataString(query);
		string url = "/data/"+ collection + "/search?query=" + query;
		if (limit>0) url += "&limit="+limit;
		if (sort!=null) url += "&sort=" + sort;
		if (skip>0) url += "&skip="+skip;
			
		Gamedonia.RunCoroutine(
			GamedoniaRequest.get(url, GamedoniaUsers.GetSessionToken(), 
				delegate (bool success, object data) {								
					if (callback!=null) {
						if (success) {
							if ((data==null) ||(data.Equals("[]")) )
								callback(success, null);
							else {
								callback(success,Json.Deserialize((string)data) as IList);
							}	
						}else {
							callback(success,null);
						}							
					}
				}
		 	 )
		);
	}

	public static void Count(string collection, string query, Action<bool, int> callback = null)
	{
		query= Uri.EscapeDataString(query);
		string url = "/data/"+ collection + "/count?query=" + query;
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.get(url, GamedoniaUsers.GetSessionToken(),
		    	delegate (bool success, object data) {
					if (callback != null) {
						if (success) {
							IDictionary response = Json.Deserialize((string)data) as IDictionary;
							int count = int.Parse(response["count"].ToString());
							callback(success, count);
						} else {
							callback(success, -1);
						}
					}
				}
			)
		);
	}

}
