using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace Gamedonia.Backend {

	public interface IGamedoniaSilent {

		string GetSilentId();

	}

	public class GamedoniaSilent {

		public static string GetSilentId() {

			IGamedoniaSilent silentImpl = null;
			if (Application.isEditor) {
				silentImpl = new GamedoniaDefaultSilentImpl();
			} else {
				silentImpl = new GamedoniaSilentImpl();
			}
		
			return silentImpl.GetSilentId ();
		}
	}

	/*
	 * Default Impl used always on Editor
	 */ 
	public class GamedoniaDefaultSilentImpl: IGamedoniaSilent {

		public string GetSilentId() {
			PhysicalAddress mac = GetCurrentMAC ("api.gamedonia.com");
			
			string silentId = "GDU_" + Convert.ToBase64String (mac.GetAddressBytes());		
			return silentId;
		}
		
		private static PhysicalAddress GetCurrentMAC(string allowedURL)
		{
			TcpClient client = new TcpClient();
			client.Client.Connect(new IPEndPoint(Dns.GetHostAddresses(allowedURL)[0], 80));
			while(!client.Connected)
			{
				Thread.Sleep(500);  
			}
			IPAddress address2 = ((IPEndPoint)client.Client.LocalEndPoint).Address;
			client.Client.Disconnect(false);
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			client.Close();
			if(allNetworkInterfaces.Length > 0)
			{
				foreach(NetworkInterface interface2 in allNetworkInterfaces)
				{
					UnicastIPAddressInformationCollection unicastAddresses = interface2.GetIPProperties().UnicastAddresses;
					if((unicastAddresses != null) && (unicastAddresses.Count > 0))
					{
						for(int i = 0; i < unicastAddresses.Count; i++)
							if(unicastAddresses[i].Address.Equals(address2))
								return interface2.GetPhysicalAddress();
					}
				}
			}
			return null;
		}
		
	}
	
	#if UNITY_IPHONE 
	
	public class GamedoniaSilentImpl: IGamedoniaSilent {

		[DllImport ("__Internal")]
		public static extern string GetPlatformDeviceId ();

		public string GetSilentId() {

			return GamedoniaSilentImpl.GetPlatformDeviceId();
			
		}

	}

	#elif UNITY_ANDROID

	public class GamedoniaSilentImpl: IGamedoniaSilent {

		private static string GetPlatformDeviceId () {
			AndroidJNI.AttachCurrentThread();

			AndroidJavaClass oudidManagerClass = new AndroidJavaClass("com.gamedonia.silent.SilentPlugin");						
			return oudidManagerClass.CallStatic<string>("getPlatformDeviceId",new object [] {});
		}

		public string GetSilentId() {
			return GamedoniaSilentImpl.GetPlatformDeviceId();
		}

	}
	
	#else

	public class GamedoniaSilentImpl: IGamedoniaSilent {

		public string GetSilentId() {
			return new GamedoniaDefaultSilentImpl ().GetSilentId ();	
		}

	}

	#endif
	
}
