# 🕷️ HTML Crawler – Course Project

This project is a console-based HTML document parser and editor, developed as a university course project for Data Structures & Algorithms using **C#** and **Windows Forms** for visualization.

It features:
- A tree-based HTML model parser
- XPath-like queries for traversal
- Parallel search with threading
- In-place node updates and shallow/deep copying
- Custom Huffman compression for saving/loading
- Basic GUI-based HTML visualization using GDI

---

## 🚀 Features

### 🌳 Tree-Based HTML Model
- Parses raw HTML into a tree structure using a custom `HtmlNode` class
- Handles unmatched tags, self-closing tags (`<img>`, `<br>`, etc.)
- Fully custom stack and list structures (no .NET collections!)

### 🔍 XPath-Like Searching
Supports:
- `//` for root
- `/` for children
- `*` for wildcard tags
- `[n]` for index-based matching
- `[@attr='value']` for attribute-based search

> Example: `PRINT "//html/body/p[2]"` → prints only the second `<p>` tag’s content

### ⚡ Parallel Search
- Uses threads to improve performance in large trees
- Locking ensures safe result collection across threads

### 🛠 Node Modification
- Update content in-place with `SET` commands
- Replace elements or inject subtrees using parsed HTML strings
- Performs targeted mutation without full tree rebuild

### 📋 Node Copying
- Implements **shallow copying** and **smart deep copy on modification**
- Supports commands like:  
  `COPY "//p[1]" "//div[@id='target']"`

### 🗜️ Huffman Compression
- Custom Huffman encoder/decoder (no libraries!)
- Stores HTML + resources (e.g. `.bmp` images) in a single `.hcz` archive
- Supports full file restore

### 🖼 HTML Visualizer (WinForms)
- GDI-based rendering of basic HTML elements
- Shows table outlines, text, images (`<img src="..." />`), and links
- Scales for nested layout and updates dynamically

---

## 🛠 Tech Stack

| Part            | Technology       |
|-----------------|------------------|
| Language        | C# (.NET)        |
| UI              | Windows Forms    |
| Compression     | Custom Huffman   |
| Data Structures | Fully Custom     |
| Search          | BFS / Parallel BFS (Threads) |
| Visualization   | GDI              |

---

## 🧠 Key Data Structures
- `CustomList<T>`
- `CustomLinkedList<T>`
- `CustomStack<T>`
- `CustomDictionary<K,V>`
- `HtmlNode`, `HtmlParser`, `HuffmanCompressor`

---

## 🧪 How to Use

```bash
# Search command:
PRINT "//html/body/table[@id='table2']/tr[2]/td"

# Modify nodes:
SET "//p" "New content"

# Copy nodes:
COPY "//div" "//table/tr/td"

# Save compressed:
SAVE "example.hcz"

# Load from file:
LOAD "example.hcz"
