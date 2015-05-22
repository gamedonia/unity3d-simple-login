using UnityEngine;
using System.Collections;

public class ResetPassword : MonoBehaviour {

	public Texture2D backgroundImg;
	public GUISkin skin;
	
	private string email = "";
	private string errorMsg = "";
	
	void OnGUI () {
		
		GUI.skin = skin;
		
		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,20)),"eMail*","LabelBold");
		email = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 30, 220, 40)), email, 50);
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,80, 220, 50)), "Reset Password")) {
			
			if (email != "") {			
				GamedoniaUsers.ResetPassword(email, OnResetPassword);				
			}else {
				errorMsg = "eMail field can't be empty";
				Debug.Log(errorMsg);
			}
		}
		
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,140, 220, 50)), "Back")) {

			Application.LoadLevel("LoginScene");		
		}
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
				
	}
	
	void OnResetPassword(bool success) {
		
		if (success) {		
			errorMsg = "Password reset successfully, please check your email for instructions on how to complete the process.";
			Debug.Log(errorMsg);
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
	}
}
