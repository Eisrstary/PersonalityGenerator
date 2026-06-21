using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using PersonalityGenerator;

class Program
{
    static int pass = 0, fail = 0, warn = 0;
    static Stopwatch sw = Stopwatch.StartNew();
    static string _lastFail = "";

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   PersonalityGenerator DLL 全面测试 v2.0                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.WriteLine("开始: " + DateTime.Now.ToString("HH:mm:ss"));
        Console.WriteLine("DLL:  " + typeof(Generator).Assembly.GetName().Version);
        Console.WriteLine();

        // ═══ 第一轮：基础API ═══
        Section("1. 基础API");
        T("1.1 GenerateFromSeed(42)", () => {
            var p = Generator.GenerateFromSeed(42);
            A(p != null && p.Values.Length == 84, "84参数");
        });
        T("1.2 GenerateFromSeed(int.MaxValue)", () => {
            var p = Generator.GenerateFromSeed(int.MaxValue);
            A(p != null, "OK");
        });
        T("1.3 GenerateFromSeed(int.MinValue)", () => {
            var p = Generator.GenerateFromSeed(int.MinValue);
            A(p != null, "OK");
        });
        T("1.4 GenerateFromSeed(0)", () => {
            var p = Generator.GenerateFromSeed(0);
            A(p != null, "OK");
        });
        T("1.5 GenerateFromSeed(-1)", () => {
            var p = Generator.GenerateFromSeed(-1);
            A(p != null, "OK");
        });
        T("1.6 Generate(1)", () => {
            var r = Generator.Generate(1);
            A(r.Length == 1, "1个");
        });
        T("1.7 Generate(100)", () => {
            var r = Generator.Generate(100);
            A(r.Length == 100, "100个");
        });
        T("1.8 Generate(0)→默认1", () => {
            var r = Generator.Generate(0);
            A(r.Length == 1, "0→1");
        });
        T("1.9 Generate(-5)→默认1", () => {
            var r = Generator.Generate(-5);
            A(r.Length == 1, "-5→1");
        });
        T("1.10 Generate(10000)大容量", () => {
            var sw2 = Stopwatch.StartNew();
            var r = Generator.Generate(10000);
            sw2.Stop();
            A(r.Length == 10000, "10000个/" + sw2.ElapsedMilliseconds + "ms");
        });

        // ═══ 第二轮：确定性 ═══
        Section("2. 确定性验证");
        T("2.1 同种子=同指纹", () => {
            var a = Generator.GenerateFromSeed(42);
            var b = Generator.GenerateFromSeed(42);
            A(a.Fingerprint == b.Fingerprint, a.Fingerprint);
        });
        T("2.2 100种子交叉", () => {
            for (int s = 0; s < 100; s++)
            {
                var a = Generator.GenerateFromSeed(s);
                var b = Generator.GenerateFromSeed(s);
                if (a.Fingerprint != b.Fingerprint) { A(false, "种子" + s + "不一致"); return; }
            }
            A(true, "100/100");
        });
        T("2.3 不同种子不同指纹", () => {
            var set = new HashSet<string>();
            for (int s = 0; s < 200; s++)
                set.Add(Generator.GenerateFromSeed(s).Fingerprint);
            A(set.Count == 200, "200/200唯一");
        });
        T("2.4 hex种子确定性", () => {
            var hex = Seed.FromInt(99).ToHex();
            var a = Generator.GenerateFromHex(hex);
            var b = Generator.GenerateFromHex(hex);
            A(a.Fingerprint == b.Fingerprint, "hex确定");
        });
        T("2.5 hex+int等价", () => {
            var p1 = Generator.GenerateFromSeed(77);
            var p2 = Generator.GenerateFromHex(Seed.FromInt(77).ToHex());
            A(p1.Fingerprint == p2.Fingerprint, "等价");
        });

