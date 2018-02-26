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
            for (int i = 0; i < 100; i++) {
                LogBook.SetInt(i.ToString(), i);
                int val2 = LogBook.GetInt(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.EqualInt(i.ToString(), val2), LogBook.GetInt(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThanInt(i.ToString(), val2), LogBook.GetInt(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestDouble()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                LogBook.SetDouble(i.ToString(), i);
                double val2 = LogBook.GetInt(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.EqualDouble(i.ToString(), val2), LogBook.GetDouble(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThanDouble(i.ToString(), val2), LogBook.GetDouble(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestFloat()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                LogBook.SetFloat(i.ToString(), i);
                float val2 = LogBook.GetFloat(i.ToString()) + rand.Next(-2, 2);
                Assert.AreEqual(Predicate.EqualFloat(i.ToString(), val2), LogBook.GetFloat(i.ToString()) == val2);
                Assert.AreEqual(Predicate.GreaterThanFloat(i.ToString(), val2), LogBook.GetFloat(i.ToString()) > val2);
            }
        }

        [TestMethod]
        public void TestBool() {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                float v1 = rand.Next(-10, 10);
                float v2 = rand.Next(-10, 10);
                bool val1 = v1 < 0 ? true : false;
                bool val2 = v2 < 0 ? true : false;
                LogBook.SetBool(i.ToString(), val1);
                Assert.AreEqual(Predicate.EqualBool(i.ToString(), val2), LogBook.GetBool(i.ToString()) == val2);
            }
        }
    }


}
