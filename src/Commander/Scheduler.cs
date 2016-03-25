using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Commander
{
    public class Scheduler
    {
        Action<ICommand> executer;
        List<TaskPlan> plans = new  List<TaskPlan>();
        ILog log = null;
        Timer Timer;
        public bool IsKilled { get; private set; }
        object locker = new object();
        public Scheduler(Action<ICommand> executer, ILog log = null,  int resolutionInMsec = 1000)
        {
            this.log = log ?? new ConsoleLog();
            this.IsKilled = false;
            this.executer = executer;
            this.Timer = new Timer((double) resolutionInMsec);
            this.Timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
            this.Timer.Start();
        }

        public virtual void AddTask(ICommandAbstractFactory commandFactory, CommandRunProperties properties) {
            this.ThrowIfKilled();

            var firstTime = DateTime.Now;
            if (properties.At.HasValue){
                firstTime = DateTime.Now.Date.Add(properties.At.Value);
                if(firstTime<DateTime.Now)
                    firstTime = firstTime.AddDays(1);
            }
            var plan = new TaskPlan {
                CommandFactory    = commandFactory,
                maxExecutionCount = properties.Count,
                intervalInMsec    = properties.Every.HasValue?(int)properties.Every.Value.TotalMilliseconds:int.MaxValue,
                plannedTime       = firstTime
            };

            lock (locker) {
                plans.Add(plan);
            }
        }

        public void Kill() {
            this.ThrowIfKilled();
            this.Timer.Stop();
            IsKilled = true;
        }

        private void ThrowIfKilled() {
            if (IsKilled)
                throw new InvalidOperationException("Вы не можете выполнить операцию, так как планировщик уже убит(IsKilled = true)");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            TaskPlan[] plansArray = null;
            
            lock(locker)
                plansArray = this.plans.Where(p => p.plannedTime <= DateTime.Now).ToArray();
            
            foreach (var plan in plansArray) {

                if (IsKilled)
                    break;

                var exemplar = plan.CommandFactory.GetExemplar();

                log.WriteMessage("Regular task \""
                    + Tools.NormilizeCommandTypeName(exemplar.GetType().Name)
                    + "\". Executed: "+ (plan.executedCount+1) 
                    + (plan.maxExecutionCount.HasValue?(" of "+ plan.maxExecutionCount.Value):"")
                    + ". Interval: "+ (plan.intervalInMsec)
                    + " msec");
                
                plan.plannedTime += TimeSpan.FromMilliseconds((double) plan.intervalInMsec);
               
                this.executer(exemplar);
                plan.executedCount++;

                if (plan.maxExecutionCount.HasValue && plan.executedCount >= plan.maxExecutionCount) {
                    lock(locker)
                        plans.Remove(plan);
                    log.WriteMessage("Regular task \""+ Tools.NormilizeCommandTypeName(exemplar.GetType().Name+"\" were finished." ));
                }
            }
        }
    }
}
