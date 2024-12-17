using Html_Crawler_Final_version.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CustomLinkedList<T> : IEnumerable<T>
{
    public class Node
    {
        public T Value { get; set; }
        public Node Next { get; set; }

        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }

    public Node head;
    private Node tail;
    public int Count { get; private set; }

    public CustomLinkedList()
    {
        head = null;
        tail = null;
        Count = 0;
    }

    
    public void AddLast(T value)
    {
        Node newNode = new Node(value);

        if (head == null)
        {
            head = newNode;
            tail = newNode;
        }
        else
        {
            tail.Next = newNode;
            tail = newNode;
        }

        Count++;
    }

    public void AddBefore(Node node, T value)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        if (head == null) throw new InvalidOperationException("List is empty.");

        Node newNode = new Node(value);

        if (node == head) 
        {
            newNode.Next = head;
            head = newNode;
        }
        else
        {
            Node current = head;
            while (current != null && current.Next != node)
            {
                current = current.Next;
            }

            if (current == null) throw new InvalidOperationException("The specified node is not in the list.");

            newNode.Next = node;
            current.Next = newNode;
        }

        Count++;
    }

    
    public T GetAt(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException("Индексът е извън границите на списъка.");
        }

        Node current = head;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }

        return current.Value;
    }

    
    public void Remove(T value)
    {
        if (head == null)
        {
            return;
        }

        
        if ((head.Value == null && value == null) || (head.Value != null && head.Value.Equals(value)))
        {
            head = head.Next;
            if (head == null)
            {
                tail = null;
            }

            Count--;
            return;
        }

        Node current = head;
        while (current.Next != null)
        {
            if ((current.Next.Value == null && value == null) || (current.Next.Value != null && current.Next.Value.Equals(value)))
            {
                current.Next = current.Next.Next;
                if (current.Next == null)
                {
                    tail = current;
                }

                Count--;
                return;
            }

            current = current.Next;
        }
    }

    public void RemoveFirst()
    {
        if (head == null)
        {
            throw new InvalidOperationException("The list is empty.");
        }

        
        head = head.Next;

        
        if (head == null)
        {
            tail = null;
        }

        Count--;
    }


    
    public bool Contains(T value)
    {
        Node current = head;
        while (current != null)
        {
            if ((current.Value == null && value == null) || (current.Value != null && current.Value.Equals(value)))
            {
                return true;
            }

            current = current.Next;
        }

        return false;
    }

   
    public bool IsEmpty()
    {
        return Count == 0;
    }

    
    public IEnumerator<T> GetEnumerator()
    {
        Node current = head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    public void Clear()
    {
        head = null; 
        tail = null; 
        Count = 0;   
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

