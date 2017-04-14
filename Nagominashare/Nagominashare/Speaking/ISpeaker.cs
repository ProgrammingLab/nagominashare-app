using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare.Speaking
{
	public interface ISpeaker
	{
		/// <summary>
		/// ダジャレを読み上げる
		/// </summary>
		/// <param name="dajare">ダジャレ</param>
		Task Speak(IDajare dajare);
	}
}
