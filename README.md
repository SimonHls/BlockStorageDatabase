# Building a Block-Based Database with B-Tree Indexing

The goal of this project is to build a block based database. The data will be stored in a single file. Furthermore, each column will be indexed using a b-tree, with the indexing data being stored in individual files, with one file per indexed column.

> [!NOTE]
>  The project was inspired by [this article](https://www.codeproject.com/Articles/1029838/Build-Your-Own-Database). It strikes a great balance of setting guide rails while not taking away the joy of figuring things out yourself. If you want to build a similar project, i would . 

## Technologies used 🛠
<div style="display: flex; flex-direction: row; gap: 10px">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
</div>

The main point of this project is to learn more about databases. I will use C# and .NET technologies for the implementation. I will also impose some rules on myself to make this a little more challenging:

### Rules

1. **No AI** to write code, except for simple syntax or theory questions.
2. **No NuGet packages** for the database-specific stuff, like the B+ tree and block storage implementation. Only standard language features are allowed.
3. **Clean Architecture**: The architecture should be reasonably clean and testable.
4. **Reliability**: There should be reasonable test coverage.

## Project goals 🎯

To be considered successful, the database should fulfill the following critera:

- [ ] The db should store numerical and text data in a table.
- [ ] The db should support queries with a simple SQL-like syntax (`SELECT`, `INSERT`, `UPDATE`, `DELETE`, `WHERE`).
- [ ] The `WHERE` should support full text search, including wildcards.
- [ ] The `WHERE` should support equals, greater than, and less then operators for numeric values.
- [ ] The db should be reasonably fast (benchmark to be determined).

# Documentation

I am writing a comprehensive project documentation while working on the project. I will probably add a shortened version here, and post the full version on my github page. I'll take care of this after finishing the first version of the project.
