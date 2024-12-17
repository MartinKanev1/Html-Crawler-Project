using Html_Crawler_Final_version.Data_Structures;
using Html_Crawler_Final_version.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public class HuffmanNode
    {
        public byte? Byte { get; set; } 
        public int Freq { get; set; } 
        public HuffmanNode Left { get; set; } 
        public HuffmanNode Right { get; set; } 
    }

    public class HuffmanCompression
    {
        private const int BYTE_VALUES = 256;

       

    public static HuffmanNode BuildTree(byte[] content)
    {
        var bytesFreqs = new int[256];

        
        for (int c = 0; c < content.Length; c++)
            bytesFreqs[content[c]]++;

        
        var linkedList = new CustomLinkedList<HuffmanNode>();
        for (int i = 0; i < bytesFreqs.Length; i++)
        {
            if (bytesFreqs[i] > 0)
            {
                Insert(linkedList, new HuffmanNode
                {
                    Byte = (byte)i,
                    Freq = bytesFreqs[i]
                });
            }
        }

        
        while (linkedList.Count > 1)
        {
            
            var left = linkedList.head.Value;
            linkedList.RemoveFirst();

            var right = linkedList.head.Value;
            linkedList.RemoveFirst();

            
            var node = new HuffmanNode
            {
                Left = left,
                Right = right,
                Freq = left.Freq + right.Freq
            };

            
            Insert(linkedList, node);
        }

        return linkedList.head.Value; 
    }


    private static void Insert(CustomLinkedList<HuffmanNode> linkedList, HuffmanNode node)
    {
        
        if (linkedList.IsEmpty())
        {
            linkedList.AddLast(node);
            return;
        }

        
        var current = linkedList.head;
        CustomLinkedList<HuffmanNode>.Node previous = null;

        while (current != null && current.Value.Freq < node.Freq)
        {
            previous = current;
            current = current.Next;
        }

        if (previous == null)
        {
            
            linkedList.AddBefore(linkedList.head, node);
        }
        else if (current == null)
        {
            
            linkedList.AddLast(node);
        }
        else
        {
            
            linkedList.AddBefore(current, node);
        }
    }




    
    public static CustomDictionary<byte, string> GenerateCodes(HuffmanNode root)
        {
            var codes = new CustomDictionary<byte, string>();
            GenerateCodesRecursive(root, "", codes);
            return codes;
        }

        private static void GenerateCodesRecursive(HuffmanNode node, string currentCode, CustomDictionary<byte, string> codes)
        {
            if (node == null) return;

            if (node.Byte.HasValue)
            {
                
                codes[node.Byte.Value] = currentCode;
            }
            else
            {
                
                GenerateCodesRecursive(node.Left, currentCode + "0", codes);
                GenerateCodesRecursive(node.Right, currentCode + "1", codes);
            }
        }

  


    public static string Encode(byte[] data, CustomDictionary<byte, string> codes)
    {
        
        int totalLength = 0;
        foreach (var b in data)
        {
            totalLength += codes[b].Length;
        }

        
        char[] result = new char[totalLength];
        int position = 0;

        
        foreach (var b in data)
        {
            string code = codes[b];
            for (int i = 0; i < code.Length; i++)
            {
                result[position++] = code[i];
            }
        }

        
        return new string(result);
    }





    
    public static byte[] Decode(string encodedData, HuffmanNode root)
        {
        
        if (CustomStringEditor.IsNullOrWhiteSpace(encodedData))
        {
                throw new Exception("Encoded data is empty or null.");
            }

            if (root == null)
            {
                throw new Exception("Huffman tree root is null.");
            }

            var decoded = new CustomList<byte>();
            var current = root;

            foreach (var bit in encodedData)
            {
                current = (bit == '0') ? current.Left : current.Right;

                if (current == null)
                {
                    throw new Exception("Unexpected null node during decoding.");
                }

                if (current.Left == null && current.Right == null)
                {
                    
                    decoded.Add(current.Byte.Value);
                    current = root;
                }
            }

            return decoded.ToArray();
        }


        
        public static void ToBytes(HuffmanNode node, CustomList<byte> bytes)
        {
            if (node == null)
            {
                bytes.Add(0); 
                return;
            }

            bytes.Add(1); 
            bytes.Add(node.Byte.HasValue ? (byte)1 : (byte)0);

            if (node.Byte.HasValue)
            {
                bytes.Add(node.Byte.Value);
            }
            else
            {
                ToBytes(node.Left, bytes);
                ToBytes(node.Right, bytes);
            }
        }


        public static (byte[] encodedData, int bitCount) HuffmanEncode(HuffmanNode root, byte[] content)
        {
            
            var codes = GenerateCodes(root);

            
            string encodedString = Encode(content, codes);

            
            var encodedBytes = new CustomList<byte>();
            int bitCount = 0;

            foreach (var bit in encodedString)
            {
                int byteIndex = bitCount / 8;

                if (byteIndex == encodedBytes.Count)
                    encodedBytes.Add(0); 

                if (bit == '1')
                {
                    encodedBytes[byteIndex] |= (byte)(1 << (7 - (bitCount % 8)));
                }

                bitCount++;
            }

            return (encodedBytes.ToArray(), bitCount);
        }



        
        public static HuffmanNode RestoreTree(byte[] data, ref int index)
        {
            if (index >= data.Length)
            {
                throw new Exception("Invalid index during Huffman tree restoration.");
            }

            if (data[index++] == 0)
            {
                return null; 
            }

            if (data[index++] == 1)
            {
                if (index >= data.Length)
                {
                    throw new Exception("Unexpected end of data during leaf node restoration.");
                }

                return new HuffmanNode
                {
                    Byte = data[index++]
                };
            }

            var left = RestoreTree(data, ref index);
            var right = RestoreTree(data, ref index);

            return new HuffmanNode
            {
                Left = left,
                Right = right
            };
        }


}

