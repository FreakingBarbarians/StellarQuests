using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StellarMissions;

namespace StellarMissionsTest
{
    [TestClass]
    public class VariablesTest {

        [TestMethod]
        public void TestNames() {

            Predicate hello_p = new GreaterThan<double>("a", "b");
            Predicate goodbye_p = new GreaterThan<double>(0, "a");
            Predicate hello_goodbye_p = new And(hello_p, goodbye_p);

            Assert.AreEqual(2, hello_p.GetVariableNames().Count());
            Assert.AreEqual(1, goodbye_p.GetVariableNames().Count());
            Assert.AreEqual(2, hello_goodbye_p.GetVariableNames().Count());

            Condition Hello = new Condition(hello_p, "Hello");
            Condition Goodbye = new Condition(goodbye_p, "Goodbye");
            Condition Hello_Goodbye = new Condition(hello_goodbye_p, "Hello_Goodbye");

            Assert.AreEqual(2, Hello.GetVariableNames().Count());
            Assert.AreEqual(1, Goodbye.GetVariableNames().Count());
            Assert.AreEqual(2, Hello_Goodbye.GetVariableNames().Count());

            Milestone Hello_Milestone = new Milestone("Hello");
            Milestone Goodbye_Milestone = new Milestone("Goodbye");
            Milestone Hello_Goodbye_Milestone = new Milestone("Hello_Goodbye");
            Hello_Milestone.RegisterCondition(Hello);
            Goodbye_Milestone.RegisterCondition(Goodbye);
            Hello_Goodbye_Milestone.RegisterCondition(Hello_Goodbye);

            Mission HelloWorld = new Mission("HelloWorld");
            HelloWorld.RegisterMilestone(Hello_Milestone);
            HelloWorld.RegisterMilestone(Goodbye_Milestone);
            HelloWorld.RegisterMilestone(Hello_Goodbye_Milestone);

            int a_count = 0;
            int b_count = 0;
            foreach (string variable in HelloWorld.GetVariableNames()) {
                if (variable.Equals("a")) {
                    a_count++;
                }
                if (variable.Equals("b")) {
                    b_count++;
                }
            }

            Assert.AreEqual(1, a_count);
            Assert.AreEqual(1, b_count);
        }

    }
}
