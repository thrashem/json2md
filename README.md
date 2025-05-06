## json2md
Usage: json2md  
Reads JSON from standard input and outputs Markdown to standard output.  
Input must be UTF-8 encoded JSON without surrounding double quotes.  
Outputs a bullet list with top-level keys, indented subkeys, and all values (key: value for scalars), reflecting the JSON structure.  
Examples:  
ECHO {"users":[{"id":1,"name":"John Doe"}],"settings":{"region":"Asia"}} | json2md  
Output:  
\- users  
&nbsp;&nbsp;\- id: 1  
&nbsp;&nbsp;\- name: "John Doe"  
\- settings  
&nbsp;&nbsp;\- region: "Asia"
