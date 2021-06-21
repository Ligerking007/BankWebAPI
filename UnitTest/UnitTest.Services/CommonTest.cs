using Core.Common;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTest.Services
{
    [TestClass]
    public class CommonTest
    {


        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void ConvertDate()
        {
            Initialize();

            DateTime date = ("20210621").StringToDate();

            Assert.AreEqual(date.Year, 2021);
            Assert.AreEqual(date.Month, 6);
            Assert.AreEqual(date.Day, 21);
        }

        [TestMethod]
        public void DateString()
        {
            Initialize();
            DateTime date = new DateTime(2021, 06, 21, 16, 29, 23);
            Assert.AreEqual(date.DateToString(), "20210621");
        }

        [TestMethod]
        public void TimeString()
        {
            Initialize();
            DateTime date = new DateTime(2021, 06, 21, 16, 29, 23);
            Assert.AreEqual(date.TimeToString(), "162923");
        }
    }
}