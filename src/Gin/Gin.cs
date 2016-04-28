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
    /// <summary>
    /// Gin that can execute your commands up to your wishes...
    /// </summary>
    public class Gin: ILoggable
    {
        ILog _log;
        readonly ManualResetEvent _localTaskIsDone;
        readonly IExecutor _executor;
        public readonly Interpreter Interpreter;
        public readonly Scheduler Scheduler;
        /// <summary>
        /// Creates a Gin's instance with the default settings
        /// </summary>
        /// <param name="addHelpCommand">Adds a help command to the gin's command list</param>
        /// <param name="addExitCommand">Adds an exit command to the gin's command list</param>
        /// <param name="searchBehaviour">Command type searching instructions</param>
        /// <param name="logFileName">Name of a text file in the application folder where the log will be written. 
        /// If file is not exist - it will be created. 
        /// The log will be append to eof otherwise.
        /// Set null to turn off the file-logging.
        /// </param>
        public Gin(bool addHelpCommand= true,
                   bool addExitCommand = true,
                   SearchCommandBehaviour searchBehaviour = SearchCommandBehaviour.ScanAllSolutionAssemblies, 
                   string logFileName = null)
        {
            this._localTaskIsDone = new ManualResetEvent(true);
            this.Library = new CommandScanner();
            this._log = string.IsNullOrWhiteSpace(logFileName)
                ? (ILog) new ConsoleLog()
                : (ILog) new DecoratorLog(
                     new ConsoleLog(),
                     new FileLog(int.MaxValue, logFileName, FileLogFilter.All));

            this._executor = new Executor(this.Log);
            this.Scheduler = new Scheduler(this._executor, this.Log);
            this.Interpreter = new Interpreter(this.Library);
            
            if( searchBehaviour== SearchCommandBehaviour.ScanAllSolutionAssemblies) {
                foreach(var asm in ReflectionTools.GetAllRefferencedAssemblies()) {
                    (this.Library as CommandScanner).ScanAssembly(asm);
                }
            }
            else if( searchBehaviour== SearchCommandBehaviour.ScanExecutingAssembly) {
                (this.Library as CommandScanner).ScanAssembly(Assembly.GetEntryAssembly());
            }
            if (addHelpCommand) {
                this.AddHelp();
            }
            if (addExitCommand) {
                this.AddExit();
            }
        }
        /// <summary>
        /// Creates a Gin's instance with the specified behaviors
        /// </summary>
        /// <param name="library">library of command sketches</param>
        /// <param name="log">Log writer (set null for a console log)</param>
        /// <param name="executor">Command executor (set null for the default command executor behavior)</param>
        public Gin(ICommandLibrary library, ILog log = null, IExecutor executor = null) {
            this._localTaskIsDone =  new ManualResetEvent(true);
            this.Library   = library;
            this.Scheduler     = new Scheduler(this._executor, this._log);
            this.Interpreter   = new Interpreter(library);
            this._log = log ?? new ConsoleLog();
            this._executor = executor ?? new Executor(this.Log);
            this._executor.Log = this.Log;
            
        }
        
        /// <summary>
        /// Log writer which is using by the current Gin instance and all its nested types
        /// </summary>
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
        /// <summary>
        /// A command sketches library that is using by Gin's interpretator
        /// </summary>
        public ICommandLibrary Library { get; private set; }
        /// <summary>
        /// Flag that shows to environment that command input loop has to be done
        /// </summary>
        public bool NeedToExit { get; set; }
        
        /// <summary>
        /// Waits for the moment when all nested tasks will be finished
        /// </summary>
        public void WaitForFinsh() {
            _localTaskIsDone.WaitOne();
            this.Scheduler.WaitForFinish();
        }
        /// <summary>
        /// Waits for the moment when all nested tasks will be finished, but no more that specified time
        /// </summary>
        /// <param name="maxAwaitTimeInMsec">Max time of awaiting</param>
        /// <returns>True, if tasks were finished. False otherwise.</returns>
        public bool WaitForFinsh(int maxAwaitTimeInMsec = 0) {
            var sw = new Stopwatch();
            sw.Start();
            var ans = _localTaskIsDone.WaitOne(maxAwaitTimeInMsec);
            if (!ans)
                return false;
            sw.Stop();
            maxAwaitTimeInMsec = Math.Max(maxAwaitTimeInMsec - (int)sw.ElapsedMilliseconds, 0);
            return this.Scheduler.WaitForFinish(maxAwaitTimeInMsec);
        }

        #region execute overload
        /// <summary>
        /// Executes a command of a specified type. Сonfiguring is not possible.
        /// </summary>
        /// <param name="scheduleSettings">Launch settings. Use null for a single sync launch</param>
        public void Execute(Type commandType, CommandScheduleSettings scheduleSettings = null) {
            ReflectionTools.ThrowIfItIsNotValidCommand(commandType);
            Execute(() => (ICommand)Activator.CreateInstance(commandType), scheduleSettings);
        }
        /// <summary>
        /// Executes a command of a specified type. Сonfiguring is not possible.
        /// </summary>
        /// <param name="scheduleSettings">Launch settings. Use null for a single sync launch</param>
        public void Execute<T>(CommandScheduleSettings scheduleSettings = null) where T : ICommand, new() {
            Execute(() => new T(), scheduleSettings);
        }
        /// <summary>
        /// Parses and executes the command by input string 
        /// </summary>
        public void Execute(string inputString){
            Execute(ParseTools.SmartSplit(inputString));
        }
        /// <summary>
        /// Executes an instance that is accepted from a specified locator.
        /// Locator will be called every time before command execution
        /// </summary>
        /// <param name="scheduleSettings">Launch settings. Use null for a single sync launch</param>
        public void Execute(Func<ICommand> instanceLocator, CommandScheduleSettings scheduleSettings = null) {
            Execute(new Instruction {
                Locator = new CommandLocator(instanceLocator),
                ScheduleSettings = scheduleSettings ?? new CommandScheduleSettings {  Count = 1 }
            });
        }
        /// <summary>
        /// Executes specified instance.
        /// </summary>
        public void Execute(ICommand cmd) {
            Execute(cmd, new CommandScheduleSettings { Count = 1 });
        }
        /// <summary>
        /// Executes specified instance. Every time the same instance will be executed.
        /// </summary>
        /// <param name="cmd">Ready to go command instance</param>
        /// <param name="scheduleSettings">Launch settings. Use null for a single sync launch</param>
        public void Execute(ICommand cmd, CommandScheduleSettings scheduleSettings)
        {
            Execute( new Instruction{
                      Locator           = new CommandLocator(()=>cmd, new Dictionary<PropertyInfo,object>()),
                      ScheduleSettings  = scheduleSettings ?? new CommandScheduleSettings { Count = 1 }
                 });
        }
        /// <summary>
        /// Executes a command according to the instructions
        /// </summary>
        public void Execute(Instruction instruction)
        {
            if (instruction.ScheduleSettings == null)
                throw new ArgumentNullException("instruction.ScheduleSettings");

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

        /// <summary>
        /// Parses and executes the command by splitted input string
        /// </summary>
        public void Execute(IEnumerable<string> args)
        {
            _localTaskIsDone.Reset();
            Log.WriteMessage(">> " + string.Concat(args.Select(a => a + " ")));

            try
            {
                var instruction = Interpreter.Create(args.ToList());
                Execute(instruction);
            }
            catch (UnknownCommandNameException ex)
            {
                Log.WriteError("Command " + ex.CommandName + " not found");
            }
            catch (UnknownArgumentsException ex)
            {
                if (ex.ArgumentNames.Length > 1)
                    Log.WriteError("Unknown arguments: \" " + string.Concat(ex.ArgumentNames.Select(a => a + " ")) + "\"");
                else
                    Log.WriteError("Unknown argument: \"" + ex.ArgumentNames.FirstOrDefault() + "\"");
            }
            catch (MissedArgumentsException ex)
            {
                if (ex.ArgumentNames.Length > 1)
                    Log.WriteError("Neccessary arguments were missed: \" " + string.Concat(ex.ArgumentNames.Select(a => a + " ")) + "\"");
                else
                    Log.WriteError("Neccessary argument \"" + ex.ArgumentNames.FirstOrDefault() + "\" has missed");
            }
            catch (InvalidArgumentException ex)
            {
                Log.WriteError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Log.WriteError("IncorrectCommand ");
            }
            catch (Exception ex)
            {
                Log.WriteError("Exception: \r\n" + ex.ToString());
            }
            _localTaskIsDone.Set();
        }
        #endregion

    }
}
