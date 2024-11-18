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

### 1.3.1
Added read-only models support.

### 1.3.2
- Added global csv column selection setting.
- Added per-class column order setting override support.
- Added per-class csv column selection setting override support.

### 1.4.0
- Fallback to CLR double parser when culture is not en-US, ja-JP or zh-CN.
- Supported nullable primitive value types.
- Refactored some preview members.
- Supported Chinese and Japanese currency symbol in the built-in double parser.

### 1.4.1
- Added support for running in multi-threaded code.

### 1.5.0
- Improved compatibility of broken CSV rows that have more elements than columns.
- Updated dependencies to solve vulnerability warnings. 
- Reduced allocation by removing boxing and unboxing with dynamic methods.