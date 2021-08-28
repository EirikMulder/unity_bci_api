using System.Collections.Generic;
using System;
using System.Linq;

public class TimedData
{
    public DateTime time;
    public double value;

    public static implicit operator double(TimedData data) => data.value;

    public TimedData(double value, DateTime time)
    {
        this.time = time;
        this.value = value;
    }

    public TimedData(double value)
    {
        this.time = DateTime.Now;
        this.value = value;
    }
}

public class DataList
{
    public List<TimedData> data;
    public int Count { get { return data.Count; } }
    public int limit;
    public double deviationLimit;

    public DataList(int limit = 10, int deviationLimit = 0)
    {
        data = new List<TimedData>();
        this.limit = limit;
        this.deviationLimit = deviationLimit;
    }

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

    public void Add(double value)
    {
        data.Add(new TimedData(value, DateTime.Now));
        if (Count > limit) { data.RemoveRange(0, Count-limit); }
    }

    public void Add(double value, DateTime time)
    {
        data.Add(new TimedData(value, time));
        if (Count > limit) { data.RemoveRange(0, Count-limit); }
    }

    public void Add(TimedData value)
    {
        data.Add(value);
    }

    public double Average()
    {
        return data.Average(x => x.value);
    }

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
