using Html_Crawler_Final_version.Data_Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Html_Crawler_Final_version.Tools
{
    
    public class HtmlParser
    {
        private CustomStack<HtmlNode> nodeStack = new CustomStack<HtmlNode>();
        public HtmlNode Root { get; private set; }

        private readonly string[] SelfClosingTags = { "img", "br", "hr", "meta", "input", "link" };

       

        public void Parse(string html)
        {
            int position = 0;
            while (position < html.Length)
            {
                if (html[position] == '<') 
                {
                    int endTag = CustomStringEditor.IndexOf(html, '>', position);
                    if (endTag == -1) throw new Exception("Невалиден HTML: липсва '>'");

                    
                    string tagContent = CustomStringEditor.Trim(CustomStringEditor.Substring(html, position + 1, endTag - position - 1));

                    if (CustomStringEditor.StartsWith(tagContent, "/"))
                    {
                        
                        HandleClosingTag(CustomStringEditor.Trim(CustomStringEditor.Substring(tagContent, 1, tagContent.Length - 1)));

                    }
                    else 
                    {
                        bool isSelfClosing = CustomStringEditor.EndsWith(tagContent, "/") || IsSelfClosingTag(CustomStringEditor.Split(tagContent, ' ')[0]);

                        
                        if (isSelfClosing) tagContent = CustomStringEditor.Trim(CustomStringEditor.TrimEnd(tagContent, '/'));
                        HandleOpeningTag(tagContent, isSelfClosing);
                    }

                    position = endTag + 1;
                }
                else 
                {
                    int nextTag = CustomStringEditor.IndexOf(html, '<', position);
                    if (nextTag == -1) nextTag = html.Length;

                    
                    string textContent = CustomStringEditor.Trim(CustomStringEditor.Substring(html, position, nextTag - position));
                    HandleTextNode(textContent);

                   position = nextTag;
                }
            }

            if (nodeStack.Count > 0)
            {
                throw new Exception($"HTML документът съдържа незатворени тагове: {nodeStack.Peek().TagName}");
            }
        }





        private void HandleOpeningTag(string tagContent, bool isSelfClosing)
        {


            int firstSpaceIndex = CustomStringEditor.IndexOf(tagContent, ' ');

            string tagName = firstSpaceIndex == -1
                ? tagContent
                : CustomStringEditor.Substring(tagContent, 0, firstSpaceIndex);

            string attributesPart = firstSpaceIndex == -1
                ? string.Empty
                : CustomStringEditor.Trim(
                    CustomStringEditor.Substring(tagContent, firstSpaceIndex + 1, tagContent.Length - firstSpaceIndex - 1)
                );



            HtmlNode newNode = new HtmlNode(tagName);
            ParseAttributes(attributesPart, newNode);

            if (Root == null)
            {
                Root = newNode;
            }

            if (nodeStack.Count > 0)
            {
                HtmlNode parent = nodeStack.Peek();
                newNode.Parent = parent;
                nodeStack.Peek().Children.AddLast(newNode);
                
            }

            if (!isSelfClosing)
            {
                nodeStack.Push(newNode);
            }
        }

        private void HandleClosingTag(string tagName)
        {
            if (nodeStack.Count == 0 || nodeStack.Peek().TagName != tagName)
            {
                throw new Exception($"HTML грешка: Липсва затварящ таг за <{tagName}> или таговете са неправилно подредени.");
            }

            nodeStack.Pop();
        }


        private void HandleTextNode(string text)
        {
            if (nodeStack.Count > 0 && !CustomStringEditor.IsNullOrWhiteSpace(text))
            {
                HtmlNode current = nodeStack.Peek();


                current.InnerText += (CustomStringEditor.IsNullOrWhiteSpace(current.InnerText) ? "" : " ") + text;

            }
        }




        private void ParseAttributes(string attributesPart, HtmlNode node)
        {
            string[] attributes = CustomStringEditor.Split(attributesPart, ' '); 
            foreach (string attribute in attributes)
            {
                int equalsIndex = CustomStringEditor.IndexOf(attribute, '='); 
                if (equalsIndex != -1)
                {
                    string key = CustomStringEditor.Trim(
                        CustomStringEditor.Substring(attribute, 0, equalsIndex) 
                    );
                    string value = CustomStringEditor.Trim(
                        CustomStringEditor.Substring(attribute, equalsIndex + 1, attribute.Length - equalsIndex - 1).Trim('\'', '"') 
                    );
                    node.Attributes[key] = value;
                }
                else
                {
                    
                    //node.Attributes[attribute] = string.Empty;
                    node.Attributes[attribute] = "";
                }
            }
        }

        private bool IsSelfClosingTag(string tagName)
        {
            foreach (var tag in SelfClosingTags)
            {
                if (CustomStringEditor.Equals(tag, tagName, true)) 
                {
                    return true;
                }
            }
            return false;
        }


    }
    

   
}
