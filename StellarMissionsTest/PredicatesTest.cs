using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StellarMissions;

namespace StellarMissionsTest
{
    [TestClass]
    public class TestPredicates
    {
        [TestMethod]
        public void CompositeTest()
        {
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

                bool v3 = rand.Next(-2, 2) > 0 ? true : false;
                LogBook.Set<bool>("p3", rand.Next(-2, 2) > 0 ? true : false);

                int v4 = rand.Next(-2, 2);
                LogBook.Set<int>("p4", rand.Next(-2, 2));

                GreaterThan<double> p1 = new GreaterThan<double>("p1", v1);
                Not np1 = new Not(p1);
                GreaterThan<float> p2 = new GreaterThan<float>("p2", v2);
                Equal<bool> p3 = new Equal<bool>("p3", v3);
                Equal<int> p4 = new Equal<int>("p4", v4);
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
