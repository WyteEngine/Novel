using System.Collections.Generic;

namespace Novel.Models
{
	/// <summary>
	/// Novel の実行可能コード モデルの実装です．
	/// </summary>
	internal class NCode : INCode
	{
		public Dictionary<string, int> Labels { get; }
		public INStatement[] Statements { get; }

		public NCode(Dictionary<string, int> labels, INStatement[] statements)
		{
			Labels = labels;
			Statements = statements;
		}
	}
}