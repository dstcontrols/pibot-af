using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Bot_Application1
{
    public static class LUISParse
    {
        public static string ParseDateTime(Entity e)
        {
            string result = null;
            string format;
            DateTime? d;
            switch (e.type)
            {
                case "builtin.datetime.time":
                    format = "yyyy-MM-ddThh";
                     d = ConvertFormatedDate(e.resolution.time, format);
                    if (d != null)
                    {
                        result = d.ToString();
                    }
                    else
                    {
                        //monday XXXX-WXX-1
                        format = @"XXXX-WXX-\dT\d\d";
                        Regex r = new Regex(format, RegexOptions.IgnoreCase);
                        var match = r.IsMatch(e.resolution.time);
                        if (match)
                        {
                            string n = e.resolution.time;
                            result = DayOfTheWeekConversion(n.Substring(0, n.Length - 3)).ToString();
                        }
                        else
                        {
                            result = "";

                        }
                    }
                        break;
                case "builtin.datetime.date":
                    format = "yyyy-MM-ddThh";
                     d = ConvertFormatedDate(e.resolution.date, format);
                    if (d != null)
                    {
                        result = d.ToString();
                    }
                    else
                    {
                        //monday XXXX-WXX-1
                        format = @"XXXX-WXX-\d";
                        Regex r = new Regex(format, RegexOptions.IgnoreCase);
                        var match = r.IsMatch(e.resolution.date);
                        if(match)
                        {
                            result = DayOfTheWeekConversion(e.resolution.date).ToString();
                        }
                        else
                        {
                            format = "yyyy-MM";
                            d = ConvertFormatedDate(e.resolution.date, format);
                            if (d != null)
                            {
                                result = d.ToString();
                            }
                            else
                            {
                                format = "yyyy-MM-dd";
                                d = ConvertFormatedDate(e.resolution.date, format);
                                if (d != null)
                                {
                                    result = d.ToString();
                                }
                                else
                                {
                                    result = "";
                                }
                            }

                        }
                    }
                    break;
                default:
                    break;
            }

            //            builtin.datetime.date
            //            tomorrow
            //{
            //                "type": "builtin.datetime.date",
            //    "entity": "tomorrow",
            //	"resolution": {
            //                    "date": "2015-08-15"
            //    }
            //            }
            //            january 10 2009
            //{
            //                "type": "builtin.datetime.date",
            //    "entity": "january 10 2009",
            //	"resolution": {
            //                    "date": "2009-01-10"
            //    }
            //            }
            //            monday
            //{
            //                "entity": "monday",
            //    "type": "builtin.datetime.date",
            //    "resolution": {
            //                    "date": "XXXX-WXX-1"
            //    }
            //            }
            //            next week
            //{
            //                "entity": "next week",
            //    "type": "builtin.datetime.date",
            //    "resolution": {
            //                    "date": "2015-W34"
            //    }
            //            }
            //            next monday
            //{
            //                "entity": "next monday",
            //    "type": "builtin.datetime.date",
            //    "resolution": {
            //                    "date": "2015-08-17"
            //    }
            //            }
            //            week of september 30th
            //{
            //                "entity": "week of september 30th",
            //    "type": "builtin.datetime.date",
            //    "resolution": {
            //                    "comment": "weekof",
            //        "date": "XXXX-09-30"
            //    }
            //            }
            //            builtin.datetime.time
            //3 : 00
            //{
            //                "type": "builtin.datetime.time",
            //    "entity": "3 : 00",
            //    "resolution": {
            //                    "comment": "ampm",
            //        "time": "T03:00"
            //    }
            //            }
            //            4 pm
            //{
            //                "type": "builtin.datetime.time",
            //    "entity": "4 pm",
            //    "resolution": {
            //                    "time": "T16"
            //    }
            //            }
            //            tomorrow morning
            //{
            //                "entity": "tomorrow morning",
            //    "type": "builtin.datetime.time",
            //    "resolution": {
            //                    "time": "2015-08-15TMO"
            //    }
            //            }
            //            tonight
            //{
            //                "entity": "tonight",
            //    "type": "builtin.datetime.time",
            //    "resolution": {
            //                    "time": "2015-08-14TNI"
            //    }
            //            }
            //        }



            return result;
        }

        private static DateTime? ConvertFormatedDate(string dateString, string format)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime? result = null;
            try
            {
                result = DateTime.ParseExact(dateString, format, provider);
                Console.WriteLine("{0} converts to {1}.", dateString, result.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} is not in the correct format.", dateString);
            }
            return result;
        }

        private static DateTime? DayOfTheWeekConversion(string dateString)
        {
            DateTime now = DateTime.Now;
            int dow = (int)now.DayOfWeek;
            int request;
            bool s = int.TryParse(dateString.Substring(dateString.Length - 1), out request);
            if (request <= dow)
            {
                now = now.AddDays(request - dow);
            }
            else
            {
                now = now.AddDays(-7 + (request - dow));
            }
            return now;
        }


    }
}