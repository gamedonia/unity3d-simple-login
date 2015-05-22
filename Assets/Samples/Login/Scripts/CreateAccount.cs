using UnityEngine;
using System.Collections;
using System;

public class CreateAccount : MonoBehaviour {

	public Texture2D backgroundImg;
	public GUISkin skin;
	
	private string errorMsg = "";
	private string email = "";
	private string password = "";
	private string repassword = "";
	private string nickname = "";
	
	void OnGUI () {
		
		GUI.skin = skin;

		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,20)),"eMail*","LabelBold");
		email = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 30, 220, 40)), email, 100);

		GUI.Label(UtilResize.ResizeGUI(new Rect(80,75,220,20)),"Password*","LabelBold");
		password = GUI.PasswordField (UtilResize.ResizeGUI(new Rect (80, 100, 220, 40)), password,'*');
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,145,200,20)),"Repeat Password*","LabelBold");
		repassword = GUI.PasswordField (UtilResize.ResizeGUI(new Rect (80, 170, 220, 40)), repassword,'*');
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,215,200,20)),"Nickname*","LabelBold");
		nickname = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 240, 220, 40)), nickname,25);
		

		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,290, 220, 50)), "Create")) {
			
			if ((email != "")
				&& (password != "")
				&& (repassword != "")
				&& (password == repassword)) {
			
				Credentials credentials = new Credentials();
				credentials.email = email.ToLower();
				credentials.password = password;
				GDUser user = new GDUser();
				user.credentials = credentials;
				user.profile.Add("nickname", nickname);
				user.profile.Add("registerDate", DateTime.Now);	
				
				GamedoniaUsers.CreateUser(user,OnCreateUser);
								
			}else {
				errorMsg = "Fill all the fields with (*) correctly";
				Debug.Log(errorMsg);				
			}
		}
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,345, 220, 50)), "Cancel")) {			
			Application.LoadLevel ("LoginScene");			
		}
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
				
	}
	
	void OnCreateUser(bool success) {
		
		if (success) {				
			GamedoniaUsers.LoginUserWithEmail(email.ToLower(),password,OnLogin);					
		}else {			
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
		
	}
	
	void OnLogin(bool success) {
		
		if (success) {			
			Application.LoadLevel("UserDetailsScene");			
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
		
	}
	
}
