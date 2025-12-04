<Query Kind="Program" />

private static Regex _regex = new Regex("[^A-Za-z0-9]+");
private static string _root = @"D:\Development\bitbucket\Sumit Ghosh\dominatorhouse-social\DominatorHouse\bin\Debug";

void Main()
{
	ComponentFromDir(_root, "INSTALLFOLDER");
}

public static void ComponentFromDir(string path, string name)
{
	var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(a=>!a.EndsWith(".pdb"));
	if (files.Any())
	{
		var sb = new StringBuilder();
		var id = path == _root ? "root" : $"Component{name}";
		sb.AppendLine($"<Component Id=\"{id}\" Directory=\"{name}Dir\" Guid=\"{Guid.NewGuid().ToString().ToUpper()}\">");
		foreach (var f in files.OrderBy(a => a))
		{
			var filename = Path.GetFileName(f);
			var filenameWithPath = f.Replace(_root, string.Empty);
			sb.AppendLine($"	<File Id=\"{_regex.Replace(filenameWithPath, string.Empty)}\" Name=\"{filename}\" Source=\"$(var.SourceDir){filenameWithPath}\"/>");
		}
		sb.AppendLine(@"</Component>");
		sb.ToString().Dump();
	}

	foreach (var dir in Directory.GetDirectories(path))
	{
		ComponentFromDir(dir, _regex.Replace(dir.Replace(_root, string.Empty), string.Empty));
	}
}


// Define other methods and classes here