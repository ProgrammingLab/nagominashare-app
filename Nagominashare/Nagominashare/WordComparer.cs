using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nagominashare.DajareGenerator
{
	class WordComparer : IWordComparer
	{
		private class Sound
		{
			public enum TailType { Long, N, None }

			public char Consonant { get; }
			public char Vowel { get; }
			public char Vowel2 { get; }
			public bool IsDouble { get; }
			public TailType Tail { get; }

			public Sound(char consonant, char vowel, char vowel2, bool isDouble, TailType tail)
			{
				Consonant = consonant;
				Vowel = vowel;
				Vowel2 = vowel2;
				IsDouble = isDouble;
				Tail = tail;
			}
		}

		private static WordComparer instance;
		public static WordComparer GetInstance()
		{
			if (instance == null) instance = new WordComparer();
			return instance;
		}

		private static double[,] rate;

		private static void setRate(char a, char b, double value)
		{
			rate[a - 'a', b - 'a'] = rate[b - 'a', a - 'a'] = value;
		}
		private static double getRate(char a, char b)
		{
			if (a == '\0' && b == '\0') return 1.0;
			if (a == '\0' || b == '\0') return 0.5;
			return rate[a - 'a', b - 'a'];
		}
		private static bool isLower(char c)
		{
			return 'a' <= c && c <= 'z';
		}
		private static bool isVowel(char c)
		{
			return c == 'a' || c == 'i' || c == 'u' || c == 'e' || c == 'o';
		}
		private static bool isConsonant(char c)
		{
			return isLower(c) && !isVowel(c);
		}
		private static List<Sound> dissassemble(string roma)
		{
			List<Sound> res = new List<Sound>();
			roma += "\'";
			for (int i = 0; i < roma.Length;)
			{
				if (!isConsonant(roma[i]) && !isVowel(roma[i])) return res;
				char consonant = '\0';
				bool isDoulbe = false;
				if (isConsonant(roma[i]))
				{
					consonant = roma[i];
					i++;
					if (isConsonant(roma[i]))
					{
						isDoulbe = true;
						i++;
					}
				}
				char vowel2 = '\0';
				if (consonant != 'n' && roma[i] == '\'') { vowel2 = 'h'; i++; }
				else if (isConsonant(roma[i])) { vowel2 = roma[i]; i++; }
				char vowel = '\0';
				if (isVowel(roma[i])) { vowel = roma[i]; i++; }
				Sound.TailType tail = Sound.TailType.None;
				if (roma[i] == '-') { tail = Sound.TailType.Long; i++; }
				if (roma[i] == 'n' && (isConsonant(roma[i + 1]) || roma[i + 1] == '\''))
				{
					tail = Sound.TailType.N;
					i++;
				}
				if (vowel == '\0')
				{
					consonant = '\0';
					tail = Sound.TailType.N;
				}
				res.Add(new Sound(consonant, vowel, vowel2, isDoulbe, tail));
			}
			return res;
		}
		private static double compareSound(Sound s, Sound t)
		{
			double res = getRate(s.Consonant, t.Consonant)*getRate(s.Vowel, t.Vowel);
			if (s.Vowel2 != t.Vowel2) res -= 0.2;
			if (s.IsDouble != t.IsDouble) res -= 0.2;
			if (s.Tail != t.Tail) res -= 0.2;
			return res;
		}
		static WordComparer()
		{
			rate = new double[26, 26]; //初期値は0.0らしい
			{
				for (int i = 0; i < 26; i++) rate[i, i] = 1.0;
				setRate('t', 's', 0.5);
				setRate('b', 'm', 0.5);
				setRate('k', 't', 0.5);
				setRate('n', 'r', 0.5);
				setRate('n', 'm', 0.5);
				setRate('h', 's', 0.5);
				setRate('a', 'i', 0.5);
				setRate('a', 'u', 0.5);
				setRate('a', 'e', 0.5);
				setRate('a', 'o', 0.5);
				setRate('i', 'u', 0.5);
				setRate('i', 'e', 0.5);
				setRate('i', 'o', 0.5);
				setRate('u', 'e', 0.5);
				setRate('u', 'o', 0.5);
				setRate('e', 'o', 0.5);
			}
		}
		/// <summary>
		/// </summary>
		/// <param name="w1"></param>
		/// <param name="w2"></param>
		/// <returns>
		/// dp[i, j, k] : w1のi文字目とw2のj文字目を最後に使い, k回飛ばした時の最大一致度
		/// k <= 1
		/// </returns>
		public void DoDP(IWord w1, IWord w2, out double[,,] dp)
		{
			List<Sound> sounds1 = dissassemble(w1.ToRoma());
			List<Sound> sounds2 = dissassemble(w2.ToRoma());
			int n = sounds1.Count, m = sounds2.Count;
			dp = new double[n + 1, m + 1, 2];
			if (n == 0 || m == 0) return;
			for (int i = 0; i < n + 1; i++)
			{
				for (int j = 0; j < m + 1; j++)
				{
					dp[i, j, 1] = -1e10;
					if (i == 0 || j == 0) continue;
					dp[i, j, 0] = -1e10;
				}
			}
			for (int i = 0; i < n + 1; i++)
			{
				for (int j = 0; j < m + 1; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						if (i < n && j < m)
							dp[i + 1, j + 1, k] = Math.Max(
								dp[i + 1, j + 1, k],
								dp[i, j, k] + compareSound(sounds1[i], sounds2[j])
							);
						if (k == 1) continue;
						if (i < n)
							dp[i + 1, j, k + 1] = Math.Max(dp[i + 1, j, k + 1], dp[i, j, k] - 2);
						if (j < m)
							dp[i, j + 1, k + 1] = Math.Max(dp[i, j + 1, k + 1], dp[i, j, k] - 2);
					}
				}
			}
			return;
		}
	}
}
