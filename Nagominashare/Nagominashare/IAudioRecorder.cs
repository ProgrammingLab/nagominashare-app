using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Media;

namespace Nagominashare {
    public interface IAudioRecorder {
        Task<IAudioBuffer> StartRecord(TimeSpan length);
    }
}
