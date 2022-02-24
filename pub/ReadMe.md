# Creating a new .nupkg file

* Update AssemblyInfo.cs with new version numbers
* Build Release in Visual Studio
* Update MultiPlug.Ext.SMEMA.nuspec with new version numbers
* Run pack.bat (you need nuget.exe below the project directory)
* Upload it to https://www.nuget.org/