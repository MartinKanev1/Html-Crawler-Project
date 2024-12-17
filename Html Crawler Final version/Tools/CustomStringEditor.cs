using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Crawler_Final_version.Tools
{
    public class CustomStringEditor
    {
        public static string[] Split(string input, char delimiter)
        {
            string[] result = new string[0];
            string current = "";

            foreach (char c in input)
            {
                if (c == delimiter)
                {
                    result = AddToArray(result, current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            if (current != "")
            {
                result = AddToArray(result, current);
            }

            return result;
        }

        private static string[] AddToArray(string[] array, string newElement)
        {
            string[] newArray = new string[array.Length + 1];

            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            newArray[array.Length] = newElement;
            return newArray;
        }



        

        public static string Trim(string input)
        {
            if (IsEmpty(input)) return input; 

            int start = 0;
            int end = input.Length - 1;

            while (start <= end && IsWhiteSpace(input[start]))
            {
                start++;
            }

            while (end >= start && IsWhiteSpace(input[end]))
            {
                end--;
            }

            char[] result = new char[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                result[i - start] = input[i];
            }

            return new string(result);
        }




        public static string TrimEnd(string input)
        {
            if (IsEmpty(input)) return input; 

            int end = input.Length - 1;

            while (end >= 0 && (input[end] == ' ' || input[end] == '\t'))
            {
                end--;
            }

            char[] result = new char[end + 1];
            for (int i = 0; i <= end; i++)
            {
                result[i] = input[i];
            }

            return new string(result);
        }

        public static string TrimEnd(string input, char characterToTrim)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int end = input.Length - 1;

            while (end >= 0 && input[end] == characterToTrim)
            {
                end--;
            }

            char[] result = new char[end + 1];
            for (int i = 0; i <= end; i++)
            {
                result[i] = input[i];
            }

            return new string(result);
        }


        public static bool IsNullOrWhiteSpace(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;

            foreach (char c in input)
            {
                if (!IsWhiteSpace(c)) 
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsEmpty(string input)
        {
            return input != null && input.Length == 0;
        }


        public static bool StartsWith(string input, string prefix)
        {
            if (prefix.Length > input.Length) return false;

            for (int i = 0; i < prefix.Length; i++)
            {
                if (input[i] != prefix[i]) return false;
            }

            return true;
        }

        public static bool EndsWith(string input, string suffix)
        {
            if (suffix.Length > input.Length) return false;

            int startIndex = input.Length - suffix.Length;
            for (int i = 0; i < suffix.Length; i++)
            {
                if (input[startIndex + i] != suffix[i]) return false;
            }

            return true;
        }

        public static int IndexOf(string input, char ch, int startIndex = 0)
        {
            for (int i = startIndex; i < input.Length; i++)
            {
                if (input[i] == ch)
                {
                    return i;
                }
            }

            return -1; 
        }

        public static string Substring(string input, int startIndex, int length)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (startIndex < 0 || startIndex >= input.Length) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0 || startIndex + length > input.Length) throw new ArgumentOutOfRangeException(nameof(length));

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = input[startIndex + i];
            }
            return new string(result);
        }

        public static bool Equals(string str1, string str2, bool ignoreCase = false) //correct
        {
            if (str1 == str2) return true; 
            if (str1 == null || str2 == null) return false;

            if (str1.Length != str2.Length) return false;

            for (int i = 0; i < str1.Length; i++)
            {
                char c1 = str1[i];
                char c2 = str2[i];

                if (ignoreCase)
                {
                    c1 = ToLower(c1);
                    c2 = ToLower(c2);
                }

                if (c1 != c2) return false;
            }

            return true;
        }

        public static char ToLower(char c)
        {
            
            if (c >= 'A' && c <= 'Z')
            {
                return (char)(c + 32); 
            }
            
            return c;
        }

        public static bool IsWhiteSpace(char c)
        {
            
            return c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\v' || c == '\f';
        }

        public static bool Contains(string input, char value)
        {
            if (input == null)
                return false;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == value)
                    return true;
            }

            return false;
        }

        public static bool Contains(string input, string value)
        {
            if (input == null || value == null || value.Length > input.Length)
                return false;

            for (int i = 0; i <= input.Length - value.Length; i++)
            {
                bool match = true;

                for (int j = 0; j < value.Length; j++)
                {
                    if (input[i + j] != value[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return true;
            }

            return false;
        }

        public static int IndexOf(string step, string value, int startIndex)
        {
            if (step == null || value == null || startIndex < 0 || startIndex >= step.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid input parameters.");
            }

            for (int i = startIndex; i <= step.Length - value.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < value.Length; j++)
                {
                    if (step[i + j] != value[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }

            return -1;
        }


        public static string Join(string separator, string[] values)
        {
            if (values == null || values.Length == 0) return "";

            string result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result += separator + values[i];
            }

            return result;
        }

        public static string[] ToArray(char character, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");

            string[] result = new string[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = character.ToString();
            }
            return result;
        }

        public static string Substring(string input, int startIndex)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (startIndex < 0 || startIndex >= input.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            char[] result = new char[input.Length - startIndex];
            for (int i = startIndex; i < input.Length; i++)
            {
                result[i - startIndex] = input[i];
            }

            return new string(result);
        }

        public static string Trim(string input, char characterToTrim)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int start = 0;
            int end = input.Length - 1;

            
            while (start <= end && input[start] == characterToTrim)
            {
                start++;
            }

            
            while (end >= start && input[end] == characterToTrim)
            {
                end--;
            }

            
            char[] result = new char[end - start + 1];
            for (int i = start; i <= end; i++)
            {
                result[i - start] = input[i];
            }

            return new string(result);
        }

        public static int LastIndexOf(string input, string value)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                throw new ArgumentException("Value cannot be an empty string.", nameof(value));

            for (int i = input.Length - value.Length; i >= 0; i--)
            {
                bool match = true;
                for (int j = 0; j < value.Length; j++)
                {
                    if (input[i + j] != value[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }

            return -1; 
        }

        public static string ToLower(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            char[] result = new char[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                
                if (c >= 'A' && c <= 'Z')
                {
                    result[i] = (char)(c + 32); 
                }
                else
                {
                    result[i] = c; 
                }
            }

            return new string(result);
        }

    }
}
