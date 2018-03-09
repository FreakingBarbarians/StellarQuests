using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StellarMissions;

namespace StellarMissionsTest
{
    [TestClass]
    public class TestPredicates
    {

        [TestInitialize]
        public void Initialize() {
            LogBook.ClearAll();
        }

        [TestMethod]
        public void TestInt()
        {
            Random rand = new Random();
            LogBook.RegisterLog(typeof(int));
            for (int i = 0; i < 100; i++) {
                LogBook.Set(i.ToString(), i);
                int val2 = LogBook.Get<int>(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.Equals<int>(i.ToString(), val2), LogBook.Get<int>(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThan<int>(i.ToString(), val2), LogBook.Get<int>(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestDouble()
        {
            Random rand = new Random();
            LogBook.RegisterLog(typeof(double));
            for (int i = 0; i < 100; i++)
            {
                LogBook.Set<double>(i.ToString(), i);
                double val2 = LogBook.Get<double>(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.Equals<double>(i.ToString(), val2), LogBook.Get<double>(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThan<double>(i.ToString(), val2), LogBook.Get<double>(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestFloat()
        {
            Random rand = new Random();
            LogBook.RegisterLog(typeof(float));
            for (int i = 0; i < 100; i++)
            {
                LogBook.Set<float>(i.ToString(), i);
                float val2 = LogBook.Get<float>(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.Equals<float>(i.ToString(), val2), LogBook.Get<float>(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThan<float>(i.ToString(), val2), LogBook.Get<float>(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestBool() {
            Random rand = new Random();
            LogBook.RegisterLog(typeof(bool));
            for (int i = 0; i < 100; i++)
            {
                float v1 = rand.Next(-10, 10);
                float v2 = rand.Next(-10, 10);
                bool val1 = v1 < 0 ? true : false;
                bool val2 = v2 < 0 ? true : false;
                LogBook.Set<bool>(i.ToString(), val1);
                Assert.AreEqual(Predicate.Equals<bool>(i.ToString(), val2), LogBook.Get<bool>(i.ToString()) == val2);
            }
        }
    }


}
