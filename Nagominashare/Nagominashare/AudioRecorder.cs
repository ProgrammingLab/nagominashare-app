using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Encoding = Android.Media.Encoding;

namespace Nagominashare {
    class AudioRecorder : IAudioRecorder {
        public async Task<IAudioBuffer> StartRecord(TimeSpan length) {
			int sampleRateInHz;
#if WINJII
			sampleRateInHz = 8000;
#else
			sampleRateInHz = 44100;
#endif

			var frame = AudioRecord.GetMinBufferSize(sampleRateInHz, ChannelIn.Mono,
                Encoding.Pcm16bit);
            var buffer = new AudioBuffer() { FrameCount = frame };
            using (
                var audioRecord = new AudioRecord(AudioSource.Mic, sampleRateInHz, ChannelIn.Mono, Encoding.Pcm16bit,
                    frame*2)) {
                audioRecord.SetRecordPositionUpdateListener(
                    new OnRecordPositionUpdateListener(buffer));
                audioRecord.SetPositionNotificationPeriod(frame);
                audioRecord.StartRecording();
                await Task.Delay(length);
                audioRecord.Stop();
            }
            lock (buffer) {
                return buffer;
            }
        }

        private class OnRecordPositionUpdateListener : Java.Lang.Object,
            AudioRecord.IOnRecordPositionUpdateListener {

            private AudioBuffer _buffer;

            public OnRecordPositionUpdateListener(AudioBuffer buffer) {
                _buffer = buffer;
            }

            public void OnMarkerReached(AudioRecord recorder) {
                lock (_buffer) {
                    _buffer.Write(recorder);
                }
            }

            public void OnPeriodicNotification(AudioRecord recorder) {
                lock (_buffer) {
                    _buffer.Write(recorder);
                }
            }
        }
    }
}