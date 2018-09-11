//
// Util.cs - The Building Coder Revit API utility methods
//
// Copyright (C) 2008-2018 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

namespace BuildingCoder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtensions
    {
        //// (C) Jonathan Skeet
        //// from https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/MinBy.cs
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MinBy<TSource, TKey>(
          this IEnumerable<TSource> source,
          Func<TSource, TKey> selector,
          IComparer<TKey> comparer)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null) {
                throw new ArgumentNullException(nameof(selector));
            }
            if (comparer == null) {
                throw new ArgumentNullException(nameof(comparer));
            }
            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator()) {
                if (!sourceIterator.MoveNext()) {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext()) {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0) {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// Create HashSet from IEnumerable given selector and comparer.
        /// http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/31/c.net-toolbox-adding-a-tohashset-extension-method.aspx
        /// </summary>
        public static HashSet<TElement> ToHashSet<TSource, TElement>(
          this IEnumerable<TSource> source,
          Func<TSource, TElement> elementSelector,
          IEqualityComparer<TElement> comparer)
        {
            if (source == null) {
                throw new ArgumentNullException("source");
            }
            if (elementSelector == null) {
                throw new ArgumentNullException("elementSelector");
            }

            //// you can unroll this into a foreach if you want efficiency gain, but for brevity...
            return new HashSet<TElement>(source.Select(elementSelector), comparer);
        }

        /// <summary>
        /// Create a HashSet of TSource from an IEnumerable
        /// of TSource using the identity selector and
        /// default equality comparer.
        /// </summary>
        public static HashSet<TSource> ToHashSet<TSource>(
          this IEnumerable<TSource> source)
        {
            // key selector is identity fxn and null is default comparer
            return source.ToHashSet<TSource, TSource>(
              item => item, null);
        }

        /// <summary>
        /// Create a HashSet of TSource from an IEnumerable
        /// of TSource using the identity selector and
        /// specified equality comparer.
        /// </summary>
        public static HashSet<TSource> ToHashSet<TSource>(
          this IEnumerable<TSource> source,
          IEqualityComparer<TSource> comparer)
        {
            return source.ToHashSet<TSource, TSource>(
              item => item, comparer);
        }

        /// <summary>
        /// Create a HashSet of TElement from an IEnumerable
        /// of TSource using the specified element selector
        /// and default equality comparer.
        /// </summary>
        public static HashSet<TElement> ToHashSet<TSource, TElement>(
          this IEnumerable<TSource> source,
          Func<TSource, TElement> elementSelector)
        {
            return source.ToHashSet<TSource, TElement>(
              elementSelector, null);
        }
    }
}