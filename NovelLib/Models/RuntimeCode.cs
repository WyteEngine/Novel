namespace Novel.Models
{
	struct RuntimeCode
	{
		public INCode Code { get; }
		public int EntryPoint { get; }

		public RuntimeCode(INCode code, int entryPoint)
		{
			Code = code;
			EntryPoint = entryPoint;
		}
	}
}