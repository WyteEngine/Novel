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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Novel.NovelRuntime"/> class.
		/// </summary>
		/// <param name="code">ソースコード．</param>
		public NovelRuntime(string code)
		{
			// Null チェックをしつつ，コードの分析
			runtimeCode = NParser.Parse(code ?? throw new ArgumentNullException(nameof(code)));

			commands = new Dictionary<string, NFunc>();
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

		/// <summary>
		/// [内部的に使用するメソッド] 指定した実行可能コードを逐次実行します．
		/// </summary>
		/// <param name="code">実行するべきコード.</param>
		/// <param name="num">実行する先頭の行番号.</param>
		void Run(INCode code, int num = 0)
		{
			for (int i = num; i < code.Statements.Length; i++)
			{
				var statement = code.Statements[i];

				try
				{
					CallFunc(statement.SpriteTag, statement.CommandName, statement.Arguments);
				}
				catch(NRuntimeException ex)
				{
					// ランタイムエラー表示
					Console.Error.WriteLine($"エラー(行 {i}, {statement.CommandName}): {ex.Message}");
				}
				catch(Exception ex)
				{
					// バグだ．
					Console.Error.WriteLine($"内部エラー({ex.Message})\n{ex.StackTrace}");
				}

				//デバッグコードにつきコメントアウト
				//Console.WriteLine($"{statement.SpriteTag}+{statement.CommandName} {string.Join(", ", statement.Arguments)}");
			}
		}

		string CallFunc(string spTag, string cmdName, params string[] args)
		{
			if (!commands.ContainsKey(cmdName))
				throw new NRuntimeException($"Couldn't find the command '{cmdName}'.");
			return commands[cmdName]?.Invoke(spTag, args);
		}

		/// <summary>
		/// Novel コードをコンパイルし，実行します．
		/// </summary>
		/// <param name="code">ソースコード．</param>
		public void Run(string code) => Run(NParser.Parse(code ?? throw new ArgumentNullException(nameof(code))));

		/// <summary>
		/// Novel コードをコンパイルし，実行します．
		/// </summary>
		/// <param name="code">ソースコード．</param>
		/// <param name="label">はじめに実行する場所のラベル．</param>
		public void Run(string code, string label)
		{
			var c = NParser.Parse(code ?? throw new ArgumentNullException(nameof(code)));
			if (!c.Labels.ContainsKey(label ?? throw new ArgumentNullException(nameof(label))))
				throw new ArgumentException($"コード内にラベル {label} は存在しません．");
			Run(c, c.Labels[label]);
		}
	}
}
