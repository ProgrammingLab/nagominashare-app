using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare.WordDictionary
{
    public interface IDictionaryWord
    {
		/// <summary>
		/// 単語データを返す
		/// </summary>
		/// <returns></returns>
        IWord GetWord();
		/// <summary>
		/// 評価値を返す
		/// </summary>
		/// <returns>単語の評価値(高いほど良い)</returns>
		double GetEvalution();

		int GetUsingCount();

        void AddUsingCount();

		void FeedBack(int starCount);
    }
}
