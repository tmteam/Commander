using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class Interpreter
    {
        public readonly CommandFactory Factory;
        public readonly Scheduler Scheduler;
        public ILog Log { get; set; }
        public TypeScanner CommandInformation { get; set; }
        public bool ExitFlag { get; set; }

        public Interpreter(TypeScanner scanner, ILog log = null) {
            this.CommandInformation = scanner;
            this.Log       = log?? new ConsoleLog();
            this.Scheduler = new Scheduler(executer: this.Execute, log: this.Log);
            this.Factory   = new CommandFactory(scanner, this.Scheduler);
        }

        public void AddHelpCommand() {
            CommandInformation.Registrate(new HelpCommand(CommandInformation.Descriptions));
        }
        public void AddExitCommand() {
            CommandInformation.Registrate(new ExitCommand(this));
        }
        public void Execute(string inputString){
            Execute(Tools.ParseToConsoleArgs(inputString));
        }

        public void Execute(string[] args) {
            Log.WriteMessage(">> " + string.Concat(args.Select(a => a + " ")));
            Dictionary<string, string> parsedArgs;
            string commandName;
            if (!Tools.TryParse(args, out commandName, out parsedArgs))
                Log.WriteError("Cannot parse the command");
            else { 
                ICommand cmd = null;

                try {
                    cmd = CreateAndConfigure(commandName, parsedArgs);
                    AttachLogTo(cmd);
                } catch (UnknownCommandNameException ex) {
                    Log.WriteError("Command " + ex.CommandName + " not found");
                }
                catch (UnknownArgumentException ex) {
                    Log.WriteError("Argument " + ex.ArgumentName + " is unknown");
                }
                catch (MissedArgumentsException ex) {
                    var str = new StringBuilder();
                    if (ex.ArgumentsName.Length> 1) {
                        str.Append("Neccessary arguments were missed: ");
                        foreach (var arg in ex.ArgumentsName) {
                            str.Append("-" + arg);
                            str.Append(" ");
                        }
                    }
                    else
                        str.Append("Neccessary argument -" + ex.ArgumentsName[0] + " was missed");
                    Log.WriteError(str.ToString());
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
                
                if (cmd != null) {
                    Execute(cmd);
                }
             }
        }
        public void Execute(ICommand cmd, TimeSpan interval, TimeSpan? launchTime = null, int? launchCount = null) {
            AttachLogTo(cmd);
            Scheduler.AddTask(
                commandFactory: new SingletoneCommandAbstractFactory(cmd),
                properties:     new CommandRunProperties {
                     At     = launchTime,
                     Count  = launchCount,
                     Every  = interval,
                });
        }
        public void Execute(ICommand cmd) {
            AttachLogTo(cmd);
            try {
                cmd.Run();
                var func = cmd as IFuncCommand;
                if (func != null)
                    Console.WriteLine("Result: " + Tools.DescribeResults(func.UntypedResult, "\t"));
            } catch (Exception ex) {
                Log.WriteError("Exception: \r\n" + ex.ToString());
            }
        }

        public void RunInputLoop() {
            while (!ExitFlag) {
                Console.Write("\r\n> ");
                var command = Console.ReadLine();
                Execute(command);
            }
        }

        ICommand CreateAndConfigure(string commandName, Dictionary<string, string> args)
        {
            var argumentsDescription
                 = Tools.GetArgumentsDescription(typeof(CommandRunProperties));
            
            var intervalArguments = new Dictionary<string, string>();
            foreach (var description in argumentsDescription) {
                var key = description.Description.ShortAlias.ToLower();
                if (args.ContainsKey(key)) {
                    intervalArguments.Add(key, args[key]);
                    args.Remove(key);
                }
            }

            var intervalSettings = new CommandRunProperties();
            Tools.SetValuesToObject(intervalArguments, argumentsDescription, intervalSettings);

            var command = Factory.CreateAndConfigure(commandName, args);

            if (intervalSettings.Count.HasValue && !intervalSettings.Every.HasValue) {
                return new RunInCycleWrapper(command, intervalSettings.Count.Value, Execute);
            }
            else if ((this.Scheduler != null) && (!intervalSettings.IsEmpty)) {
                if (intervalSettings.Every.HasValue)
                    return new RunAsThreadWrapper(this.Scheduler, intervalSettings, command);
                else if (intervalSettings.At.HasValue)
                {
                    if (intervalSettings.Count.HasValue)
                        command = new RunInCycleWrapper(command, intervalSettings.Count.Value, Execute);

                    return new RunAsThreadWrapper(this.Scheduler,
                            new CommandRunProperties {
                                Count = 1,
                                At = intervalSettings.At
                            },
                            command);
                }
                else
                    throw new InvalidOperationException();
            }
            else
            {
                return command;
            }
        }
        void AttachLogTo(ICommand cmd) {
            var loggable = cmd as ILoggable;
            if (loggable != null)
                loggable.Log = this.Log;
        }
    }
}