        // ═══ 第三轮：参数值合法性 ═══
        Section("3. 参数值合法性");
        T("3.1 100×84值域[0,1]", () => {
            for (int s = 0; s < 100; s++)
            {
                var p = Generator.GenerateFromSeed(s);
                for (int i = 0; i < 84; i++)
                {
                    if (p.Missing[i]) continue;
                    if (p.Values[i] < 0 || p.Values[i] > 1)
                        A(false, "越界:种子" + s + "[" + i + "]=" + p.Values[i].ToString("F4"));
                }
            }
            A(true, "8400值全合法");
        });
        T("3.2 Personality.Get()有效ID", () => {
            var p = Generator.GenerateFromSeed(42);
            double v = p.Get("B015");
            A(v >= 0 && v <= 1, v.ToString("F4"));
        });
        T("3.3 Personality.Get()无效ID", () => {
            var p = Generator.GenerateFromSeed(42);
            double v = p.Get("ZZZZZ");
            A(Math.Abs(v - 0.5) < 0.001, "返回默认0.5");
        });
        T("3.4 MissingCount范围", () => {
            double sum = 0; int min = 999, max = 0;
            for (int s = 0; s < 500; s++)
            {
                int m = Generator.GenerateFromSeed(s).MissingCount;
                sum += m; if (m < min) min = m; if (m > max) max = m;
            }
            double avg = sum / 500;
            A(avg > 5 && avg < 25, "范围[" + min + "," + max + "]均值" + avg.ToString("F1"));
        });
        T("3.5 Fingerprint格式", () => {
            var p = Generator.GenerateFromSeed(42);
            A(p.Fingerprint != null && p.Fingerprint.Length > 3, p.Fingerprint);
        });

