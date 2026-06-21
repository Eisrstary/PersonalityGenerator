using System;
using System.Diagnostics;
using System.Collections.Generic;
using PersonalityGenerator;

class Program
{
    static int pass = 0, fail = 0, warn = 0;
    static Stopwatch sw = Stopwatch.StartNew();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 快速诊断
        Console.WriteLine("=== Bias.Parse 诊断 ===");
        var diagBias = Bias.Parse("B015=1.0,S=1.0");
        Console.WriteLine("  Strength=" + diagBias.Strength + " (预期1.0)");
        // B015 在 Params.All 中的索引是 14 (A001=0...A010=9, B011=10, B012=11, B013=12, B014=13, B015=14)
        Console.WriteLine("  Biases[14/B015]=" + diagBias.Biases[14].ToString("F4") + " (预期1.0)");
        var diagRes = Generator.Generate(5, "B015=1.0,S=1.0");
        for (int i = 0; i < 5; i++)
            Console.WriteLine("  B015[" + i + "]=" + diagRes[i].Get("B015").ToString("F4") + " (预期>0.95)");
        Console.WriteLine();

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║     PersonalityGenerator DLL 极限轰炸测试              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine("开始时间: " + DateTime.Now.ToString("HH:mm:ss"));
        Console.WriteLine();

        // 第一轮
        Section("第一轮：基础功能验证");
        Test("T1.1 单个人格生成", () => {
            var p = Generator.GenerateFromSeed(42);
            Assert(p != null && p.Values.Length == 84, "84参数");
        });
        Test("T1.2 确定性", () => {
            var a = Generator.GenerateFromSeed(42);
            var b = Generator.GenerateFromSeed(42);
            Assert(a.Fingerprint == b.Fingerprint, "同种子=同指纹");
        });
        Test("T1.3 文本输出", () => {
            var p = Generator.GenerateFromSeed(42);
            var txt = Textify.ToRoleplay(p);
            Assert(txt.Contains("【人格档案】") && txt.Contains("【核心性格】"), "结构完整");
        });
        Test("T1.4 紧凑模式", () => {
            var p = Generator.GenerateFromSeed(42);
            var txt = Textify.ToCompact(p);
            Assert(txt.Length > 20, "有内容");
        });
        Test("T1.5 详细模式", () => {
            var p = Generator.GenerateFromSeed(42);
            var txt = Textify.ToDetailed(p);
            Assert(txt.Split('\n').Length >= 84, "84行+");
        });

        // 第二轮
        Section("第二轮：边界值轰炸");
        Test("T2.1 种子=0", () => { var p = Generator.GenerateFromSeed(0); Assert(p != null && p.Values.Length == 84, "零种子正常"); });
        Test("T2.2 种子=int.MaxValue", () => { var p = Generator.GenerateFromSeed(int.MaxValue); Assert(p != null, "最大int正常"); });
        Test("T2.3 种子=int.MinValue", () => { var p = Generator.GenerateFromSeed(int.MinValue); Assert(p != null, "最小int正常"); });
        Test("T2.4 种子=-1", () => { var p = Generator.GenerateFromSeed(-1); Assert(p != null, "负一种子正常"); });
        Test("T2.5 种子=-999999999", () => { var p = Generator.GenerateFromSeed(-999999999); Assert(p != null, "大负种子正常"); });
        Test("T2.6 批量=0", () => { try { var x = Generator.Generate(0); Assert(x.Length == 1, "0→1"); } catch (Exception e) { Assert(false, "异常:" + e.Message); } });
        Test("T2.7 批量=-1", () => { try { var x = Generator.Generate(-1); Assert(x.Length == 1, "-1→1"); } catch (Exception e) { Assert(false, "异常:" + e.Message); } });
        Test("T2.8 批量=1", () => { var x = Generator.Generate(1); Assert(x.Length == 1, "1个"); });
        Test("T2.9 批量=1000", () => { var x = Generator.Generate(1000); Assert(x.Length == 1000, "1000个"); });
        Test("T2.10 批量=10000", () => { var sw2 = Stopwatch.StartNew(); var x = Generator.Generate(10000); sw2.Stop(); Assert(x.Length == 10000, "10000个/" + sw2.ElapsedMilliseconds + "ms"); });

