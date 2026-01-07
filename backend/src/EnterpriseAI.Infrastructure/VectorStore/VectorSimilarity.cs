namespace EnterpriseAI.Infrastructure.VectorStore;

/// <summary>
/// Utility class for vector similarity calculations.
/// </summary>
public static class VectorSimilarity
{
    /// <summary>
    /// Calculates cosine similarity between two vectors.
    /// Returns a value between -1 and 1, where 1 means identical direction.
    /// </summary>
    public static float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must have the same dimensions");
        }

        if (vectorA.Length == 0)
        {
            return 0f;
        }

        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        magnitudeA = MathF.Sqrt(magnitudeA);
        magnitudeB = MathF.Sqrt(magnitudeB);

        if (magnitudeA == 0f || magnitudeB == 0f)
        {
            return 0f;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
