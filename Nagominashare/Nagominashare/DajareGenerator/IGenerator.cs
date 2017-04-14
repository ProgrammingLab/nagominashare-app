using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nagominashare.DajareGenerator
{
    public interface IGenerator
    {
        /// <summary>
        /// ダジャレを生成する
        /// </summary>
        /// <param name="keywords">なるべくこれらの単語に関連したダジャレを生成する</param>
        /// <returns></returns>
        Task<IDajare> Generate(List<IWord> keywords);
    }
}
