using System;
using System.Linq;
using System.Collections.Generic;

namespace StellarMissions
{

    public class LogBook {
        private static Dictionary<Type, Dictionary<string, object>> root = new Dictionary<Type, Dictionary<string, object>>();
        private static Dictionary<string, List<Mission>> reverse_root = new Dictionary<string, List<Mission>>();
        static LogBook() {
        }

        public static void UnregisterAll() {
            root.Clear();
            reverse_root.Clear();
        }

        public static void ClearAll() {
            foreach (Dictionary<string, object> dict in root.Values) {
                dict.Clear();
            }
            reverse_root.Clear();
        }

        public static void RegisterLog(Type type) {
            Dictionary<string, object> dc = new Dictionary<string, object>();
            root.Add(type, dc);
        }

        public static void Set<T>(string key, T val) where T : IComparable {
            root[typeof(T)][key] = val;

            if (reverse_root.ContainsKey(key)) {
                foreach (Mission mission in reverse_root[key]) {

                }
            }
        }

        public static T Get<T>(string key) where T : IComparable {
            return (T)root[typeof(T)][key];
        }

        public static void RegisterMission(Mission mission) {
            foreach (String variable in mission.GetVariableNames()) {
                if (reverse_root.ContainsKey(variable))
                {
                    reverse_root[variable].Add(mission);
                }
                else {
                    List<Mission> missionList = new List<Mission>();
                    missionList.Add(mission);
                    reverse_root[variable] = missionList;
                }
            }
        }
    }

    public class StellarQuestMessage {
        public enum QuestMessage{
            FAIL,
            ACTIVATED,
            DISCOVERED
        };

        public readonly QuestMessage Message;

        public Milestone MilestoneData;
        public Mission MissionData;

        public StellarQuestMessage(QuestMessage message, Milestone milestoneData = null, Mission missionData = null) {
            Message = message;
            MilestoneData = milestoneData;
            MissionData = missionData;
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

        public readonly string Name;
        public List<Milestone> Frontier;
        public MissionStatus Status { get; private set; }
        private Dictionary<string, List<Milestone>> VariableToMilestone = new Dictionary<string, List<Milestone>>();
        public List<IMissionListener> listeners = new List<IMissionListener>();

        public Mission(string Name)
        {
            this.Name = Name;
        }

        public void OnVariableChanged(string key) {
            if (VariableToMilestone.ContainsKey(key)) {
                foreach (Milestone milestone in VariableToMilestone[key]) {
                    milestone.Evaluate(); // evaluate milestones
                                          // consider how to send events!
                                          // back to game
                    /**
                     * something like. ..
                     * if(milestone.Evaluate()) {
                     *  frontier.add(milestone.getPaths());
                     *  Communicator.GenerateMessage(mission X's milestone Y passed);
                     * }
                     */

                     foreach(IMissionListener listener in listeners) {
                        listener.MissionUpdate(this);
                     }

                }
            }
        }

        public void RegisterMilestone(Milestone milestone) {
            foreach (String variable in milestone.GetVariableNames()) {
                if (VariableToMilestone.ContainsKey(variable)) {
                    if (!VariableToMilestone[variable].Contains(milestone))
                    {
                        VariableToMilestone[variable].Add(milestone);
                    }
                } else {
                    List<Milestone> milestoneList = new List<Milestone>();
                    milestoneList.Add(milestone);
                    VariableToMilestone[variable] = milestoneList;
                }
            }
        }

        public void RegisterListener(IMissionListener listener) {
            if(!listeners.Contains(listener)) {
                listeners.Add(listener);    
            }
        }

        public void UnregisterListener(IMissionListener listener) {
            listeners.Remove(listener);
        }

        public IEnumerable<String> GetVariableNames() {
            return (from item in VariableToMilestone.Keys select item).Distinct();
        }


    }

    // we use subscriber-obs pattern
    public interface IMissionListener {
        void MissionUpdate(Mission);
    }

    public class Milestone {
        // Consider adding something that generates && enforces the correct conditions.

        // logs the report of the last  evaluation attempt
        //                name    success? message
        public string Name;
        //1: Name of condition, 2: Satisfaction of cond., 3. return message
        public List<Tuple<string, bool, string>> report;
        public List<Condition> Conditions;
        public Condition VisibilityCondition;
        public List<Milestone> Paths;

        // When does a Milestone fail? Can they fail? hmm...
        // I guess we just have terminal nodes...

        public Milestone(string name, Condition visibilityCondition = null) {
            VisibilityCondition = visibilityCondition == null ? new TrueCondition("default") : visibilityCondition;
            this.Name = name;
            report = null;
            Conditions = new List<Condition>();
            Paths = new List<Milestone>();
        }

        public bool Evaluate() {
            bool result = true;
            report.Clear();
            foreach (Condition cond in Conditions) {
                if (!cond.Evaluate())
                {
                    report.Add(new Tuple<string, bool, string>(cond.Name, false, cond.FailureMessage));
                    result = false;
                }
                else {
                    report.Add(new Tuple<string, bool, string>(cond.Name, true, cond.SuccessMessage));
                } 
            }
            return result;
        }

        public List<Milestone> GetPaths() {
            return Paths;
        }

        public void AddPath(Milestone milestone) {
            if (Paths.Contains(milestone)) {
                return;
            }
            Paths.Add(milestone);
        }

        public void RegisterCondition(Condition condition) {
            Conditions.Add(condition); // extra stuff?
        }

        public void UnregisterCondition(Condition condition) {
            Conditions.Remove(condition);
        }

        // Select all variables from all conditions
        public IEnumerable<String> GetVariableNames() {
            List<String> retval = new List<String>();

            foreach (Condition condition in Conditions) {
                retval.AddRange((from item in condition.GetVariableNames() select item));
            }

            return retval.Distinct();
        }

    }

    public class Condition {
        Predicate predicate;
        public readonly string Name;
        public readonly string SuccessMessage;
        public readonly string FailureMessage;

        protected Condition() { }

        protected Condition(string name, string successMessage = "", string failureMessage = "")
        {
            this.Name = name;
            this.SuccessMessage = successMessage;
            this.FailureMessage = failureMessage;
        }

        public Condition(Predicate predicate, string name, string successMessage = "", string failureMessage = "") {
            this.predicate = predicate;
            this.Name = name;
            this.SuccessMessage = successMessage;
            this.FailureMessage = failureMessage;
        }

        public virtual bool Evaluate() {
            return predicate.Evaluate();
        }

        public IEnumerable<String> GetVariableNames() {
            return predicate.GetVariableNames();
        }
    }

    // Condition that's always true
    public class TrueCondition : Condition {
        public TrueCondition(string name, string successMessage = "", string failureMessage = "") :base(name, successMessage, failureMessage) { }
        override public bool Evaluate() {
            return true;
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

        public IEnumerable<String> GetVariableNames() {
            List<String> retval = new List<String>();
            foreach(object obj in args) {
                if (obj is String)
                {
                    retval.Add((string)obj);
                }
                else if (obj is Predicate) {
                    retval.AddRange(((Predicate)obj).GetVariableNames());
                }
            }
            return retval.Distinct();
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

    public class Equal<T> : Predicate where T : IComparable {
        public Equal(params object[] args) : base(args) {}
        public override bool Evaluate()
        {
            return Predicate.Equals<T>(args);
        }
    }

    public class GreaterThan<T> : Predicate where T : IComparable {
        public GreaterThan(params object[] args) : base(args) { }
        public override bool Evaluate()
        {
            return Predicate.GreaterThan<T>(args);
        }
    }
}