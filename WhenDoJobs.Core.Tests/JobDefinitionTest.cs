using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Tests.Helpers;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class JobDefinitionTest
    {
        [TestMethod]
        public void ConvertFullDefinitionToJob()
        {
            var jobDefinition = new JobDefinition()
            {
                Context = "TestMessage",
                When = "TestMessage.IntValue == 3",
                Id = "TestJobDefinition",
                Do = new List<CommandDefinition>()
                    {
                        new CommandDefinition()
                            {
                                Command = "DoSomething",
                                Execution = new ExecutionStrategyDefinition()
                                    {
                                        Mode = ExecutionMode.Default
                                    },
                                Parameters = new Dictionary<string, object>() { { "Param1", "blabla"} },
                                Type = "TestCommand"
                            }
                }
            };

            var job = jobDefinition.ToJob<TestMessage>();

            Assert.IsNotNull(job.Condition);
            Assert.AreEqual(1, job.Commands.Count());
            Assert.AreEqual("TestJobDefinition", job.Id);
            Assert.AreEqual("blabla", job.Commands.FirstOrDefault().Parameters["Param1"]);
        }
    }
}
