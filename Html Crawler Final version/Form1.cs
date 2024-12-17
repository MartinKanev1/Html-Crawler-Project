using Html_Crawler_Final_version.Data_Structures;
using Html_Crawler_Final_version.Tools;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Html_Crawler_Final_version
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Instance = this;
        }


        private string htmlFilePath;

        private HtmlNode loadedHtmlRoot;
        public static Form1 Instance { get; private set; }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_load_Click(object sender, EventArgs e)
        {

            DebugLog("Button clicked, starting load process...");

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "HTML and Compressed Files|*.html;*.txt;*.hcz|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {




                htmlFilePath = openFileDialog.FileName;


                if (CustomStringEditor.Contains(CustomStringEditor.ToLower(htmlFilePath), ".hcz"))
                {
                    DebugLog("Compressed file detected, attempting to load...");

                    string htmlContent = LoadCompressedFile(htmlFilePath);


                    if (!CustomStringEditor.IsNullOrWhiteSpace(htmlContent))
                    {
                        DebugLog("Loading decompressed HTML content...");
                        LoadHtmlContent(htmlContent);



                    }
                    else
                    {
                        DebugLog("Decompressed HTML content is empty or null.");
                        MessageBox.Show("Failed to load HTML content from compressed file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


                else
                {
                    DebugLog("Normal HTML file detected, attempting to load...");
                    //string htmlContent = File.ReadAllText(openFileDialog.FileName); -------------
                    string htmlContent = ReadFileManually(openFileDialog.FileName);
                    LoadHtmlContent(htmlContent);
                }


            }

        }

        string ReadFileManually(string filePath)
        {
            CustomList<char> characters = new CustomList<char>();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                while (fileStream.Position < fileStream.Length)
                {
                    char currentChar = (char)binaryReader.ReadByte();
                    characters.Add(currentChar);
                }
            }


            return new string(characters.ToArray());
        }



        private static string LoadCompressedFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                try
                {
                    
                    Debug.WriteLine($"Stream Position (Start): {binaryReader.BaseStream.Position}");
                    int treeSize = binaryReader.ReadInt32();
                    Debug.WriteLine($"Tree Size: {treeSize}");

                    byte[] treeBytes = binaryReader.ReadBytes(treeSize);
                    Debug.WriteLine($"Stream Position (After Tree): {binaryReader.BaseStream.Position}");

                    
                    int index = 0;
                    var root = HuffmanCompression.RestoreTree(treeBytes, ref index);

                    
                    int bitCount = binaryReader.ReadInt32();
                    Debug.WriteLine($"Bit Count: {bitCount}");

                    
                    long remainingDataLength = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
                    Debug.WriteLine($"Remaining Data Length: {remainingDataLength}");

                    byte[] encodedBytes = binaryReader.ReadBytes((int)remainingDataLength);
                    Debug.WriteLine($"Encoded Data Length: {encodedBytes.Length}");

                    
                    var encodedString = ConvertToBitString(encodedBytes, bitCount);

                    
                    var decodedBytes = HuffmanCompression.Decode(encodedString, root);
                    string htmlContent = Encoding.UTF8.GetString(decodedBytes);
                    Debug.WriteLine($"HTML Content Length: {htmlContent.Length}");

                    
                    if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        int resourceCount = binaryReader.ReadInt32();
                        Debug.WriteLine($"Resource Count: {resourceCount}");

                        var resources = new CustomDictionary<string, byte[]>();

                        for (int i = 0; i < resourceCount; i++)
                        {
                            //string resourceName = binaryReader.ReadString(); ---------
                            string resourceName = ReadStringManually(binaryReader);
                            int resourceSize = binaryReader.ReadInt32();
                            Debug.WriteLine($"Resource {i + 1}/{resourceCount}: {resourceName} ({resourceSize} bytes)");

                            byte[] resourceData = binaryReader.ReadBytes(resourceSize);
                            resources[resourceName] = resourceData;
                        }


                        LoadResources(resources);
                    }
                    else
                    {
                        Debug.WriteLine("No resources to read.");
                    }

                    return htmlContent;
                }
                catch (EndOfStreamException ex)
                {
                    Debug.WriteLine("Error: Reached end of stream unexpectedly.");
                    throw new Exception("Error reading compressed file: File may be corrupted or incomplete.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }


        public static string ReadStringManually(BinaryReader reader)
        {
            
            int length = reader.ReadInt32();

            
            char[] chars = new char[length];

            
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)reader.ReadByte();
            }

           
            return new string(chars);
        }




        private static string ConvertToBitString(byte[] bytes, int bitCount) 
        {

            char[] result = new char[bitCount];
            for (int i = 0; i < bitCount; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                bool isSet = (bytes[byteIndex] & (1 << bitIndex)) != 0;
                result[i] = isSet ? '1' : '0';
            }
            return new string(result);
        }



        private void DebugLog(string message)
        {
            if (txtHtmlCode != null)
            {
                txtHtmlCode.AppendText(message + Environment.NewLine);
            }
            else
            {
                MessageBox.Show("txtHtmlCode is null!");
            }
        }



        private void LoadHtmlContent(string htmlContent)
        {
            HtmlParser parser = new HtmlParser();
            parser.Parse(htmlContent);

            HtmlNode root = parser.Root;
            loadedHtmlRoot = parser.Root;

            txtHtmlCode.Text = GenerateHtmlTreeString(root, 0);



        }

       


        private static void LoadResources(CustomDictionary<string, byte[]> resources)
        {
            foreach (var resource in resources)
            {
                string resourcePath = Path.Combine(Path.GetTempPath(), resource.Key);

                using (var fileStream = new FileStream(resourcePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = resource.Value;
                    for (int i = 0; i < data.Length; i++)
                    {
                        fileStream.WriteByte(data[i]);
                    }
                }
            }

        }


        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Compressed HTML File|*.hcz|All Files|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                
                string updatedHtmlContent = txtHtmlCode.Text;

                
                SaveCompressedFile(filePath, updatedHtmlContent);
            }

        }

        
        private static void SaveCompressedFile(string filePath, string htmlContent)
        {
            try
            {
                
                CustomList<byte> contentBytes = ConvertStringToBytes(htmlContent);

                
                var root = HuffmanCompression.BuildTree(contentBytes.ToArray());

                
                var (encodedBytes, bitCount) = HuffmanCompression.HuffmanEncode(root, contentBytes.ToArray());

                
                var treeBytes = new CustomList<byte>();
                HuffmanCompression.ToBytes(root, treeBytes);

                
                var resources = ExtractResources(filePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    
                    WriteIntToStream(fileStream, treeBytes.Count);
                    WriteBytesToStream(fileStream, treeBytes.ToArray());

                    
                    WriteIntToStream(fileStream, bitCount);

                    
                    WriteBytesToStream(fileStream, encodedBytes);

                    
                    WriteIntToStream(fileStream, resources.Count); 
                    foreach (var resource in resources)
                    {
                        WriteStringToStream(fileStream, resource.Key); 
                        WriteIntToStream(fileStream, resource.Value.Length); 
                        WriteBytesToStream(fileStream, resource.Value); 
                    }
                }

                MessageBox.Show("File saved successfully!", "Save File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving file: " + ex.Message);
            }
        }

        private static CustomList<byte> ConvertStringToBytes(string input)
        {
            CustomList<byte> result = new CustomList<byte>();
            foreach (char c in input)
            {
                if (c < 128)
                    result.Add((byte)c); 
                else
                    result.AddRange(Encoding.UTF8.GetBytes(new[] { c }));
            }
            return result;
        }

        private static void WriteIntToStream(FileStream stream, int value)
        {
            for (int i = 0; i < 4; i++)
            {
                stream.WriteByte((byte)(value >> (8 * i) & 0xFF));
            }
        }

        private static void WriteBytesToStream(FileStream stream, byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                stream.WriteByte(b);
            }
        }

        private static void WriteStringToStream(FileStream stream, string value)
        {
            var bytes = ConvertStringToBytes(value).ToArray();
            WriteIntToStream(stream, bytes.Length); 
            WriteBytesToStream(stream, bytes); 
        }





        

        private static CustomDictionary<string, byte[]> ExtractResources(string htmlFilePath)
        {
            var resources = new CustomDictionary<string, byte[]>();

            foreach (var imgTag in FindImageTags())
            {
                if (imgTag.Attributes.ContainsKey("src"))
                {
                    string src = imgTag.Attributes["src"];
                    string resourcePath = Path.Combine(Path.GetDirectoryName(htmlFilePath), src);

                    if (File.Exists(resourcePath))
                    {
                        
                        byte[] fileBytes = ReadFileManually2(resourcePath);
                        resources[src] = fileBytes;
                    }
                }
            }

            return resources;
        }

        private static byte[] ReadFileManually2(string filePath)
        {
            var fileBytes = new CustomList<byte>();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int currentByte;
                while ((currentByte = fileStream.ReadByte()) != -1)
                {
                    fileBytes.Add((byte)currentByte);
                }
            }

            return fileBytes.ToArray();
        }




        private static CustomList<HtmlNode> FindImageTags()
        {
            var images = new CustomList<HtmlNode>();
            void Traverse(HtmlNode node)
            {
                if (node != null)
                {

                    if (!CustomStringEditor.IsNullOrWhiteSpace(node.TagName) && CustomStringEditor.Equals(node.TagName, "img", true))
                    {

                        images.Add(node);
                    }

                    foreach (var child in node.Children)
                    {
                        Traverse(child);
                    }
                }
            }

            Traverse(new HtmlParser().Root); 
            return images;
        }



        public static string GenerateHtmlTreeString(HtmlNode node, int depth)
        {
            if (node == null)
            {
                return ""; 
            }

            string result = "";
            string indent = new string(' ', depth * 4); 

            
            string attributes = "";
            foreach (var attribute in node.Attributes)
            {
                if (!CustomStringEditor.IsNullOrWhiteSpace(attributes))
                {
                    attributes += " ";
                }
                attributes += $"{attribute.Key}=\"{attribute.Value}\"";
            }

            
            if (IsSelfClosingTag(node.TagName))
            {
                result += $"{indent}<{node.TagName}{(CustomStringEditor.IsNullOrWhiteSpace(attributes) ? "" : " " + attributes)}/>{Environment.NewLine}";
            }
            else
            {
                
                result += $"{indent}<{node.TagName}{(CustomStringEditor.IsNullOrWhiteSpace(attributes) ? "" : " " + attributes)}>{Environment.NewLine}";

                
                if (!CustomStringEditor.IsNullOrWhiteSpace(node.InnerText))
                {
                    string textIndent = new string(' ', (depth + 1) * 4);
                    result += $"{textIndent}{CustomStringEditor.Trim(node.InnerText)}{Environment.NewLine}";
                }

                
                foreach (var child in node.Children)
                {
                    result += GenerateHtmlTreeString(child, depth + 1);
                }

                
                result += $"{indent}</{node.TagName}>{Environment.NewLine}";
            }

            return result;
        }


        public static bool IsSelfClosingTag(string tagName)
        {
            string[] selfClosingTags = { "img", "br", "hr", "meta", "input", "link" };

            foreach (string tag in selfClosingTags)
            {
                if (CustomStringEditor.Equals(tag, tagName, true)) 
                {
                    return true;
                }
            }

            return false;
        }




        private void btn_execute_Click(object sender, EventArgs e)
        {
            try
            {
                 
                string command = CustomStringEditor.Trim(txtCommand.Text);
                if (CustomStringEditor.IsNullOrWhiteSpace(command))
                {
                    lblStatus.Text = "Please enter a valid command.";
                    return;
                }

                if (loadedHtmlRoot == null)
                {
                    lblStatus.Text = "No HTML document loaded. Please load a file first.";
                    return;
                }

                lblStatus.Text = "Executing command...";
                txtHtmlCode.Clear();


                var searcher = new Searcher(loadedHtmlRoot);


                if (CustomStringEditor.StartsWith(command, "PRINT"))
                {
                    searcher.PrintCommand(command);
                }
                else if (CustomStringEditor.StartsWith(command, "SET"))
                {
                    searcher.SetCommand(command);
                }
                else if (CustomStringEditor.StartsWith(command, "COPY"))
                {
                    searcher.CopyCommand(command);
                }
                else
                {
                    lblStatus.Text = "Unsupported command format.";
                    return;
                }



                txtHtmlCode.Refresh();
                lblStatus.Text = "Command executed successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error executing command: {ex.Message}";
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            txtCommand.Clear();
        }

        private void btn_vizualize_Click(object sender, EventArgs e)
        {
            string htmlContent = txtHtmlCode.Text;

            if (CustomStringEditor.IsNullOrWhiteSpace(htmlContent))
            {
                MessageBox.Show("Please enter valid HTML code.");
                return;
            }


            VisualizeHtml(htmlContent);
        }

        private void VisualizeHtml(string htmlContent)
        {
            HtmlParser parser = new HtmlParser();
            try
            {
                
                parser.Parse(htmlContent);
                HtmlNode root = parser.Root;

               
                int bitmapWidth = PictureBox1.Width > 0 ? PictureBox1.Width : 800;
                int bitmapHeight = PictureBox1.Height > 0 ? PictureBox1.Height : 600;

                Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White); 
                    DrawHtmlTree(g, root, 10, 10); 
                }

                
                PictureBox1.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing HTML: {ex.Message}");
            }
        }

        private int DrawHtmlTree(Graphics g, HtmlNode node, int x, int y)
        {
            if (node == null)
            {
                return y; 
            }

            foreach (var child in node.Children)
            {
                
                switch (CustomStringEditor.ToLower(child.TagName))
                {
                    case "p":
                        
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.Black, x, y);
                        y += 20; 
                        break;


                    case "table":
                        y += 10; 
                        int currentY = y; 
                        int startX = x;   

                        foreach (var row in child.Children) 
                        {
                            int rowHeight = 0; 
                            int[] cellHeights = new int[row.Children.Count];
                            int cellIndex = 0;

                            
                            foreach (var cell in row.Children)
                            {
                                int cellWidth = 100; 

                               
                                StringFormat format = new StringFormat
                                {
                                    Alignment = StringAlignment.Near,
                                    LineAlignment = StringAlignment.Near,
                                    Trimming = StringTrimming.Word
                                };

                                
                                SizeF textSize = g.MeasureString(cell.InnerText, SystemFonts.DefaultFont, cellWidth - 10, format);
                                int cellHeight = (int)textSize.Height + 10; 

                                cellHeights[cellIndex++] = cellHeight;
                            }

                            
                            //rowHeight = cellHeights.Max(); ---------------------------------
                            rowHeight = FindMax(cellHeights);

                            
                            int cellX = startX; 
                            cellIndex = 0;
                            foreach (var cell in row.Children)
                            {
                                int cellWidth = 100; 

                                
                                g.DrawRectangle(Pens.Black, cellX, currentY, cellWidth, rowHeight);

                                
                                StringFormat format = new StringFormat
                                {
                                    Alignment = StringAlignment.Near,
                                    LineAlignment = StringAlignment.Near,
                                    Trimming = StringTrimming.Word
                                };

                                
                                RectangleF textRect = new RectangleF(cellX + 5, currentY + 5, cellWidth - 10, rowHeight - 10);
                                g.DrawString(cell.InnerText, SystemFonts.DefaultFont, Brushes.Black, textRect, format);

                                cellX += cellWidth; 
                                cellIndex++;
                            }

                            currentY += rowHeight; 
                        }
                        y = currentY + 10; 
                        break;







                    case "img":
                        if (child.Attributes.ContainsKey("src"))
                        {
                            string src = child.Attributes["src"];

                            
                            if (!CustomStringEditor.IsNullOrWhiteSpace(htmlFilePath)) 
                            {
                                string htmlDirectory = Path.GetDirectoryName(htmlFilePath);
                                string fullPath = Path.Combine(htmlDirectory, src);
                                y += 20; 

                                if (File.Exists(fullPath))
                                {
                                    try
                                    {
                                        using (Image img = Image.FromFile(fullPath))
                                        {
                                            g.DrawImage(img, x, y, 100, 100); 
                                            y += 120; 
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        g.DrawString($"Error loading image: {ex.Message}", SystemFonts.DefaultFont, Brushes.Red, x, y);
                                        y += 20;
                                    }
                                }
                                else
                                {
                                    g.DrawString($"Image not found: {fullPath}", SystemFonts.DefaultFont, Brushes.Red, x, y);
                                    y += 20;
                                }
                            }
                        }
                        break;

                    case "div":
                        
                        if (!CustomStringEditor.IsNullOrWhiteSpace(child.InnerText))
                        {
                            g.DrawString(CustomStringEditor.Trim(child.InnerText), SystemFonts.DefaultFont, Brushes.Black, x, y); //-------------------
                            y += 20; 
                        }

                        
                        y = DrawHtmlTree(g, child, x, y);
                        break;

                    case "strong":
                        using (Font boldFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold))
                        {
                            g.DrawString(child.InnerText, boldFont, Brushes.Black, x, y);
                            y += 20;
                        }
                        break;

                    case "mark":
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.YellowGreen, x, y);
                        y += 20;
                        break;


                    case "footer":
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.Gray, x, y);
                        y += 20;
                        break;

                    case "title":
                        g.DrawString("Title: " + child.InnerText, SystemFonts.DefaultFont, Brushes.DarkBlue, x, y);
                        y += 30;
                        break;

                    case "a":
                        
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.Blue, x, y);
                        y += 30; 
                        break;


                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        float fontSize = child.TagName switch
                        {
                            "h1" => 20f,
                            "h2" => 18f,
                            "h3" => 16f,
                            _ => 14f
                        };
                        using (Font headingFont = new Font(SystemFonts.DefaultFont.FontFamily, fontSize, FontStyle.Bold))
                        {
                            g.DrawString(child.InnerText, headingFont, Brushes.Black, x, y);
                            y += (int)fontSize + 10; 
                        }
                        break;


                    case "span":
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.DarkGreen, x, y);
                        y += 20; 
                        break;


                    case "b":
                        using (Font boldFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold))
                        {
                            g.DrawString(child.InnerText, boldFont, Brushes.Black, x, y);
                            y += 20; 
                        }
                        break;


                    case "ul":
                        y += 10; 
                        int listIndent = 20; 
                        foreach (var listItem in child.Children) 
                        {
                            y = DrawHtmlTree(g, listItem, x + listIndent, y); 
                        }
                        y += 10; 
                        break;


                    case "li":
                        
                        g.DrawString("•", SystemFonts.DefaultFont, Brushes.Black, x, y);

                        
                        g.DrawString(child.InnerText, SystemFonts.DefaultFont, Brushes.Black, x + 15, y);
                        y += 20; 
                        break;




                    default:
                        
                        y = DrawHtmlTree(g, child, x, y); 
                        break;
                }
            }

            return y; 
        }

        private int FindMax(int[] values)
        {
            int max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                    max = values[i];
            }
            return max;
        }

        
    }
}
