using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenDoJobs.Core.Tests
{
    [TestClass]
    public class WhenDoExtensionsTest
    {
        [TestMethod]
        public void ShouldFind2ProvidersInExpression()
        {
            var expression = @"this is @dtp.Now and @bla=34 and @bla>45";
            var result = expression.ExtractProviders();
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void ShouldFindDtpAndBlaProvidersInExpression()
        {
            var expression = @"this is @dtp.Now and @bla=34 and @bla>45";
            var result = expression.ExtractProviders();
            Assert.AreEqual("dtp", result.First());
            Assert.AreEqual("bla", result.Last());
        }

        [TestMethod]
        public void ShouldFindProviderAtStartOfExpression()
        {
            var expression = @"@dtp.Now and @bla=34 and @bla>45";
            var result = expression.ExtractProviders();
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void ShouldFindProviderAtEndOfExpression()
        {
            var expression = @"this is bla bla and 45<@bla";
            var result = expression.ExtractProviders();
            Assert.AreEqual("bla", result.First());
        }

        [TestMethod]
        public void ShouldFindProviderWithWhitespacesAtStartAndEndOfExpression()
        {
            var expression = @" @dtp.Now and @bla=34 and @bla.Temperature > 45 @kk ";
            var result = expression.ExtractProviders();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("dtp", result.First());
            Assert.AreEqual("bla", result.ElementAt(1));
            Assert.AreEqual("kk", result.ElementAt(2));
        }
    }
}
