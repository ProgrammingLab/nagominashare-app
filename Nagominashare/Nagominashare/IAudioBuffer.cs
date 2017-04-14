using System.Collections.Generic;
using Android.Media;

namespace Nagominashare {
    public interface IAudioBuffer : IEnumerable<short[]> {

        void Write(AudioRecord record);
    }
}
