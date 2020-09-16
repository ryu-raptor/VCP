using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace VCP.API
{
    /// <summary>
    /// 関数の登録と呼び出しを行う
    /// </summary>
    public class ControlDelegate
    {
        struct Command
        {
            public ControlProvider.Command Info;
            public Func<IList<ControlRequest.Argument>, JsonCompatible> Body;
        }

        struct ReflectionCommand
        {
            public ControlProvider.Command Info;
            public object Instance;
            public MethodInfo Body;
        }

        Dictionary<string, Command> commands;
        Dictionary<string, ReflectionCommand> reflectionCommands;

        public ControlDelegate()
        {
            commands = new Dictionary<string, Command>();
            reflectionCommands = new Dictionary<string, ReflectionCommand>();
        }

        /// <summary>
        /// 関数の登録
        /// 同じ識別子での二重登録(オーバーロード)はできない
        /// </summary>
        /// <param name="info">登録する関数の情報</param>
        /// <param name="body">登録する関数の本体</param>
        public void Regist(ControlProvider.Command info, Func<IList<ControlRequest.Argument>, JsonCompatible> body)
        {
            if (commands.ContainsKey(info.Name)) {
                throw new Exception($"ControlDelegate: Command {info.Name} is already registed.");
            }

            commands.Add(info.Name, new Command() {
                Info = info, Body = body
            });
        }

        /// <summary>
        /// 既存の登録を上書きする
        /// 既存の登録がない場合は例外をスローする(濫用防止のため)
        /// </summary>
        /// <param name="info">上書きする関数情報(Name が上書き対象と一致していること)</param>
        /// <param name="body">上書きする関数本体</param>
        public void OverwriteRegist(ControlProvider.Command info, Func<IList<ControlRequest.Argument>, JsonCompatible> body)
        {
            if (!commands.ContainsKey(info.Name)) {
                throw new Exception($"ControlDelegate: Command {info.Name} has no overwrite candidates. Use Regist().");
            }

            commands[info.Name] = new Command() {
                Info = info, Body = body
            };
        }

        /// <summary>
        /// リフレクションを利用してネイティブメソッドから呼出を生成する
        /// オーバーロードには対応していない．オーバーロードが存在するメソッドを指定した際の動作は未定義
        /// デフォルト値が設定されているパラメータは Required が <see langword="false"/> になる
        /// リフレクションを利用するためメタデータを参照できない場合は情報が欠落する可能性がある
        /// </summary>
        /// <param name="instance">メソッドの所属するインスタンス</param>
        /// <param name="selector">メソッド名</param>
        public void RegistAuto(object instance, string selector, string comname = null, string description = "")
        {
            var t = instance.GetType();
            var m = t.GetMethod(selector);
            var param = m.GetParameters();
            // build command info
            var info = new ControlProvider.Command();
            info.Name = comname ?? selector;
            info.Description = description;
            info.ArgInfos = new List<ControlProvider.Command.ArgInfo>(param.Length);
            foreach (var p in param) {
                // 型を判定
                TypeInfo argtype = TypeInfo.Undefined;
                if (p.ParameterType == typeof(string)) {
                    argtype = TypeInfo.String;
                }
                else if (p.ParameterType == typeof(int) || p.ParameterType == typeof(double) || p.ParameterType == typeof(float)) {
                    argtype = TypeInfo.Number;
                }
                else if (p.ParameterType == typeof(bool)) {
                    argtype = TypeInfo.Boolean;
                }

                info.ArgInfos.Add(
                    new ControlProvider.Command.ArgInfo() {
                        Name = p.Name,
                        Type = argtype,
                        Required = !p.IsOptional,
                        Description = "",
                    }
                );
            }

            reflectionCommands.Add(comname ?? selector, new ReflectionCommand(){
                Info = info,
                Instance = instance,
                Body = m
            });
        }

        /// <summary>
        /// 関数の呼び出し
        /// マッチする関数がない場合は null を返し，例外はスローされない．
        /// </summary>
        /// <param name="request">呼び出し情報</param>
        /// <returns>正常終了ならその関数からの戻り値，呼び出しに失敗したら必ず null</returns>
        public JsonCompatible Call(ControlRequest request)
        {
            if (commands.TryGetValue(request.Name, out var command)) {
                return command.Body(request.Args);
            }
            else if (reflectionCommands.TryGetValue(request.Name, out var refcom)) {
                return RefCall(refcom, request);
            }
            return null;
        }

        JsonCompatible RefCall(ReflectionCommand refcom, ControlRequest request)
        {
            // 値をキャスト
            var param = refcom.Body.GetParameters();
            var args = new object[param.Length];
            for (int i = 0; i < param.Length; i++) {
                var name = param[i].Name;
                // キャスト対象
                object cast = null;

                // 引数が見つからなかった
                if (request.Args.Count((a) => a.Name == name) == 0) {
                    // オプショナルだっけ?
                    if (param[i].IsOptional) {
                        // 規定値を代入
                        cast = param[i].DefaultValue;
                    }
                    // 不正な呼出
                    else {
                        throw new Exception("Parameter {name} is Required but not specified.");
                    }
                }
                else
                {
                    var match = request.Args.FirstOrDefault((a) => a.Name == name);

                    // キャストして代入
                    // string ならそのまま
                    if (match.Type == TypeInfo.String)
                    {
                        cast = match.Value;
                    }
                    else
                    {
                        // パーサーを取得
                        var t = param[i].ParameterType;
                        var parser = t.GetMethod("TryParse", new[] { typeof(string), t.MakeByRefType() });
                        if (parser == null) { throw new Exception($"No matching {t}.TryParse(string, {t})"); }
                        else
                        {
                            var parseArg = new object[] { match.Value, null };
                            if ((bool)parser.Invoke(null, parseArg))
                            {
                                // 値の取り出し
                                cast = parseArg[1];
                            }
                        }
                    }
                }

                args[param[i].Position] = cast;
            }

            // 呼出
            return (JsonCompatible) refcom.Body.Invoke(refcom.Instance, args);
        }
    }
}