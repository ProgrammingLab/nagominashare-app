using System;
using System.Collections.ObjectModel;

namespace Nagominashare
{
    public interface IDajare
    {
        /// <summary>
        /// ダジャレがどのくらい良かったかフィードバックをもらう
        /// </summary>
        /// <param name="evaluation">[0, 5)の5段階評価</param>
        void FeedBack(int evaluation);

		void Add(IWord word);

        void AddUsingCount();

        ReadOnlyCollection<IWord> GetWords(); 

		string ToString();

        string ToKanji();

        string ToRoma();
    }
}