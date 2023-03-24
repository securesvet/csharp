using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private bool _treeContainsValue;
        public T Value { get; private set; }
        public BinaryTree<T> Left { get; private set; }
        public BinaryTree<T> Right { get; private set; }
        public void Add(T value)
        {
            // value > Value
            if (!_treeContainsValue)
            {
                Value = value;
                _treeContainsValue = true;
                return;
            }

            if (value.CompareTo(Value) > 0)
            {
                if (Right == null)
                {
                    Right = new BinaryTree<T>();
                }

                Right.Add(value);
            }
            else
            {
                if (Left == null)
                {
                    Left = new BinaryTree<T>();
                }

                Left.Add(value);
            }
        }

        // При помощи yield return выбрасываем коллекции в порядке возрастания
        public IEnumerator<T> GetEnumerator()
        {
            if (!_treeContainsValue)
            {
                yield break;
            }

            if (Left != null)
            {
                var temp = Left.GetEnumerator();
                // Пока есть в temp элементы
                while (temp.MoveNext())
                {
                    yield return temp.Current;
                }
            }

            yield return Value;

            if (Right != null)
            {
                var temp = Right.GetEnumerator();
                while (temp.MoveNext())
                {
                    yield return temp.Current;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] collection) where T : IComparable<T>
        {
            var tree = new BinaryTree<T>();
            foreach (var e in collection)
            {
                tree.Add(e);
            }

            return tree;
        }
    }
}