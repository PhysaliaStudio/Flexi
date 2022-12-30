using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.GraphViewEditor
{
    public class NodeTypeSearchTree : IEnumerable<NodeTypeSearchTree.Node>
    {
        public class Node
        {
            private readonly Node parent;
            private readonly string text;
            private readonly Type type;
            private readonly int level;

            private readonly List<Node> children = new();

            public Node Parent => parent;
            public string Text => text;
            public Type Type => type;
            public int Level => level;
            public bool IsLeaf => children.Count == 0;
            public int ChildrenCount => children.Count;

            public Node(Node parent, string text, Type type)
            {
                this.parent = parent;
                this.text = text;
                this.type = type;
                level = parent != null ? parent.Level + 1 : 0;
            }

            public Node GetChild(int index)
            {
                return children[index];
            }

            public Node InsertOrGetChild(string text, Type type)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    int compare = text.CompareTo(children[i].Text);
                    if (compare == 0)
                    {
                        return children[i];
                    }
                    else if (compare < 0)
                    {
                        // The new child should be created before this child
                        var child = new Node(this, text, type);
                        children.Insert(i, child);
                        return child;
                    }
                }

                {
                    // The new child should be created at the last
                    var child = new Node(this, text, type);
                    children.Add(child);
                    return child;
                }
            }

            public Node ForceInsert(string text, Type type)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    int compare = text.CompareTo(children[i].Text);
                    if (compare < 0)
                    {
                        // The new child should be created before this child
                        var child = new Node(this, text, type);
                        children.Insert(i, child);
                        return child;
                    }
                }

                {
                    // The new child should be created at the last
                    var child = new Node(this, text, type);
                    children.Add(child);
                    return child;
                }
            }
        }

        public class Enumerator : IEnumerator<Node>
        {
            private readonly Node root;
            private readonly Stack<int> nodeIndexPath = new();

            private Node current;
            private int index;

            public Node Current => current;
            object IEnumerator.Current => current;

            public Enumerator(Node root)
            {
                this.root = root;
                current = root;
                index = -1;
            }

            public bool MoveNext()
            {
                index++;
                if (index >= current.ChildrenCount)
                {
                    if (current.Parent == null)
                    {
                        return false;
                    }
                    else
                    {
                        current = current.Parent;
                        index = nodeIndexPath.Pop();
                        return MoveNext();
                    }
                }
                else
                {
                    current = current.GetChild(index);
                    nodeIndexPath.Push(index);
                    index = -1;
                    return true;
                }
            }

            public void Reset()
            {
                current = root;
                index = -1;
            }

            public void Dispose()
            {

            }
        }

        private readonly Node root = new(null, "", null);

        public void Insert(string path, Type type)
        {
            string[] texts = path.Split('/');

            Node current = root;
            for (var i = 0; i < texts.Length; i++)
            {
                if (i != texts.Length - 1)
                {
                    current = current.InsertOrGetChild(texts[i], null);
                }
                else
                {
                    Node leaf = current.InsertOrGetChild(texts[i], type);
                    if (leaf.Type != type)  // Leaves conflict
                    {
                        Debug.LogWarning($"[{nameof(NodeSearchWindowProvider)}] Path conflict! \"{leaf.Type.FullName}\" and \"{type.FullName}\" use the same path: \"{path}\"");
                        current.ForceInsert(texts[i], type);
                    }
                }
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new Enumerator(root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(root);
        }
    }
}
