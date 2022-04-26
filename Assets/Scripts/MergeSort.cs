using System;
using System.Collections.Generic;
internal static class MergeSort<T>
{
    public static void Sort(List<T> list, Comparison<T> comparison)
    {
        if (list.Count <= 1)
            return;
        var mid = list.Count / 2;
        var left = GetList();
        var right = GetList();
        for (var i = 0; i < mid; ++i)
        {
            left.Add(list[i]);
        }
        for (var i = mid; i < list.Count; ++i)
        {
            right.Add(list[i]);
        }
        Sort(left, comparison);
        Sort(right, comparison);
        Merge(list, left, right, comparison);
        ReturnList(left);
        ReturnList(right);
    }
    private static void Merge(List<T> merged, List<T> left, List<T> right, Comparison<T> comparison)
    {
        merged.Clear();
        while (left.Count > 0 && right.Count > 0)
        {
            if (comparison(left[0], right[0]) > 0)
            {
                merged.Add(right[0]);
                right.RemoveAt(0);
            }
            else
            {
                merged.Add(left[0]);
                left.RemoveAt(0);
            }
        }
        for (var i = 0; i < left.Count; ++i)
        {
            merged.Add(left[i]);
        }
        for (var i = 0; i < right.Count; ++i)
        {
            merged.Add(right[i]);
        }
    }
    private static Queue<List<T>> listPool_ = new Queue<List<T>>();
    private static List<T> GetList()
    {
        if (listPool_.Count > 0)
        {
            var list = listPool_.Dequeue();
            list.Clear();
            return list;
        }
        return new List<T>();
    }
    private static void ReturnList(List<T> list)
    {
        listPool_.Enqueue(list);
    }
}
