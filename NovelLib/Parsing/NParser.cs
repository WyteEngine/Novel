using System.Collections.Generic;

namespace Novel.Parsing
{
	using System.Text;
	using Novel.Exceptions;
	using Novel.Models;

	using LabelMap = Dictionary<string, int>;
	/// <summary>
	/// Novel の構文解析を行います．
	/// </summary>
	public static class NParser
	{
		/// <summary>
		/// コマンドステートメントを表すキーワード．
		/// </summary>
		private const char COMMAND_PREFIX = '+';
		/// <summary>
		/// メッセージステートメントを表すキーワード．
		/// </summary>
		private const char MESSAGE_PREFIX = ':';
		/// <summary>
		/// ラベルを表すキーワード．
		/// </summary>
		private const char LABEL_PREFIX = '#';
		/// <summary>
		/// コマンドステートメントにおける区切り文字．
		/// </summary>
		private const char COMMAND_SEPARATOR = ' ';

		/// <summary>
		/// 構文解析時に内部で使用する状態オブジェクト．
		/// </summary>
		enum Token
		{
			/// <summary>
			/// スプライトタグ．
			/// </summary>
			SpriteTag,
			/// <summary>
			/// コマンド/メッセージプレフィックス．
			/// </summary>
			Prefix,
			/// <summary>
			/// コマンド名．
			/// </summary>
			Name,
			/// <summary>
			/// 引数．
			/// </summary>
			Arguments,
			/// <summary>
			/// ラベル番号．
			/// </summary>
			LabelName
		}

		/// <summary>
		/// スクリプトコードを構文解析して，実行可能
		/// </summary>
		/// <returns>The parse.</returns>
		/// <param name="code">Code.</param>
		/// <param name="echoCommand">Echo command.</param>
		public static INCode Parse(string code, string echoCommand = "say")
		{
			// スクリプトをステートメントに区切る
			var statements = code.SplitLine();

			// 汎用文字列バッファ
			var buffer = new StringBuilder();

			// 文字列リテラルかどうか
			var isQuotation = false;

			// 解析用トークン
			var token = Token.SpriteTag;

			// 一時保存変数の定義
			string spTag, comName;
			var args = new List<string>();

			// ステートメント
			var statementList = new List<INStatement>();

			// ラベル一覧
			var labels = new LabelMap();

			// 行番号
			var line = 0;

			foreach (var statement in statements)
			{

				// 一時保存変数やフラグの初期化
				spTag = comName = "";
				args?.Clear();
				token = Token.SpriteTag;
				isQuotation = false;

				// ここからステートメントの解析
				for (var i = 0; i < statement.Length; i++)
				{
					// 現在パースしている文字
					var chr = statement[i];
					// chrの1つ前またはnull文字
					var cm1 = (i > 0) ? statement[i - 1] : '\0';
					// chrの1つ後またはnull文字
					var cp1 = (i < statement.Length - 1) ? statement[i + 1] : '\0';

					// リテラル外において，空白などは無視される
					if ((!isQuotation) && (char.IsControl(chr) || char.IsSeparator(chr) || char.IsWhiteSpace(chr)))
					{
						continue;
					}

					// コメント文
					if ((!isQuotation) && chr == '/' && cp1 == '/')
					{
						break;
					}

					// 解析処理
					switch (token)
					{
						// スプライトタグ
						case Token.SpriteTag:
							// 現在地がプレフィックスであれば抜ける
							if (chr == COMMAND_PREFIX || chr == MESSAGE_PREFIX || chr == LABEL_PREFIX)
							{
								// スプライトタグをバッファの文字列に設定する
								spTag = buffer.ToString();

								// トークンの切り替え
								token = Token.Prefix;

								// バッファの掃除
								buffer.Clear();

								// 次の段階でプレフィックスのチェックをするためiを前にずらす (for文によるiの増加に合わせる)
								i--;

								// 次にすっ飛ばす
								continue;
							}

							//読み取った文字をバッファに入れる
							buffer.Append(chr);
							break;

						// プレフィックスの解析
						case Token.Prefix:
							switch (chr)
							{
								case COMMAND_PREFIX:
									// コマンドの解析に飛ぶ
									token = Token.Name;
									continue;

								case MESSAGE_PREFIX:
									// メッセージコマンドということにして
									comName = echoCommand;
									// 引数解析にすっ飛ばす
									token = Token.Arguments;
									continue;

								case LABEL_PREFIX:
									// コマンドではない処理を行う
									token = Token.LabelName;
									continue;
								default:
									throw new ParseStatementException($"Invalid prefix {(chr != '\0' ? chr.ToString() : "")}", line, i);
							}

						// コマンド名の解析
						case Token.Name:
							//読み取った文字をバッファに入れる
							buffer.Append(chr);

							// 区切り文字が登場したら
							if (cp1 == COMMAND_SEPARATOR)
							{
								// コマンド名を設定して
								comName = buffer.ToString();
								// バッファをお掃除して
								buffer.Clear();
								// 区切り文字の分カーソルを進めて
								i++;
								// トークンを切り替えて
								token = Token.Arguments;
								// ｲｯﾃﾖｼ
								continue;
							}
							break;

						// 引数の構文解析
						case Token.Arguments:

							switch (chr)
							{
								// エスケープシーケンスの処理
								case '\\':
									switch (cp1)
									{
										// 改行
										case 'n':
											buffer = buffer.Append('\n');
											break;
										// ダブルクォート
										case '"':
											buffer = buffer.Append('"');
											break;
										// バックスラッシュ
										case '\\':
											buffer = buffer.Append('\\');
											break;
										// 予期しないやつ
										default:
											throw new ParseStatementException($@"Invalid escape sequence \{(cp1 != '\0' ? cp1.ToString() : "")}", line, i);
									}
									// エスケープ文字分飛ばす
									i++;
									break;
								// ダブルクォート
								case '"':
									// ダブルクォートで包んだ文字列は，空白や,の影響を受けない
									isQuotation = !isQuotation;
									break;
								// その他の文字
								default:
									// 引数の区切り
									if ((chr == ',' && !isQuotation))
									{
										args?.Add(buffer.ToString());
										buffer.Clear();
									}
									// 読み取った引数の文字をバッファに入れる
									else
									{
										buffer.Append(chr);
									}
									break;
							}
							break;
						// ラベル名
						case Token.LabelName:
							// 念のためステートメントとして登録できないようコマンド名を初期化
							comName = "";
							buffer.Append(chr);

							break;
						// 異常なトークン(プログラムのバグでない限りありえない)
						default:
							throw new ParseStatementException($@"(Bug!!) Unknown token ""{token}""", line, i);
					}
				}

				// バッファが空でない場合，データが残っているので加味
				if (!string.IsNullOrEmpty(buffer.ToString()))
				{
					if (token == Token.LabelName)
					{
						labels.Add(buffer.ToString(), line);
					}
					else
					{
						args.Add(buffer.ToString());
					}
					buffer.Clear();
				}

				// コマンド名がカラッポ = ラベルやコメント行や空行なのでスルー
				if (string.IsNullOrEmpty(comName))
					continue;

				statementList.Add(new NStatement(comName, spTag, args.ToArray()));

				line++;
			}
			return new NCode(labels, statementList.ToArray());
		}
	}

}
