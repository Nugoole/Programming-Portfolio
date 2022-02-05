using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LineScanViewer.Model
{
    public class Entry
    {
        public int Key { get; set; }
        public string Name { get; set; }
    }


    public class Node<T>
    {

    }

    public class Tree<T> : List<T>, INotifyPropertyChanged where T : class
    {
        public List<Tree<T>> ChildTrees { get; set; }

        public Tree()
        {
            ChildTrees = new List<Tree<T>>();
        }

        public void AddChild(T child)
        {
            var newTree = new Tree<T>();
            newTree.Add(child);
            ChildTrees.Add(newTree);
        }


        public new void Add(T item)
        {
            base.Add(item);
            RaisePropertyChanged();
        }

        public new void Clear()
        {
            base.Clear();
            RaisePropertyChanged();
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            RaisePropertyChanged();
        }

        public new void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            RaisePropertyChanged();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            RaisePropertyChanged();
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            RaisePropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
