using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Nagominashare.VoiceRecogning;
using System.Threading.Tasks;
using System.Xml;
using Android.Util;
using Org.Json;

namespace Nagominashare.VoiceRecognizing {
    class Recognizer : IRecognizer {

        private static TaskFactory<string> _taskFactory = new TaskFactory<string>();
        
        public async Task<bool> IsDajarable(IAudioBuffer audioRecord)
            => string.IsNullOrEmpty(await Recognize(audioRecord));

        public async Task<IEnumerable<IWord>> ExtractWords(IAudioBuffer audioRecord)
            => ParseSentence(await Recognize(audioRecord));

        private static async Task<string> Recognize(IAudioBuffer audioData) {
            byte[] data;
            lock (audioData) {
                data = GetByteArray(audioData);
            }

            const string key = Variables.GoogleApiKey;
            var uri =
                "https://www.google.com/speech-api/v2/recognize?output=json&lang=ja-JP&key=" + key;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = 10000;
            request.Method = "POST";
            request.Host = "www.google.com";
            request.KeepAlive = true;
            request.SendChunked = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "audio/l16; rate=44100";
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.8,en;q=0.6");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";

            return await _taskFactory.StartNew(() => {
                using (var reqStream = request.GetRequestStream()) {
                    reqStream.Write(data, 0, data.Length);
                }

                try {
                    WebResponse response = request.GetResponse();
                    var respStream = response.GetResponseStream();

                    if (response.ContentType == "application/json; charset=utf-8") {
                        using (var sr = new StreamReader(respStream)) {
                            var res = sr.ReadToEnd().Trim().Split('\n');
                            if (1 < res.Length) {
                                Log.Debug("speech", res[1]);
                                var transcript = GetTranscript(res[1]);
                                Log.Debug("transcript", transcript);
                                return transcript;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    Log.Debug("recognizer", e.Message);
                    throw;
                }

                return string.Empty;
            });
        }

        private static IEnumerable<IWord> ParseSentence(string sentence) {
            if (string.IsNullOrEmpty(sentence)) {
                return new List<IWord>();
            }

            const string appid = Variables.YahooAppId;
            var uri = "http://jlp.yahooapis.jp/MAService/V1/parse?appid=" + appid + "&sentence=" +
                      sentence + "&results=ma";
            var request = (HttpWebRequest) WebRequest.Create(uri);
            try {
                WebResponse response = request.GetResponse();
                var respStream = response.GetResponseStream();

                using (var sr = new StreamReader(respStream)) {
                    var res = sr.ReadToEnd();
                    Log.Debug("yahoo", res);
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(res);
                    var root = document.DocumentElement;
                    var wordList = root["ma_result"].GetElementsByTagName("word");

                    var wordsResult = new List<IWord>();
                    for (var i = 0; i < wordList.Count; ++i) {
                        var wordNode = wordList[i];
                        var word = new Word(wordNode["surface"].InnerText,
                            wordNode["reading"].InnerText, wordNode["pos"].InnerText);
                        if (word.ContainsAlpha()) continue;
                        Log.Debug("yahoo", word.ToRoma());
                        wordsResult.Add(word);
                    }

                    return wordsResult;
                }
            } catch (Exception e) {
                Log.Debug("speech", e.Message);
                return null;
            }
        }

        private static string GetTranscript(string rawJson) {
            var json = new JSONObject(rawJson);
            var res =
                json.GetJSONArray("result")?
                    .GetJSONObject(0)?
                    .GetJSONArray("alternative")?
                    .GetJSONObject(0)?
                    .GetString("transcript");
            return res;
        }

        private static double GetRootMeanSquare(IReadOnlyCollection<short> audioData) {
            var res = audioData.Aggregate(0.0d, (current, s) => current + s * s);
            return Math.Log10(Math.Sqrt(res / audioData.Count));
        }

        private static short[] GetShortArray(IAudioBuffer audioRecord) {
            var res = new List<short>();
            foreach (var frame in audioRecord) {
                res.AddRange(frame);
            }

            return res.ToArray();
        }

        private static byte[] GetByteArray(IAudioBuffer audioBuffer) {
            var res = new List<byte>();
            foreach (var s in audioBuffer.SelectMany(frame => frame)) {
                res.AddRange(BitConverter.GetBytes(s));
            }

            return res.ToArray();
        }
    }
}