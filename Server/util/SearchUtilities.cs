namespace Server.Util;
/**
Code Created by Github Copilot
*/

public static class SearchUtilities
{
    public static int PartialLevenshtein(string query, string target)
    {
        if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(target))
            return Math.Max(query.Length, target.Length);

        int minDist = int.MaxValue;
        int window = query.Length;

        // If target is shorter than query, just compare them directly
        if (target.Length < window)
            return Levenshtein(query, target);

        for (int i = 0; i <= target.Length - window; i++)
        {
            string sub = target.Substring(i, window);
            int dist = Levenshtein(query, sub);
            if (dist < minDist)
                minDist = dist;
            // Early exit if perfect match
            if (minDist == 0)
                break;
        }
        return minDist;
    }

    private static int Levenshtein(string source, string target)
    {
        int[,] distanceMatrix = new int[source.Length + 1, target.Length + 1];

        // Initialize the first row and column with edit distances
        for (int i = 0; i <= source.Length; i++)
        {
            distanceMatrix[i, 0] = i; // Cost of deletions
        }

        for (int j = 0; j <= target.Length; j++)
        {
            distanceMatrix[0, j] = j; // Cost of insertions
        }

        // Compute the Levenshtein distance
        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int costForSubstitution = (source[i - 1] == target[j - 1]) ? 0 : 1;

                int deletionCost = distanceMatrix[i - 1, j] + 1;
                int insertionCost = distanceMatrix[i, j - 1] + 1;
                int substitutionCost = distanceMatrix[i - 1, j - 1] + costForSubstitution;

                distanceMatrix[i, j] = Math.Min(Math.Min(deletionCost, insertionCost), substitutionCost);
            }
        }

        return distanceMatrix[source.Length, target.Length];
    }
}