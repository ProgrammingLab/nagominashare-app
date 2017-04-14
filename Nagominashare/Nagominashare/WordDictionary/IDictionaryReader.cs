using System.Collections.Generic;

namespace Nagominashare.WordDictionary {
    public interface IDictionaryReader {

        IEnumerable<string> ReadAll();
    }
}
