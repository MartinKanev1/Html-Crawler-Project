using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Crawler_Final_version.Data_Structures
{
    public class CustomStack<T> : IEnumerable<T>
    {
        private class Node
        {
            public T Value { get; set; }
            public Node Next { get; set; }

            public Node(T value)
            {
                Value = value;
                Next = null;
            }
        }

        private Node top; 
        public int Count { get; private set; } 

        public CustomStack()
        {
            top = null;
            Count = 0;
        }

        
        public void Push(T value)
        {
            Node newNode = new Node(value)
            {
                Next = top
            };
            top = newNode;
            Count++;
        }

        
        public T Pop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Стекът е празен.");
            }

            T value = top.Value;
            top = top.Next;
            Count--;
            return value;
        }

        
        public T Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Стекът е празен.");
            }

            return top.Value;
        }

        
        public bool IsEmpty()
        {
            return Count == 0;
        }

        
        public IEnumerator<T> GetEnumerator()
        {
            Node current = top;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
