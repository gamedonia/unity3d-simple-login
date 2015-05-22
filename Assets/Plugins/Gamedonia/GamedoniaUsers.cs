using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LitJson_Gamedonia;
using MiniJSON_Gamedonia;

public class GamedoniaUsers
{
	
	private static GDSessionToken sessionToken;
	public static GDUserProfile me;
	
	public GamedoniaUsers ()
	{
	}
	
	public static bool isLoggedIn() {
		return (sessionToken != null);
	}
	
	public static string GetSessionToken() {
		return sessionToken.session_token;
	}
	
	public static void Authenticate(Action<bool> callback) {
		Authenticate(Gamedonia.INSTANCE.AuthenticationMode, new Dictionary<string,object>(), callback);
	}
	
	public static void Authenticate(Gamedonia.CredentialsType authenticationType, Dictionary<string,object> credentials, Action<bool> callback) {
		
		IGamedoniaAuthentication authentication = null;
		switch (authenticationType) {
			case Gamedonia.CredentialsType.GAMECENTER:
				authentication = new GamecenterAuthentication();				
				break;
			case Gamedonia.CredentialsType.FACEBOOK:
				authentication = new FacebookAuthentication((string) credentials["fb_uid"], (string) credentials["fb_access_token"]);
				break;
			case Gamedonia.CredentialsType.SILENT:
				authentication = new SilentAuthentication();				
				break;
			default:
				authentication = new SessionTokenAuthentication();
				break;
		}
		
		authentication.Authenticate(callback);
	}
	
