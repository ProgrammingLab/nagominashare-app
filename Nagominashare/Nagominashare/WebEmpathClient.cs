using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nagominashare;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace Nagominashare
{
	class WebEmpathClient : IWebEmpathClient
	{
		private static byte[] getByteArray(IAudioBuffer audioBuffer)
		{
			var res = new List<byte>();
			foreach (var s in audioBuffer.SelectMany(frame => frame))
			{
				res.AddRange(BitConverter.GetBytes(s));
			}
			return res.ToArray();
		}

		public async Task<IFeeling> Analyze(string apiKey, IAudioBuffer audioData)
		{
            byte[] binary = getByteArray(audioData);
#if WINJII
#else
			binary = binary.Where((v, i) => i % 4 == 0).ToArray();
#endif
            Wave waveData = new Wave(16, 1, 11025, binary);
			
			string url = "http://api.webempath.net:8080/v1/analyzeWav";
            string boundary = "114514-1919-810-" + System.Environment.TickCount;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Timeout = 1000;
			request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            string body = "";
            body += "--" + boundary + "\r\n";
            body += "Content-Disposition: form-data; name=\"apikey\"\r\n";
            body += "Content-Type: text/plain; charset=UTF-8\r\n";
            body += "\r\n";
            body += apiKey + "\r\n";
            body += "--" + boundary + "\r\n";
            body += "Content-Disposition: form-data; name=\"wav\"; filename=\"8931919.wav\"\r\n";
            body += "Content-Type: audio/x-wav\r\n";
            body += "\r\n";
            string footer = "\r\n--" + boundary + "--\r\n";

            byte[] bodyData = Encoding.UTF8.GetBytes(body);
            byte[] footerData = Encoding.UTF8.GetBytes(footer);

			try
			{
				using (Stream reqStream = await request.GetRequestStreamAsync())
				{
                    reqStream.Write(bodyData, 0, bodyData.Length);
                    waveData.Write(new BinaryWriter(reqStream));
					reqStream.Write(footerData, 0, footerData.Length);
					WebResponse response = await request.GetResponseAsync();
					StreamReader reader = new StreamReader(response.GetResponseStream());
					return new Feeling(reader.ReadToEnd());
				}
			} catch (Exception e)
			{
				throw e;
			}
		}
	}
}