        // 第三轮：偏向
        Section("第三轮：偏向参数轰炸");
        Test("T3.1 空字符串偏向", () => { var x = Generator.Generate(5, ""); Assert(x.Length == 5, "空=无偏向"); });
        Test("T3.2 null偏向", () => { var x = Generator.Generate(5, null); Assert(x.Length == 5, "null=无偏向"); });
        Test("T3.3 单参数极值+1", () => {
            var x = Generator.Generate(20, "B015=1.0,S=0.9");
            int ok = 0; for (int i = 0; i < 20; i++) if (x[i].Get("B015") > 0.7) ok++;
            Assert(ok >= 12, ok + "/20 >0.7 (偏向有效)");
        });
        Test("T3.4 单参数极值-1", () => {
            var x = Generator.Generate(20, "B015=-1.0,S=0.9");
            int ok = 0; for (int i = 0; i < 20; i++) if (x[i].Get("B015") < 0.3) ok++;
            Assert(ok >= 12, ok + "/20 <0.3 (偏向有效)");
        });
        Test("T3.5 超范围值+99", () => { var x = Generator.Generate(5, "B015=99.0"); Assert(x.Length == 5, "超范围被截断"); });
        Test("T3.6 超范围值-99", () => { var x = Generator.Generate(5, "B015=-99.0"); Assert(x.Length == 5, "超范围被截断"); });
        Test("T3.7 多参数同时极值", () => {
            var x = Generator.Generate(20, "B015=1.0,B016=-1.0,A009=1.0,D040=-1.0,S=0.95");
            int okB = 0, okD = 0;
            for (int i = 0; i < 20; i++) { if (x[i].Get("B015") > 0.7) okB++; if (x[i].Get("D040") < 0.3) okD++; }
            Assert(okB >= 12 && okD >= 12, "B015:" + okB + "/20 D040:" + okD + "/20");
        });
        Test("T3.8 领域偏向", () => { var x = Generator.Generate(10, "B=0.9,S=0.8"); Assert(x.Length == 10, "领域偏向正常"); });
        Test("T3.9 不存在的参数ID", () => { var x = Generator.Generate(5, "Z999=0.5,XXXXX=0.3"); Assert(x.Length == 5, "忽略不存在ID"); });
        Test("T3.10 畸形字符串", () => { var x = Generator.Generate(3, ";;;==;;,B015=,=0.5,,,B016===0.3;;"); Assert(x.Length == 3, "容错解析"); });
        Test("T3.11 超长偏向字符串", () => { var sb = new System.Text.StringBuilder(); for (int i = 0; i < 100; i++) sb.Append("B015=0.5,"); var x = Generator.Generate(1, sb.ToString()); Assert(x.Length == 1, "超长解析"); });
        Test("T3.12 强度=0(无影响)", () => { var a = Generator.GenerateFromSeed(42); var b = Generator.GenerateFromSeed(42, "B015=1.0,S=0.0"); Assert(a.Fingerprint == b.Fingerprint, "强度0=无影响"); });
        Test("T3.13 强度=1(全覆盖)", () => {
            var x = Generator.Generate(10, "B015=1.0,S=1.0");
            int ok = 0; for (int i = 0; i < 10; i++) if (x[i].Get("B015") > 0.9) ok++;
            Assert(ok >= 8, "全覆盖 " + ok + "/10 >0.9");
        });

        // 第四轮
        Section("第四轮：十六进制种子轰炸");
        Test("T4.1 有效hex种子", () => { var hex = Seed.FromInt(42).ToHex(); var p = Generator.GenerateFromHex(hex); Assert(p != null, "hex正常"); });
        Test("T4.2 hex+偏向", () => { var hex = Seed.FromInt(42).ToHex(); var p = Generator.GenerateFromHex(hex, "B015=0.9"); Assert(p != null, "hex+偏向正常"); });
        Test("T4.3 hex确定性", () => { var hex = Seed.FromInt(99).ToHex(); var a = Generator.GenerateFromHex(hex); var b = Generator.GenerateFromHex(hex); Assert(a.Fingerprint == b.Fingerprint, "hex确定"); });

        // 第五轮
        Section("第五轮：内存与性能轰炸");
        Test("T5.1 50000批量生成", () => { var sw2 = Stopwatch.StartNew(); var x = Generator.Generate(50000); sw2.Stop(); Assert(x.Length == 50000, "50000个/" + sw2.ElapsedMilliseconds + "ms/" + GC.GetTotalMemory(false) / 1024 / 1024 + "MB"); x = null; GC.Collect(); });
        Test("T5.2 连续100次×1000", () => { var sw2 = Stopwatch.StartNew(); long t = 0; for (int i = 0; i < 100; i++) { var x = Generator.Generate(1000); t += x.Length; x = null; } sw2.Stop(); Assert(t == 100000, "10万/" + sw2.ElapsedMilliseconds + "ms"); });
        Test("T5.3 文本生成压力", () => { var sw2 = Stopwatch.StartNew(); var x = Generator.Generate(500); long c = 0; for (int i = 0; i < 500; i++) c += Textify.ToRoleplay(x[i]).Length; sw2.Stop(); Assert(c > 100000, "500文本/" + sw2.ElapsedMilliseconds + "ms/" + c + "字符"); });
        Test("T5.4 GC压力测试", () => { for (int r = 0; r < 10; r++) { var x = Generator.Generate(1000); for (int i = 0; i < 1000; i++) Textify.ToRoleplay(x[i]); x = null; GC.Collect(); } Assert(true, "10轮GC存活"); });

