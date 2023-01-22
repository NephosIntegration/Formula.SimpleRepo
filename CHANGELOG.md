# Change Log

## 1.0.2

- Added functionality for NULL comparisons

## 1.0.3

- Added support for alternate connection types (DB2, SQLServer, etc..) as well as ability to specify the dialect, all from the connection details attribute.
- Adde support for registering repositories contained in seperate library / assembly

## 1.1.1

- Added support for transformations `TransformConstraints`
- Added additional debugging when debugger is attached

## 1.1.2

- Added ability to skip contraints that have specific attributes tied to them `[NotMapped]` and `[IgnoreSelect]`

## 1.6.1

- ADS-7146 - Refactored SimpleCrud from static to needing to be instantiated / injected to solve encapsulation problems under load (everything needed to be scoped instead of global)
- ADS-6437 - Parameters don't clear on re-use of same repository
- Added Unit tests
- Added workflows / actions
