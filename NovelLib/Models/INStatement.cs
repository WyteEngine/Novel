namespace Novel.Models
{
	/// <summary>
	/// Novel のステートメントを定義します．
	/// </summary>
	public interface INStatement
	{
		string CommandName { get; }
		string SpriteTag { get; }
		string[] Arguments { get; }
	}
}