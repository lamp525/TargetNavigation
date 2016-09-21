using System;
using System.Collections.Generic;
using System.Reflection;

namespace MB.New.Common
{
    public class ModelMapping
    {
        public static K JsonMapping<T, K>(T source)
            where T : class
            where K : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<K>(Newtonsoft.Json.JsonConvert.SerializeObject(source));
        }

        /// <summary>
        /// 单个对象映射
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <typeparam name="K">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="dest">目标对象</param>
        public static void SingleMapping<T, K>(T source, K dest)
            where T : class
            where K : class
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            // 取得源对象中所有的属性
            PropertyInfo[] sourcePropertyInfo = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo propertyInfo = null;

            // 循环源对象的所有类型
            foreach (var item in sourcePropertyInfo)
            {
                // 取得目标对象的对应类型
                propertyInfo = dest.GetType().GetProperty(item.Name);

                // 类型不存在
                if (propertyInfo == null)
                {
                    continue;
                }

                // 目标对象中类型存在，将源对象中值拷贝
                propertyInfo.SetValue(dest, item.GetValue(source));
            }
        }

        /// <summary>
        /// 复数对象映射
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <typeparam name="K">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="dest">目标对象</param>
        public static void MultiMapping<T, K>(List<T> source, List<K> dest)
            where T : class
            where K : class
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            // 对象为空，返回
            if (source.Count == 0)
            {
                return;
            }

            // 取得源对象中所有的属性
            PropertyInfo[] sourcePropertyInfo = source[0].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            PropertyInfo propertyInfo = null;

            // 取得目标对象的类型
            Type type = dest.GetType().GetGenericArguments()[0];

            // 循环源对象
            for (int i = 0; i < source.Count; i++)
            {
                // 创建目标对象
                dest.Add((K)Activator.CreateInstance(type));

                // 循环源对象的所有类型
                foreach (var item in sourcePropertyInfo)
                {
                    // 取得目标对象的对应类型
                    propertyInfo = dest[i].GetType().GetProperty(item.Name);

                    // 类型不存在
                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    // 目标对象中类型存在，将源对象中值拷贝
                    propertyInfo.SetValue(dest[i], item.GetValue(source[i]));
                }
            }
        }
    }
}