using System.Collections.Generic;

namespace Novel.Models
{
	public interface INCode
	{
		Dictionary<string, int> Labels { get; }
		INStatement[] Statements { get; }
	}

}
