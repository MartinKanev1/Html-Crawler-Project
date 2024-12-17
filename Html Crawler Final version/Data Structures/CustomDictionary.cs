using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Crawler_Final_version.Data_Structures
{
    public class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private class Entry
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public bool IsDeleted { get; set; }

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                IsDeleted = false;
            }
        }

        private Entry[] buckets;
        private int count;

        public int Count => count;

        public CustomDictionary(int capacity = 16)
        {
            buckets = new Entry[capacity];
            count = 0;
        }

        public CustomDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            buckets = new Entry[16]; 
            count = 0;

            foreach (var pair in source)
            {
                Add(pair.Key, pair.Value);
            }
        }


        private int GetBucketIndex(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return Math.Abs(key.GetHashCode()) % buckets.Length;
        }

        public void Add(TKey key, TValue value)
        {
            if (count >= buckets.Length * 0.75)
            {
                Resize();
            }

            int index = GetBucketIndex(key);
            while (buckets[index] != null && !buckets[index].IsDeleted && !Equals(buckets[index].Key, key))
            {
                index = (index + 1) % buckets.Length;
            }

            if (buckets[index] == null || buckets[index].IsDeleted)
            {
                buckets[index] = new Entry(key, value);
                count++;
            }
            else
            {
                throw new ArgumentException("Ключът вече съществува.");
            }
        }

        public TValue Get(TKey key)
        {
            int index = FindIndex(key);
            if (index == -1)
            {
                throw new KeyNotFoundException("Ключът не е намерен.");
            }

            return buckets[index].Value;
        }

        public void Remove(TKey key)
        {
            int index = FindIndex(key);
            if (index == -1)
            {
                throw new KeyNotFoundException("Ключът не е намерен.");
            }

            buckets[index].IsDeleted = true;
            count--;
        }

        public bool ContainsKey(TKey key)
        {
            return FindIndex(key) != -1;
        }

        public TValue this[TKey key]
        {
            get => Get(key);
            set
            {
                int index = FindIndex(key);
                if (index == -1)
                {
                    Add(key, value);
                }
                else
                {
                    buckets[index].Value = value;
                }
            }
        }

        private int FindIndex(TKey key)
        {
            int index = GetBucketIndex(key);
            int startIndex = index;

            while (buckets[index] != null)
            {
                if (!buckets[index].IsDeleted && Equals(buckets[index].Key, key))
                {
                    return index;
                }

                index = (index + 1) % buckets.Length;

                if (index == startIndex)
                {
                    break;
                }
            }

            return -1;
        }

        private void Resize()
        {
            var oldBuckets = buckets;
            buckets = new Entry[buckets.Length * 2];
            count = 0;

            foreach (var entry in oldBuckets)
            {
                if (entry != null && !entry.IsDeleted)
                {
                    Add(entry.Key, entry.Value);
                }
            }
        }

        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var entry in buckets)
            {
                if (entry != null && !entry.IsDeleted)
                {
                    yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        
        
            
        

        public bool TryGetValue(TKey key, out TValue value)
        {
            
            int index = FindIndex(key);

            if (index != -1 && !buckets[index].IsDeleted)
            {
                value = buckets[index].Value; 
                return true; 
            }

            
            value = default; 
            return false; 
        }

    }
}
