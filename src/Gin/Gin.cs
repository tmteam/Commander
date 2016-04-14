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
    public class Gin
    {
        public readonly Interpreter Interpreter;
        public readonly Scheduler Scheduler;
        public ILog Log { get; set; }
        public ICommandLibrary Library { get; set; }
        
        public bool ExitFlag { get; set; }

        ManualResetEvent LocalTaskIsDone = new ManualResetEvent(true);
        Executor Executor;
        public Gin(ICommandLibrary library, ILog log = null) {
            this.Library   = library;
            this.Log       = log?? new ConsoleLog();
            this.Executor  = new Executor(this.Log);
            this.Scheduler     = new Scheduler(this.Executor, this.Log);
            this.Interpreter   = new Interpreter(library);
        }
        
        public void Execute(string inputString){
            Execute(ParseTools.SmartSplit(inputString));
        }

        public void Execute(string[] args) {
            Execute(args.ToList());
        }
        void Execute(List<string> args)
        {
            LocalTaskIsDone.Reset();
            Log.WriteMessage(">> " + string.Concat(args.Select(a => a + " ")));
            
            try {
                var instruction = Interpreter.Create(args);


           
            } catch (UnknownCommandNameException ex) {
                Log.WriteError("Command " + ex.CommandName + " not found");
            }
            catch (UnknownArgumentsException ex) {
                if(ex.ArgumentNames.Length>1)
                    Log.WriteError("Unknown arguments: \" " + string.Concat(ex.ArgumentNames.Select(a => a + " "))+"\"");
                else
                    Log.WriteError("Unknown argument: \"" + ex.ArgumentNames.FirstOrDefault() + "\"");
            }
            catch (MissedArgumentsException ex) {
                if (ex.ArgumentNames.Length> 1) 
                    Log.WriteError("Neccessary arguments were missed: \" " + string.Concat(ex.ArgumentNames.Select(a=>a + " "))+"\"");
                else
                    Log.WriteError("Neccessary argument \"" + ex.ArgumentNames.FirstOrDefault() + "\" has missed");
            }
            catch (InvalidArgumentException ex) {
                Log.WriteError(ex.Message);
            }
            catch (ArgumentException ex){
                Log.WriteError("IncorrectCommand ");
            }
            catch (Exception ex) {
                Log.WriteError("Exception: \r\n" + ex.ToString());
            }
            LocalTaskIsDone.Set();
        }
        void Execute(Instruction instruction)
        {
            if (!instruction.ScheduleProperties.At.HasValue
                    && instruction.ScheduleProperties.Count.HasValue
                    && !instruction.ScheduleProperties.Every.HasValue)
                Executor.Run(new RunInCycleWrapper(instruction.ConfiguredCommand, instruction.ScheduleProperties.Count.Value, Executor));
            else if (!instruction.ScheduleProperties.IsEmpty) {
                Scheduler.Add(instruction);
            }
            else
                Executor.Run(instruction.ConfiguredCommand);
        }
        public void Execute(ICommand cmd, CommandRunProperties scheduleProperties) {
            Execute( new Instruction{
                      ConfiguredCommand     = cmd,
                      ScheduleProperties    = scheduleProperties
                 });
        }

        public void Execute(ICommand cmd) {
            Executor.Run(cmd);
        }
        
        public void WaitForFinsh() {
            LocalTaskIsDone.WaitOne();
            this.Scheduler.WaitForFinish();
        }
        
        public bool WaitForFinsh(int msec = 0) {
            
            var sw = new Stopwatch();
            sw.Start();
            var ans = LocalTaskIsDone.WaitOne(msec);
            if (!ans)
                return false;
            sw.Stop();
            msec = Math.Max(msec - (int)sw.ElapsedMilliseconds, 0);
            return this.Scheduler.WaitForFinish(msec);
        }
        
    }
}
