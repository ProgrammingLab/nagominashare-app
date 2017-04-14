using Android.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare.VoiceRecogning
{
	public interface IRecognizer
	{
        /// <summary>
        /// 音声ファイルを調べ、発言可能なタイミングか判定
        /// </summary>
        /// <param name="audioRecord">音声データ(PCM_16BIT)</param>
        /// <returns></returns>
        Task<bool> IsDajarable(IAudioBuffer audioRecord);

        /// <summary>
        /// 音声に含まれる単語のコレクションを返す。
        /// </summary>
        /// <param name="audioRecord">音声データ(PCM_16BIT)</param>
        /// <returns></returns>
	    Task<IEnumerable<IWord>> ExtractWords(IAudioBuffer audioRecord);
	}
}
