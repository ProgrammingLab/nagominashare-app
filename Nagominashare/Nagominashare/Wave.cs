using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Nagominashare
{
    class Wave
    {
        //RIFFヘッダ
        private byte[] riffID = Encoding.ASCII.GetBytes("RIFF");
        private UInt32 size;
        private byte[] waveID = Encoding.ASCII.GetBytes("WAVE");
        //fmtチャンク
        private byte[] fmtID = Encoding.ASCII.GetBytes("fmt ");
        private UInt32 formatSize = 16;
        private Int16 format = 0x0001;
        private UInt16 channels;
        private UInt32 sampleRate;
        private UInt32 bytePerSec;
        private UInt16 blockSize;
        private UInt16 bit;
        //dataチャンク
        private byte[] dataID = Encoding.ASCII.GetBytes("data");
        private UInt32 dataSize;
        private byte[] audioData;

        public Wave(UInt16 stages, UInt16 channels, UInt16 sampleRate, byte[] audioData)
        {
            this.channels = channels;
            this.sampleRate = sampleRate;
            this.audioData = audioData;
            dataSize = (UInt32)audioData.Length;
            size = dataSize + 36;
            bit = (UInt16)(stages * channels);
            bytePerSec = (UInt32)(bit / 2 * sampleRate);
            blockSize = (UInt16)(bit / 8);
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(riffID);
            bw.Write(size);
            bw.Write(waveID);
            bw.Write(fmtID);
            bw.Write(formatSize);
            bw.Write(format);
            bw.Write(channels);
            bw.Write(sampleRate);
            bw.Write(bytePerSec);
            bw.Write(blockSize);
            bw.Write(bit);
            bw.Write(dataID);
            bw.Write(dataSize);
            bw.Write(audioData);
        }
    }
}