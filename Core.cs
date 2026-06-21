using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalityGenerator
{
    // ═══════════════════════════════════════════════════════════
    // 参数定义
    // ═══════════════════════════════════════════════════════════
    internal class ParamDef
    {
        public string Id, Name, LowDesc, HighDesc;
        public double Min, Max;
        public bool Bipolar;
        public char Domain;

        public ParamDef(string id, char dom, string name, string low, string high,
            double min, double max, bool bipolar = false)
        {
            Id = id; Domain = dom; Name = name;
            LowDesc = low; HighDesc = high;
            Min = min; Max = max; Bipolar = bipolar;
        }

        public string Describe(double raw)
        {
            double n = (raw - Min) / (Max - Min);
            if (n < 0.2) return "[极低] " + LowDesc;
            if (n < 0.4) return "[偏低] " + LowDesc;
            if (n < 0.6) return "[中等] " + LowDesc + "与" + HighDesc + "之间";
            if (n < 0.8) return "[偏高] " + HighDesc;
            return "[极高] " + HighDesc;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 参数注册表（84个参数）
    // ═══════════════════════════════════════════════════════════
    internal static class Params
    {
        public static readonly List<ParamDef> All = new List<ParamDef>(84);

        static Params()
        {
            // A: 信息摄入 (A001-A010)
            Add("A001", 'A', "视觉采样率", "凝视锁定(1Hz)", "高速扫描(10Hz)", 1, 10);
            Add("A002", 'A', "听觉歧义容忍", "立即消歧(0s)", "无限悬置(10s)", 0, 10);
            Add("A003", 'A', "内感受分辨率", "完全不觉察", "每个心跳都清晰感知", 0, 1);
            Add("A004", 'A', "社会性线索优先级", "面孔=物体", "面孔自动捕获注意", 0, 1);
            Add("A005", 'A', "新异刺激打断阈值", "雷打不动(80dB)", "落叶惊心(20dB)", 20, 80);
            Add("A006", 'A', "背景-前景分离效率", "淹没在噪音中(500ms)", "鸡尾酒效应大师(50ms)", 50, 500);
            Add("A007", 'A', "预期违背消耗", "意外=无所谓(0%)", "意外=认知地震(100%)", 0, 100);
            Add("A008", 'A', "威胁线索放大系数", "威胁=客观威胁", "中性表情=敌意信号", 0, 1);
            Add("A009", 'A', "痛苦线索敏感度", "他人痛苦=背景噪音", "他人皱眉=自己心痛", 0, 1);
            Add("A010", 'A', "猎物/捕食者注意偏向", "注意流向弱者", "注意流向强者", -1, 1, true);

            // B: 情绪 (B011-B024)
            Add("B011", 'B', "基础情绪唤醒阈值", "极易唤醒", "极难唤醒", 0, 1);
            Add("B012", 'B', "情绪颗粒度", "感觉=一团糟", "恼火/愤懑/愠怒分明", 0, 100);
            Add("B013", 'B', "自动思维情绪附着力", "想法=纯认知", "一个想法=情绪炸弹", 0, 1);
            Add("B014", 'B', "情绪调节策略库", "只有本能反应", "20+种主动调节方法", 0, 20);
            Add("B015", 'B', "内疚感基线", "伤害他人=完全无感", "伤害他人=自我折磨", 0, 1);
            Add("B016", 'B', "他人痛苦-自身愉悦转化", "他人痛苦=不适", "他人痛苦=愉悦", -1, 1, true);
            Add("B017", 'B', "羞耻感基线", "出丑=无所谓", "出丑=想消失", 0, 1);
            Add("B018", 'B', "积极情绪维持能力", "快乐=瞬间", "快乐=持续一整天", 0, 1);
            Add("B019", 'B', "愤怒-攻击转化率", "愤怒=内心体验", "愤怒=立即行动", 0, 1);
            Add("B020", 'B', "情绪标签命名速度", "说不出感受(10s)", "瞬间精准命名(0.5s)", 0.5, 10);
            Add("B021", 'B', "情绪传染易感性", "他人哭泣=干眼", "他人哭泣=立刻泪崩", 0, 1);
            Add("B022", 'B', "怨恨衰减半衰期", "冒犯=秒忘(0天)", "冒犯=终身铭记(3650天)", 0, 3650);
            Add("B023", 'B', "嫉妒触发敏感度", "他人优势=完全无感", "微小差距=嫉妒燃烧", 0, 1);
            Add("B024", 'B', "幸灾乐祸阈限", "他人不幸=不适", "微小不幸=暗喜", 0, 1);

            // C: 动机与价值 (C025-C038)
            Add("C025", 'C', "趋近-回避基线", "默认后撤", "默认前倾", -1, 1, true);
            Add("C026", 'C', "意义寻求强度", "活着就好", "每件事都追问意义", 0, 100);
            Add("C027", 'C', "延迟折扣率", "只要现在(1.0)", "全押未来(0.0)", 0, 1);
            Add("C028", 'C', "自主性需求", "被指令=舒适", "被指令=本能反抗", 0, 100);
            Add("C029", 'C', "胜任感锚点", "永远不够好", "做了一点就够了", 0, 100);
            Add("C030", 'C', "冲动控制缓冲", "冲动=行动(0s)", "冲动…缓冲…行动(300s)", 0, 300);
            Add("C031", 'C', "支配-顺从倾向", "自愿服从", "必须主导", -1, 1, true);
            Add("C032", 'C', "权力动机", "影响他人=无感", "控制他人=核心驱力", 0, 1);
            Add("C033", 'C', "亲和动机", "人际=工具", "人际=目的", 0, 1);
            Add("C034", 'C', "地位渴求", "地位=无所谓", "地位=生命意义", 0, 1);
            Add("C035", 'C', "利他惩罚倾向", "不公=无视", "不公=自掏成本也要罚", 0, 1);
            Add("C036", 'C', "欺骗接受度", "谎言=不可接受", "谎言=合理工具", 0, 1);
            Add("C037", 'C', "价值-行为一致性", "说的≠做的", "言行完全一致", 0, 1);
            Add("C038", 'C', "刺激寻求", "平静=理想", "刺激=必需", 0, 1);

            // D: 行为执行 (D039-D042)
            Add("D039", 'D', "行为蓄能时间", "决定=行动(0s)", "决定…(∞)…行动(3600s)", 0, 3600);
            Add("D040", 'D', "攻击行为基线", "从不攻击", "主动攻击", 0, 1);
            Add("D041", 'D', "规则遵循度", "规则=建议", "规则=铁律", 0, 1);
            Add("D042", 'D', "行为灵活性", "受阻=卡死", "受阻=秒换方案", 0, 1);

            // E: 元认知与自我 (E043-E055)
            Add("E043", 'E', "思维标签化频率", "思维=透明", "频繁观察自己的思维", 0, 1);
            Add("E044", 'E', "反刍思维强度", "负面经历=翻篇", "负面经历=无限循环", 0, 1);
            Add("E045", 'E', "内隐自尊", "潜意识自我=负面", "潜意识自我=正面", -1, 1, true);
            Add("E046", 'E', "外显自尊", "声称的自我价值=低", "声称的自我价值=高", 0, 1);
            Add("E047", 'E', "自我感知校准度", "自我评价=严重偏差", "自我评价=客观精准", 0, 100);
            Add("E048", 'E', "道德推脱能力", "错=错", "错=可合理化", 0, 1);
            Add("E049", 'E', "责任归因偏向", "问题=我", "问题=世界", -1, 1, true);
            Add("E050", 'E', "自我批评强度", "错误=无视", "错误=自我鞭笞", 0, 1);
            Add("E051", 'E', "使命感清晰度", "为何而活=？", "为何而活=！", 0, 1);
            Add("E052", 'E', "道德-审美耦合度", "善≠美", "善=美", 0, 1);
            Add("E053", 'E', "矛盾共存耐受", "冲突=必须解决", "冲突=可以共存", 0, 1440);
            Add("E054", 'E', "框架重构力", "失败=失败", "失败=数据", 0, 1);
            Add("E055", 'E', "自我欺骗强度", "对自己诚实", "完全相信自己编织的谎言", 0, 1);

            // F: 社交信号 (F056-F062)
            Add("F056", 'F', "面部镜像延迟", "对方笑=瞬间同步(0ms)", "对方笑=无反应(2000ms)", 0, 2000);
            Add("F057", 'F', "自我暴露深度梯度", "初次见面=全盘托出", "十年好友=仍设防", 0, 100);
            Add("F058", 'F', "社交代价敏感度", "说'不'=轻松", "说'不'前模拟N种反应", 0, 1);
            Add("F059", 'F', "欺骗生理舒适度", "说谎=心跳加速", "说谎=心率完全平稳", 0, 1);
            Add("F060", 'F', "印象管理精细度", "不在乎形象", "精心设计每一面", 0, 1);
            Add("F061", 'F', "信任默认值", "陌生人=敌人", "陌生人=朋友", 0, 1);
            Add("F062", 'F', "背叛检测灵敏度", "利用=看不见", "蛛丝马迹=警觉", 0, 1);

            // G: 时间性与发展 (G063-G066)
            Add("G063", 'G', "参数漂移速率", "人格=固定", "人格=流动", 0, 1);
            Add("G064", 'G', "重大事件相变阈值", "什么事都改不了我", "小事也能改变我", 0, 100);
            Add("G065", 'G', "情境人格切换幅度", "在家=在职场", "在家≠在职场/判若两人", 0, 100);
            Add("G066", 'G', "身份叙事更新速率", "自我定义=固定", "自我定义=持续重写", 0, 1);

            // H: 身体-环境耦合 (H067-H084)
            Add("H067", 'H', "坐姿-思维关联", "驼背=无关", "驼背=消极想法↑", 0, 100);
            Add("H068", 'H', "呼吸-情绪耦联", "呼吸=自主", "呼吸=情绪晴雨表", -1, 1, true);
            Add("H069", 'H', "手势-语速锁定", "手势=随机", "手势=语音同步", 0, 500);
            Add("H070", 'H', "温度-社交距离", "冷=无关", "冷=想靠近", 0, 100);
            Add("H071", 'H', "饱腹-慷慨系数", "饿=自私", "饱=慷慨", -1, 1, true);
            Add("H072", 'H', "昼夜节律-创造力", "创造力=恒定", "创造力=时段函数", -100, 100);
            Add("H073", 'H', "微表情抑制力", "表情=自动", "表情=可控", 0, 1);
            Add("H074", 'H', "疼痛-攻击链接", "痛=痛", "痛=想攻击", 0, 1);
            Add("H075", 'H', "光照-决策速度", "暗=决策不变", "暗=决策延迟", 0, 60);
            Add("H076", 'H', "运动-情绪提升", "运动=纯身体", "运动=情绪药", 0, 200);
            Add("H077", 'H', "睡眠债务-认知衰减", "少睡=无影响", "少睡=认知崩溃", 0, 50);
            Add("H078", 'H', "噪音-压力耦联", "噪音=背景", "噪音=皮质醇↑", 0, 100);
            Add("H079", 'H', "气味-记忆唤起率", "气味=气味", "气味=时光机", 0, 1);
            Add("H080", 'H', "触觉-信任关联", "被触=无关", "被触=信任变化", -1, 1, true);
            Add("H081", 'H', "饥饿-风险偏好", "饿=保守", "饿=冒险", -1, 1, true);
            Add("H082", 'H', "姿势-权力感映射", "姿势=姿势", "扩张姿势=睾酮↑", 0, 100);
            Add("H083", 'H', "温度-攻击性", "热=无影响", "热=攻击↑", 0, 100);
            Add("H084", 'H', "海拔-思维抽象度", "高度=无关", "高度=抽象思维↑", -1, 1, true);
        }

        static void Add(string id, char dom, string name, string low, string high,
            double min, double max, bool bipolar = false)
        {
            All.Add(new ParamDef(id, dom, name, low, high, min, max, bipolar));
        }

        public static ParamDef Get(string id)
        {
            foreach (var p in All) if (p.Id == id) return p;
            return null;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 人格参数堆 —— 一次性生成结果
    // ═══════════════════════════════════════════════════════════
    public class Personality
    {
        public double[] Values;     // 84个归一化值 [0,1]
        public bool[] Missing;      // 84个缺失标记
        public string Fingerprint;  // 指纹

        internal Personality(double[] vals, bool[] miss)
        {
            Values = vals; Missing = miss;
            var sb = new StringBuilder();
            int c = 0;
            for (int i = 0; i < 84 && c < 4; i++)
            {
                if (!miss[i]) { sb.Append(vals[i].ToString("F4")); if (c < 3) sb.Append("|"); c++; }
            }
            // 补充缺失情况下的兜底
            if (c == 0) sb.Append("ALL_MISSING");
            Fingerprint = sb.ToString();
        }

        public double Get(string id)
        {
            if (Generator._indexCache.TryGetValue(id, out int i))
                return Values[i];
            return 0.5;
        }

        public int MissingCount { get { int n = 0; for (int i = 0; i < 84; i++) if (Missing[i]) n++; return n; } }
    }

    // ═══════════════════════════════════════════════════════════
    // 偏向配置
    // ═══════════════════════════════════════════════════════════
    public class Bias
    {
        public double[] Biases = new double[84];
        public double Strength = 0.7; // 默认强度提升到0.7，让"B015=1.0"直接有效

        public Bias() { for (int i = 0; i < 84; i++) Biases[i] = 0; }

        public Bias Set(string id, double v)
        {
            if (Generator._indexCache.TryGetValue(id, out int i))
                Biases[i] = Clamp(v, -1, 1);
            return this;
        }

        public Bias SetDomain(char dom, double v)
        {
            for (int i = 0; i < Params.All.Count; i++)
                if (Params.All[i].Domain == dom) Biases[i] = Clamp(v, -1, 1);
            return this;
        }

        public Bias SetStrength(double s) { Strength = Clamp(s, 0, 1); return this; }

        public static Bias Parse(string spec)
        {
            var b = new Bias();
            if (string.IsNullOrEmpty(spec)) return b;
            foreach (var part in spec.Split(',', ';'))
            {
                var kv = part.Split('=');
                if (kv.Length != 2) continue;
                string key = kv[0].Trim().ToUpper();
                double val;
                if (!double.TryParse(kv[1].Trim(),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out val))
                    continue;

                if (key.Length == 1 && key[0] >= 'A' && key[0] <= 'H')
                    b.SetDomain(key[0], val);
                else if (key == "STRENGTH" || key == "S" || key == "STR")
                    b.SetStrength(val);
                else
                    b.Set(key, val);
            }
            return b;
        }

        static double Clamp(double v, double lo, double hi) { return v < lo ? lo : v > hi ? hi : v; }
    }

    // ═══════════════════════════════════════════════════════════
    // 核心生成器 —— 唯一的公共入口
    // ═══════════════════════════════════════════════════════════
    public static class Generator
    {
        // 参数索引查找缓存
        internal static readonly Dictionary<string, int> _indexCache = new Dictionary<string, int>();
        static Generator()
        {
            for (int i = 0; i < Params.All.Count; i++)
                _indexCache[Params.All[i].Id] = i;
        }

        /// <summary>生成一批人格。数字=数量，"B015=0.9,C031=-0.7"=偏向。</summary>
        public static Personality[] Generate(int count, string biasSpec = null)
        {
            if (count <= 0) count = 1;
            var bias = Bias.Parse(biasSpec);
            var results = new Personality[count];
            int baseSeed = Environment.TickCount;

            for (int i = 0; i < count; i++)
                results[i] = GenerateOne(Seed.FromInt(baseSeed + i), bias);

            return results;
        }

        /// <summary>从指定整数种子生成单个人格。</summary>
        public static Personality GenerateFromSeed(int seed, string biasSpec = null)
        {
            return GenerateOne(Seed.FromInt(seed), Bias.Parse(biasSpec));
        }

        /// <summary>从十六进制种子生成单个人格。</summary>
        public static Personality GenerateFromHex(string hex, string biasSpec = null)
        {
            return GenerateOne(Seed.FromString(hex), Bias.Parse(biasSpec));
        }

        static Personality GenerateOne(Seed seed, Bias bias)
        {
            seed.Reset();
            double[] vals = new double[84];
            bool[] miss = new bool[84];

            for (int i = 0; i < 84; i++)
            {
                var def = Params.All[i];
                bool missing = false;
                double raw;

                // 缺失判断：种子耗尽时优雅降级
                try { missing = seed.ReadFloat() < 0.15; }
                catch { missing = true; }

                if (missing)
                {
                    raw = def.Bipolar ? 0 : (def.Min + def.Max) / 2;
                }
                else
                {
                    try
                    {
                        double rnd = def.Bipolar
                            ? seed.ReadDouble() * 2.0 - 1.0   // [-1, 1)
                            : seed.ReadDouble();                // [0, 1)

                        // 偏向：非线性拉动力，极端偏向自动获得更强拉力
                        if (bias != null && bias.Strength > 0)
                        {
                            double eb = bias.Biases[i];
                            if (Math.Abs(eb) > 0.001)
                            {
                                // 有效拉力 = strength × |eb|^0.5
                                // 平方根使极端值(1.0)拉力=1.0，温和值(0.25)拉力=0.5
                                double effectivePull = bias.Strength * Math.Sqrt(Math.Abs(eb));

                                if (def.Bipolar)
                                {
                                    // 目标就是偏向值本身
                                    rnd += (eb - rnd) * effectivePull;
                                    if (rnd < -1.0) rnd = -1.0;
                                    if (rnd > 1.0) rnd = 1.0;
                                }
                                else
                                {
                                    // 目标：eb>0→1.0, eb<0→0.0
                                    double tgt = eb > 0 ? 1.0 : 0.0;
                                    rnd += (tgt - rnd) * effectivePull;
                                    if (rnd < 0.0) rnd = 0.0;
                                    if (rnd > 1.0) rnd = 1.0;
                                }
                            }
                        }

                        raw = def.Bipolar
                            ? (def.Min + def.Max) / 2.0 + rnd * (def.Max - def.Min) / 2.0
                            : def.Min + rnd * (def.Max - def.Min);
                    }
                    catch
                    {
                        raw = def.Bipolar ? 0 : (def.Min + def.Max) / 2.0;
                        missing = true;
                    }
                }

                if (raw < def.Min) raw = def.Min;
                if (raw > def.Max) raw = def.Max;

                vals[i] = def.Max > def.Min ? (raw - def.Min) / (def.Max - def.Min) : 0.5;
                miss[i] = missing;
            }

            return new Personality(vals, miss);
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 文本生成器
    // ═══════════════════════════════════════════════════════════
    public static class Textify
    {
        /// <summary>将人格转换为角色扮演人设文本。</summary>
        public static string ToRoleplay(Personality p)
        {
            var sb = new StringBuilder();
            sb.AppendLine("【人格档案】");
            sb.AppendLine("指纹: " + p.Fingerprint + " | 缺失: " + p.MissingCount + "/84");
            sb.AppendLine();

            sb.AppendLine("【核心性格】");
            sb.AppendLine(CoreTraits(p));
            sb.AppendLine();

            char[] doms = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            string[] domNames = { "信息处理", "情绪模式", "动机与价值观", "行为风格",
                                  "自我认知", "社交特征", "发展特征", "身体-环境反应" };
            for (int d = 0; d < doms.Length; d++)
            {
                sb.AppendLine("【" + domNames[d] + "】");
                sb.AppendLine(DomainDesc(p, doms[d]));
                sb.AppendLine();
            }

            sb.AppendLine("【角色扮演提示】");
            sb.AppendLine(RoleHints(p));
            return sb.ToString();
        }

        /// <summary>紧凑模式。</summary>
        public static string ToCompact(Personality p)
        {
            var sb = new StringBuilder();
            sb.AppendLine("指纹: " + p.Fingerprint + " | 缺失: " + p.MissingCount + "/84");
            for (int i = 0; i < 84; i++)
            {
                if (p.Missing[i]) continue;
                double dev = Math.Abs(p.Values[i] - 0.5);
                if (dev > 0.3)
                {
                    string arrow = p.Values[i] > 0.5 ? "↑" : "↓";
                    sb.AppendLine("  " + Params.All[i].Id + " " + Params.All[i].Name + ": " + arrow + " (" + p.Values[i].ToString("F2") + ")");
                }
            }
            return sb.ToString();
        }

        /// <summary>详细模式。</summary>
        public static string ToDetailed(Personality p)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 84; i++)
            {
                var def = Params.All[i];
                string desc = p.Missing[i] ? "[缺失]" : def.Describe(def.Min + p.Values[i] * (def.Max - def.Min));
                sb.AppendLine(def.Id + " " + def.Name + ": " + desc);
            }
            return sb.ToString();
        }

        static string CoreTraits(Personality p)
        {
            var sb = new StringBuilder();
            double c025 = p.Get("C025"), c033 = p.Get("C033");
            if (c025 > 0.6 && c033 > 0.5) sb.AppendLine("  • 外向亲和——主动接近他人，渴望温暖关系");
            else if (c025 < 0.4 && c033 < 0.5) sb.AppendLine("  • 内向疏离——倾向独处，人际非核心需求");
            else if (c025 > 0.6 && c033 < 0.5) sb.AppendLine("  • 外向工具型——主动社交但视人际为手段");

            double b015 = p.Get("B015"), b021 = p.Get("B021");
            if (b015 > 0.6 && b021 > 0.5) sb.AppendLine("  • 高共情——易被感染，伤害他人后强烈内疚");
            else if (b015 < 0.4 && b021 < 0.5) sb.AppendLine("  • 情感淡漠——不易感染，伤害他人较少内疚");

            double c032 = p.Get("C032"), e051 = p.Get("E051");
            if (c032 > 0.6 && e051 > 0.5) sb.AppendLine("  • 使命型权力——权力是完成使命的工具");
            else if (c032 > 0.6) sb.AppendLine("  • 权力导向——渴望影响和控制他人");
            else if (e051 > 0.6) sb.AppendLine("  • 使命驱动——有清晰的人生目标");

            double d040 = p.Get("D040"), c030 = p.Get("C030");
            if (d040 > 0.6 && c030 < 0.4) sb.AppendLine("  • 冲动攻击——高攻击倾向且缺乏控制");
            else if (d040 > 0.6 && c030 > 0.6) sb.AppendLine("  • 克制攻击——有攻击倾向但能有效控制");
            else if (d040 < 0.4) sb.AppendLine("  • 和平倾向——攻击基线较低");

            return sb.ToString().TrimEnd();
        }

        static string DomainDesc(Personality p, char dom)
        {
            var sb = new StringBuilder();
            int count = 0;
            for (int i = 0; i < 84; i++)
            {
                if (Params.All[i].Domain != dom || p.Missing[i]) continue;
                double dev = Math.Abs(p.Values[i] - 0.5);
                if (dev > 0.2)
                {
                    var def = Params.All[i];
                    double raw = def.Min + p.Values[i] * (def.Max - def.Min);
                    sb.AppendLine("  " + def.Describe(raw));
                    count++;
                }
            }
            if (count == 0) sb.AppendLine("  [表现均衡，无显著极端特征]");
            return sb.ToString().TrimEnd();
        }

        static string RoleHints(Personality p)
        {
            var sb = new StringBuilder();
            double c028 = p.Get("C028");
            if (c028 > 0.7) sb.AppendLine("  • 说话风格：独立自主，抵触被指挥");
            else if (c028 < 0.3) sb.AppendLine("  • 说话风格：顺从配合，接受安排");

            double b017 = p.Get("B017");
            if (b017 > 0.7) sb.AppendLine("  • 情绪表达：容易羞耻，社交失误后长时间纠结");
            else if (b017 < 0.3) sb.AppendLine("  • 情绪表达：不在意他人眼光，恢复快");

            double c030 = p.Get("C030"), d039 = p.Get("D039");
            if (c030 < 0.3) sb.AppendLine("  • 决策风格：冲动型，快速决定立即行动");
            else if (d039 > 0.7) sb.AppendLine("  • 决策风格：拖延型，决定后需较长时间启动");
            else sb.AppendLine("  • 决策风格：平衡型，思考与行动节奏合理");

            double f061 = p.Get("F061"), f062 = p.Get("F062");
            if (f061 < 0.3 && f062 > 0.7) sb.AppendLine("  • 人际关系：高度警觉，默认不信任，时刻准备发现背叛");
            else if (f061 > 0.7) sb.AppendLine("  • 人际关系：容易信任，对陌生人持开放态度");
            else sb.AppendLine("  • 人际关系：谨慎开放，需时间建立信任");

            return sb.ToString().TrimEnd();
        }
    }
}
