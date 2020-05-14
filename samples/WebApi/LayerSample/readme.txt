We use the unmanaged implementation of SQLite provided by the creators via a NuGet package.  Specifically, we use the package ‘System.Data.SQLite Core (x86/x64)’.  In orde to use ThinkGeo's SQLiteExtension, it will  require you to add a reference to the following NuGet package so that the unmanaged assemblies will be automatically included in your bin folder. 

The specific NuGet package you need to refrence is:

<package id = "System.Data.SQLite.Core" version= "1.0.98.1" targetFramework= "net4" />

For more information, please refer to: https://www.nuget.org/packages/System.Data.SQLite.Core/