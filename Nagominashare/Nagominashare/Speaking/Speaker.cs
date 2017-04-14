using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Android.Media;
using Encoding = Android.Media.Encoding;

namespace Nagominashare.Speaking {
    class Speaker : ISpeaker {
        public Task Speak(IDajare dajare) {
            return Speak(dajare.ToString());
        }

        public async Task Speak(string text) {
            const string key = Variables.DocomoApiKey;
            const string uri =
                "https://api.apigw.smt.docomo.ne.jp/crayon/v1/textToSpeech?APIKEY=" + key;
            using (var webClient = new WebClient()) {
                var collection = new NameValueCollection {
                    {"Command", "AP_Synth"},
                    {"SpeakerID", "1"},
                    {"StyleID", "1"},
                    {"TextData", text},
                    {"AudioFileFormat", "2"}
                };
                var wave = webClient.UploadValues(uri, collection);
                var player = new AudioTrack(Stream.Music, 22050, ChannelOut.Default,
                    Encoding.Pcm16bit, wave.Length, AudioTrackMode.Stream);
                player.Play();
                player.Write(wave, 0, wave.Length);
                await
                    Task.Delay(
                        TimeSpan.FromSeconds(wave.Length /
                                             (double) (player.SampleRate * player.ChannelCount * 2)));
            }
        }
    }
}