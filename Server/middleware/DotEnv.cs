namespace Server.Middleware;

public static class DotEnv
{
	public static void Config(string path)
	{
		if (!File.Exists(path)) throw new Exception(".env could not be found");

		foreach (var line in File.ReadAllLines(path))
		{
			var trimmedLine = line.Trim();

			// Skip empty lines or comments
			if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
				continue;

			// Split only on the first '='
			var index = trimmedLine.IndexOf('=');
			if (index == -1)
				continue; // Skip invalid lines

			var key = trimmedLine.Substring(0, index).Trim();
			var value = trimmedLine.Substring(index + 1).Trim();

			Environment.SetEnvironmentVariable(key, value);
		}
    }
	
}
