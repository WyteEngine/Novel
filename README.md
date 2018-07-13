Novel Language
=================

Novel 言語は， .NET アプリケーションに組み込んで使用できる，カスタマイズ可能なスクリプティング言語およびエンジンです．

次のような構文でHello worldを記述できます．

```novel
#boot
:Hello, world!!		// メッセージ文

+say "Hello, world!!"	// コマンド文
```

また，コード上からコマンドを自由に追加することができます．

例:

```cs
var runtime = new NovelRuntime("");

runtime.RegisterCommand("add", (s, a) => Console.WriteLine(int.Parse(a[0] + a[1])));
```

```novel
+add 4, 3 // 7と表示される
```

必要な要件
---------------

- .NET Standard 2.0 互換の .NET プラットフォーム
- MSBuild

ライセンス
--------------
このプロジェクトは MIT ライセンスで提供されます． [ライセンス(英語)](LICENSE)をご覧ください．
