using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeetleX.WebApiTester.Codes
{
    public class Statistics
    {
        public Statistics()
        {
            CodeStatistics = new CodeStatistics[701];
            for (int i = 0; i < 701; i++)
            {
                CodeStatistics[i] = new CodeStatistics(i.ToString());
            }

            All = new CodeStatistics("All");
            Name = "NULL";
        }

        public string Name { get; set; }

        public CodeStatistics OtherStatus { get; private set; } = new CodeStatistics("Other");

        public CodeStatistics Status_1xx { get; private set; } = new CodeStatistics("1xx");

        public CodeStatistics Status_2xx { get; private set; } = new CodeStatistics("2xx");

        public CodeStatistics Status_3xx { get; private set; } = new CodeStatistics("3xx");

        public CodeStatistics Status_4xx { get; private set; } = new CodeStatistics("4xx");

        public CodeStatistics Status_5xx { get; private set; } = new CodeStatistics("5xx");

        public CodeStatistics All { get; private set; }

        public CodeStatistics[] CodeStatistics { get; private set; }

        public void Add(int code, long time)
        {
            All.Add(time);
            if (code >= 100 && code < 200)
                Status_1xx.Add(time);
            else if (code >= 200 && code < 300)
                Status_2xx.Add(time);
            else if (code >= 300 && code < 400)
                Status_3xx.Add(time);
            else if (code >= 400 && code < 500)
                Status_4xx.Add(time);
            else if (code >= 500 && code < 600)
                Status_5xx.Add(time);
            else
            {
                OtherStatus.Add(time);
            }

            if (code >= 701)
            {
                CodeStatistics[700].Add(time);
            }
            else
            {
                CodeStatistics[code].Add(time);
            }
        }


        public StatisticsData[] GetRegion(int start, int end)
        {
            List<StatisticsData> result = new List<StatisticsData>();
            for (int i = start; i < end; i++)
            {
                if (CodeStatistics[i].Count > 0)
                {
                    result.Add(CodeStatistics[i].GetData());
                }
            }
            return result.ToArray();
        }

        public StatisticsGroup GetData()
        {
            StatisticsGroup result = new StatisticsGroup();
            result.Url = Name;
            result.Items.Add(OtherStatus.GetData());
            result.Items.Add(Status_1xx.GetData());
            result.Items.Add(Status_2xx.GetData());
            result.Items.Add(Status_3xx.GetData());
            result.Items.Add(Status_4xx.GetData());
            result.Items.Add(Status_5xx.GetData());
            result.Items.Add(All.GetData());
            foreach (var item in CodeStatistics)
            {
                if (item.Count > 0)
                {
                    result.Items.Add(item.GetData());
                }
            }
            return result;
        }
    }

    public class CodeStatistics
    {
        public CodeStatistics(string name)
        {
            mLastTime = BeetleX.TimeWatch.GetTotalSeconds();
            Name = name;
        }

        public string Name { get; set; }

        private long mCount;

        public long Count => mCount;

        private double mLastTime;

        private long mLastCount;

        private double mFirstTime;

        public int AvgRps
        {
            get; set;
        }

        public int MaxRps
        {
            get; set;
        }

        public int Rps
        {
            get
            {
                double time = TimeWatch.GetTotalSeconds() - mLastTime;
                int value = (int)((double)(mCount - mLastCount) / time);
                mLastTime = TimeWatch.GetTotalSeconds();
                mLastCount = mCount;
                if (value > MaxRps)
                    MaxRps = value;

                AvgRps = (int)(mCount / (TimeWatch.GetTotalSeconds() - mFirstTime));

                return value;
            }
        }

        public void Add(long time)
        {
            long value = System.Threading.Interlocked.Increment(ref mCount);
            if (value == 1)
            {
                mFirstTime = TimeWatch.GetTotalSeconds();
                mLastTime = mFirstTime;
            }
            if (time <= 10)
                System.Threading.Interlocked.Increment(ref ms10);
            else if (time <= 20)
                System.Threading.Interlocked.Increment(ref ms20);
            else if (time <= 50)
                System.Threading.Interlocked.Increment(ref ms50);
            else if (time <= 100)
                System.Threading.Interlocked.Increment(ref ms100);
            else if (time <= 200)
                System.Threading.Interlocked.Increment(ref ms200);
            else if (time <= 500)
                System.Threading.Interlocked.Increment(ref ms500);
            else if (time <= 1000)
                System.Threading.Interlocked.Increment(ref ms1000);
            else if (time <= 2000)
                System.Threading.Interlocked.Increment(ref ms2000);
            else if (time <= 5000)
                System.Threading.Interlocked.Increment(ref ms5000);
            else if (time <= 10000)
                System.Threading.Interlocked.Increment(ref ms10000);
            else
                System.Threading.Interlocked.Increment(ref msOther);
        }

        public override string ToString()
        {
            return mCount.ToString();
        }

        private long ms10;

        private long ms10LastCount;

        public long Time10ms => ms10;

        private long ms20;

        private long ms20LastCount;

        public long Time20ms => ms20;

        private long ms50;

        private long ms50LastCount;

        public long Time50ms => ms50;

        private long ms100;

        private long ms100LastCount;

        public long Time100ms => ms100;

        private long ms200;

        private long ms200LastCount;

        public long Time200ms => ms200;

        private long ms500;

        private long ms500LastCount;

        public long Time500ms => ms500;

        private long ms1000;

        private long ms1000LastCount;

        public long Time1000ms => ms1000;

        private long ms2000;

        private long ms2000LastCount;

        public long Time2000ms => ms2000;

        private long ms5000;

        private long ms5000LastCount;

        public long Time5000ms => ms5000;

        private long ms10000;

        private long ms10000LastCount;

        public long Time10000ms => ms10000;

        private long msOther;

        private long msOtherLastCount;

        public long TimeOtherms => msOther;

        private double mLastRpsTime = 0;

        public StatisticsData GetData()
        {
            StatisticsData result = new StatisticsData();
            result.Count = Count;
            result.Rps = Rps;
            result.MaxRps = MaxRps;
            result.AvgRps = AvgRps;

            result.Name = Name;
            result.Times.Add(Time10ms);
            result.Times.Add(Time20ms);
            result.Times.Add(Time50ms);
            result.Times.Add(Time100ms);
            result.Times.Add(Time200ms);
            result.Times.Add(Time500ms);
            result.Times.Add(Time1000ms);
            result.Times.Add(Time2000ms);
            result.Times.Add(Time5000ms);
            result.Times.Add(Time10000ms);
            result.Times.Add(TimeOtherms);
            double now = TimeWatch.GetTotalSeconds();
            double time = now - mLastRpsTime;

            int value = (int)((double)(ms10 - ms10LastCount) / time);
            ms10LastCount = ms10;
            result.TimesRps.Add(value);


            value = (int)((double)(ms20 - ms20LastCount) / time);
            ms20LastCount = ms20;
            result.TimesRps.Add(value);


            value = (int)((double)(ms50 - ms50LastCount) / time);
            ms50LastCount = ms50;
            result.TimesRps.Add(value);


            value = (int)((double)(ms100 - ms100LastCount) / time);
            ms100LastCount = ms100;
            result.TimesRps.Add(value);


            value = (int)((double)(ms200 - ms200LastCount) / time);
            ms200LastCount = ms200;
            result.TimesRps.Add(value);


            value = (int)((double)(ms500 - ms500LastCount) / time);
            ms500LastCount = ms500;
            result.TimesRps.Add(value);


            value = (int)((double)(ms1000 - ms1000LastCount) / time);
            ms1000LastCount = ms1000;
            result.TimesRps.Add(value);


            value = (int)((double)(ms2000 - ms2000LastCount) / time);
            ms2000LastCount = ms2000;
            result.TimesRps.Add(value);


            value = (int)((double)(ms5000 - ms5000LastCount) / time);
            ms5000LastCount = ms5000;
            result.TimesRps.Add(value);


            value = (int)((double)(ms10000 - ms10000LastCount) / time);
            ms10000LastCount = ms10000;
            result.TimesRps.Add(value);


            value = (int)((double)(msOther - msOtherLastCount) / time);
            msOtherLastCount = msOther;
            result.TimesRps.Add(value);


            mLastRpsTime = now;
            return result;
        }

    }

    public class StatisticsGroup
    {
        public StatisticsGroup()
        {
            Items = new List<StatisticsData>();
        }

        public String Url { get; set; }

        public List<StatisticsData> Items { get; set; }
    }

    public class StatisticsData
    {
        public string Name { get; set; }

        public long Count { get; set; }

        public long Rps { get; set; }

        public long MaxRps { get; set; }

        public long AvgRps { get; set; }

        public List<long> Times { get; set; } = new List<long>();

        public List<long> TimesRps { get; set; } = new List<long>();


        public TimeStats[] GetTimeStats(int count = 20)
        {
            List<TimeStats> result = new List<TimeStats>();
            result.Add(new TimeStats { EndTime = 10, Count = Times[0], Color = 0, Rps = TimesRps[0] });
            result.Add(new TimeStats { EndTime = 20, StartTime = 10, Count = Times[1], Color = 1, Rps = TimesRps[1] });
            result.Add(new TimeStats { EndTime = 50, StartTime = 20, Count = Times[2], Color = 2, Rps = TimesRps[2] });
            result.Add(new TimeStats { EndTime = 100, StartTime = 50, Count = Times[3], Color = 3, Rps = TimesRps[3] });
            result.Add(new TimeStats { EndTime = 200, StartTime = 100, Count = Times[4], Color = 4, Rps = TimesRps[4] });
            result.Add(new TimeStats { EndTime = 500, StartTime = 200, Count = Times[5], Color = 5, Rps = TimesRps[5] });
            result.Add(new TimeStats { EndTime = 1000, StartTime = 500, Count = Times[6], Color = 6, Rps = TimesRps[6] });
            result.Add(new TimeStats { EndTime = 2000, StartTime = 1000, Count = Times[7], Color = 7, Rps = TimesRps[7] });
            result.Add(new TimeStats { EndTime = 5000, StartTime = 2000, Count = Times[8], Color = 8, Rps = TimesRps[8] });
            result.Add(new TimeStats { EndTime = 10000, StartTime = 5000, Count = Times[9], Color = 9, Rps = TimesRps[9] });
            result.Add(new TimeStats { StartTime = 10000, Count = Times[10], Color = 10, Rps = TimesRps[10] });
            var items = (from a in result select a).Take(count).ToArray();
            return items;
        }

        public TimeStats[] GetTimeCountStats(int count = 20)
        {
            List<TimeStats> result = new List<TimeStats>();
            result.Add(new TimeStats { EndTime = 10, Count = Times[0], Color = 0 });
            result.Add(new TimeStats { EndTime = 20, StartTime = 10, Count = Times[1], Color = 1 });
            result.Add(new TimeStats { EndTime = 50, StartTime = 20, Count = Times[2], Color = 2 });
            result.Add(new TimeStats { EndTime = 100, StartTime = 50, Count = Times[3], Color = 3 });
            result.Add(new TimeStats { EndTime = 200, StartTime = 100, Count = Times[4], Color = 4 });
            result.Add(new TimeStats { EndTime = 500, StartTime = 200, Count = Times[5], Color = 5 });
            result.Add(new TimeStats { EndTime = 1000, StartTime = 500, Count = Times[6], Color = 6 });
            result.Add(new TimeStats { EndTime = 2000, StartTime = 1000, Count = Times[7], Color = 7 });
            result.Add(new TimeStats { EndTime = 5000, StartTime = 2000, Count = Times[8], Color = 8 });
            result.Add(new TimeStats { EndTime = 10000, StartTime = 5000, Count = Times[9], Color = 9 });
            result.Add(new TimeStats { StartTime = 10000, Count = Times[10], Color = 10 });
            var items = (from a in result select a).Take(count).ToArray();
            return items;
        }

        public TimeStats[] GetTimeRpsStats(int count = 20)
        {
            List<TimeStats> result = new List<TimeStats>();
            result.Add(new TimeStats { EndTime = 10, Count = TimesRps[0], Color = 0 });
            result.Add(new TimeStats { EndTime = 20, StartTime = 10, Count = TimesRps[1], Color = 1 });
            result.Add(new TimeStats { EndTime = 50, StartTime = 20, Count = TimesRps[2], Color = 2 });
            result.Add(new TimeStats { EndTime = 100, StartTime = 50, Count = TimesRps[3], Color = 3 });
            result.Add(new TimeStats { EndTime = 200, StartTime = 100, Count = TimesRps[4], Color = 4 });
            result.Add(new TimeStats { EndTime = 500, StartTime = 200, Count = TimesRps[5], Color = 5 });
            result.Add(new TimeStats { EndTime = 1000, StartTime = 500, Count = TimesRps[6], Color = 6 });
            result.Add(new TimeStats { EndTime = 2000, StartTime = 1000, Count = TimesRps[7], Color = 7 });
            result.Add(new TimeStats { EndTime = 5000, StartTime = 2000, Count = TimesRps[8], Color = 8 });
            result.Add(new TimeStats { EndTime = 10000, StartTime = 5000, Count = TimesRps[9], Color = 9 });
            result.Add(new TimeStats { StartTime = 5000, Count = TimesRps[10], Color = 10 });
            var items = (from a in result select a).Take(count).ToArray();
            return items;
        }

    }

    public class TimeStats
    {
        public long Count { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public int Color { get; set; }

        public long Rps { get; set; }

    }


    public class StatsBaseItem
    {
        public StatsBaseItem()
        {

        }

        public StatsBaseItem(string label, long data, int colorIndex = 0)
        {
            this.name = label;
            this.value = data;
            this.color = colorIndex;
        }

        public StatsBaseItem(TimeStats timeStats)
        {
            this.color = timeStats.Color;
            rps = timeStats.Rps;
            if (timeStats.StartTime > 0 && timeStats.EndTime > 0)
            {
                if (timeStats.StartTime >= 1000)
                    name = $"{timeStats.StartTime / 1000}s";
                else
                    name = $"{timeStats.StartTime}ms";

                if (timeStats.EndTime >= 1000)
                    name += $"-{timeStats.EndTime / 1000}s";
                else
                    name += $"-{timeStats.EndTime}ms";

            }
            else if (timeStats.StartTime > 0)
            {
                if (timeStats.StartTime >= 1000)
                    name = $">{timeStats.StartTime / 1000}s";
                else
                    name = $">{timeStats.StartTime}ms";
            }
            else
            {
                name = $"<{timeStats.EndTime}ms";
            }
            value = timeStats.Count;
        }

        public StatsBaseItem PercentWith(long count)
        {
            double p = (double)value / (double)count;
            if (p > 0)
            {
                percent = $"({(int)(p * 10000) / 100d}%)";
            }
            return this;
        }

        public StatsBaseItem AddItems(IEnumerable<StatisticsData> items)
        {
            foreach (var item in items)
            {
                Items.Add(new StatsBaseItem(item));
            }

            return this;
        }

        public List<StatsBaseItem> Items { get; set; } = new List<StatsBaseItem>();

        public StatsBaseItem(StatisticsData statisticsData, int colorIndex = -1)
        {
            maxRps = statisticsData.MaxRps;
            avgRps = statisticsData.AvgRps;
            value = statisticsData.Count;
            if (colorIndex != -1)
                this.color = colorIndex;
            else
            {
                if (int.TryParse(statisticsData.Name, out int code))
                {
                    if (code < 200)
                    {
                        this.color = 0;
                    }
                    else if (code < 300)
                        this.color = 1;
                    else if (code < 400)
                        this.color = 2;
                    else if (code < 500)
                        this.color = 3;
                    else if (code < 6)
                        this.color = 4;
                    else
                        this.color = 5;
                }
            }
            name = statisticsData.Name;
            rps = statisticsData.Rps;

        }

        public string percent { get; set; }

        public long maxRps { get; set; }

        public long avgRps { get; set; }

        public string name { get; set; }

        public long value { get; set; }

        public long rps { get; set; }

        public int color { get; set; }
    }
}