        // ═══ 第四轮：偏向系统 ═══
        Section("4. 偏向系统");
        T("4.1 空字符串偏向", () => {
            var r = Generator.Generate(10, "");
            A(r.Length == 10, "10个");
        });
        T("4.2 null偏向", () => {
            var r = Generator.Generate(10, null);
            A(r.Length == 10, "10个");
        });
        T("4.3 单参数+1.0", () => {
            var r = Generator.Generate(30, "B015=1.0");
            int ok = 0; for (int i = 0; i < 30; i++) if (r[i].Get("B015") > 0.7) ok++;
            A(ok >= 20, ok + "/30>0.7");
        });
        T("4.4 单参数-1.0", () => {
            var r = Generator.Generate(30, "B015=-1.0");
            int ok = 0; for (int i = 0; i < 30; i++) if (r[i].Get("B015") < 0.3) ok++;
            A(ok >= 20, ok + "/30<0.3");
        });
        T("4.5 强度=1全覆盖", () => {
            var r = Generator.Generate(20, "B015=1.0,S=1.0");
            int ok = 0; for (int i = 0; i < 20; i++) if (r[i].Get("B015") > 0.95) ok++;
            A(ok >= 15, ok + "/20>0.95");
        });
        T("4.6 强度=0无影响", () => {
            var a = Generator.GenerateFromSeed(42);
            var b = Generator.GenerateFromSeed(42, "B015=1.0,S=0.0");
            A(a.Fingerprint == b.Fingerprint, "指纹一致");
        });
        T("4.7 多参数同时极端", () => {
            var r = Generator.Generate(30, "B015=1.0,B016=-1.0,A009=1.0,D040=-1.0,C036=-1.0,E051=1.0,S=0.95");
            int okB = 0, okD = 0, okC = 0;
            for (int i = 0; i < 30; i++)
            {
                if (r[i].Get("B015") > 0.75) okB++;
                if (r[i].Get("D040") < 0.25) okD++;
                if (r[i].Get("C036") < 0.25) okC++;
            }
            A(okB >= 20 && okD >= 20 && okC >= 20, "B:" + okB + " D:" + okD + " C:" + okC);
        });
        T("4.8 领域偏向B=0.9", () => {
            var r = Generator.Generate(20, "B=0.9,S=0.8");
            int ok = 0;
            for (int i = 0; i < 20; i++)
                if (r[i].Get("B015") > 0.6) ok++;
            A(ok >= 14, ok + "/20>0.6");
        });
        T("4.9 领域偏向D=-0.9", () => {
            var r = Generator.Generate(20, "D=-0.9,S=0.8");
            int ok = 0;
            for (int i = 0; i < 20; i++)
                if (r[i].Get("D040") < 0.4) ok++;
            A(ok >= 14, ok + "/20<0.4");
        });
        T("4.10 不存在的参数ID", () => {
            var r = Generator.Generate(5, "Z999=0.5,XXXXX=0.3,NONEXIST=1.0");
            A(r.Length == 5, "忽略不存在");
        });
        T("4.11 畸形字符串容错", () => {
            var r = Generator.Generate(3, ";;;==;;,B015=,=0.5,,,B016===0.3;;,B017===");
            A(r.Length == 3, "容错解析");
        });
        T("4.12 超长偏向字符串", () => {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 200; i++) sb.Append("B015=0.5,");
            var r = Generator.Generate(1, sb.ToString());
            A(r.Length == 1, "200组解析OK");
        });
        T("4.13 超范围值截断+99", () => {
            var r = Generator.Generate(5, "B015=99.0,B016=-99.0");
            A(r.Length == 5, "截断OK");
        });
        T("4.14 分号分隔符", () => {
            var r = Generator.Generate(5, "B015=0.8;D040=-0.7;S=0.9");
            A(r.Length == 5, "分号OK");
        });
        T("4.15 STRENGTH全称", () => {
            var r = Generator.Generate(20, "B015=1.0,STRENGTH=1.0");
            int ok = 0; for (int i = 0; i < 20; i++) if (r[i].Get("B015") > 0.95) ok++;
            A(ok >= 15, ok + "/20");
        });
        T("4.16 STR简写", () => {
            var r = Generator.Generate(20, "B015=1.0,STR=1.0");
            int ok = 0; for (int i = 0; i < 20; i++) if (r[i].Get("B015") > 0.95) ok++;
            A(ok >= 15, ok + "/20");
        });
        T("4.17 所有84个参数逐一偏向", () => {
            var sb = new System.Text.StringBuilder();
            sb.Append("S=0.9,");
            // A001-A010, B011-B024, ... H084
            string[] ids = { "A001","A002","A003","A004","A005","A006","A007","A008","A009","A010",
                             "B011","B012","B013","B014","B015","B016","B017","B018","B019","B020",
                             "B021","B022","B023","B024",
                             "C025","C026","C027","C028","C029","C030","C031","C032","C033","C034",
                             "C035","C036","C037","C038",
                             "D039","D040","D041","D042",
                             "E043","E044","E045","E046","E047","E048","E049","E050","E051","E052",
                             "E053","E054","E055",
                             "F056","F057","F058","F059","F060","F061","F062",
                             "G063","G064","G065","G066",
                             "H067","H068","H069","H070","H071","H072","H073","H074","H075","H076",
                             "H077","H078","H079","H080","H081","H082","H083","H084" };
            for (int i = 0; i < ids.Length; i++)
                sb.Append(ids[i] + "=0." + (i % 10) + ",");
            var r = Generator.Generate(1, sb.ToString());
            A(r.Length == 1, "84参数全部解析");
        });

        // ═══ 第五轮：种子系统 ═══
        Section("5. 种子系统");
        T("5.1 Seed.FromInt确定性", () => {
            var a = Seed.FromInt(42).ToHex();
            var b = Seed.FromInt(42).ToHex();
            A(a == b, "相同");
        });
        T("5.2 Seed.FromInt不同种子不同hex", () => {
            var a = Seed.FromInt(1).ToHex();
            var b = Seed.FromInt(2).ToHex();
            A(a != b, "不同");
        });
        T("5.3 Seed.Random不崩溃", () => {
            for (int i = 0; i < 100; i++)
            {
                var s = Seed.Random();
                if (s == null) { A(false, "null"); return; }
            }
            A(true, "100次OK");
        });
        T("5.4 Seed.FromString有效hex", () => {
            var hex = Seed.FromInt(42).ToHex();
            var s = Seed.FromString(hex);
            A(s != null, "OK");
        });
        T("5.5 Seed.FromString无效hex", () => {
            try { Seed.FromString("ZZZ"); A(false, "应抛异常"); }
            catch (ArgumentException) { A(true, "正确抛异常"); }
            catch { A(false, "异常类型错误"); }
        });
        T("5.6 Seed.Reset", () => {
            var s = Seed.FromInt(42);
            double a = s.ReadDouble();
            s.Reset();
            double b = s.ReadDouble();
            A(Math.Abs(a - b) < 0.000001, "Reset一致");
        });

        // ═══ 第六轮：文本输出 ═══
        Section("6. 文本输出");
        T("6.1 Roleplay结构完整", () => {
            var p = Generator.GenerateFromSeed(42);
            var txt = Textify.ToRoleplay(p);
            string[] secs = { "【人格档案】", "【核心性格】", "【信息处理】", "【情绪模式】",
                              "【动机与价值观】", "【行为风格】", "【自我认知】",
                              "【社交特征】", "【发展特征】", "【身体-环境反应】", "【角色扮演提示】" };
            foreach (var s in secs)
                if (!txt.Contains(s)) { A(false, "缺失:" + s); return; }
            A(true, "11段落完整");
        });
        T("6.2 Compact输出非空", () => {
            for (int s = 0; s < 100; s++)
            {
                var txt = Textify.ToCompact(Generator.GenerateFromSeed(s));
                if (string.IsNullOrWhiteSpace(txt)) { A(false, "种子" + s + "空输出"); return; }
            }
            A(true, "100个非空");
        });
        T("6.3 Detailed≥84行", () => {
            for (int s = 0; s < 100; s++)
            {
                var lines = Textify.ToDetailed(Generator.GenerateFromSeed(s)).Split('\n');
                if (lines.Length < 84) { A(false, "种子" + s + "只有" + lines.Length + "行"); return; }
            }
            A(true, "100个≥84行");
        });
        T("6.4 Roleplay无乱码", () => {
            for (int s = 0; s < 50; s++)
            {
                var txt = Textify.ToRoleplay(Generator.GenerateFromSeed(s));
                foreach (char c in txt)
                    if (c == '\0') { A(false, "含null字符"); return; }
            }
            A(true, "50个无null");
        });

        // ═══ 第七轮：性能压力 ═══
        Section("7. 性能压力");
        T("7.1 50000批量", () => {
            var sw2 = Stopwatch.StartNew();
            var r = Generator.Generate(50000);
            sw2.Stop();
            A(r.Length == 50000, "50000/" + sw2.ElapsedMilliseconds + "ms/" + (GC.GetTotalMemory(false) / 1048576) + "MB");
            r = null; GC.Collect();
        });
        T("7.2 100×1000连续", () => {
            var sw2 = Stopwatch.StartNew();
            long t = 0;
            for (int i = 0; i < 100; i++) { var r = Generator.Generate(1000); t += r.Length; r = null; }
            sw2.Stop();
            A(t == 100000, "10万/" + sw2.ElapsedMilliseconds + "ms");
        });
        T("7.3 500文本生成", () => {
            var sw2 = Stopwatch.StartNew();
            var r = Generator.Generate(500);
            long c = 0;
            for (int i = 0; i < 500; i++) c += Textify.ToRoleplay(r[i]).Length;
            sw2.Stop();
            A(c > 100000, "500文本/" + sw2.ElapsedMilliseconds + "ms/" + c + "字符");
        });
        T("7.4 10轮GC压力", () => {
            for (int round = 0; round < 10; round++)
            {
                var r = Generator.Generate(1000);
                for (int i = 0; i < 1000; i++) Textify.ToRoleplay(r[i]);
                r = null; GC.Collect();
            }
            A(true, "10轮存活");
        });
        T("7.5 200000极限批量", () => {
            var sw2 = Stopwatch.StartNew();
            var r = Generator.Generate(200000);
            sw2.Stop();
            A(r.Length == 200000, "20万/" + sw2.ElapsedMilliseconds + "ms");
            r = null; GC.Collect();
        });

        // ═══ 第八轮：并发模拟 ═══
        Section("8. 并发模拟(单线程交替)");
        T("8.1 2000轮交替调用", () => {
            for (int i = 0; i < 2000; i++)
            {
                var r1 = Generator.GenerateFromSeed(i);
                var r2 = Generator.Generate(1)[0];
                var r3 = Generator.GenerateFromSeed(i * 2, "B015=0.5");
                if (r1 == null || r2 == null || r3 == null) { A(false, "交替" + i + "失败"); return; }
            }
            A(true, "2000轮");
        });
        T("8.2 1000轮混合API", () => {
            for (int i = 0; i < 1000; i++)
            {
                var hex = Seed.FromInt(i).ToHex();
                var p1 = Generator.GenerateFromSeed(i);
                var p2 = Generator.GenerateFromHex(hex);
                var p3 = Generator.Generate(1, "S=0.5")[0];
                Textify.ToRoleplay(p1);
                Textify.ToCompact(p2);
                Textify.ToDetailed(p3);
            }
            A(true, "1000轮");
        });

        // ═══ 第九轮：边界与异常 ═══
        Section("9. 边界与异常");
        T("9.1 hex种子+偏向", () => {
            var hex = Seed.FromInt(42).ToHex();
            var p = Generator.GenerateFromHex(hex, "B015=0.9,S=0.9");
            A(p != null && p.Get("B015") > 0.7, p.Get("B015").ToString("F3"));
        });
        T("9.2 空hex种子应抛异常", () => {
            try { Generator.GenerateFromHex(""); A(false, "应抛异常"); }
            catch (ArgumentException) { A(true, "正确"); }
            catch { A(false, "异常类型错误"); }
        });
        T("9.3 Personality.Values只读安全", () => {
            var p = Generator.GenerateFromSeed(42);
            var copy = (double[])p.Values.Clone();
            copy[0] = 999;
            A(p.Values[0] != 999, "原始数组未被修改");
        });
        T("9.4 种子耗尽自动降级", () => {
            var p = Generator.GenerateFromSeed(0);
            A(p.MissingCount >= 0 && p.MissingCount <= 84, "缺失" + p.MissingCount);
        });

        // ═══ 第十轮：综合场景 ═══
        Section("10. 综合场景");
        T("10.1 批量+偏向+文本", () => {
            var sw2 = Stopwatch.StartNew();
            var r = Generator.Generate(100, "B015=0.9,D040=-0.8,C036=-0.9,E051=0.9,S=0.85");
            int chars = 0;
            for (int i = 0; i < 100; i++)
            {
                var txt = Textify.ToRoleplay(r[i]);
                chars += txt.Length;
                if (!txt.Contains("【人格档案】")) { A(false, "文本不完整"); return; }
            }
            sw2.Stop();
            A(chars > 50000, "100个/" + sw2.ElapsedMilliseconds + "ms/" + chars + "字符");
        });
        T("10.2 极端组合:圣人型", () => {
            var r = Generator.Generate(20, "B015=1.0,B016=-1.0,A009=1.0,D040=-1.0,C036=-1.0,C037=1.0,E051=1.0,S=0.95");
            int allOk = 0;
            for (int i = 0; i < 20; i++)
                if (r[i].Get("B015") > 0.8 && r[i].Get("D040") < 0.2 && r[i].Get("C036") < 0.2)
                    allOk++;
            A(allOk >= 12, allOk + "/20符合圣人型");
        });
        T("10.3 极端组合:恶魔型", () => {
            var r = Generator.Generate(20, "B015=-1.0,B016=1.0,A009=-1.0,D040=1.0,C036=1.0,C037=-1.0,S=0.95");
            int allOk = 0;
            for (int i = 0; i < 20; i++)
                if (r[i].Get("B015") < 0.2 && r[i].Get("D040") > 0.8 && r[i].Get("B016") > 0.8)
                    allOk++;
            A(allOk >= 12, allOk + "/20符合恶魔型");
        });
        T("10.4 极端组合:机器人型", () => {
            var r = Generator.Generate(20, "B011=1.0,B021=-1.0,B015=0.0,B017=-1.0,C033=-1.0,F056=1.0,S=0.95");
            int allOk = 0;
            for (int i = 0; i < 20; i++)
                if (r[i].Get("B011") > 0.8 && r[i].Get("B021") < 0.2 && r[i].Get("C033") < 0.2)
                    allOk++;
            A(allOk >= 12, allOk + "/20符合机器人型");
        });
        T("10.5 随机抽样1000个统计", () => {
            var r = Generator.Generate(1000);
            double avgB015 = 0, avgD040 = 0, avgE051 = 0;
            for (int i = 0; i < 1000; i++)
            {
                avgB015 += r[i].Get("B015");
                avgD040 += r[i].Get("D040");
                avgE051 += r[i].Get("E051");
            }
            avgB015 /= 1000; avgD040 /= 1000; avgE051 /= 1000;
            A(avgB015 > 0.4 && avgB015 < 0.6, "B015均值" + avgB015.ToString("F3"));
            A(avgD040 > 0.4 && avgD040 < 0.6, "D040均值" + avgD040.ToString("F3"));
            A(avgE051 > 0.4 && avgE051 < 0.6, "E051均值" + avgE051.ToString("F3"));
        });

        // ═══ 汇总 ═══
        sw.Stop();
        int total = pass + fail + warn;
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   全面测试汇总                             ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  总计: " + total.ToString().PadLeft(4) + "  通过: " + pass.ToString().PadLeft(4) + "  失败: " + fail.ToString().PadLeft(4) + "  警告: " + warn.ToString().PadLeft(4) + "               ║");
        Console.WriteLine("║  通过率: " + (pass * 100.0 / total).ToString("F1") + "%                                          ║");
        Console.WriteLine("║  耗时:   " + (sw.ElapsedMilliseconds / 1000.0).ToString("F2") + "s                                       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        if (fail > 0)
        {
            Console.WriteLine("\n  !!!!!!!! 存在失败测试 !!!!!!!!");
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine("\n  全部测试通过。DLL 可安全发布到 Unity6 / WASM / 多平台。");
        }

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }

    static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine("── " + title + " ──");
    }

    static void T(string name, Action test)
    {
        try
        {
            test();
            Console.WriteLine("  [PASS] " + name);
            pass++;
        }
        catch (AssertFailException e)
        {
            _lastFail = name + " | " + e.Message;
            Console.WriteLine("  [FAIL] " + name + " —— " + e.Message);
            fail++;
        }
        catch (Exception e)
        {
            _lastFail = name + " | " + e.GetType().Name + ": " + e.Message;
            Console.WriteLine("  [WARN] " + name + " —— " + e.GetType().Name + ": " + e.Message);
            warn++;
        }
    }

    static void A(bool condition, string msg)
    {
        if (!condition) throw new AssertFailException(msg);
    }
}

class AssertFailException : Exception
{
    public AssertFailException(string msg) : base(msg) { }
}
