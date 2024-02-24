using System.Diagnostics;

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

			Debug.WriteLine(parts[1]);
			if (parts.Length != 2) { continue;  }

			Environment.SetEnvironmentVariable(parts[0], parts[1]);

		}
	}
}
