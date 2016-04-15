using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheGin
{
    public class Gin: ILoggable
    {
        ILog _log;
        readonly ManualResetEvent _localTaskIsDone;
        readonly IExecutor _executor;
        public readonly Interpreter Interpreter;
        public readonly Scheduler Scheduler;
        /*
        public Gin(bool useHelpCommand= true,
                   bool useExitCommand = true,
                   ScanBehaviour scanBehaviour = ScanBehaviour.ScanAllSolutionAssembly)
        {
            throw new NotImplementedException();
        }*/

        public Gin(ICommandLibrary library, ILog log = null, IExecutor executor = null) {
            this._localTaskIsDone =  new ManualResetEvent(true);
            this.Library   = library;
            this.Log       = log?? new ConsoleLog();
            this._executor     = executor?? new Executor(this.Log);
            this._executor.Log = this.Log;
            this.Scheduler     = new Scheduler(this._executor, this.Log);
            this.Interpreter   = new Interpreter(library);
        }
        
        public ILog Log {
            get { return _log; }
            set { 
                if (value == null)
                    throw new ArgumentNullException();
                _log = value;
                Scheduler.Log = value;
                _executor.Log = value;
            }
        }
        public ICommandLibrary Library { get; private set; }
        
        public bool NeedToExit { get; set; }

        public void Execute(string inputString){
            Execute(ParseTools.SmartSplit(inputString));
        }

        public void Execute(string[] args) {
            Execute(args.ToList());
        }
        public void Execute(ICommand cmd, CommandScheduleSettings scheduleSettings) {
            Execute( new Instruction{
                      Locator           = new CommandLocator(()=>cmd, new Dictionary<PropertyInfo,object>()),
                      ScheduleSettings  = scheduleSettings
                 });
        }

        public void Execute(ICommand cmd) {
            _executor.Run(cmd);
        }
        
        public void WaitForFinsh() {
            _localTaskIsDone.WaitOne();
            this.Scheduler.WaitForFinish();
        }
        
        public bool WaitForFinsh(int msec = 0) {
            
            var sw = new Stopwatch();
            sw.Start();
            var ans = _localTaskIsDone.WaitOne(msec);
            if (!ans)
                return false;
            sw.Stop();
            msec = Math.Max(msec - (int)sw.ElapsedMilliseconds, 0);
            return this.Scheduler.WaitForFinish(msec);
        }
        void Execute(List<string> args)
        {
            _localTaskIsDone.Reset();
            Log.WriteMessage(">> " + string.Concat(args.Select(a => a + " ")));

            try
            {
                var instruction = Interpreter.Create(args);
                Execute(instruction);
            }
            catch (UnknownCommandNameException ex) {
                Log.WriteError("Command " + ex.CommandName + " not found");
            }
            catch (UnknownArgumentsException ex) {
                if (ex.ArgumentNames.Length > 1)
                    Log.WriteError("Unknown arguments: \" " + string.Concat(ex.ArgumentNames.Select(a => a + " ")) + "\"");
                else
                    Log.WriteError("Unknown argument: \"" + ex.ArgumentNames.FirstOrDefault() + "\"");
            }
            catch (MissedArgumentsException ex) {
                if (ex.ArgumentNames.Length > 1)
                    Log.WriteError("Neccessary arguments were missed: \" " + string.Concat(ex.ArgumentNames.Select(a => a + " ")) + "\"");
                else
                    Log.WriteError("Neccessary argument \"" + ex.ArgumentNames.FirstOrDefault() + "\" has missed");
            }
            catch (InvalidArgumentException ex) {
                Log.WriteError(ex.Message);
            }
            catch (ArgumentException ex) {
                Log.WriteError("IncorrectCommand ");
            }
            catch (Exception ex) {
                Log.WriteError("Exception: \r\n" + ex.ToString());
            }
            _localTaskIsDone.Set();
        }
        void Execute(Instruction instruction)
        {
            if (!instruction.ScheduleSettings.At.HasValue
                    && instruction.ScheduleSettings.Count.HasValue
                    && !instruction.ScheduleSettings.Every.HasValue)
            {
                _executor.Run(
                    new RunInCycleWrapper(
                        locator: instruction.Locator, 
                        iterationsCount: instruction.ScheduleSettings.Count.Value, 
                        executor: _executor));
            }
            else if (!instruction.ScheduleSettings.IsEmpty)
            {
                Scheduler.AddTask(instruction);
            }
            else
                _executor.Run(instruction.Locator.GetReadyToGoInstance());
        }
    }
}
