using UnityEngine;
using System.Collections;


	public class UtilResize
	{
		public static Rect ResizeGUI(Rect _rect)
		{
		    float FilScreenWidth = _rect.width / 320;
		    float rectWidth = FilScreenWidth * Screen.width;
		    float FilScreenHeight = _rect.height / 480;
		    float rectHeight = FilScreenHeight * Screen.height;
		    float rectX = (_rect.x / 320) * Screen.width;
		    float rectY = (_rect.y / 480) * Screen.height;
	 
	    	return new Rect(rectX,rectY,rectWidth,rectHeight);
		}
	
		public static int resMultiplier() {
		
			return Screen.width / 320;
			
		}
		
	}


