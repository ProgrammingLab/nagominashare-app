using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare
{
	//singleton
	public interface IWordComparer
	{
		/// <summary>
		/// 単語同士の一致度を計算
		/// </summary>
		/// <param name="a">比較対象</param>
		/// <param name="b">比較対象</param>
		/// <returns>一致度(0-1)</returns>
		void DoDP(IWord a, IWord b, out double[,,] dp);
	}
}