	public static void CreateUser(GDUser user, Action<bool> callback) {
		
		string json = JsonMapper.ToJson(user);		
				
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/create",json,
				delegate (bool success, object data) {
					if (callback!=null) callback(success);				
				}
		 	 )
		);
		
	}
	
	public static void LoginUserWithGameCenterId(string id, Action<bool> callback) {
		
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("gamecenter|" + id));
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("Authorization",auth);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/login",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {	
					if (success) {
						sessionToken = JsonMapper.ToObject<GDSessionToken>((string)data);
						PlayerPrefs.SetString("gd_session_token", sessionToken.session_token);
						RegisterDeviceAfterLogin(callback);
					}else {
						if (callback != null) callback(success);
					}
				}
		 	 )
		);
		
	}
	
	public static void LoginUserWithOpenUDID(string id, Action<bool> callback) {
						
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("silent|" + id));
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("Authorization",auth);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/login",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {								
					if (success) {
						sessionToken = JsonMapper.ToObject<GDSessionToken>((string)data);
						PlayerPrefs.SetString("gd_session_token", sessionToken.session_token);							
						RegisterDeviceAfterLogin(callback);						
					}else {
						if (callback != null) callback(success);	
					}	
									
				}
		 	 )
		);
		
	}

	private static void RegisterDeviceAfterLogin(Action<bool> callback) {

		if (Gamedonia.INSTANCE.IsDeviceRegisterNeeded()) {
			GamedoniaDevices.GetProfile(
				delegate (bool successDevProfile, GDDeviceProfile device) {
					if (successDevProfile) {
						switch(device.deviceType) {
							case "ios":
							case "android":	
								if (GamedoniaUsers.me == null) {								
									GamedoniaUsers.GetMe(
										delegate(bool success, GDUserProfile profile) {
											if (success) {
												GamedoniaDevices.device.uid = GamedoniaUsers.me._id;
												GamedoniaDevices.Register(device,
													delegate (bool successRegister) {
														if (callback != null) callback(successRegister);
													}
												);
											}else {
												if (callback != null) callback(success);
											}
										}
									);

								}else {
									GamedoniaDevices.device.uid = GamedoniaUsers.me._id;
									GamedoniaDevices.Register(device,
								    	delegate (bool successRegister) {
											if (callback != null) callback(successRegister);
										}
									);
								}
								break;
							default:
								if (callback != null) callback(successDevProfile);
								break;
						}
					} else {
						Debug.LogWarning("The device has not been registered due to an error");
						if (callback != null) callback(successDevProfile);
					}			
				}
			);
		}else {
			if (callback != null) callback(true);
		}
	}

	public static void LoginUserWithEmail(string email, string password, Action<bool> callback) {
			
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("email|" + email + "|" + password));
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("Authorization",auth);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/login",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {								
					if (success) {
						sessionToken = JsonMapper.ToObject<GDSessionToken>((string)data);
						PlayerPrefs.SetString("gd_session_token", sessionToken.session_token);
						RegisterDeviceAfterLogin(callback);
					}else {
						if (callback != null) callback(success);
					}
				}
		 	 )
		);
		
	}
	
	public static void LoginUserWithFacebook(string fbuid, string fbAccessToken, Action<bool> callback) {
		
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("facebook|" + fbuid + "|" + fbAccessToken));
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("Authorization",auth);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/login",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {								
					if (success) {
						sessionToken = JsonMapper.ToObject<GDSessionToken>((string)data);
						PlayerPrefs.SetString("gd_session_token", sessionToken.session_token);
						RegisterDeviceAfterLogin(callback);
					}else {
						if (callback != null) callback(success);					
					}
				}
		 	 )
		);
		
	}
	
	public static void LoginUserWithTwitter(string twuid, string twTokenSecret, string twToken, Action<bool> callback) {
		
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("twitter|" + twuid + "|" + twTokenSecret + "|" + twToken));
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add(GamedoniaRequest.GD_AUTH,auth);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/login",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {								
					if (success) {
						sessionToken = JsonMapper.ToObject<GDSessionToken>((string)data);
						PlayerPrefs.SetString("gd_session_token", sessionToken.session_token);
						RegisterDeviceAfterLogin(callback);
					}else {
						if (callback != null) callback(success);					
					}
				}
		 	 )
		);
		
	}
	
	public static void LoginUserWithSessionToken(Action<bool> callback) {

		string session_token = PlayerPrefs.GetString("gd_session_token");
		if (session_token != null && session_token.Length > 0) {
			string auth = System.Convert.ToBase64String (Encoding.UTF8.GetBytes ("session_token|" + session_token));

			Dictionary<string,string> body = new Dictionary<string, string> ();
			body.Add (GamedoniaRequest.GD_AUTH, auth);
			Gamedonia.RunCoroutine (
			GamedoniaRequest.post ("/account/login", JsonMapper.ToJson (body), auth, null, null,
				delegate (bool success, object data) {								
							if (success) {
									sessionToken = JsonMapper.ToObject<GDSessionToken> ((string)data);
									RegisterDeviceAfterLogin (callback);
							} else {
									if (callback != null)
											callback (success);					
							}
					}
				)
			);
		} else {
			Debug.LogWarning("No sessionToken stored in PlayerPrefs");
			if (callback!=null) callback(false);
		}
		
	}
	
	public static void LogoutUser(Action<bool> callback) {
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add(GamedoniaRequest.GD_SESSION_TOKEN, sessionToken.session_token);
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/logout",JsonMapper.ToJson(body),null,sessionToken.session_token,null,
				delegate (bool success, object data) {													
					if (callback != null) callback(success);					
				}
		 	 )
		);
		
	}
	
	public static void GetUser(string userId, Action<bool, GDUserProfile> callback) {
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("_id",userId);
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/retrieve",JsonMapper.ToJson(body),null,sessionToken.session_token,null,
				delegate (bool success, object data) {
					GDUserProfile user = null;
					if (success) user = DeserializeUserProfile((string)data);
					if (callback != null) callback(success, user);					
				}
		 	 )
		);
		
	}
	
	public static void GetMe(Action<bool, GDUserProfile> callback) {
				
		Gamedonia.RunCoroutine(
			GamedoniaRequest.get("/account/me",sessionToken.session_token,
				delegate (bool success, object data) {
					//if (success) me = JsonMapper.ToObject<GDUserProfile>((string)data);
					if (success) me = DeserializeUserProfile((string)data);
					if (callback != null) callback(success, me);					
				}
		 	 )
		);
		
	}

	public static void LinkUser(Credentials credentials, Action<bool, GDUserProfile> callback) {

		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/link", JsonMapper.ToJson(credentials), null, sessionToken.session_token, null,
				delegate (bool success, object data) {
					if (success) me = DeserializeUserProfile((string)data);
					if (callback != null) callback(success, me);					
				}
			)
		);
		
	}
	
	public static void UpdateUser(Dictionary<string,object> profile, Action<bool> callback = null, bool overwrite = false) {
		
		if (!overwrite) {
			Gamedonia.RunCoroutine(
				GamedoniaRequest.post("/account/update",JsonMapper.ToJson(profile),null,sessionToken.session_token,null,
					delegate (bool success, object data) {	
						if (success) me = DeserializeUserProfile((string)data);
						if (callback != null) callback(success);					
					}
			 	 )
			);
		}else {
			Gamedonia.RunCoroutine(
				GamedoniaRequest.put("/account/update",JsonMapper.ToJson(profile),null,sessionToken.session_token,null,
					delegate (bool success, object data) {	
						if (success) me = DeserializeUserProfile((string)data);
						if (callback != null) callback(success);					
					}
			 	 )
			);			
		}
		
	}
	
	public static void ChangePassword(string email, string currentPassword, string newPassword, Action<bool> callback) {
		
		string auth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes("email|" + email + "|" + currentPassword));
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("password",newPassword);
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/password/change",JsonMapper.ToJson(body),auth,null,null,
				delegate (bool success, object data) {					
					if (callback != null) callback(success);					
				}
		 	 )
		);	
		
	}
	
	public static void ResetPassword(string email, Action<bool> callback) {
				
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("email",email);
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/password/reset",JsonMapper.ToJson(body),null,null,null,
				delegate (bool success, object data) {					
					if (callback != null) callback(success);					
				}
		 	 )
		);	
		
	}
	
	public static void RestorePassword(string restoreToken, string newPassword, Action<bool> callback) {
		
		Dictionary<string,string> body = new Dictionary<string, string>();
		body.Add("password",newPassword);
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.post("/account/password/restore/" + restoreToken,JsonMapper.ToJson(body),null,sessionToken.session_token,null,
				delegate (bool success, object data) {					
					if (callback != null) callback(success);					
				}
		 	 )
		);	
		
	}
	
	
	public static void Search(string query, Action<bool, IList> callback = null) {
		Search(query, 0, null, 0, callback);
	}
	
	public static void Search(string query, int limit=0, string sort=null, int skip=0, Action<bool, IList> callback = null) {
		
		string url = "/account/search?query=" + query;
		if (limit>0) url += "&limit="+limit;
		if (sort!=null) url += "&sort=" + sort;
		if (skip>0) url += "&skip="+skip;
		
		Gamedonia.RunCoroutine(
			GamedoniaRequest.get(url,sessionToken.session_token,
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
		
	public static void Count(string query, Action<bool, int> callback = null) {

		string url = "/account/count?query=" + query;

		Gamedonia.RunCoroutine(
			GamedoniaRequest.get(url, sessionToken.session_token, delegate (bool success, object data)
			{
				if (callback != null) {
					if (success) {
						IDictionary response = Json.Deserialize((string)data) as IDictionary;
						int count = int.Parse(response["count"].ToString());
						callback(success, count);
					} else {
						callback(success, -1);
					}
				}
			})
		);
	}


	private static GDUserProfile DeserializeUserProfile (string data) {

		Debug.Log ("Deserializing user");
		IDictionary userMap = Json.Deserialize((string)data) as IDictionary;
		GDUserProfile user = new GDUserProfile ();

		user._id = userMap["_id"] as string;
		user.profile = userMap["profile"] as Dictionary<string,object>;

		return user;
	}
}

public class GDUserProfile {
	
	public string _id;
	public Dictionary<string,object> profile = new Dictionary<string,object>();
	
}

public class GDUser {
	
	public string _id;
	public Credentials credentials;		
	public Dictionary<string,object> profile = new Dictionary<string,object>();
	
	public GDUser ()
	{
	}
}

public class GDUserConstants {
	
	public const string SILENT 		= "silent";
	public const string MAIL 		= "mail";
	public const string FACEBOOK 	= "facebook";
	public const string TWITTER 	= "twitter";
	public const string GAMECENTER 	= "gamecenter";
	
}

public class Credentials {
	
	
	public string space;
	private string _email;
	public string email {
		set {
			this._email = value;
			if (type.IndexOf(GDUserConstants.MAIL) == -1) type.Add(GDUserConstants.MAIL);
		}
		
		get {
			return this._email;
		}
	}	
	public string password;
	
	
	public string fb_access_token;	
	private string _fb_uid;
	public string fb_uid {
		set {
			this._fb_uid = value;
			if (type.IndexOf(GDUserConstants.FACEBOOK) == -1) type.Add(GDUserConstants.FACEBOOK);
		}
		
		get {
			return this._fb_uid;
		}
	}
	
	
	public string tw_token_secret;
	public string tw_token;	
	private string _tw_uid;
	public string tw_uid {		
		set {
			this._tw_uid = value;
			if (type.IndexOf(GDUserConstants.TWITTER) == -1) type.Add(GDUserConstants.TWITTER);
		}
		
		get {
			return this._tw_uid;
		}
	}
	
	private string _open_udid;
	public string open_udid {		
		set {
			this._open_udid = value;
			if (type.IndexOf(GDUserConstants.SILENT) == -1) type.Add(GDUserConstants.SILENT);
		}
		
		get { return this._open_udid;}
	}
	
	private string _gamecenter_id;
	public string gamecenter_id {		
		
        set 
		{ 
			this._gamecenter_id = value; 
			if (type.IndexOf(GDUserConstants.GAMECENTER) == -1) type.Add(GDUserConstants.GAMECENTER);
		}
        
        get { return this._gamecenter_id; }
	}
	
	
	public List<string> type = new List<string>();
	
}


public class GDSessionToken {

	public string session_token;
	public string expiration_date;
}


/*
 * Authentication modes
 */ 
interface IGamedoniaAuthentication
{
    void Authenticate(Action<bool> callback);
}

public class GamecenterAuthentication:IGamedoniaAuthentication {
	
	private Action<bool> callback;
	
	public void Authenticate(Action<bool> callback) {
		if (Gamedonia.INSTANCE.debug) Debug.Log("GameCenter Authentication");
		this.callback = callback;
		Social.localUser.Authenticate(ProcessAuthentication);
	}
	
	void ProcessAuthentication (bool success) {
		
		GDUser user = new GDUser();
		Credentials credentials = new Credentials();
		credentials.open_udid = OpenUDIDPlugin.GetOpenUDID();
		user.credentials = credentials;
		
        if (success) credentials.gamecenter_id = Social.localUser.id;			                        	
        	
		GamedoniaUsers.CreateUser(user, ProcessCreateUser);	
		
    }
	
	void ProcessCreateUser(bool success) {
		
		
		if (Social.localUser.authenticated) {
			//Login with gamecenter id
			GamedoniaUsers.LoginUserWithGameCenterId(Social.localUser.id, ProcessLogin);
		}else {
			//Login with open_udid
			GamedoniaUsers.LoginUserWithOpenUDID(OpenUDIDPlugin.GetOpenUDID(), null);
		}
		
		
	}
	
	void ProcessLogin(bool success) {		
		if (success) {
			GamedoniaUsers.GetMe(HandleGetMe);
		}else {
			Debug.LogError("Gamedonia session couldn't be started!");
			if (this.callback != null) callback(false);
		}
	}
	
	void HandleGetMe(bool success, GDUserProfile profile) {
		if (this.callback != null) callback(success);
	}
	
}

public class SilentAuthentication:IGamedoniaAuthentication {
	
	private Action<bool> callback;
	
	public void Authenticate(Action<bool> callback) {
		if (Gamedonia.INSTANCE.debug) Debug.Log("Silent Authentication");
		this.callback = callback;
		
		GDUser user = new GDUser();
		Credentials credentials = new Credentials();
		credentials.open_udid = OpenUDIDPlugin.GetOpenUDID();
		user.credentials = credentials;                        	
        	
		GamedoniaUsers.CreateUser(user, ProcessCreateUser);	
	}
	
	void ProcessCreateUser(bool success) {
		
		//Login with open_udid
		GamedoniaUsers.LoginUserWithOpenUDID(OpenUDIDPlugin.GetOpenUDID(), ProcessLogin);

	}
	
	void ProcessLogin(bool success) {		
		if (success) {
			//Debug.Log("Process Login");
			if (GamedoniaUsers.me == null) {
				GamedoniaUsers.GetMe(HandleGetMe);
			}else {
				if (this.callback != null) callback(success);
			}
		}else {
			Debug.LogError("Gamedonia session couldn't be started!");
			if (this.callback != null) callback(false);
		}
	}
	
	void HandleGetMe(bool success, GDUserProfile profile) {
		if (this.callback != null) callback(success);
	}
	
}

public class FacebookAuthentication:IGamedoniaAuthentication {
	
	private Action<bool> _callback;
	private string _fb_uid;
	private string _fb_access_token;
	
	public FacebookAuthentication (string fb_uid, string fb_access_token) {
		_fb_uid = fb_uid;
		_fb_access_token = fb_access_token;
	}
	
	public void Authenticate(Action<bool> callback) {
		if (Gamedonia.INSTANCE.debug) Debug.Log("Facebook Authentication");
		_callback = callback;
		
		if (!String.IsNullOrEmpty(_fb_uid) && !String.IsNullOrEmpty(_fb_access_token)) {
			GDUser user = new GDUser();
			Credentials credentials = new Credentials();
			credentials.fb_uid = _fb_uid;
			credentials.fb_access_token = _fb_access_token;
			user.credentials = credentials; 
			
			GamedoniaUsers.CreateUser(user, ProcessCreateUser);	
		} else {
			Debug.LogError("Facebook id or token not present impossible to perform login with it");
			if (_callback != null) _callback(false);
		}	
		
	}
	
	void ProcessCreateUser(bool success) {
		
		//Login with open_udid
		GamedoniaUsers.LoginUserWithFacebook(_fb_uid, _fb_access_token, ProcessLogin);

	}	
	
	void ProcessLogin(bool success) {		
		if (success) {
			if (GamedoniaUsers.me == null) {
				GamedoniaUsers.GetMe(HandleGetMe);
			}else {
				if (this._callback != null) _callback(success);
			}
		}else {
			Debug.LogError("Gamedonia session couldn't be started!");
			if (_callback != null) _callback(false);
		}
	}
	
	void HandleGetMe(bool success, GDUserProfile profile) {
		if (_callback != null) _callback(true);
	}
	
}

public class SessionTokenAuthentication:IGamedoniaAuthentication {
	
	private Action<bool> callback;
	
	public void Authenticate(Action<bool> callback) {
		if (Gamedonia.INSTANCE.debug) Debug.Log("Session Token Authentication");
		this.callback = callback;
		string session_token = PlayerPrefs.GetString("gd_session_token");
		if (!String.IsNullOrEmpty(session_token)) {
			GamedoniaUsers.LoginUserWithSessionToken(ProcessLogin);
		}else {
			Debug.LogError("Session Token not present impossible to perform login with it");
			if (this.callback != null) callback(false);
		}
	}
	
	void ProcessLogin(bool success) {		
		if (success) {
			if (GamedoniaUsers.me == null) {
				GamedoniaUsers.GetMe(HandleGetMe);
			}else {
				if (this.callback != null) callback(success);
			}
		}else {
			Debug.LogError("Gamedonia session couldn't be started!");
			if (this.callback != null) callback(false);
		}
	}
	
	void HandleGetMe(bool success, GDUserProfile profile) {
		if (this.callback != null) callback(true);
	}
	
}


	