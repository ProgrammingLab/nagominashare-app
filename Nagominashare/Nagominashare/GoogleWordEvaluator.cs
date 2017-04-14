using System;
using System.IO;
using System.Net;
using Android.Util;
using Org.Json;

namespace Nagominashare {
    class GoogleWordEvaluator : WordEvaluator {
        public override long Evaluate(string word) {
            const string key = Variables.GoogleApiKey;
            const string cx = Variables.GoogleCustomSearchCx;
            var url = $"https://www.googleapis.com/customsearch/v1?key={key}&cx={cx}&q={word}";
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Timeout = 1000 * 10;
            request.Method = "GET";

            try {
                WebResponse response = request.GetResponse();
                var responseStream = response.GetResponseStream();

                using (var sr = new StreamReader(responseStream)) {
                    var raw = sr.ReadToEnd();
                    return ParseResponse(raw);
                }
            } catch (Exception e) {
                Log.Debug("WordEvaluator", e.ToString());
                throw;
            }
        }

        private static long ParseResponse(string raw) {
            var json = new JSONObject(raw);
            var rawTotalResults =
                json.GetJSONObject("searchInformation")?.GetString("totalResults");
            return long.Parse(rawTotalResults);
        }
    }
}