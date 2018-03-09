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
            LogBook.UnregisterAll();
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

        [TestMethod]
        public void CompositeTest() {
            Random rand = new Random();

            LogBook.RegisterLog(typeof(int));
            LogBook.RegisterLog(typeof(double));
            LogBook.RegisterLog(typeof(float));
            LogBook.RegisterLog(typeof(bool));

            for (int i = 0; i < 100; i++)
            {

                double v1 = (rand.NextDouble() - 0.5) * 100;
                LogBook.Set<double>("p1", (rand.NextDouble() - 0.5) * 100);

                float v2 = (float)(rand.NextDouble() - 0.5) * 100;
                LogBook.Set<float>("p2", (float)(rand.NextDouble() - 0.5) * 100);

                bool v3 = rand.Next(-2, 2) > 0 ? true:false;
                LogBook.Set<bool>("p3", rand.Next(-2, 2) > 0 ? true : false);

                int v4 = rand.Next(-2, 2);
                LogBook.Set<int>("p4", rand.Next(-2, 2));

                GreaterThanDouble p1 = new GreaterThanDouble("p1", v1);
                Not np1 = new Not(p1);
                GreaterThanFloat p2 = new GreaterThanFloat("p2", v2);
                EqualBool p3 = new EqualBool("p3", v3);
                EqualInt p4 = new EqualInt("p4", v4);
                Or p5 = new Or(np1, p2);
                Or p6 = new Or(p3, p4);
                And p7 = new And(p5, p6);
                Assert.AreEqual(p7.Evaluate(),
                    (!(LogBook.Get<double>("p1") > v1) || LogBook.Get<float>("p2") > v2) &&
                    (LogBook.Get<bool>("p3") == v3 || LogBook.Get<int>("p4") == v4));
            }
        }
    }
}
