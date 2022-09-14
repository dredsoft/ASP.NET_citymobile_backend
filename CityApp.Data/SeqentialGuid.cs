using System;

public class SequentialGuid
{
    static readonly DateTime epoch = new DateTime(1900, 1, 1);
    static readonly int[] sqlOrderMap = new int[16] { 3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10 };

    public static Guid GenerateComb()
    {
        DateTime now = DateTime.Now;
        TimeSpan span = new TimeSpan(now.Ticks - epoch.Ticks);
        TimeSpan timeOfDay = now.TimeOfDay;

        byte[] destinationArray = Guid.NewGuid().ToByteArray();
        byte[] bytes = BitConverter.GetBytes(span.Days);
        byte[] array = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));

        Array.Reverse(bytes);
        Array.Reverse(array);

        Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);
        Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);

        return new Guid(destinationArray);
    }

    public Guid CurrentGuid { get; private set; }

    public SequentialGuid()
    {
        CurrentGuid = GenerateComb();
    }

    public SequentialGuid(Guid previousGuid)
    {
        CurrentGuid = previousGuid;
    }

    public static SequentialGuid operator ++(SequentialGuid sequentialGuid)
    {
        byte[] bytes = sequentialGuid.CurrentGuid.ToByteArray();
        for (int mapIndex = 0; mapIndex < 16; mapIndex++)
        {
            int bytesIndex = sqlOrderMap[mapIndex];
            bytes[bytesIndex]++;
            if (bytes[bytesIndex] != 0)
            {
                break; // No need to increment more significant bytes
            }
        }
        sequentialGuid.CurrentGuid = new Guid(bytes);
        return sequentialGuid;
    }
}