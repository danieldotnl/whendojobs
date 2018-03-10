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
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static MethodInfo GetMethod(this Type type, string name, bool generic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return type.GetMethods()
                .FirstOrDefault(method => method.Name == name & method.IsGenericMethod == generic);
        }

        public static IWhenDoCommand ToCommand(this CommandDefinition definition)
        {
            var command = new WhenDoCommand()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                Type = definition.Type,
                MethodName = definition.Command,
                Parameters = definition.Parameters,
                ExecutionStrategy = definition.Execution.ToExecutionStrategy()
            };
            return command;
        }

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

        public static ExecutionStrategy ToExecutionStrategy(this ExecutionStrategyDefinition definition)
        {
            return new ExecutionStrategy()
            {
                Mode = definition.Mode,
                Time = definition.Time
            };
        }
    }
}
