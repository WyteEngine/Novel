namespace Novel.Models
{
	/// <summary>
	/// Novel のステートメント モデルの実装です．
	/// </summary>
	internal class NStatement : INStatement
	{
		public string CommandName { get; }

		public string SpriteTag { get; }

		public string[] Arguments { get; }

		public NStatement(string commandName, string spriteTag, string[] arguments)
		{
			CommandName = commandName;
			SpriteTag = spriteTag;
			Arguments = arguments;
		}
	}
}
