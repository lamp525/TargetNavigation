using System;
using System.Collections.Generic;
using System.Text;

namespace MB.New.Common
{
    public class StringUtility
    {
        /// <summary>
        /// 将字符串中的条件表达式按照正常的符号判断
        /// </summary>
        /// <param name="expression">条件表达式如：(true||flase)&&true  /param>
        /// <returns>返回计算的值，可以为任意合法的数据类型</returns>
        public static object StringCalculate(string expression)
        {
            object retvar = null;

            var provider = new Microsoft.CSharp.CSharpCodeProvider();
            var cp = new System.CodeDom.Compiler.CompilerParameters(new string[] { @"System.dll" });
            var builder = new StringBuilder("using System;class CalcExp{public static object Calc(){ return \"expression\";}}");
            builder.Replace("\"expression\"", expression);
            var code = builder.ToString();
            var results = provider.CompileAssemblyFromSource(cp, new string[] { code });
            if (results.Errors.HasErrors)
            {
                retvar = null;
            }
            else
            {
                System.Reflection.Assembly a = results.CompiledAssembly;
                Type t = a.GetType("CalcExp");
                retvar = t.InvokeMember("Calc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.InvokeMethod, System.Type.DefaultBinder, null, null);
            }
            return retvar;
        }

        /// <summary>
        /// 把list转换为一个用指定分隔的字符串
        /// </summary>
        /// <param name="target">字符串数组</param>
        /// <param name="splitChar">分隔符</param>
        /// <returns>指定符号分隔的字符串</returns>
        public static string ListToString(List<string> target, string splitChar)
        {
            if (target == null)
            {
                return null;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < target.Count; i++)
            {
                if (i < target.Count - 1)
                {
                    sb.Append(target[i] + splitChar);
                }
                else
                {
                    sb.Append(target[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 字母数字字符随机数
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>随机字符串</returns>
        public static string GetNumcharRandom(int length)
        {
            return MakeRandomCharacter("NUMCHAR", length);
        }

        /// <summary>
        /// 生成各类随机字符,包括纯字母,纯数字,带特殊字符等,除非字母大写密码类型,其余方式都将采用小写密码
        /// </summary>
        /// <param name="characterType">字符类型 大写为"UPPER",小写为"LOWER",数字为"NUMBER",字母与数字为"NUMCHAR",数字字母字符都包括为"ALL" </param>
        /// <param name="length">字符长度,最小为4位</param>
        /// <returns></returns>
        private static string MakeRandomCharacter(string characterType, int length)
        {
            // 定义密码字符的范围,小写,大写字母,数字以及特殊字符
            string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numnberChars = "0123456789";
            // "\" 转义字符不添加 "号不添加
            string specialCahrs = "~!@#$%^&*()_+|-=,./[]{}:;':";

            string tmpStr = "";

            int iRandNum;
            Random rnd = new Random();

            // 密码长度必须大于4,否则自动取4
            length = (length < 4) ? 4 : length;

            // LOWER为小写 UPPER为大写 NUMBER为数字 NUMCHAR为数字和字母 ALL全部包含 五种方式
            // 只有当选择UPPER才会有大写字母产生,其余方式中的字母都为小写,避免有些时候字母不区分大小写
            if (characterType == "LOWER")
            {
                for (int i = 0; i < length; i++)
                {
                    iRandNum = rnd.Next(lowerChars.Length);
                    tmpStr += lowerChars[iRandNum];
                }
            }
            else if (characterType == "UPPER")
            {
                for (int i = 0; i < length; i++)
                {
                    iRandNum = rnd.Next(upperChars.Length);
                    tmpStr += upperChars[iRandNum];
                }
            }
            else if (characterType == "NUMBER")
            {
                for (int i = 0; i < length; i++)
                {
                    iRandNum = rnd.Next(numnberChars.Length);
                    tmpStr += numnberChars[iRandNum];
                }
            }
            else if (characterType == "NUMCHAR")
            {
                int numLength = rnd.Next(length);

                // 去掉随机数为0的情况
                if (numLength == 0)
                {
                    numLength = 1;
                }
                int charLength = length - numLength;
                string rndStr = "";
                for (int i = 0; i < numLength; i++)
                {
                    iRandNum = rnd.Next(numnberChars.Length);
                    tmpStr += numnberChars[iRandNum];
                }
                for (int i = 0; i < charLength; i++)
                {
                    iRandNum = rnd.Next(lowerChars.Length);
                    tmpStr += lowerChars[iRandNum];
                }

                // 将取得的字符串随机打乱
                for (int i = 0; i < length; i++)
                {
                    int n = rnd.Next(tmpStr.Length);

                    // 去除n随机为0的情况
                    // n = (n == 0) ? 1 : n;
                    rndStr += tmpStr[n];
                    tmpStr = tmpStr.Remove(n, 1);
                }
                tmpStr = rndStr;
            }
            else if (characterType == "ALL")
            {
                int numLength = rnd.Next(length - 1);

                // 去掉随机数为0的情况
                if (numLength == 0)
                {
                    numLength = 1;
                }
                int charLength = rnd.Next(length - numLength);
                if (charLength == 0)
                {
                    charLength = 1;
                }
                int specCharLength = length - numLength - charLength;
                string rndStr = "";
                for (int i = 0; i < numLength; i++)
                {
                    iRandNum = rnd.Next(numnberChars.Length);
                    tmpStr += numnberChars[iRandNum];
                }
                for (int i = 0; i < charLength; i++)
                {
                    iRandNum = rnd.Next(lowerChars.Length);
                    tmpStr += lowerChars[iRandNum];
                }
                for (int i = 0; i < specCharLength; i++)
                {
                    iRandNum = rnd.Next(specialCahrs.Length);
                    tmpStr += specialCahrs[iRandNum];
                }
                // 将取得的字符串随机打乱
                for (int i = 0; i < length; i++)
                {
                    int n = rnd.Next(tmpStr.Length);
                    // 去除n随机为0的情况
                    // n = (n == 0) ? 1 : n;
                    rndStr += tmpStr[n];
                    tmpStr = tmpStr.Remove(n, 1);
                }
                tmpStr = rndStr;
            }
            // 默认将返回数字类型的密码
            else
            {
                for (int i = 0; i < length; i++)
                {
                    iRandNum = rnd.Next(numnberChars.Length);
                    tmpStr += numnberChars[iRandNum];
                }
            }
            return tmpStr;
        }
    }
}