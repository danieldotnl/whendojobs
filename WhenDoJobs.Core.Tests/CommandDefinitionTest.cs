using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class CommandDefinitionTest
    {
        [TestMethod]
        public void ConvertCommandDefWithoutParameters()
        {
            var commandDef = new CommandDefinition()
            {
                Command = "Test",
                Type = "Bla"
            };

            //var command = commandDef.ToCommand();

            //Assert.AreEqual("Test", command.MethodName);
            //Assert.AreEqual("Bla", command.Type);
        }
    }
}
