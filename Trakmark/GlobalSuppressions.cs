// This file contains assembly-level suppression attributes for analyzer rules
// that do not apply to auto-generated code in this project.
using System.Diagnostics.CodeAnalysis;

// CA1861: EF Core migration files are auto-generated; extracting constant array
// arguments into static readonly fields would be overwritten on next scaffold.
[assembly: SuppressMessage(
    "Performance",
    "CA1861:Avoid constant arrays as arguments",
    Justification = "Auto-generated EF Core migration files — pragma suppression would be lost on re-scaffold.",
    Scope = "namespaceanddescendants",
    Target = "~N:Trakmark.Migrations")]
