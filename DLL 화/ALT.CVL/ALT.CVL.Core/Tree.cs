using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Core
{

    public class Node<T> : INotifyPropertyChanged
    {
        private T content;

        public T Content
        {
            get => content; set
            {
                content = value;
                RaisePropertyChanged();
            }
        }

        public Node<T> ParentNode { get; private set; }

        public ObservableCollection<Node<T>> ChildNodes { get; set; }

        public Node()
        {
            ChildNodes = new ObservableCollection<Node<T>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public void AddChild(T newContent)
        {
            ChildNodes.Add(new Node<T>() { Content = newContent });
            ChildNodes.Last().ParentNode = this;
        }
    }

    public class Tree<T> : IEnumerable<T>
    {
        private int level;
        private bool isLastNode = false;

        public Node<T> MainNode { get; set; }

        public Tree()
        {
            MainNode = new Node<T>();
        }

        public void Clear()
        {
            MainNode.ChildNodes.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in GetNextVal(MainNode))
            {
                yield return item;
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in GetNextVal(MainNode))
            {
                yield return item;
            }
        }

        private IEnumerable<T> GetNextVal(Node<T> CurrNode)
        {
            if (CurrNode.ChildNodes.Count != 0)
            {
                level++;
                foreach (Node<T> item in CurrNode.ChildNodes)
                {
                    foreach (T item2 in GetNextVal(item))
                        yield return item2;
                }
            }

            if (level == 0)
                isLastNode = true;
            else
                level--;
            yield return CurrNode.Content;
        }
    }


    class TreeNodeEnumerator<T> : IEnumerator<T>
    {
        private bool isLastNode = false;
        private int listIndex = -1;
        private Collection<Node<T>> CurrentNodeList;
        private Node<T> CurrentNode;
        private int level;

        public T Current
        {
            get
            {
                return default(T);
                

            }
        }
        object IEnumerator.Current => Current;

        public TreeNodeEnumerator(Node<T> topNode)
        {
            CurrentNode = topNode;

            
        }

        public void Dispose()
        {
            CurrentNodeList = null;
            CurrentNode = null;
            listIndex = -1;
        }

        public bool MoveNext()
        {
            return isLastNode;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        private IEnumerable<T> GetNextVal(Node<T> CurrNode)
        {
            if (CurrNode.ChildNodes.Count != 0)
            {
                level++;
                foreach (Node<T> item in CurrNode.ChildNodes)
                {
                    foreach (T item2 in GetNextVal(item))
                        yield return item2;
                }
                level--;
            }

            if (level == 0)
                isLastNode = true;
            yield return CurrNode.Content;
        }
    }
}
