using System;
using System.Linq;
using System.Text;
using Novel.Models;

namespace Novel.Parsing
{
	static class Extensions
	{
		/// <summary>
		/// 文字列を改行で区切り配列にします．いかなる改行コードにも対応します．
		/// </summary>
		/// <returns>改行で区切られた文字列．</returns>
		public static string[] SplitLine(this string self)
			=> self.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

		/// <summary>
		/// Novel の実行可能コードを文字列に変換します．
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string ToString(this INCode c)
		{
			var sb = new StringBuilder();

			// ラベルの書き出し
			sb.AppendLine($"{nameof(c.Labels)}: ");
			sb.AppendLine(string.Join(Environment.NewLine, c.Labels.Select(l => $"  {l.Key} : {l.Value}")));

			// ステートメントの書き出し
			sb.AppendLine($"{nameof(c.Statements)}: ");
			sb.AppendLine(string.Join(Environment.NewLine, c.Statements.Select((s, i) => $"  {i:##0} {s.SpriteTag}+{s.CommandName} {string.Join(", ", s.Arguments)}")));

			return sb.ToString();
		}
	}

}
