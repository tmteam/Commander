using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TheGin
{
    /// <summary>
    /// Launches the commands according to specified instructions
    /// </summary>
    public class Scheduler: ILoggable
    {
        readonly List<TaskPlan> _plans;
        readonly System.Timers.Timer _timer;
        readonly object _locker;
        readonly ManualResetEvent _tasksDone;
        readonly IExecutor _executor;
        
        public Scheduler(IExecutor executor, ILog log = null,  int resolutionInMsec = 1000)
        {
            if(executor==null)
                throw  new ArgumentNullException("executor");
            this.IsKilled = false;

            this.Log = log ?? new ConsoleLog();
            this._executor = executor;
            this._tasksDone = new ManualResetEvent(true);
            this._locker = new object();
            this._plans = new  List<TaskPlan>();

            this._timer = new System.Timers.Timer((double)resolutionInMsec);
            this._timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
            this._timer.Start();
        }
        public ILog Log { get; set; }
        public bool IsKilled { get; private set; }
        /// <summary>
        /// Waits for the moment when all tasks will be finished
        /// </summary>
        public void WaitForFinish()
        {
            this._tasksDone.WaitOne();
        }
        /// <summary>
        /// Waits for the moment when all tasks will be finished, but no more that specified time
        /// </summary>
        /// <param name="maxAwaitTimeInMsec">Max time of awaiting</param>
        /// <returns>True, if tasks were finished. False otherwise.</returns>
        public bool WaitForFinish(int msec = 0)
        {
           return _tasksDone.WaitOne(msec);
        }
        /// <summary>
        /// Adds new task to the schedule
        /// </summary>
        public void AddTask(Instruction instruction)
        {
            this.ThrowIfKilled();
            var firstTime = DateTime.Now;
            if (instruction.ScheduleSettings.At.HasValue)
            {
                firstTime = instruction.ScheduleSettings.At.Value;
                if (firstTime < DateTime.Now)
                    firstTime = firstTime.AddDays(1);
            }
            var plan = new TaskPlan {
                Instruction = instruction,
                PlannedTime = firstTime
            };

            lock (_locker) {
                _tasksDone.Reset();
                _plans.Add(plan);
            }
        }
        /// <summary>
        /// Stops the scheduler. Cancel all planned tasks
        /// </summary>
        public void Kill() {
            this.ThrowIfKilled();
            this._timer.Stop();
            IsKilled = true;
        }

        void ThrowIfKilled() {
            if (IsKilled)
                throw new InvalidOperationException("Operation cannot be performed because scheduler is already killed (IsKilled = true)");
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            TaskPlan[] plansArray = null;

            lock (_locker) {
                plansArray = this._plans
                    .Where(p => p.PlannedTime <= DateTime.Now)
                    .ToArray();
            }
            foreach (var plan in plansArray) {

                if (IsKilled)
                    break;

                var exemplar = plan.Instruction.Locator.GetReadyToGoInstance();

                Log.WriteMessage("Regular task \""
                    + ParseTools.GetCommandName(exemplar.GetType())
                    + "\". Executed: "+ (plan.ExecutedCount+1)
                    + (plan.Instruction.ScheduleSettings.Count.HasValue ? (" of " + plan.Instruction.ScheduleSettings.Count.Value) : "") + ". "
                    + (plan.Instruction.ScheduleSettings.Every.HasValue ? ("Interval: " + (plan.Instruction.ScheduleSettings.Every.Value)) : ""));

                if (plan.Instruction.ScheduleSettings.Every.HasValue)
                    plan.PlannedTime += plan.Instruction.ScheduleSettings.Every.Value;
               
                this._executor.Run(exemplar);
                plan.ExecutedCount++;

                if (plan.Instruction.ScheduleSettings.Count.HasValue
                    && plan.ExecutedCount >= plan.Instruction.ScheduleSettings.Count)
                {
                    bool hasOtherTasks = false;
                    lock (_locker)
                    {
                        _plans.Remove(plan);
                        hasOtherTasks = _plans.Any();
                    }
                    Log.WriteMessage("Regular task \"" + ParseTools.GetCommandName(exemplar.GetType()) + "\" were finished.");
                    if (!hasOtherTasks)
                        _tasksDone.Set();
                }
            }
            
        }
        
        class TaskPlan
        {
            public Instruction Instruction;
            public ulong ExecutedCount = 0;
            public DateTime? PlannedTime;
        }

    }
}
