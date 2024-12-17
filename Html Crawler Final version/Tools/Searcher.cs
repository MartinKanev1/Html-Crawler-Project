using Html_Crawler_Final_version.Data_Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Crawler_Final_version.Tools
{
    public class Searcher
    {
        private HtmlNode root;

        public Searcher(HtmlNode root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
        }


        private int GetTreeSize(HtmlNode node)
        {
            if (node == null)
            {
                return 0;
            }

            int size = 1; 

            foreach (var child in node.Children) 
            {
                size += GetTreeSize(child); 
            }

            return size;
        }



        public void PrintCommand(string command)
        {
            Form1.Instance.txtHtmlCode.Clear();

            if (command == "PRINT \"//\"")
            {
                
                Form1.Instance.txtHtmlCode.Text = Form1.GenerateHtmlTreeString(root, 0);
                return;
            }

            
            if (!CustomStringEditor.StartsWith(command, "PRINT \"") || !CustomStringEditor.EndsWith(command, "\""))
            {
                Form1.Instance.txtHtmlCode.Text = "Unsupported command format.";
                return;
            }

            
            string path = CustomStringEditor.Trim(CustomStringEditor.Substring(command, 7, command.Length - 8));
            //Form1.Instance.txtHtmlCode.AppendText($"Executing PRINT for path: {path}\n");

            int treeSize = GetTreeSize(root);
            
            CustomList<HtmlNode> results;
            bool isComplexQuery = CustomStringEditor.Contains(path, "[@") || (CustomStringEditor.Contains(path, "[") && CustomStringEditor.Contains(path, "]"));
            

            try
            {
                if (isComplexQuery || treeSize > 1000)
                {
                    //Form1.Instance.txtHtmlCode.AppendText("Using Parallel BFS for complex search.\n");
                    results = ParallelBfsSearch(path);
                }
                else
                {
                    //Form1.Instance.txtHtmlCode.AppendText("Using BFS for general search.\n");
                    results = BfsSearch(path);
                }
            }
            catch (Exception ex)
            {
                Form1.Instance.txtHtmlCode.AppendText($"Error during search: {ex.Message}\n");
                return;
            }

            
            if (results == null || results.Count == 0)
            {
                Form1.Instance.txtHtmlCode.AppendText("No matching nodes found.\n");
                return;
            }

            
            bool isWildcardSearch = CustomStringEditor.Contains(path, "/*");

            
            foreach (var node in results)
            {
                if (isWildcardSearch)
                {
                    
                    if (!CustomStringEditor.IsNullOrWhiteSpace(node.InnerText))
                    {
                        Form1.Instance.txtHtmlCode.AppendText(CustomStringEditor.Trim(node.InnerText) + Environment.NewLine); 
                    }
                }
                else
                {
                    
                    if (node.Children.Count == 0 && !CustomStringEditor.IsNullOrWhiteSpace(node.InnerText))
                    {
                        Form1.Instance.txtHtmlCode.AppendText(CustomStringEditor.Trim(node.InnerText) + Environment.NewLine); 
                    }
                    else
                    {
                        Form1.Instance.txtHtmlCode.AppendText(CustomStringEditor.Trim(GetInnerHtmlContent(node)) + Environment.NewLine); 
                    }
                }
            }
        }



        

        public void SetCommand(string command)
        {
            Form1.Instance.txtHtmlCode.Text = ("DEBUG: Starting SET command processing...");

            if (!CustomStringEditor.StartsWith(command, "SET \"") || !CustomStringEditor.Contains(command, "\" ") || !CustomStringEditor.EndsWith(command, "\""))
            {
                Form1.Instance.txtHtmlCode.Text = ("DEBUG: Unsupported command format.");
                return;
            }

            
            int firstQuote = CustomStringEditor.IndexOf(command, '"', 0);
            int secondQuote = CustomStringEditor.IndexOf(command, '"', firstQuote + 1);
            string path = CustomStringEditor.Trim(CustomStringEditor.Substring(command, firstQuote + 1, secondQuote - firstQuote - 1));

            string newContent = CustomStringEditor.Trim(CustomStringEditor.Trim(CustomStringEditor.Substring(command, secondQuote + 2), '"'));

            Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Path: {path}, New Content: {newContent}");

            
            int treeSize = GetTreeSize(root);
            CustomList<HtmlNode> nodesToUpdate;
            bool isComplexQuery = CustomStringEditor.Contains(path, "[@") || (CustomStringEditor.Contains(path, "[") && CustomStringEditor.Contains(path, "]"));
            if  (isComplexQuery || treeSize > 1000)
                {
                
                Form1.Instance.txtHtmlCode.Text = ("Using Parallel BFS for complex search.");
                nodesToUpdate = ParallelBfsSearch(path);
            }
            else
            {
                
                Form1.Instance.txtHtmlCode.Text = ("Using BFS for general search.");
                nodesToUpdate = BfsSearch(path);
            }

            
            if (nodesToUpdate == null || nodesToUpdate.Count == 0)
            {
                Form1.Instance.txtHtmlCode.Text = ("DEBUG: No matching nodes found for the given path.");
                return;
            }

            Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Found {nodesToUpdate.Count} node(s) to update.");

            foreach (var node in nodesToUpdate)
            {
                Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Processing node <{node.TagName}>...");

                if (CustomStringEditor.StartsWith(newContent, "<") && CustomStringEditor.EndsWith(newContent, ">"))
                {
                    
                    Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Parsing new subtree: {newContent}");
                    HtmlParser parser = new HtmlParser();
                    parser.Parse(newContent);

                    if (parser.Root != null)
                    {
                        Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Parsed root: <{parser.Root.TagName}>, InnerText: {parser.Root.InnerText}");

                        
                        node.Children.Clear();
                        //node.InnerText = string.Empty;
                        node.InnerText = "";

                        
                        Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Adding parsed root <{parser.Root.TagName}> as child to node <{node.TagName}>."); ;
                        node.AddChild(parser.Root);

                        Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Updated node <{node.TagName}> with new subtree.");
                    }
                    else
                    {
                        Form1.Instance.txtHtmlCode.Text = ("DEBUG: Invalid HTML content for subtree.");
                    }
                }
                else
                {
                    
                    Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Updating InnerText of node <{node.TagName}> to: {newContent}");
                    node.InnerText = newContent;
                    node.Children.Clear(); 
                    Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Cleared children of node <{node.TagName}>.");
                }
            }

            Form1.Instance.txtHtmlCode.Text = ("DEBUG: SET command processing completed.");
        }


        public void CopyCommand(string command)
        {
            Form1.Instance.txtHtmlCode.Text = ("DEBUG: Starting COPY command processing...");

            if (!CustomStringEditor.StartsWith(command, "COPY \"") || !CustomStringEditor.Contains(command, "\" TO \"") || (!CustomStringEditor.EndsWith(command, "\"") && !CustomStringEditor.EndsWith(command, "CLEAR")))
            {
                Form1.Instance.txtHtmlCode.Text = ("DEBUG: Unsupported COPY command format.");
                return;
            }

            bool shouldClear = CustomStringEditor.EndsWith(command, "CLEAR");

            int firstQuote = CustomStringEditor.IndexOf(command, '"', 0);
            int secondQuote = CustomStringEditor.IndexOf(command, '"', firstQuote + 1);
            int toIndex = CustomStringEditor.IndexOf(command, " TO ", secondQuote);

            string sourcePath = CustomStringEditor.Trim(CustomStringEditor.Substring(command, firstQuote + 1, secondQuote - firstQuote - 1));
            string targetPath = CustomStringEditor.Trim(CustomStringEditor.Substring(command, toIndex + 4));

            if (shouldClear)
            {
                targetPath = CustomStringEditor.Trim(CustomStringEditor.Substring(targetPath, 0, CustomStringEditor.LastIndexOf(targetPath, "CLEAR")));
            }
            targetPath = CustomStringEditor.Trim(targetPath, '"');

            Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Source Path: {sourcePath}, Target Path: {targetPath}, Should Clear: {shouldClear}");

            
            CustomList<HtmlNode> sourceNodes = ParallelBfsSearch(sourcePath);
            CustomList<HtmlNode> targetNodes = ParallelBfsSearch(targetPath);

            if (sourceNodes == null || sourceNodes.Count == 0)
            {
                Form1.Instance.txtHtmlCode.Text = ("DEBUG: No matching source nodes found.");
                return;
            }

            if (targetNodes == null || targetNodes.Count == 0)
            {
                Form1.Instance.txtHtmlCode.Text = ("DEBUG: No matching target nodes found.");
                return;
            }

            foreach (var targetNode in targetNodes)
            {
                if (shouldClear)
                {
                    
                    Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Clearing content of node <{targetNode.TagName}>.");
                    targetNode.Children.Clear();
                    //targetNode.InnerText = string.Empty;
                    targetNode.InnerText = "";
                }

                foreach (var sourceNode in sourceNodes)
                {
                    Form1.Instance.txtHtmlCode.Text = ($"DEBUG: Copying content of node <{sourceNode.TagName}> to <{targetNode.TagName}>...");

                    
                    if (!CustomStringEditor.IsNullOrWhiteSpace(sourceNode.InnerText))
                    {
                        targetNode.InnerText += " " + sourceNode.InnerText;
                    }

                    
                    foreach (var child in sourceNode.Children)
                    {
                        var childCopy = child.ShallowCopy(); 
                        targetNode.AddChild(childCopy);
                    }
                }

                
                targetNode.InnerText = CustomStringEditor.Trim(targetNode.InnerText);
            }

            Form1.Instance.txtHtmlCode.Text = ("DEBUG: COPY command processing completed.");
        }


        public CustomList<HtmlNode> ParallelBfsSearch(string path, int threadCount = 4)
        {
            if (CustomStringEditor.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be empty.");

            var steps = ParsePath(path);
            var currentNodes = new CustomList<HtmlNode> { root };

            foreach (var (tag, attributeKey, attributeValue, index) in steps)
            {
                var nextLevel = new CustomList<HtmlNode>();
                var lockObject = new object();
                var nodesPerThread = SplitNodes(currentNodes, threadCount);
                var threads = new CustomList<Thread>();

                foreach (var nodeGroup in nodesPerThread)
                {
                    var thread = new Thread(() =>
                    {
                        var localNextLevel = new CustomList<HtmlNode>();

                        foreach (var node in nodeGroup)
                        {
                            if (MatchesStep(node, tag, attributeKey, attributeValue))
                            {
                                localNextLevel.Add(node); 
                            }

                            foreach (var child in node.Children)
                            {
                                if (MatchesStep(child, tag, attributeKey, attributeValue))
                                {
                                    localNextLevel.Add(child); 
                                }
                            }
                        }

                        
                        lock (lockObject)
                        {
                            nextLevel.AddRange(localNextLevel);
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                
                foreach (var thread in threads)
                {
                    thread.Join();
                }

                
                if (index.HasValue)
                {
                    if (index.Value > 0 && index.Value <= nextLevel.Count)
                    {
                        currentNodes = new CustomList<HtmlNode> { nextLevel[index.Value - 1] };
                    }
                    else
                    {
                        return new CustomList<HtmlNode>(); 
                    }
                }
                else
                {
                    currentNodes = nextLevel;
                }

                if (currentNodes.Count == 0)
                {
                    return new CustomList<HtmlNode>();
                }
            }

            return currentNodes;
        }


             



        private CustomList<CustomList<HtmlNode>> SplitNodes(CustomList<HtmlNode> nodes, int threadCount)
        {
            var result = new CustomList<CustomList<HtmlNode>>();
            int groupSize = (int)Math.Ceiling((double)nodes.Count / threadCount);

            for (int i = 0; i < nodes.Count; i += groupSize)
            {
                result.Add(nodes.GetRange(i, Math.Min(groupSize, nodes.Count - i)));
            }

            return result;
        }

        private bool MatchesStep(HtmlNode node, string tag, string attributeKey = null, string attributeValue = null)
        {
            if (CustomStringEditor.IsNullOrWhiteSpace(node.TagName))
                return false;

            if (CustomStringEditor.Equals(tag, "*"))
                return true;

            if (!CustomStringEditor.Equals(node.TagName, tag))
                return false;

            if (!CustomStringEditor.IsNullOrWhiteSpace(attributeKey) && !CustomStringEditor.IsNullOrWhiteSpace(attributeValue))
            {
                if (node.Attributes.TryGetValue(attributeKey, out string value) && CustomStringEditor.Equals(value, attributeValue))
                {
                    return true;
                }
                return false;
            }

            return true;
        }




        


        private string GetHtmlContent(HtmlNode node, int depth = 0)
        {
            
            string indent = CustomStringEditor.Join("", CustomStringEditor.ToArray(' ', depth * 4));

            
            string attributes = "";
            foreach (var attribute in node.Attributes)
            {
                if (!CustomStringEditor.IsNullOrWhiteSpace(attributes))
                {
                    attributes += " ";
                }
                attributes += $"{attribute.Key}=\"{attribute.Value}\"";
            }

            
            if (Form1.IsSelfClosingTag(node.TagName))
            {
                return $"{indent}<{node.TagName} {attributes}/>{Environment.NewLine}";
            }

           
            var openingTag = CustomStringEditor.TrimEnd($"{indent}<{node.TagName} {attributes}>");
            var closingTag = $"{indent}</{node.TagName}>";

            
            var innerText = CustomStringEditor.IsNullOrWhiteSpace(node.InnerText)
                ? ""
                : $"{indent}    {node.InnerText}{Environment.NewLine}";

            
            string innerContent = "";
            foreach (var child in node.Children)
            {
                innerContent += GetHtmlContent(child, depth + 1);
            }

            
            return $"{openingTag}{Environment.NewLine}{innerText}{innerContent}{closingTag}{Environment.NewLine}";
        }

        private string GetInnerHtmlContent(HtmlNode node)
        {
            
            string innerContent = "";
            foreach (var child in node.Children)
            {
                innerContent += GetHtmlContent(child, 1); 
            }

            return innerContent;
        }




        private CustomList<(string TagName, string AttributeKey, string AttributeValue, int? Index)> ParsePath(string path)
        {
            var steps = new CustomList<(string, string, string, int?)>();
            int i = 0;

            if (CustomStringEditor.StartsWith(path, "//"))
            {
                i = 2; 
            }

            while (i < path.Length)
            {
                int start = i;

                while (i < path.Length && !CustomStringEditor.Equals(path[i], '/'))
                {
                    i++;
                }

                var step = CustomStringEditor.Substring(path, start, i - start);

                
                int? index = null;
                if (CustomStringEditor.Contains(step, '[') && CustomStringEditor.Contains(step, ']') && !CustomStringEditor.Contains(step, "@"))
                {
                    int startIndex = CustomStringEditor.IndexOf(step, '[', 0) + 1;
                    int endIndex = CustomStringEditor.IndexOf(step, ']', startIndex);
                    if (int.TryParse(CustomStringEditor.Substring(step, startIndex, endIndex - startIndex), out int parsedIndex))
                    {
                        index = parsedIndex;
                        step = CustomStringEditor.Substring(step, 0, CustomStringEditor.IndexOf(step, '[', 0)); 
                    }
                }

                
                string attributeKey = null;
                string attributeValue = null;
                if (CustomStringEditor.Contains(step, "[@") && CustomStringEditor.Contains(step, "='"))
                {
                    int attrStart = CustomStringEditor.IndexOf(step, "[@", 0) + 2;
                    int attrEnd = CustomStringEditor.IndexOf(step, '=', attrStart);
                    attributeKey = CustomStringEditor.Substring(step, attrStart, attrEnd - attrStart);

                    int valueStart = CustomStringEditor.IndexOf(step, '\'', attrEnd) + 1;
                    int valueEnd = CustomStringEditor.IndexOf(step, '\'', valueStart);
                    attributeValue = CustomStringEditor.Substring(step, valueStart, valueEnd - valueStart);

                    step = CustomStringEditor.Substring(step, 0, CustomStringEditor.IndexOf(step, "[@", 0)); 
                }

                
                steps.Add((step, attributeKey, attributeValue, index));

                
                if (i < path.Length && path[i] == '/')
                {
                    i++;
                }
            }

            return steps;
        }





        public CustomList<HtmlNode> BfsSearch(string path)
        {
            if (CustomStringEditor.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be empty.");

            var steps = ParsePath(path);
            var currentNodes = new CustomList<HtmlNode> { root };

            foreach (var (tag, attributeKey, attributeValue, index) in steps)
            {
                var nextLevel = new CustomList<HtmlNode>();

                foreach (var node in currentNodes)
                {
                    if (MatchesStepbfs(node, tag, attributeKey, attributeValue))
                    {
                        nextLevel.Add(node);
                    }

                    foreach (var child in node.Children)
                    {
                        if (MatchesStepbfs(child, tag, attributeKey, attributeValue))
                        {
                            nextLevel.Add(child);
                        }
                    }
                }

                if (index.HasValue)
                {
                    if (index.Value > 0 && index.Value <= nextLevel.Count)
                    {
                        currentNodes = new CustomList<HtmlNode> { nextLevel[index.Value - 1] };
                    }
                    else
                    {
                        return new CustomList<HtmlNode>(); 
                    }
                }
                else
                {
                    currentNodes = nextLevel;
                }

                if (currentNodes.Count == 0)
                {
                    return new CustomList<HtmlNode>();
                }
            }

            return currentNodes;
        }





        private bool MatchesStepbfs(HtmlNode node, string tag, string attributeKey = null, string attributeValue = null)
        {
            if (CustomStringEditor.IsNullOrWhiteSpace(node.TagName))
                return false;

            
            if (CustomStringEditor.Equals(tag, "*"))
                return true;

            
            if (!CustomStringEditor.Equals(node.TagName, tag))
                return false;

            
            if (!CustomStringEditor.IsNullOrWhiteSpace(attributeKey) && !CustomStringEditor.IsNullOrWhiteSpace(attributeValue))
            {
                if (node.Attributes.TryGetValue(attributeKey, out string value) && CustomStringEditor.Equals(value, attributeValue))
                {
                    return true;
                }
                return false;
            }

            return true; 
        }










    }
}
