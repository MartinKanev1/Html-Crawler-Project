using Html_Crawler_Final_version.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Crawler_Final_version.Data_Structures
{
    
    public class HtmlNode
    {
        public string TagName { get; set; } 
        public CustomDictionary<string, string> Attributes { get; set; } 
        public CustomLinkedList<HtmlNode> Children { get; set; } 
        public string InnerText { get; set; } 

        public HtmlNode Parent { get; set; }

        public HtmlNode(string tagName)
        {
            TagName = tagName;
            Attributes = new CustomDictionary<string, string>();
            Children = new CustomLinkedList<HtmlNode>();
            //InnerText = string.Empty;
            InnerText = "";
            Parent = null;
        }


        public void AddChild(HtmlNode child)
        {
            if (child == null) return;
            child.Parent = this; 
            Children.AddLast(child);
        }

        public HtmlNode ShallowCopy()
        {
            HtmlNode copy = new HtmlNode(this.TagName)
            {
                InnerText = this.InnerText,
                Attributes = new CustomDictionary<string, string>(this.Attributes)
            };
            copy.Children = this.Children; 
            return copy;
        }

        public HtmlNode DeepCopy()
        {
            HtmlNode copy = new HtmlNode(this.TagName)
            {
                InnerText = this.InnerText,
                Attributes = new CustomDictionary<string, string>(this.Attributes)
            };

            foreach (var child in this.Children)
            {
                copy.AddChild(child.DeepCopy()); 
            }

            return copy;
        }




        public override string ToString()
        {
            string attributes = "";
            foreach (var attr in Attributes)
            {
                attributes += $"{attr.Key}=\"{attr.Value}\" ";
            }
            attributes = CustomStringEditor.TrimEnd(attributes); 

            return $"<{TagName} {attributes}> {InnerText} ({Children.Count} children)";
        }
    }
    

  
}