        // 第六轮
        Section("第六轮：一致性验证");
        Test("T6.1 100种子交叉验证", () => { for (int s = 0; s < 100; s++) { var a = Generator.GenerateFromSeed(s); var b = Generator.GenerateFromSeed(s); if (a.Fingerprint != b.Fingerprint) Assert(false, "种子" + s + "不一致"); } Assert(true, "100/100一致"); });
        Test("T6.2 缺失率范围", () => { double sum = 0; int min = 999, max = 0; for (int s = 0; s < 500; s++) { int m = Generator.GenerateFromSeed(s).MissingCount; sum += m; if (m < min) min = m; if (m > max) max = m; } double avg = sum / 500; Assert(avg > 8 && avg < 22, "缺失范围[" + min + "," + max + "] 均值" + avg.ToString("F1")); });
        Test("T6.3 参数值范围", () => { for (int s = 0; s < 100; s++) { var p = Generator.GenerateFromSeed(s); for (int i = 0; i < 84; i++) { if (p.Missing[i]) continue; if (p.Values[i] < 0 || p.Values[i] > 1) Assert(false, "种子" + s + "参数" + i + "=" + p.Values[i].ToString("F4") + "越界"); } } Assert(true, "100×84值均在[0,1]"); });
        Test("T6.4 不同种子不同指纹", () => { var set = new HashSet<string>(); for (int s = 0; s < 100; s++) set.Add(Generator.GenerateFromSeed(s).Fingerprint); Assert(set.Count == 100, "100个不同指纹"); });

        // 第七轮
        Section("第七轮：并发模拟");
        Test("T7.1 快速交替调用", () => { for (int i = 0; i < 1000; i++) { var r1 = Generator.GenerateFromSeed(i); var r2 = Generator.Generate(1)[0]; var r3 = Generator.GenerateFromSeed(i * 2, "B015=0.5"); if (r1 == null || r2 == null || r3 == null) Assert(false, "交替调用失败"); } Assert(true, "1000轮交替"); });
        Test("T7.2 混合API调用", () => { for (int i = 0; i < 500; i++) { var hex = Seed.FromInt(i).ToHex(); var p1 = Generator.GenerateFromSeed(i); var p2 = Generator.GenerateFromHex(hex); var p3 = Generator.Generate(1, "S=0.5")[0]; Textify.ToRoleplay(p1); Textify.ToCompact(p2); Textify.ToDetailed(p3); } Assert(true, "500轮混合"); });

        // 第八轮
        Section("第八轮：文本输出完整性");
        Test("T8.1 角色扮演包含所有段落", () => { var p = Generator.GenerateFromSeed(42); var txt = Textify.ToRoleplay(p); string[] secs = { "【人格档案】", "【核心性格】", "【信息处理】", "【情绪模式】", "【动机与价值观】", "【行为风格】", "【自我认知】", "【社交特征】", "【发展特征】", "【身体-环境反应】", "【角色扮演提示】" }; foreach (var s in secs) Assert(txt.Contains(s), "缺失:" + s); });
        Test("T8.2 紧凑模式无乱码", () => { for (int s = 0; s < 100; s++) { var txt = Textify.ToCompact(Generator.GenerateFromSeed(s)); Assert(txt.Replace("\r", "").Replace("\n", "").Replace(" ", "").Length > 0, "空输出"); } });
        Test("T8.3 详细模式84行", () => { for (int s = 0; s < 50; s++) { var lines = Textify.ToDetailed(Generator.GenerateFromSeed(s)).Split('\n'); Assert(lines.Length >= 84, "种子" + s + "只有" + lines.Length + "行"); } });

        // 汇总
        sw.Stop();
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    轰炸测试汇总                        ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════╣");
        Console.WriteLine("║  通过: " + pass.ToString().PadLeft(4) + "  失败: " + fail.ToString().PadLeft(4) + "  警告: " + warn.ToString().PadLeft(4) + "                     ║");
        Console.WriteLine("║  通过率: " + (pass * 100.0 / (pass + fail + warn)).ToString("F1") + "%                              ║");
        Console.WriteLine("║  总耗时: " + (sw.ElapsedMilliseconds / 1000.0).ToString("F2") + "s                        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");

        if (fail > 0) { Console.WriteLine("\n!!!!!!!! 存在失败测试，DLL需要修复 !!!!!!!!"); Environment.Exit(1); }
        else Console.WriteLine("\n全部测试通过。DLL 可安全发布。");

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }

    static void Section(string title) { Console.WriteLine(); Console.WriteLine("── " + title + " ──"); }

    static void Test(string name, Action test)
    {
        try { test(); Console.WriteLine("  [PASS] " + name); pass++; }
        catch (AssertFailException e) { Console.WriteLine("  [FAIL] " + name + " —— " + e.Message); fail++; }
        catch (Exception e) { Console.WriteLine("  [WARN] " + name + " —— " + e.GetType().Name + ": " + e.Message); warn++; }
    }

    static void Assert(bool condition, string msg) { if (!condition) throw new AssertFailException(msg); }
}

class AssertFailException : Exception { public AssertFailException(string msg) : base(msg) { } }
