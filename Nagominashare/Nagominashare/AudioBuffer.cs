using System;
using System.Collections;
using System.Collections.Generic;
using Android.Media;
using Android.Util;

namespace Nagominashare {
    class AudioBuffer : IAudioBuffer {
        private readonly Queue<short[]> _buffer = new Queue<short[]>();

        public int FrameCount { get; set; }

        public IEnumerator<short[]> GetEnumerator() {
            return ((IEnumerable<short[]>) _buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<short[]>) _buffer).GetEnumerator();
        }

        public void Write(AudioRecord ar) {
            var buff = new short[FrameCount];
            try {
                ar.Read(buff, 0, buff.Length);
            }
            catch (ObjectDisposedException) {
                Log.Debug("audiobuffer", "AudioRecord was disposed!");
                return;
            }
            _buffer.Enqueue(buff);
        }
    }
}