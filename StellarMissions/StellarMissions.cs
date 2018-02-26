using System;
using System.Collections.Generic;

namespace StellarMissions
{

    public class LogBook {
        private static Dictionary<string, int> int_values;
        private static Dictionary<string, float> float_values;
        private static Dictionary<string, double> double_values;
        private static Dictionary<string, bool> bool_values;

        static LogBook() {
            int_values = new Dictionary<string, int>();
            float_values = new Dictionary<string, float>();
            double_values = new Dictionary<string, double>();
            bool_values = new Dictionary<string, bool>();
        }

        public static void ClearAll() {
            int_values.Clear();
            float_values.Clear();
            double_values.Clear();
            bool_values.Clear();
        }

        public static int GetInt(string key) {
            if (int_values.ContainsKey(key))
            {
                return int_values[key];
            }
            else {
                // @TODO: think about how to return error messages.
                return int.MinValue;
            }
        }

        public static float GetFloat(string key) {
            if (float_values.ContainsKey(key))
            {
                return float_values[key];
            }
            else
            {
                // @TODO: think about how to return error messages.
                return float.MinValue;
            }
        }

        public static double GetDouble(string key) {
            if (double_values.ContainsKey(key))
            {
                return double_values[key];
            }
            else
            {
                // @TODO: think about how to return error messages.
                return Double.MinValue;
            }
        }

        public static bool GetBool(string key)
        {
            if (double_values.ContainsKey(key))
            {
                return bool_values[key];
            }
            else
            {
                // @TODO: think about how to return error messages.
                return false;
            }
        }

        public static void SetInt(string key, int val) {
            int_values[key] = val;
        }

        public static void SetFloat(string key, float val) {
            float_values[key] = val;
        }

        public static void SetDouble(string key, double val) {
            double_values[key] = val;
        }

        public static void SetBool(string key, bool val) {
            bool_values[key] = val;
        }
    }

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
        public List<Condition> conditions;
    }

    public class Condition {

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

        public static bool EqualDouble(params object[] args)
        {
            double x = args[0].GetType() == typeof(string) ? LogBook.GetDouble((string)args[0]) : (double)args[0];
            double y = args[1].GetType() == typeof(string) ? LogBook.GetDouble((string)args[1]) : (double)args[1];
            return x == y;
        }

        public static bool GreaterThanInt(params object[] args)
        {
            int x = args[0].GetType() == typeof(string) ? LogBook.GetInt((string)args[0]) : (int)args[0];
            int y = args[1].GetType() == typeof(string) ? LogBook.GetInt((string)args[1]) : (int)args[1];
            return x > y;
        }

        public static bool GreaterThanFloat(params object[] args)
        {
            float x = args[0].GetType() == typeof(string) ? LogBook.GetFloat((string)args[0]) : (float)args[0];
            float y = args[1].GetType() == typeof(string) ? LogBook.GetFloat((string)args[1]) : (float)args[1];
            return x > y;
        }

        public static bool GreaterThanDouble(params object[] args)
        {
            double x = args[0].GetType() == typeof(string) ? LogBook.GetDouble((string)args[0]) : (double)args[0];
            double y = args[1].GetType() == typeof(string) ? LogBook.GetDouble((string)args[1]) : (double)args[1];
            return x > y;
        }

        public static bool EqualBool(params object[] args)
        {
            bool x = args[0].GetType() == typeof(string) ? LogBook.GetBool((string)args[0]) : (bool)args[0];
            bool y = args[1].GetType() == typeof(string) ? LogBook.GetBool((string)args[1]) : (bool)args[1];
            return x == y;
        }

        public static bool EqualInt(params object[] args)
        {
            int x = args[0].GetType() == typeof(string) ? LogBook.GetInt((string)args[0]) : (int)args[0];
            int y = args[1].GetType() == typeof(string) ? LogBook.GetInt((string)args[1]) : (int)args[1];
            return x == y;
        }

        public static bool EqualFloat(params object[] args)
        {
            float x = args[0].GetType() == typeof(string) ? LogBook.GetFloat((string)args[0]) : (float)args[0];
            float y = args[1].GetType() == typeof(string) ? LogBook.GetFloat((string)args[1]) : (float)args[1];
            return x == y;
        }
    }

    public class Not : Predicate {
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
        public override bool Evaluate()
        {
            return EqualDouble(args);
        }
    }

    public class GreaterThanDouble : Predicate {
        public override bool Evaluate()
        {
            return GreaterThanDouble(args);
        }
    }

    public class GreaterThanFloat : Predicate
    {
        public override bool Evaluate()
        {
            return GreaterThanFloat(args);
        }
    }

    public class EqualFloat : Predicate
    {
        public override bool Evaluate()
        {
            return EqualFloat(args);
        }
    }

    public class GreaterThanInt: Predicate
    {
        public override bool Evaluate()
        {
            return GreaterThanInt(args);
        }
    }

    public class EqualInt : Predicate
    {
        public override bool Evaluate()
        {
            return EqualInt(args);
        }
    }

    public class EqualBool : Predicate
    {
        public override bool Evaluate()
        {
            return EqualBool(args);
        }
    }
}