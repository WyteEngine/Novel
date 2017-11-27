using System;

namespace Novel.Exceptions
{
	public class ParseStatementException : Exception
	{
		private int line;
		private int i;

		/// <summary>
		/// 構文エラーの発生した行番号．
		/// </summary>
		public int LineNumber => line;
		/// <summary>
		/// 構文エラーの発生した列番号．
		/// </summary>
		public int ColumnNumber => i;

		public ParseStatementException() { }
		public ParseStatementException(string message) : base(message) { }
		public ParseStatementException(string message, Exception inner) : base(message, inner) { }

		public ParseStatementException(string message, int line, int i) : base(message)
		{
			this.line = line;
			this.i = i;
		}
	}
}