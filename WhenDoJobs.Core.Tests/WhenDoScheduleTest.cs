using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class WhenDoScheduleTest
    {
        [TestMethod]
        public void ShouldOrShouldNotRun()
        {
            var job = new WhenDoJob();
            job.NextRun = DateTimeOffset.Parse("2018-03-08 16:00:00");

            Assert.IsTrue(job.ShouldRun(DateTimeOffset.Parse("2018-03-08 16:04:00")));
            Assert.IsFalse(job.ShouldRun(DateTimeOffset.Parse("2018-03-08 15:59:00")));
            Assert.IsTrue(job.ShouldRun(DateTimeOffset.Parse("2018-03-08 17:00:00")));
        }

        [TestMethod]
        public void SetNextRun()
        {
            var schedule = new Dictionary<DayOfWeek, List<TimeSpan>>();
            schedule.Add(DayOfWeek.Monday, new List<TimeSpan>() { TimeSpan.Parse("12:00"), TimeSpan.Parse("15:00"), TimeSpan.Parse("18:00") });
            schedule.Add(DayOfWeek.Wednesday, new List<TimeSpan>() { TimeSpan.Parse("12:00"), TimeSpan.Parse("15:00"), TimeSpan.Parse("18:00") });
            schedule.Add(DayOfWeek.Thursday, new List<TimeSpan>() { TimeSpan.Parse("12:00"), TimeSpan.Parse("15:00"), TimeSpan.Parse("18:00") });

            var job = new WhenDoJob();
            job.Schedule = schedule;

            job.SetNextRun(DateTimeOffset.Parse("2018-03-08 13:45"));
            Assert.AreEqual(DateTimeOffset.Parse("2018-03-08 15:00:00"), job.NextRun);

            job.SetNextRun(DateTimeOffset.Parse("2018-03-08 19:45"));
            Assert.AreEqual(DateTimeOffset.Parse("2018-03-12 12:00:00"), job.NextRun);

            job.SetNextRun(DateTimeOffset.Parse("2018-03-09 13:45"));
            Assert.AreEqual(DateTimeOffset.Parse("2018-03-12 12:00:00"), job.NextRun);

            job.SetNextRun(DateTimeOffset.Parse("2018-03-15 13:45"));
            Assert.AreEqual(DateTimeOffset.Parse("2018-03-15 15:00:00"), job.NextRun);
        }

        [TestMethod]
        public void SetNextRunShouldBeTomorrow()
        {
            var def = new List<ScheduleDefinition>() { new ScheduleDefinition() { Days = new List<string>() { "any" }, TimesOfDay = new List<string>() { "16:00" } } };
            var schedule = def.ToWhenDoSchedule();
            var job = new WhenDoJob() { Schedule = schedule };
            job.SetNextRun(DateTimeOffset.Parse("2018-03-15 19:46"));
            Assert.AreEqual(DateTimeOffset.Parse("2018-03-16 16:00"), job.NextRun);
        }

        [TestMethod]
        public void SetNextRunWhenNoScheduledDays()
        {
            var schedule = new Dictionary<DayOfWeek, List<TimeSpan>>();
            var job = new WhenDoJob();
            job.Schedule = schedule;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => job.SetNextRun(DateTimeOffset.Parse("2018-03-08 13:45")));
        }
    }
}

