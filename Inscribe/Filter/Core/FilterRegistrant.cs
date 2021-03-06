﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inscribe.Filter.Core
{
    /// <summary>
    /// フィルタレジステーションポイント
    /// </summary>
    public static class FilterRegistrant
    {
        static FilterRegistrant()
        {
            // とりあえず Athenaeumにあるフィルタを登録
            RegisterAllFiltersInAsm(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// レジストされたフィルタ
        /// </summary>
        private static List<Type> filterTypes = new List<Type>();

        private static List<Tuple<Type, string>> filterLookup = new List<Tuple<Type, string>>();

        public static IEnumerable<string> FilterKeys
        {
            get { return filterLookup.Select(f => f.Item2); }
        }

        /// <summary>
        /// 登録されているフィルタ型一覧を取得します。
        /// </summary>
        public static IEnumerable<Type> RegisteredFilters
        {
            get { return filterTypes; }
        }

        /// <summary>
        /// 指定したID/エイリアスを持つフィルタを取得します。
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetFilter(string identifier)
        {
            return filterLookup.Where(i => i.Item2 == identifier).Select(i => i.Item1);
        }

        /// <summary>
        /// フィルタシステムにエレメントオブジェクトを登録します。
        /// </summary>
        /// <param name="type">登録する型(public T : Athenaeum.Filter.FilterBase)</param>
        /// <returns>登録されればtrue</returns>
        public static bool RegisterFilter(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsPublic)
            {
                throw new ArgumentException("登録するフィルタはパブリッククラスである必要があります。");
            }
            if(!type.IsSubclassOf(typeof(FilterBase)))
            {
                throw new ArgumentException("登録するフィルタはフィルタベースクラス(Athenaeum.Filter.FilterBase)から派生している必要があります。");
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException("抽象クラスフィルタは登録できません。");
            }
            if (type.IsGenericType)
            {
                throw new ArgumentException("ジェネリッククラスフィルタは登録できません。");
            }
            if (!filterTypes.Contains(type))
            {
                FilterBase inst = null;
                // Constructor not found?
                var ci = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];
                if (ci != null)
                    inst = ci.Invoke(new object[0]) as FilterBase;
                if (inst == null)
                    throw new ArgumentException("フィルタを作成できません。");
                System.Diagnostics.Debug.WriteLine("Join:" + type.ToString());
                filterTypes.Add(type);
                filterLookup.Add(new Tuple<Type, string>(type, inst.Identifier));
                foreach (var alias in inst.Aliases ?? new string[0])
                {
                    filterLookup.Add(new Tuple<Type, string>(type, alias));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// フィルタシステムにエレメントオブジェクトを登録します。
        /// </summary>
        /// <param name="type">登録する型(Athenaeum.Filter.FilterBase)</param>
        /// <returns>登録されればtrue</returns>
        public static bool RegisterFilter<T>()
            where T : FilterBase
        {
            return RegisterFilter(typeof(T));
        }

        /// <summary>
        /// 実行中のアセンブリに存在するすべてのフィルタを列挙し、レジストラントに追加します。
        /// </summary>
        public static void RegisterAllFiltersInAsm(Assembly asm)
        {

            foreach (var type in asm.GetTypes())
            {
                if (type.IsSubclassOf(typeof(FilterBase)))
                {
                    try
                    {
                        RegisterFilter(type);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
