using System;
using System.Collections.Generic;

namespace StellarMissions
{

    public class LogBook {
        private static Dictionary<Type, Dictionary<string, object>> root = new Dictionary<Type, Dictionary<string, object>>();

        static LogBook() {
        }

        public static void UnregisterAll() {
            root.Clear();
        }

        public static void ClearAll() {
            foreach(Dictionary<string, object> dict in root.Values) {
                dict.Clear();
            }
        }

        public static void RegisterLog(Type type) {
            Dictionary<string, object> dc = new Dictionary<string, object>();
            root.Add(type, dc);
        }

        public static void Set<T>(string key, T val) where T : IComparable {
            root[typeof(T)][key] = val;
        }

        public static T Get<T>(string key) where T : IComparable {
            return (T)root[typeof(T)][key];
        } 
    }

    // likely will have a "Frontier" of objectives
    // Perhaps a mapping of variables to predicates->milestone and back would be useful
    // This way we can do lazy evaluation instead of a periodic poll!
    public class Mission
    {
        public enum MissionStatus
        {
            SPAWNED,
            AVAILABLE,
            ACQUIRED,
            FAILED,
            FINISHED
        }

        public MissionStatus mission_status { get; private set; }

    }

    public class Milestone {
        // Consider adding something that generates && enforces the correct conditions.
        
        // logs the report of the last  evaluation attempt
        //                name    success? message
        public List<Tuple<string, bool,    string>> report;
        public List<Condition> conditions;
        public List<Milestone> Paths;
        public List<Milestone> Exclusions;

        public bool Evaluate() {
            bool result = true;
            report.Clear();
            foreach (Condition cond in conditions) {
                if (!cond.Evaluate())
                {
                    report.Add(new Tuple<string, bool, string>(cond.Name, false, cond.FailureMessage));
                    result = false;
                }
                else {
                    report.Add(new Tuple<string, bool, string>(cond.Name, true, cond.FailureMessage));
                } 
            }
            return result;
        }
    }

    public class Condition {
        Predicate predicate;
        public readonly string Name;
        public readonly string SuccessMessage;
        public readonly string FailureMessage;

        public Condition(Predicate predicate, string name, string successMessage = "", string failureMessage = "") {
            this.predicate = predicate;
            this.Name = name;
            this.SuccessMessage = successMessage;
            this.FailureMessage = failureMessage;
        }

        public bool Evaluate() {
            return predicate.Evaluate();
        }
    }

    // lets expose some basic ones and leave the rest for
    // the user to figure out

    public abstract class Predicate {
        // some sort of predicate code;
        protected object[] args;

        public Predicate(params object[] args) {
            this.args = args;
        }

        public abstract bool Evaluate();

        // strings will always be ref-type
        public static bool GreaterThan<T>(params object[] args) where T : IComparable{
            T x = args[0].GetType() == typeof(string) ? LogBook.Get<T>((string)args[0]) : (T)args[0];
            T y = args[1].GetType() == typeof(string) ? LogBook.Get<T>((string)args[1]) : (T)args[1];
            return x.CompareTo(y) > 0;
        }

        public static bool Equals<T>(params object[] args) where T : IComparable{
            T x = args[0].GetType() == typeof(string) ? LogBook.Get<T>((string)args[0]) : (T)args[0];
            T y = args[1].GetType() == typeof(string) ? LogBook.Get<T>((string)args[1]) : (T)args[1];
            return x.Equals(y);
        }
    }

    public class Not : Predicate {
        public Not(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            if (args[0] is Predicate)
            {
                return !((Predicate)args[0]).Evaluate();
            }
            else {
                // @TODO throw some error
                return false;
            }
        }
    }

    public class Or : Predicate{
        public Or(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            foreach (object predicate in args) {
                if (predicate is Predicate)
                {
                    if (((Predicate)predicate).Evaluate())
                    {
                        return true;
                    }
                }
                else {
                    // @TODO throw some exception
                }
            }
            return false;
        }
    }

    public class And : Predicate{
        public And(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            foreach (object predicate in args)
            {
                if (predicate is Predicate)
                {
                    if (!((Predicate)predicate).Evaluate())
                    {
                        return false;
                    }
                }
                else
                {
                    // @TODO throw some exception
                }
            }
            return true;
        }
    }

    public class EqualDouble : Predicate {
        public EqualDouble(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return Equals<double>(args);
        }
    }

    public class GreaterThanDouble : Predicate {
        public GreaterThanDouble(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return GreaterThan<double>(args);
        }
    }

    public class GreaterThanFloat : Predicate
    {
        public GreaterThanFloat(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return GreaterThan<float>(args);
        }
    }

    public class EqualFloat : Predicate
    {
        public EqualFloat(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return Equals<float>(args);
        }
    }

    public class GreaterThanInt: Predicate
    {
        public GreaterThanInt(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return GreaterThan<int>(args);
        }
    }

    public class EqualInt : Predicate
    {
        public EqualInt(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return Equals<int>(args);
        }
    }

    public class EqualBool : Predicate
    {
        public EqualBool(params object[] args) : base(args) { }

        public override bool Evaluate()
        {
            return Equals<bool>(args);
        }
    }
}