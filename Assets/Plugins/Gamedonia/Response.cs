using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;

namespace HTTP
{



	public class Response
	{

		private const long PROGRESS_CHUNK_SIZE = 1000000;

		public int status = 200;
		public string message = "OK";
		public byte[] bytes;
		public long contentLength = -1;
		public string outputFilePath;
		public long downloadedBytes = 0;
		private long progressDownloadedBytes = 0;
		private bool forceStop = false;
		private bool onlyContentLength = false;


		public RequestDelegate downloadDelegate;

		List<byte[]> chunks;
		Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> ();

		public string Text {
			get {
				if (bytes == null)
					return "";
				return System.Text.UTF8Encoding.UTF8.GetString (bytes);
			}
		}

		public string Asset {
			get {
				throw new NotSupportedException ("This can't be done, yet.");
			}
		}

		void AddHeader (string name, string value)
		{
			name = name.ToLower ().Trim ();
			value = value.Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			headers[name].Add (value);
		}

		public List<string> GetHeaders (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				headers[name] = new List<string> ();
			return headers[name];
		}

		public string GetHeader (string name)
		{
			name = name.ToLower ().Trim ();
			if (!headers.ContainsKey (name))
				return string.Empty;
			return headers[name][headers[name].Count - 1];
		}

		public Response ()
		{
			//ReadFromStream (stream);
		}

		public Response(string filePath, bool onlyContentLength) {
			this.outputFilePath = filePath;
			this.onlyContentLength = onlyContentLength;
		}

		string ReadLine (Stream stream)
		{
			var line = new List<byte> ();
			while (true) {
				byte c = (byte)stream.ReadByte ();
				if (c == Request.EOL[1])
					break;
				line.Add (c);
			}
			var s = ASCIIEncoding.ASCII.GetString (line.ToArray ()).Trim ();
			return s;
		}

		string[] ReadKeyValue (Stream stream)
		{
			string line = ReadLine (stream);
			if (line == "")
				return null;
			else {
				var split = line.IndexOf (':');
				if (split == -1)
					return null;
				var parts = new string[2];
				parts[0] = line.Substring (0, split).Trim ();
				parts[1] = line.Substring (split + 1).Trim ();
				return parts;
			}
			
		}
		
		public byte[] TakeChunk() {
			byte[] b = null;
			lock(chunks) {
				if(chunks.Count > 0) {
					b = chunks[0];
					chunks.RemoveAt(0);
					return b;
				}
			}
			return b;
		}

		public void ReadFromStream (Stream inputStream)
		{
			//Debug.Log ("Read From Stream");
			//var inputStream = new BinaryReader(inputStream);
			var top = ReadLine (inputStream).Split (new char[] { ' ' });

			/*
			Stream output = null;

			if (outputFilePath == null) {
				output = new MemoryStream ();
			} else {
				output = new FileStream(outputFilePath,FileMode.Append);
			}*/
			
			if (!int.TryParse (top[1], out status))
				throw new HTTPException ("Bad Status Code");
			
			message = string.Join (" ", top, 2, top.Length - 2);
			headers.Clear ();
			
			while (true) {
				// Collect Headers
				string[] parts = ReadKeyValue (inputStream);
				if (parts == null)
					break;
				AddHeader (parts[0], parts[1]);
			}
			
			if (GetHeader ("transfer-encoding") == "chunked") {

				var output = new MemoryStream();

				chunks = new List<byte[]> ();
				while (true) {
					// Collect Body
					string hexLength = ReadLine (inputStream);
					//Console.WriteLine("HexLength:" + hexLength);
					if (hexLength == "0") {
						lock(chunks) {
							chunks.Add(new byte[] {});
						}
						break;
					}
					int length = int.Parse (hexLength, NumberStyles.AllowHexSpecifier);
					for (int i = 0; i < length; i++)
						output.WriteByte ((byte)inputStream.ReadByte ());
					lock(chunks) {
						/*
						if (GetHeader ("content-encoding").Contains ("gzip"))
							chunks.Add (UnZip(output));
						else
							chunks.Add (output.ToArray ());
							*/
						chunks.Add (output.ToArray ());
					}
					output.SetLength (0);
					//forget the CRLF.
					inputStream.ReadByte ();
					inputStream.ReadByte ();
				}
				
				while (true) {
					//Collect Trailers
					string[] parts = ReadKeyValue (inputStream);
					if (parts == null)
						break;
					AddHeader (parts[0], parts[1]);
				}
				var unchunked = new List<byte>();
				foreach(var i in chunks) {
					unchunked.AddRange(i);
				}
				bytes = unchunked.ToArray();
				
			} else {

				// Read Body
				contentLength = 0;
				
				try {
					contentLength = int.Parse (GetHeader ("content-length"));
				} catch {
					contentLength = 0;
				}


				if (downloadDelegate != null) {
					ProgressEvent pEvent = new ProgressEvent();
					pEvent.bytesLoaded = 0;
					pEvent.bytesTotal = contentLength;
					downloadDelegate.OnResponseHeaders(pEvent);
				}

				if (outputFilePath == null) {				
					var output = new MemoryStream();
					for (int i = 0; i < contentLength; i++)
						output.WriteByte ((byte)inputStream.ReadByte ());
					
					/*if (GetHeader ("content-encoding").Contains ("gzip")) {
						bytes = UnZip(output);
					} else {
						bytes = output.ToArray ();
					}*/
					bytes = output.ToArray ();
				}else if (outputFilePath != null && !onlyContentLength){ //para descarga de ficheros

					downloadedBytes = 0;
					progressDownloadedBytes = 0;
					var output = new FileStream(outputFilePath,FileMode.Append);
					//Debug.Log("Force Stop: " + forceStop.ToString());
					for (int i = 0; (i < contentLength && !forceStop); i++) {

						output.WriteByte ((byte)inputStream.ReadByte ());
						downloadedBytes = i+1;
						if (downloadedBytes % PROGRESS_CHUNK_SIZE == 0) {

							if (downloadDelegate != null) {
								ProgressEvent pEvent = new ProgressEvent();
								pEvent.bytesLoaded = downloadedBytes - progressDownloadedBytes;
								pEvent.bytesTotal = contentLength;
								downloadDelegate.OnProgress(pEvent);
								progressDownloadedBytes = downloadedBytes;
							}
						}
					}

					//Debug.Log("Force Stop: " + forceStop.ToString());
					ProgressEvent pEvent2 = new ProgressEvent();
					pEvent2.bytesLoaded = downloadedBytes - progressDownloadedBytes;
					pEvent2.bytesTotal = contentLength;
					downloadDelegate.OnProgress(pEvent2);
					//Debug.Log("Downloaded Bytes: " + downloadedBytes);
					output.Flush();
					output.Close();
				}
			}
			
		}


		/*byte[] UnZip(MemoryStream output) {
			var cms = new MemoryStream ();
			output.Seek (0, SeekOrigin.Begin);
			using (var gz = new GZipStream (output, CompressionMode.Decompress)) {
				var buf = new byte[1024];
				int byteCount = 0;
				while ((byteCount = gz.Read (buf, 0, buf.Length)) > 0) {
					cms.Write (buf, 0, byteCount);
				}
			}
			return cms.ToArray ();	
		}*/
	
		public void Stop() {
			forceStop = true;
		}

		public bool ForceStop {
			get {
				return forceStop;
			}
		}
	}


}

