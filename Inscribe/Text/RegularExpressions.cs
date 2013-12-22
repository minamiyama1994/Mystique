using System.Text.RegularExpressions;

namespace Inscribe.Text
{
    /// <summary>
    /// StarryEyesのInscribe.Helpers.TwitterRegexPatternsに将来的に置き換えるのが望ましいと考える
    /// </summary>
    public static class RegularExpressions
    {

        /// <summary>
        /// ダイレクトメッセージ送信判定用のregex
        /// </summary>
        public static Regex DirectMessageSendRegex = new Regex(@"^d (?<![A-Za-z0-9_])@([A-Za-z0-9_]+(?:/[A-Za-z0-9_]*)?)(?![A-Za-z0-9_@]) (.*)$");
    }
}
