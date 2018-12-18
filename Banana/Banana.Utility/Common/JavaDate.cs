/***********************************
 * Coder：EminemJK
 * Date：2018-11-21
 * 
 * Last Update：2018-12-18
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Utility.Common
{
    /// <summary>
    /// 时间帮助类|UnixTime helper
    /// </summary>
    public class JavaDate
    {
        /// <summary>
        /// BaseTime
        /// </summary>
        public static readonly DateTime JavaDateBase = new DateTime(2000, 1, 1);

        /// <summary>
        /// 获取当前时间的时间戳
        /// </summary>
        /// <returns>Int整型的时间戳</returns>
        public static int GetNowTime()
        {
            return GetUnixTime(DateTime.Now);
        }

        /// <summary>
        /// DateTime转换为Int
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetUnixTime(DateTime time)
        {
            return (int)(GetLongTime(time) / 1000L);
        }

        /// <summary>
        /// 获取长整形时间，如需要转Int，如：int iDate = (int)(JavaDate.GetTime(dDate) / 1000L);
        /// </summary>
        public static long GetLongTime(DateTime time)
        {
            return (time.ToUniversalTime() - JavaDateBase).Ticks / 10000;
        }

        /// <summary>
        ///  从时间戳转换为DateTime时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(int time)
        {
            return GetDateTime(time * 1000L);
        }

        /// <summary>
        /// 长整形时间获取时间格式
        /// </summary>
        public static DateTime GetDateTime(long time)
        {
            return new DateTime(JavaDateBase.Ticks + time * 10000, DateTimeKind.Utc).ToLocalTime();
        }

        #region 根据时间获取当前是第几周
        /// <summary>
        /// Gets the current week based on the time
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        public static int GetWeekIndex(DateTime dTime)
        {
            //如果12月31号与下一年的1月1好在同一个星期则算下一年的第一周
            try
            {
                //需要判断的时间
                //DateTime dTime = Convert.ToDateTime(strDate);
                //确定此时间在一年中的位置
                int dayOfYear = dTime.DayOfYear;

                //DateTime tempDate = new DateTime(dTime.Year,1,6,calendar);
                //当年第一天
                DateTime tempDate = new DateTime(dTime.Year, 1, 1);

                //确定当年第一天
                int tempDayOfWeek = (int)tempDate.DayOfWeek;
                tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;
                //确定星期几
                int index = (int)dTime.DayOfWeek;

                index = index == 0 ? 7 : index;

                //当前周的范围
                DateTime retStartDay = dTime.AddDays(-(index - 1));
                DateTime retEndDay = dTime.AddDays(7 - index);

                //确定当前是第几周
                int weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek - 1) / 7);


                if (retStartDay.Year < retEndDay.Year)
                {
                    weekIndex = 1;
                }

                return weekIndex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //string retVal = retStartDay.ToString("yyyy/MM/dd") + "～" + retEndDay.ToString("yyyy/MM/dd");

        }

        public static int GetWeekIndex(string strDate)
        {
            try
            {
                //需要判断的时间
                DateTime dTime = Convert.ToDateTime(strDate);
                return GetWeekIndex(dTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static string GetWeekRange(int year, int weekIndex)
        {
            try
            {
                if (weekIndex < 1)
                {
                    throw new Exception("请输入大于0的整数");
                }

                int allDays = (weekIndex - 1) * 7;
                //确定当年第一天
                DateTime firstDate = new DateTime(year, 1, 1);
                int firstDayOfWeek = (int)firstDate.DayOfWeek;

                firstDayOfWeek = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;

                //周开始日
                int startAddDays = allDays + (1 - firstDayOfWeek);
                DateTime weekRangeStart = firstDate.AddDays(startAddDays);
                //周结束日
                int endAddDays = allDays + (7 - firstDayOfWeek);
                DateTime weekRangeEnd = firstDate.AddDays(endAddDays);

                if (weekRangeStart.Year > year ||
                 (weekRangeStart.Year == year && weekRangeEnd.Year > year))
                {
                    throw new Exception("今年没有第" + weekIndex + "周。");
                }

                return WeekRangeToString(weekRangeStart, weekRangeEnd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string GetWeekRange(int weekIndex)
        {
            try
            {
                return GetWeekRange(DateTime.Now.Year, weekIndex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string WeekRangeToString(DateTime weekRangeStart, DateTime weekRangeEnd)
        {
            string strWeekRangeStart = weekRangeStart.ToString("MMdd");
            string strWeekRangeend = weekRangeEnd.ToString("MMdd");

            return strWeekRangeStart + "-" + strWeekRangeend;

        }

        #endregion
    }
}
