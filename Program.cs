using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Json2Md
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引数なしかつ標準入力がない場合、Usageを表示
            if (args.Length == 0 && !Console.IsInputRedirected)
            {
                ShowUsage();
                return;
            }

            // 標準入力のエンコーディングをUTF-8に設定
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // 標準入力から1行読み込み
            string input = null;
            try
            {
                input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    ShowUsage();
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            // JSONを解析し、Markdownに変換
            string markdown = null;
            try
            {
                JObject json = JObject.Parse(input);
                markdown = ConvertToMarkdown(json);
            }
            catch (Exception)
            {
                return; // JSON構文エラーで終了
            }

            // 標準出力にMarkdownを出力（UTF-8）
            Console.Write(markdown);
            Console.Out.Flush();
        }

        static string ConvertToMarkdown(JObject json, int indentLevel = 0)
        {
            var builder = new StringBuilder();
            string indent = new string(' ', indentLevel * 2);

            // すべてのプロパティを箇条書きで処理
            foreach (var prop in json.Properties())
            {
                string key = prop.Name;
                var value = prop.Value;

                // スカラー値の場合、キー: 値で出力
                if (value is JValue jValue)
                {
                    builder.AppendLine($"{indent}- {key}: {FormatValue(jValue)}");
                }
                // オブジェクトや配列の場合、キー名をまず出力
                else
                {
                    builder.AppendLine($"{indent}- {key}");
                    // オブジェクトの場合、深い階層を処理
                    if (value is JObject subObject)
                    {
                        ProcessObject(subObject, builder, indentLevel + 1);
                    }
                    // 配列の場合、すべての要素を処理
                    else if (value is JArray subArray)
                    {
                        if (subArray.Any())
                        {
                            foreach (var item in subArray)
                            {
                                if (item is JObject itemObject)
                                {
                                    ProcessObject(itemObject, builder, indentLevel + 1);
                                }
                                else if (item is JValue itemValue)
                                {
                                    builder.AppendLine($"{indent}  - {FormatValue(itemValue)}");
                                }
                            }
                        }
                        else
                        {
                            builder.AppendLine($"{indent}  - []");
                        }
                    }
                }
            }

            return builder.ToString().TrimEnd();
        }

        static void ProcessObject(JObject obj, StringBuilder builder, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 2);

            foreach (var prop in obj.Properties())
            {
                string key = prop.Name;
                var value = prop.Value;

                // スカラー値の場合、キー: 値で出力
                if (value is JValue jValue)
                {
                    builder.AppendLine($"{indent}- {key}: {FormatValue(jValue)}");
                }
                // オブジェクトや配列の場合、キー名をまず出力
                else
                {
                    builder.AppendLine($"{indent}- {key}");
                    // ネストされたオブジェクトを再帰処理
                    if (value is JObject subObject)
                    {
                        ProcessObject(subObject, builder, indentLevel + 1);
                    }
                    // 配列の場合、すべての要素を処理
                    else if (value is JArray subArray)
                    {
                        if (subArray.Any())
                        {
                            foreach (var item in subArray)
                            {
                                if (item is JObject itemObject)
                                {
                                    ProcessObject(itemObject, builder, indentLevel + 1);
                                }
                                else if (item is JValue itemValue)
                                {
                                    builder.AppendLine($"{indent}  - {FormatValue(itemValue)}");
                                }
                            }
                        }
                        else
                        {
                            builder.AppendLine($"{indent}  - []");
                        }
                    }
                }
            }
        }

        static string FormatValue(JValue jValue)
        {
            return jValue.ToString(Formatting.None);
        }

        static void ShowUsage()
        {
            Console.WriteLine(@"Usage: json2md
Reads JSON from standard input and outputs Markdown to standard output.
Input must be UTF-8 encoded JSON without surrounding double quotes.
Outputs a bullet list with top-level keys, indented subkeys, and all values (key: value for scalars), reflecting the JSON structure.
Examples:
ECHO {""users"":[{""id"":1,""name"":""John Doe""}],""settings"":{""region"":""Asia""}} | json2md
Output:
- users
  - id: 1
  - name: ""John Doe""
- settings
  - region: ""Asia""
");
            Console.Out.Flush();
        }
    }
}