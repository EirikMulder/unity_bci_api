using System.Collections.Generic;
using System;
using System.Linq;

namespace SensorAPI
{
namespace Data
{
/**
 * @brief A double value (value) paired with a DateTime timestamp (time)
 */
public class TimedData
{
    /** @brief The timestamp when the value was measured */
    public DateTime time;
    /** @brief The saved value */
    public double value;

    /** @brief implicitly convert to double by just using the value attribute */
    public static implicit operator double(TimedData data) => data.value;

    /** @brief Initialize with the provided value and time */
    public TimedData(double value, DateTime time)
    {
        this.time = time;
        this.value = value;
    }

    /** @brief Initialize with the provided value and the current time */
    public TimedData(double value)
    {
        this.time = DateTime.Now;
        this.value = value;
    }
}

/** @brief A list of TimedData values.
 *  Keeps track of the number of values, the max number of values (for rolling averages),
 *  and the maximum absolute value of an added value.
 */
public class DataList
{
    List<TimedData> data;
    /** @brief The number of entries in the list */
    public int Count { get { return data.Count; } }
    /** @brief The max number of entries in the list.
     *  When exceeded, the earliest value added is removed
     */
    public int limit;
    /** @brief The max absolute value of an entry.
     *  When exceeded, the value is rounded to the limit.
     *  If set to 0, there is no limit.
     */
    public double deviationLimit;

    /** @brief Initialize the DataList with default limit = 10, and no deviationLimit. */
    public DataList(int limit = 10, int deviationLimit = 0)
    {
        data = new List<TimedData>();
        this.limit = limit;
        this.deviationLimit = deviationLimit;
    }

    /** @brief Initialize the DataList with a List<TimedData>. */
    public DataList(List<TimedData> data, int limit = 10, int deviationLimit = 0)
    {
        this.data = data;
        this.limit = limit;
        this.deviationLimit = deviationLimit;
    }

    public TimedData this[int i]
    {
        get { return data[i]; }
        set { data[i] = value; }
    }

    public static implicit operator double(DataList x) => x[x.Count-1].value;

    /** @brief add new double value at the current time. */
    public void Add(double value)
    {
        data.Add(new TimedData(value, DateTime.Now));
        if (Count > limit) { data.RemoveRange(0, Count-limit); }
    }

    /** @brief add new double value at the specified time. */
    public void Add(double value, DateTime time)
    {
        data.Add(new TimedData(value, time));
        if (Count > limit) { data.RemoveRange(0, Count-limit); }
    }

    /** @brief add new TimedData value */
    public void Add(TimedData value)
    {
        data.Add(value);
    }

    /** @brief Take the average of the list. */
    public double Average()
    {
        return data.Average(x => x.value);
    }

    /** @brief Take a weighted average of the list.
     * weight is multipled with the index of each value to produce the weight.
     */
    public double WeightedAverage(double weight)
    {
        double average = 0;
        double added = 0;
        for (int i = 0; i < Count; ++i)
        {
            average += data[i].value * (i + 1) * weight;
            added += (i + 1) * weight;
        }
        average /= added;
        return average;
    }
}
} // Namespace Data
} // Namespace BCIData
