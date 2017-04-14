using System;

namespace Nagominashare
{
    public interface IWord
    {
		/// <summary>
		/// 品詞を返す
		/// </summary>
		/// <returns>単語の品詞</returns>
        Hinshi GetHinshi();
		/// <summary>
		/// 漢字に変換
		/// </summary>
		/// <returns>単語の漢字+ひらがな表記</returns>
        string ToKanji();
		/// <summary>
		/// ひらがなに変換
		/// </summary>
		/// <returns>単語のひらがな表記</returns>
        string ToHiragana();
		/// <summary>
		/// ローマ字に変換
		/// </summary>
		/// <returns>単語のローマ字(何式か決まってない)表記</returns>
		//TODO: 何式のローマ字か決まってない
        string ToRoma();

        bool ContainsAlpha();
    }
}