using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;




namespace HTTP
{
	public class HTTPException : Exception
	{
		public HTTPException (string message) : base(message)
		{
		}
	}
	
	public enum RequestState {
		Waiting, Reading, Done	
	}

	public class Request
	{
		public string method = "GET";
		public string protocol = "HTTP/1.1";
		public byte[] bytes;
		public Uri uri;
		public static byte[] EOL = { (byte)'\r', (byte)'\n' };
		public Response response = null;
		public bool isDone = false;
		public int maximumRetryCount = 8;
		public bool acceptGzip = true;
		public bool useCache = false;
		public Exception exception = null;
		public RequestState state = RequestState.Waiting;
		public bool isWriting = false;
		public long lastWrite;

		public RequestDelegate downloadDelegate;
		
		Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> ();
		static Dictionary<string, string> etags = new Dictionary<string, string> ();

		public Request (string method, string uri)
		{
			this.method = method;
			this.uri = new Uri (uri);
		}

		public Request (string method, string uri, bool useCache)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.useCache = useCache;
		}

		public Request (string method, string uri, byte[] bytes)
		{
			this.method = method;
			this.uri = new Uri (uri);
			this.bytes = bytes;
		}

		public void AddHeader (string name, string value)
		{
			name = name.ToLower ().Trim ();
			value = value.Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			headers[name].Add (value);
		}

		public string GetHeader (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				return "";
			return headers[name][0];
		}

		public List<string> GetHeaders (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			return headers[name];
		}

		public void SetHeader (string name, string value)
		{
			name = name.ToLower ().Trim ();
			value = value.Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			headers[name].Clear ();
			headers[name].Add (value);
		}


		#if UNITY_STANDALONE
		public void Send (string filePath = null, bool onlyContentLength = false)
		{
			isDone = false;
			state = RequestState.Waiting;
			if (acceptGzip)
				SetHeader ("Accept-Encoding", "gzip");
			//ThreadPool.QueueUserWorkItem (new WaitCallback (delegate(object t) {
				try {
					var retry = 0;
					while (++retry < maximumRetryCount) {
						if (useCache) {
							string etag = "";
							if (etags.TryGetValue (uri.AbsoluteUri, out etag)) {
								SetHeader ("If-None-Match", etag);
							}
						}
						SetHeader ("Host", uri.Host);

						#if UNITY_WEBPLAYER
						/*
						string ip = Dns.GetHostAddresses(uri.Host)[0].ToString();
						if (!Security.PrefetchSocketPolicy (ip, 1843)) {							
							Debug.LogError("Security Exception. Policy file load failed!");							
						}*/								
						#endif
						//Init the response
						if (filePath != null) {
							response = new Response(filePath, onlyContentLength);
						}else {
							response = new Response ();
						}

						var client = new TcpClient ();


						client.SendTimeout = 5000;
						client.ReceiveTimeout = 5000;

						client.Connect (uri.Host, uri.Port);
						 
						using (var stream = client.GetStream ()) {
							var ostream = stream as Stream;
							if (uri.Scheme.ToLower() == "https") {
								ostream = new SslStream (stream, false, new RemoteCertificateValidationCallback (ValidateServerCertificate));
								try {
									var ssl = ostream as SslStream;
									ssl.AuthenticateAsClient (uri.Host);
								} catch (Exception e) {
									Debug.LogError ("Exception: " + e.Message);
									return;
								}
							}

							WriteToStream (ostream);

							if (downloadDelegate != null) response.downloadDelegate = downloadDelegate;

							state = RequestState.Reading;
							response.ReadFromStream(ostream);
						}
						client.Close ();
						switch (response.status) {
						case 307:
						case 302:
						case 301:
							uri = new Uri (response.GetHeader ("Location"));
							continue;
						default:
							retry = maximumRetryCount;
							break;
						}
					}
					if (useCache) {
						string etag = response.GetHeader ("etag");
						if (etag.Length > 0)
							etags[uri.AbsoluteUri] = etag;
					}
				

				} catch (SocketException e) {
					response.status = -100;
					response.message = "Failed";
					if (downloadDelegate != null) {
						IOErrorEvent ioErrorEvent = new IOErrorEvent();
						ioErrorEvent.ErrorCode = e.ErrorCode;
						ioErrorEvent.Message = e.Message;
						downloadDelegate.OnIOError(ioErrorEvent);
					}
					Debug.LogException(e);
				} catch (IOException ioex) {
					response.status = -100;
					response.message = "Failed";
					if (downloadDelegate != null) {
						IOErrorEvent ioErrorEvent = new IOErrorEvent();
						ioErrorEvent.Message = ioex.Message;
						downloadDelegate.OnIOError(ioErrorEvent);
					}

					Debug.LogException(ioex);
				} catch (Exception e) {
					Console.WriteLine ("Unhandled Exception, aborting request.");
					Console.WriteLine (e);
					Debug.Log(e.Message + " Type: " + e.GetType().FullName);
					exception = e;
					response = null;
				}

				//Debug.Log ("Request finished");
				state = RequestState.Done;
				isDone = true;
			//}));		
		}

