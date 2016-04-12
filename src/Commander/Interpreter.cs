﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            this.Factory   = new CommandFactory(scanner);
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
            var argsList = args.ToList();
            string commandName = Tools.ExtractCommandName(argsList);
            
            ICommand cmd = null;
            try {
                cmd = CreateAndConfigure(commandName, argsList);
                AttachLogTo(cmd);
            } catch (UnknownCommandNameException ex) {
                Log.WriteError("Command " + ex.CommandName + " not found");
            }
            catch (UnknownArgumentsException ex) {
                if(ex.ArgumentNames.Length>1)
                    Log.WriteError("Unknown arguments: \"" + string.Concat(ex.ArgumentNames.Select(a => a + " "))+"\"");
                else
                    Log.WriteError("Unknown argument: \"" + ex.ArgumentNames.FirstOrDefault() + "\"");

            }
            catch (MissedArgumentsException ex) {
                if (ex.ArgumentNames.Length> 1) 
                    Log.WriteError("Neccessary arguments were missed: \"" + string.Concat(ex.ArgumentNames.Select(a=>a + " "))+"\"");
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
                
            if (cmd != null) {
                Execute(cmd);
            }
        }
        public void Execute(ICommand cmd, TimeSpan interval, DateTime? launchTime = null, int? launchCount = null) {
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

        ICommand CreateAndConfigure(string commandName, List<string> argsList) {
            var argumentsDescription
                 = Tools.GetArgumentsDescription(typeof(CommandRunProperties));
            
            var intervalSettings = new CommandRunProperties();
            Tools.ExtractAnsSetToProperties(argsList, argumentsDescription, intervalSettings);

            var command = Factory.CreateAndConfigure(commandName, argsList);

            if (!  intervalSettings.At.HasValue 
                && intervalSettings.Count.HasValue 
                && !intervalSettings.Every.HasValue) {
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
            } else {
                return command;
            }
        }
        ManualResetEvent mres = new ManualResetEvent(false);
        public void WaitForFinshed() {
            mres.WaitOne();
        }
        void AttachLogTo(ICommand cmd) {
            var loggable = cmd as ILoggable;
            if (loggable != null)
                loggable.Log = this.Log;
        }
    }
}
