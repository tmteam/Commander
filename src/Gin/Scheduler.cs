using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TheGin
{
    public class Scheduler
    {
        public bool IsKilled { get; private set; }

        readonly List<TaskPlan> _plans;
        readonly ILog _log ;
        readonly System.Timers.Timer _timer;
        readonly object _locker;
        readonly ManualResetEvent _tasksDone;
        readonly Executor _executor;
        public Scheduler(Executor executor, ILog log = null,  int resolutionInMsec = 1000)
        {
            this.IsKilled = false;

            this._log = log ?? new ConsoleLog();
            this._executor = executor;
            this._tasksDone = new ManualResetEvent(true);
            this._locker = new object();
            this._plans = new  List<TaskPlan>();

            this._timer = new System.Timers.Timer((double)resolutionInMsec);
            this._timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
            this._timer.Start();
        }
        public void WaitForFinish()
        {
            this._tasksDone.WaitOne();
        }
        public bool WaitForFinish(int msec = 0)
        {
           return _tasksDone.WaitOne(msec);
        }
        public void Add(Instruction instruction)
        {
            this.ThrowIfKilled();
            var firstTime = DateTime.Now;
            if (instruction.ScheduleProperties.At.HasValue)
            {
                firstTime = instruction.ScheduleProperties.At.Value;
                if (firstTime < DateTime.Now)
                    firstTime = firstTime.AddDays(1);
            }
            var plan = new TaskPlan
            {
                Instruction = instruction,
                PlannedTime = firstTime
            };

            lock (_locker)
            {
                _tasksDone.Reset();
                _plans.Add(plan);
            }
        }

        public void Kill() {
            this.ThrowIfKilled();
            this._timer.Stop();
            IsKilled = true;
        }

        void ThrowIfKilled() {
            if (IsKilled)
                throw new InvalidOperationException("Вы не можете выполнить операцию, так как планировщик уже убит(IsKilled = true)");
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

                var exemplar = plan.Instruction.ConfiguredCommand;

                _log.WriteMessage("Regular task \""
                    + ParseTools.NormalizeCommandTypeName(exemplar.GetType().Name)
                    + "\". Executed: "+ (plan.ExecutedCount+1)
                    + (plan.Instruction.ScheduleProperties.Count.HasValue ? (" of " + plan.Instruction.ScheduleProperties.Count.Value) : "") + ". "
                    + (plan.Instruction.ScheduleProperties.Every.HasValue ? ("Interval: " + (plan.Instruction.ScheduleProperties.Every.Value)) : ""));

                if (plan.Instruction.ScheduleProperties.Every.HasValue)
                    plan.PlannedTime += plan.Instruction.ScheduleProperties.Every.Value;
               
                this._executor.Run(exemplar);
                plan.ExecutedCount++;

                if (plan.Instruction.ScheduleProperties.Count.HasValue 
                    && plan.ExecutedCount >= plan.Instruction.ScheduleProperties.Count)
                {
                    bool hasOtherTasks = false;
                    lock (_locker)
                    {
                        _plans.Remove(plan);
                        hasOtherTasks = _plans.Any();
                    }
                    _log.WriteMessage("Regular task \"" + ParseTools.NormalizeCommandTypeName(exemplar.GetType().Name + "\" were finished."));
                    if (!hasOtherTasks)
                        _tasksDone.Set();
                }
            }
            
        }
    }
}
