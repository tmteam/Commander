using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gin
{
    public class DateTimeValue
    {
        public DateTimeValue() { }
        public DateTimeValue(DateTime time)
        {
            Parse(time);
        }
        public DateTimeValue(TimeSpan timeSpan)
        {
            Parse(timeSpan);
        }
        public DateTimeValue(string str)
        {
            DateTime time;
            str = str.Trim();
            if (TryParseDateTime(str))
                return;
            else if (TryParseTimeSpan(str))
                return;
            else if (DateTime.TryParse(str, out time))
            {
                Parse(time);
                return;
            }
            else
                throw new InvalidCastException();
        }
        bool TryParseTimeSpan(string str)
        {
            char ch = str.ToLower().Last<char>();
            Func<double, TimeSpan> func = null;
            TimeSpan? res = null;
            switch (ch)
            {
                case 'm':
                    func = new Func<double, TimeSpan>(TimeSpan.FromMinutes);
                    break;
                case 's':
                    func = new Func<double, TimeSpan>(TimeSpan.FromSeconds);
                    break;
                case 'w':
                    func = d => TimeSpan.FromDays(d * 7.0);
                    break;
                case 'd':
                    func = new Func<double, TimeSpan>(TimeSpan.FromDays);
                    break;
                case 'h':
                    func = new Func<double, TimeSpan>(TimeSpan.FromHours);
                    break;
                default:
                    {
                        double arg;
                        if (!double.TryParse(str, out arg))
                            return false;
                        else
                            res = TimeSpan.FromMilliseconds(double.Parse(str));
                        break;
                    }
            }
            if (!res.HasValue)
            {
                double arg2;
                if (!double.TryParse(str.Remove(str.Length - 1), out arg2))
                    return false;
                res = func(arg2);
            }
            Parse(res.Value);
            return true;
        }
        void Parse(TimeSpan timeSpan)
        {
            Year = null;
            Month = null;
            Day = timeSpan.Days;
            Hour = timeSpan.Hours;
            Minute = timeSpan.Minutes;
            Second = timeSpan.Seconds;
            MiliSecond = timeSpan.Milliseconds;
        }
        void Parse(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
            MiliSecond = dateTime.Millisecond;
        }

        bool TryParseDateTime(string str)
        {
            var strArray = str.Split(new char[] { ' ' });
            if ((strArray.Length != 1) && (strArray.Length != 2))
                throw new InvalidCastException();

            string dateStr = null;
            string timeStr = null;

            if (strArray[0].Contains<char>('.'))
                dateStr = strArray[0];
            else if (strArray[0].Contains<char>(':'))
                timeStr = strArray[0];
            if (strArray.Length > 1)
            {
                if (timeStr != null)
                    return false;
                if (!strArray[1].Contains<char>(':'))
                    return false;
                timeStr = strArray[1];
            }
            if (string.IsNullOrWhiteSpace(dateStr) && string.IsNullOrWhiteSpace(timeStr))
                return false;
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                var strArray2 = dateStr.Split(new char[] { '.' });
                if ((strArray2.Length != 2) && (strArray2.Length != 3))
                    return false;
                Day = new int?(int.Parse(strArray2[0]));
                Month = new int?(int.Parse(strArray2[1]));
                if (strArray2.Length == 3)
                    Year = new int?(int.Parse(strArray2[2]));
            }
            if (!string.IsNullOrWhiteSpace(timeStr))
            {
                string[] strArray3 = timeStr.Split(new char[] { ':' });
                if ((strArray3.Length != 2) && (strArray3.Length != 3))
                    return false;
                Hour = new int?(int.Parse(strArray3[0]));
                Minute = new int?(int.Parse(strArray3[1]));
                if (strArray3.Length == 3)
                    Second = new int?(int.Parse(strArray3[2]));
            }
            return true;
        }
        public int? Year;
        public int? Month;
        public int? Day;
        public int? Hour;
        public int? Minute;
        public int? Second;
        public int? MiliSecond;
        public TimeSpan ToTimeSpan()
        {
            var ans = new TimeSpan();
            if(Day.HasValue)
                ans+= TimeSpan.FromDays(Day.Value);
            if(Hour.HasValue)
                ans+= TimeSpan.FromHours(Hour.Value);
            if(Minute.HasValue)
                ans+= TimeSpan.FromMinutes(Minute.Value);
            if(Second.HasValue)
                ans+= TimeSpan.FromSeconds(Second.Value);
            if(MiliSecond.HasValue)
                ans+= TimeSpan.FromMilliseconds(MiliSecond.Value);
            return ans;
        }
        public DateTime ToDateTime()
        {
            var ans =  new DateTime(
                Year.HasValue ? Year.GetValueOrDefault() : DateTime.Now.Year,
                Month.HasValue ? Month.GetValueOrDefault() : DateTime.Now.Month,
                Day.HasValue ? Day.GetValueOrDefault() : DateTime.Now.Day,
                Hour.HasValue ? Hour.GetValueOrDefault() : 0,
                Minute.HasValue ? Minute.GetValueOrDefault() : 0,
                Second.HasValue ? Second.GetValueOrDefault() : 0) ;
            if (MiliSecond.HasValue)
                ans += TimeSpan.FromMilliseconds(MiliSecond.Value);
            return ans;
        }
    }
}