		#else
		public void Send (string filePath = null, bool onlyContentLength = false)
		{
			isDone = false;
			state = RequestState.Waiting;
			if (acceptGzip)
				SetHeader ("Accept-Encoding", "gzip");
			ThreadPool.QueueUserWorkItem (new WaitCallback (delegate(object t) {
				try {
					var retry = 0;
					while (++retry < maximumRetryCount) {
						if (useCache) {
							string etag = "";
							if (etags.TryGetValue (uri.AbsoluteUri, out etag)) {
								SetHeader ("If-None-Match", etag);
							}
						}
						SetHeader ("Host", uri.Host);
						
						#if UNITY_WEBPLAYER
						/*
						string ip = Dns.GetHostAddresses(uri.Host)[0].ToString();
						if (!Security.PrefetchSocketPolicy (ip, 1843)) {							
							Debug.LogError("Security Exception. Policy file load failed!");							
						}	
						*/
						#endif

						//Init the response
						if (filePath != null) {
							response = new Response(filePath, onlyContentLength);
						}else {
							response = new Response ();
						}

						var client = new TcpClient ();
												
						client.SendTimeout = 5000;
						client.ReceiveTimeout = 5000;
						
						client.Connect (uri.Host, uri.Port);
						
						using (var stream = client.GetStream ()) {
							var ostream = stream as Stream;
							if (uri.Scheme.ToLower() == "https") {
								ostream = new SslStream (stream, false, new RemoteCertificateValidationCallback (ValidateServerCertificate));
								try {
									var ssl = ostream as SslStream;
									ssl.AuthenticateAsClient (uri.Host);
								} catch (Exception e) {
									Debug.LogError ("Exception: " + e.Message);
									return;
								}
							}
							
							WriteToStream (ostream);

							
							if (downloadDelegate != null) response.downloadDelegate = downloadDelegate;
							
							state = RequestState.Reading;
							response.ReadFromStream(ostream);
						}
						client.Close ();
						switch (response.status) {
						case 307:
						case 302:
						case 301:
							uri = new Uri (response.GetHeader ("Location"));
							continue;
						default:
							retry = maximumRetryCount;
							break;
						}
					}
					if (useCache) {
						string etag = response.GetHeader ("etag");
						if (etag.Length > 0)
							etags[uri.AbsoluteUri] = etag;
					}
					
					
				} catch (SocketException e) {
					response.status = -100;
					response.message = "Failed";
					if (downloadDelegate != null) {
						IOErrorEvent ioErrorEvent = new IOErrorEvent();
						ioErrorEvent.ErrorCode = e.ErrorCode;
						ioErrorEvent.Message = e.Message;
						downloadDelegate.OnIOError(ioErrorEvent);
					}
					Debug.LogException(e);
				} catch (IOException ioex) {
					response.status = -100;
					response.message = "Failed";
					if (downloadDelegate != null) {
						IOErrorEvent ioErrorEvent = new IOErrorEvent();
						ioErrorEvent.Message = ioex.Message;
						downloadDelegate.OnIOError(ioErrorEvent);
					}
					
					Debug.LogException(ioex);
				} catch (Exception e) {
					Console.WriteLine ("Unhandled Exception, aborting request.");
					Console.WriteLine (e);
					Debug.Log(e.Message + " Type: " + e.GetType().FullName);
					exception = e;
					response = null;
				}
				
				//Debug.Log ("Request finished");
				state = RequestState.Done;
				isDone = true;
			}));		
		}

		#endif



		
		public string Text {
			set { bytes = System.Text.Encoding.UTF8.GetBytes (value); }
		}


		public static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Debug.LogWarning ("SSL Cert Error:" + sslPolicyErrors.ToString ());
			return true;
		}

		void WriteToStream (Stream outputStream)
		{
			isWriting = true;

			var stream = new BinaryWriter (outputStream);
			stream.Write (ASCIIEncoding.ASCII.GetBytes (method.ToUpper () + " " + uri.PathAndQuery + " " + protocol));
			stream.Write (EOL);
			foreach (string name in headers.Keys) {
				foreach (string value in headers[name]) {
					stream.Write (ASCIIEncoding.ASCII.GetBytes (name));
					stream.Write (':');
					stream.Write (ASCIIEncoding.ASCII.GetBytes (value));
					stream.Write (EOL);
				}
			}
			if (bytes != null && bytes.Length > 0) {
				if (GetHeader ("Content-Length") == "") {
					stream.Write (ASCIIEncoding.ASCII.GetBytes ("content-length: " + bytes.Length.ToString ()));
					stream.Write (EOL);
					stream.Write (EOL);
				}
				stream.Write (bytes);
			} else {
				stream.Write (EOL);
			}
			isWriting = false;
		}

		public void Stop() {

			//Debug.Log ("Stop");

			if (response != null) {
				//Debug.Log("Request Stop");
				response.Stop ();
			}
			
		}
	}


	

	public class ProgressEvent
	{
		public long bytesLoaded;
		public long bytesTotal;
	}

	public class IOErrorEvent
	{
		public int ErrorCode;
		public string Message;
	}

	public interface RequestDelegate {

		void OnResponseHeaders(ProgressEvent pEvent);
		void OnProgress (ProgressEvent pEvent);
		void OnIOError (IOErrorEvent ioError);
	}
	
}

