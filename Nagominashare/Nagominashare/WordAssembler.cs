using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Lang;
using Exception = System.Exception;

namespace Nagominashare.DajareGenerator
{
	public class WordAssembler
	{
		private IWord meishi;
		public WordAssembler(IWord meishi)
		{
			if (meishi.GetHinshi() != Hinshi.Meishi) throw new Exception();
			this.meishi = meishi;
		}
		private IDajare connectWithMeishi(IWord meishi)
		{
			if (meishi.GetHinshi() != Hinshi.Meishi) throw new Exception();
			IDajare res = new Dajare();
			WordEvaluator we = WordEvaluator.GetInstance();
			IWord[] testers = { new Word("な", Hinshi.Joshi), new Word("は", Hinshi.Joshi) };
			long max = -114514;
			IWord better助詞 = null;
			foreach (IWord j in testers)
			{
				long eval = we.Evaluate(this.meishi.ToKanji() + j.ToKanji());
				if (max < eval)
				{
					max = eval;
					better助詞 = j;
				}
			}
			if (better助詞.ToHiragana() == "な")
			{
				res.Add(this.meishi);
				res.Add(better助詞);
				res.Add(meishi);
				return res;
			}
			IWord[] 助詞s =
			{
				new Word("は", Hinshi.Joshi),
				new Word("と", Hinshi.Joshi),
				new Word("より", Hinshi.Joshi),
				new Word("の", Hinshi.Joshi)
			};
			Random random = new Random();
			res.Add(this.meishi);
			res.Add(助詞s[random.Next(助詞s.Length)]);
			res.Add(meishi);
			return res;
		}
		private IDajare connectWithDoshi(IWord doshi)
		{
			if (doshi.GetHinshi() != Hinshi.Doshi) throw new Exception();
			IDajare res = new Dajare();
			WordEvaluator we = WordEvaluator.GetInstance();
			IWord[] 助詞s =
			{
				new Word("を", Hinshi.Joshi),
				new Word("へ", Hinshi.Joshi),
				new Word("が", Hinshi.Joshi)
			};
			IWord best助詞 = null;
			long max = 0;
			foreach (IWord j in 助詞s)
			{
				long e1 = we.Evaluate(meishi.ToKanji() + j.ToKanji());
				long e2 = we.Evaluate(j.ToKanji() + doshi.ToKanji());
				long eval = e1*e1 + e2*e2;
				if (max > eval)
				{
					max = eval;
					best助詞 = j;
				}
			}
			res.Add(this.meishi);
			res.Add(best助詞);
			res.Add(doshi);
			return res;
		}
		private IDajare connectWithKeiyoshi(IWord keiyoshi)
		{
			if (keiyoshi.GetHinshi() != Hinshi.Keiyoshi) throw new Exception();
			IDajare res = new Dajare();
			res.Add(this.meishi);
			res.Add(new Word("は", Hinshi.Joshi));
			res.Add(keiyoshi);
			return res;
		}
		private IDajare connectWithRentaishi(IWord rentaishi)
		{
			if (rentaishi.GetHinshi() != Hinshi.Rentaishi) throw new Exception();
			IDajare res = new Dajare();
			res.Add(rentaishi);
			res.Add(this.meishi);
			return res;
		}
		public IDajare Assemble(IWord word)
		{
			switch (word.GetHinshi())
			{
			case Hinshi.Meishi:
				return connectWithMeishi(word);
			case Hinshi.Doshi:
				return connectWithDoshi(word);
			case Hinshi.Keiyoshi:
				return connectWithKeiyoshi(word);
			//辞書側は形容動詞は名詞として扱う
			//case Hinshi.Keiyodoshi:
			//    break;
			//諦め
			//case Hinshi.Fukushi:
			//    break;
			case Hinshi.Rentaishi:
				return connectWithRentaishi(word);
			//諦め
			//case Hinshi.Setsuzokushi:
			//    break;
			//諦め
			//case Hinshi.Kandoshi:
			//    break;
			//無視
			//case Hinshi.Jodoshi:
			//    break;
			//case Hinshi.Joshi:
			//    break;
			//case Hinshi.Other:
			//    break;
			default:
                    throw new IllegalArgumentException("未対応の品詞");
			}
		}
	}
}
