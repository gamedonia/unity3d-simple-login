using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System;

public class Login : MonoBehaviour {

	public Texture2D backgroundImg;
	public GUISkin skin;

	private string email = "";
	private string password = "";
	private string errorMsg = "";
	private string statusMsg = "";

	private const string FB_APP_ID = "";
	private static string [] READ_PERMISSIONS = new string[] {"read_stream", "read_friendlists"};

	private string fbUserName = null;
	private string fbUserId = null;

	void Awake() {

		FacebookBinding.Init(FB_APP_ID);
	}

	void Start() {

		if (  Gamedonia.INSTANCE== null) {

			statusMsg = "Missing Api Key/Secret. Check the README.txt for more info.";
		}
	}

	void OnGUI () {

		GUI.skin = skin;

		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);

		GUI.enabled = (statusMsg == "");

		// Login Controls
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,20)),"eMail","LabelBold");
		email = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 30, 220, 40)), email, 100);

		GUI.Label(UtilResize.ResizeGUI(new Rect(80,75,220,20)),"Password", "LabelBold");
		password = GUI.PasswordField (UtilResize.ResizeGUI(new Rect (80, 100, 220, 40)), password,'*');


		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,150, 220, 50)), "Login")) {
			GamedoniaUsers.LoginUserWithEmail(email.ToLower(),password,OnLogin);
		}

		GUIStyle fbButton = GUI.skin.GetStyle ("ButtonFacebook");

		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,205, 220, 50)), "Facebook",fbButton)) {
			if (!Application.isEditor) {
				statusMsg = "Initiating Facebook session...";
				FacebookBinding.OpenSessionWithReadPermissions(READ_PERMISSIONS, OnFacebookOpenSession);
			}
			else {
				errorMsg = "Facebook won't work on Unity Editor, try it on a device.";
			}
		}

		GUIStyle separator = GUI.skin.GetStyle ("separator");
		GUI.Box(UtilResize.ResizeGUI(new Rect(80, 277, 220, 1)), "",separator);

		// Sign Up
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,300, 220, 50)), "Sign Up")) {
			//print ("you clicked the text button");
			Application.LoadLevel("CreateAccountScene");
		}

		// Password Recovery
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,355, 220, 50)), "Remember Password")) {

			Application.LoadLevel("ResetPasswordScene");
		}

		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}

		GUI.enabled = true;
		if (statusMsg != "") {
			GUI.Box (UtilResize.ResizeGUI(new Rect (80, 240 - 40, 220, 40)), statusMsg);
		}
	}


	void OnFacebookOpenSession(bool success, bool userCancelled, string message) {

		if (success) {
			statusMsg = "Recovering Facebook profile...";
			FacebookBinding.RequestWithGraphPath("/me",null,"GET",OnFacebookMe);
		}else {
			errorMsg = "Unable to open session with Facebook";
		}
	}

	void OnLogin (bool success) {

		statusMsg = "";
		if (success) {
			Application.LoadLevel("UserDetailsScene");
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}

	}


	void OnFacebookLogin (bool success) {

		if (success) {

			// Optional stuff if you want to store the Facebook username inside the Gamedonia user profile
			Dictionary<string,object> profile = new Dictionary<string, object>();
			profile.Add("nickname", fbUserName);
			profile.Add("registerDate", DateTime.Now);
			GamedoniaUsers.UpdateUser(profile, OnLogin);

		} else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}

	}

	private void OnFacebookMe(IDictionary data) {

		statusMsg = "Initiating Gamedonia session...";
		fbUserId = data ["id"] as string;
		fbUserName = data ["name"] as string;
		Debug.Log ("AccessToken: " + FacebookBinding.GetAccessToken() + " fbuid: " + fbUserId);


		Dictionary<string,object> facebookCredentials = new Dictionary<string,object> ();
		facebookCredentials.Add("fb_uid",fbUserId);
		facebookCredentials.Add("fb_access_token",FacebookBinding.GetAccessToken());

		GamedoniaUsers.Authenticate (Gamedonia.CredentialsType.FACEBOOK,facebookCredentials, OnFacebookLogin);

	}


}
