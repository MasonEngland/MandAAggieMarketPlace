namespace Server.Middleware;

public static class DotEnv
{
	public static void Config(string path)
	{
		if (!File.Exists(path))
		{
			throw new Exception(".env could not be found");
		}

		foreach (var line in File.ReadAllLines(path))
		{
			var parts = line.Split(
				'=',
				StringSplitOptions
					.RemoveEmptyEntries);

			if (parts.Length != 2) { throw new Exception("no environment var found");}

			Environment.SetEnvironmentVariable(parts[0], parts[1]);

		}
	}
}
