using Novel.Parsing;
using Novel.Models;
using System;
using System.IO;
using Novel.Exceptions;
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


	/// <summary>
	/// Novel のコマンド．
	/// </summary>
	/// <param name="spriteTag">対象となるスプライトのタグ．グローバルコマンドの場合はnull．</param>
	/// <param name="args"></param>
	public delegate string NFunc(string spriteTag, string[] args);

	public delegate void NCommand(string spriteTag, string[] args);

	/// <summary>
	/// Novel ランタイムのリファレンス実装です．
	/// </summary>
	public class NovelRuntime : INRuntime
	{
		INCode runtimeCode;

		Dictionary<string, NFunc> commands;

		public NovelRuntime(string code)
		{
			// Null チェックをしつつ，コードの分析
			runtimeCode = NParser.Parse(code ?? throw new ArgumentNullException(nameof(code)));
		}

		/// <summary>
		/// 値を返さないコマンドを実装します．
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="command">Command.</param>
		public void RegisterCommand(string name, NCommand command) => RegisterCommand(name, (s, a) =>
		{
			command(s, a);
			return null;
		});

		/// <summary>
		/// 値を返すタイプのコマンドを登録します．
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="func">Func.</param>
		public void RegisterCommand(string name, NFunc func)
		{
			commands[name] = func;
		}

		/// <summary>
		/// コードを実行します．
		/// </summary>
		/// <param name="label">実行開始ラベル．<c>null</c>か，空の場合は戦闘から実行します．</param>
		public void Call(string label)
		{
			var startIndex = 0;
			if (runtimeCode.Labels.ContainsKey(label))
			{
				// 開始位置を指定
				startIndex = runtimeCode.Labels[label];
			}

			Run(runtimeCode, startIndex);

		}

		private static void Run(INCode code, int num = 0)
		{
			for (int i = num; i < code.Statements.Length; i++)
			{
				var statement = code.Statements[i];
				Console.WriteLine($"{statement.SpriteTag}+{statement.CommandName} {string.Join(", ", statement.Arguments)}");
			}
		}

		public static void Run(string code) => Run(NParser.Parse(code ?? throw new ArgumentNullException(nameof(code))));

		public static void Run(string code, string label)
		{
			var c = NParser.Parse(code ?? throw new ArgumentNullException(nameof(code)));
			if (!c.Labels.ContainsKey(label ?? throw new ArgumentNullException(nameof(label))))
				throw new ArgumentException($"コード内にラベル {label} は存在しません．");
			Run(c, c.Labels[label]);
		}

		void INRuntime.Run(string code) => Run(code);
		void INRuntime.Run(string code, string label) => Run(code, label);




	}

}
