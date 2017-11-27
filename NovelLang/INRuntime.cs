using System.Collections.Generic;

namespace Novel
{
	public interface INRuntime
	{
		/// <summary>
		/// コードを指定したラベルから実行します．
		/// </summary>
		/// <param name="label"></param>
		void Call(string label);

		/// <summary>
		/// コードを即席でコンパイルし，実行します．
		/// </summary>
		/// <param name="code">コンパイルするコード．</param>
		/// <exception cref="ParseStatementException">構文解析で生じるエラー．</exception>
		/// <exception cref="NRuntimeException">実行時に生じるエラー．</exception>
		void Run(string code);

		/// <summary>
		/// コードを即席でコンパイルし，指定したラベルから実行します．
		/// </summary>
		/// <param name="code">コンパイルするコード．</param>
		/// <param name="label">実行を開始するラベル．</param>
		/// <exception cref="ParseStatementException">構文解析で生じるエラー．</exception>
		/// <exception cref="NRuntimeException">実行時に生じるエラー．</exception>
		void Run(string code, string label);
	}

}
