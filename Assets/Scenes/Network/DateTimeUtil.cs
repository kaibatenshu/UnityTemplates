using UnityEngine;
using System;

public static class DateTimeUtil {
	public const long A_DAY_MILISECOND = 86400000;

	private static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static long currentTimeMillis {
		get{
            return (long)(DateTime.UtcNow - dateTime1970).TotalMilliseconds;
        }
	}
}
