using System.Security.Cryptography;

namespace _01_Data.Utilities;

public static class SequentialGuid
{
    public enum SequentialGuidType
    {
        SequentialAsString, // SQL Server için
        SequentialAsBinary,
        SequentialAtEnd
    }

    public static Guid NewGuid(SequentialGuidType guidType = SequentialGuidType.SequentialAsString)
    {
        byte[] randomBytes = new byte[10];
        RandomNumberGenerator.Fill(randomBytes);

        long timestamp = DateTime.UtcNow.Ticks / 10000L; // Milisaniye cinsinden
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        byte[] guidBytes = new byte[16];

        switch (guidType)
        {
            case SequentialGuidType.SequentialAsString:
            case SequentialGuidType.SequentialAsBinary:
                Array.Copy(timestampBytes, 2, guidBytes, 0, 6); // zaman
                Array.Copy(randomBytes, 0, guidBytes, 6, 10);   // rastgele
                break;
            case SequentialGuidType.SequentialAtEnd:
                Array.Copy(randomBytes, 0, guidBytes, 0, 10);
                Array.Copy(timestampBytes, 2, guidBytes, 10, 6);
                break;
        }

        return new Guid(guidBytes);
    }
}