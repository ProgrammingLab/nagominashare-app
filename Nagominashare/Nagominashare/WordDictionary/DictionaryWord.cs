using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare.WordDictionary
{
	class DictionaryWord : IDictionaryWord, IWord {
	    private IWord _word;
	    private int usingCount;

	    public DictionaryWord(IWord word) {
	        _word = word;
	        usingCount = 0;
	    }

		public double GetEvalution()
		{
			return (double)WordEvaluator.GetInstance().Evaluate(_word.ToKanji());
		}

		public void FeedBack(int starCount)
		{
			throw new NotImplementedException();
			//TODO
		}

	    public void AddUsingCount() {
	        usingCount++;
	    }

		public int GetUsingCount() {
		    return usingCount;
		}

	    public IWord GetWord() => _word;

		public Hinshi GetHinshi()
		{
			return _word.GetHinshi();
		}
		public string ToHiragana()
		{
			return _word.ToHiragana();
		}
		public string ToKanji()
		{
			return _word.ToKanji();
		}
		public string ToRoma()
		{
			return _word.ToRoma();
		}

	    public bool ContainsAlpha() {
	        return _word.ContainsAlpha();
	    }
	}
}
