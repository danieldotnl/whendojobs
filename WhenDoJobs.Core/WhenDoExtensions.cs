using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core
{
    public static class WhenDoExtensions
    {
        

        public static Dictionary<DayOfWeek, List<TimeSpan>> ToWhenDoSchedule(this List<ScheduleDefinition> definitions)
        {
            if (definitions == null || definitions.Count == 0)
                return null;
            var schedule = new Dictionary<DayOfWeek, List<TimeSpan>>();

            foreach (var definition in definitions)
            {
                var times = StringsToTimespans(definition.TimesOfDay);
                var days = definition.Days.Distinct().ToDaysOfWeek();
                foreach (var day in days)
                {
                    if (!schedule.ContainsKey(day))
                        schedule.Add(day, new List<TimeSpan>());
                    schedule[day].AddRange(times);
                }
            }

            return schedule;
        }

        public static List<TimeSpan> StringsToTimespans(List<string> timesOfDay)
        {
            var list = new List<TimeSpan>();
            foreach (var time in timesOfDay)
            {
                list.Add(TimeSpan.Parse(time));
            }
            return list;
        }

        public static List<DayOfWeek> ToDaysOfWeek(this IEnumerable<string> dayStrings)
        {
            var days = new List<DayOfWeek>();
            foreach (var dayString in dayStrings)
            {
                if (String.IsNullOrEmpty(dayString) || dayString.ToLower().Equals("none"))
                    continue;

                else if (dayString.ToLower().Equals("any") || dayString.ToLower().Equals("all"))
                    days.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>());

                else if(dayString.ToLower().Equals("weekend"))
                    days.AddRange(new[] { DayOfWeek.Saturday, DayOfWeek.Sunday });

                else
                    days.Add(dayString.ToDayOfWeek());                 
            }
            return days;
        }

        public static DayOfWeek ToDayOfWeek(this string dayString)
        {
            var success = Enum.TryParse<DayOfWeek>(dayString, out DayOfWeek day);
            if (!success)
                throw new ArgumentException("Days in job schedule invalid");
            return day;
        }        

        public static IEnumerable<string> ExtractProviderShortNames(this string expression)
        {
            var providers = new List<string>();
            var parts = expression.Split('@');
            for (int i = 1; i < parts.Length; i++)
            {
                var pos = parts[i].IndexOf<char>(c => !char.IsLetterOrDigit(c));
                if (pos != -1)
                    providers.Add(parts[i].Substring(0, pos));
                else
                    providers.Add(parts[i]);
            }
            return providers.Distinct();
        }

        public static IEnumerable<string> ExtractProviderFullNames(this Delegate del)
        {
            var providerFullNames = new List<string>();
            var parameters = del.Method.GetParameters().ToList();
            for (int i = 1; i < parameters.Count(); i++)
            {
                var paramName = parameters[i].ParameterType.Name;
                providerFullNames.Add(paramName);
            }

            return providerFullNames;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            int i = 0;

            foreach (TSource element in source)
            {
                if (predicate(element))
                    return i;
                i++;
            }

            return -1;
        }
    }
}
