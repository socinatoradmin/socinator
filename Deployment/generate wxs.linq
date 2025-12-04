<Query Kind="Statements" />

var folder = @"D:\Development\Socinator\dominatorhouse-social\DominatorHouse\bin\Debug";
var files = Directory.GetFiles(folder);
foreach(var f in files.Select(a=>Path.GetFileName(a)).Where(a=>!a.EndsWith("pdb")).Distinct())
{
	Console.WriteLine(string.Format("<File Id=\"{0}\" Name=\"{1}\" Source=\"$(var.SourceDir)\\{2}\"/>",
	f.Replace(".","").Replace("_","").Replace(" ","").Replace("-",""),f,f));
}