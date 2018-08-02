### 1.0.0
Initial release.

### 1.0.1
Improved broken csv text compatibility.

### 1.0.2
Added System.Single and System.Int64 support.

### 1.0.3
Fixed unexpected behavior when dialing with System.Single and System.Int64.

### 1.1.0
Improved performance for huge csv strings.

### 1.2.0
Allowed custom record formatters and parsers.

### 1.3.0
Added escape and MSExcel double number parse compatibility.
- If a string contains quote or separator, it will be surrounded with quotes. quotes are escaped with Visual Basic's rules of escaping quotes in strings.
- Supports to parse MSExcel's double string. US Dollar symbols, whitespaces, scientific notations, parentheses and thousands delimiters are allowed in a string of double. 
- MSExcel's OADate and decimal fractions are not supported since they will cause ambiguity problems.