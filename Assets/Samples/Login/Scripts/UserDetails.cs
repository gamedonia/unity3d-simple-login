using UnityEngine;
using System.Collections;

public class UserDetails : MonoBehaviour {

	
	public Texture2D backgroundImg;
	public GUISkin skin;	
	private string errorMsg = "";
	
	private GDUserProfile userProfile;
	
	void Start() {
		
		GamedoniaUsers.GetMe(OnGetMe);
	
	}
	
	void OnGUI () {
		
		GUI.skin = skin;
		
		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,25)),"Account Details:","LabelBold");
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,35,220,25)),"Nickname: ", "LabelSmallBold");
		
		if (userProfile != null && userProfile.profile["nickname"] != null) 
			GUI.Label(UtilResize.ResizeGUI(new Rect(170,35,220,25)), userProfile.profile["nickname"] as string, "LabelSmall");
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,60,220,25)),"Regis. Date: ", "LabelSmallBold");
		
		if (userProfile != null && userProfile.profile["registerDate"] != null) 
			GUI.Label(UtilResize.ResizeGUI(new Rect(170,60,220,25)), userProfile.profile["registerDate"] as string, "LabelSmall");
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,150, 220, 50)), "Logout")) {
			
			GamedoniaUsers.LogoutUser(OnLogout);
			
		}
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
		
	}
	
	void OnLogout(bool success) {
		
		if (success) {
			
			Application.LoadLevel("LoginScene");
			
		}else {
			
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);	
			
		}
		
	}
	
	void OnGetMe(bool success, GDUserProfile userProfile) {
		
		if (success) {
			this.userProfile = userProfile;
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
		
	}
}
