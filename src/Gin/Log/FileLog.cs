using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    /// <summary>
    /// The log which writes to a specified file 
    /// </summary>
    public class FileLog : ILog
    {
        string filePath;
        object locker;
        int maxLogLength;
        int writedCount;
        FileLogFilter writeFilter;

        public FileLog(int maxLogLength, string relativeFileName, FileLogFilter writeFilter = FileLogFilter.All) {
            this.writedCount = 0;
            this.writeFilter = FileLogFilter.All;
            this.locker = new object();
            this.filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relativeFileName);
            this.maxLogLength = maxLogLength;
            this.writeFilter = writeFilter;
        }

        public FileLog(string absoluteFilePath, int maxLogLength, FileLogFilter writeFilter = FileLogFilter.All) {
            this.writedCount = 0;
            this.writeFilter = FileLogFilter.All;
            this.locker = new object();
            this.filePath = absoluteFilePath;
            this.maxLogLength = maxLogLength;
            this.writeFilter = writeFilter;
        }

        public void WriteError(string str)
        {
            if (this.writeFilter.HasFlag(FileLogFilter.Errors)) {
                this.WriteLine("[ERROR] " + str);
            }
        }

        public void WriteLine(string text)
        {
            if (this.writedCount < this.maxLogLength) {
                lock (this.locker) {
                    using (var writer = new StreamWriter(this.filePath, true)) {
                        writer.WriteLine(text);
                    }
                    this.writedCount++;
                }
            }
        }

        public void WriteMessage(string str)
        {
            if (this.writeFilter.HasFlag(FileLogFilter.Messages)) {
                this.WriteLine(str);
            }
        }

        public void WriteWarning(string str)
        {
            if (this.writeFilter.HasFlag(FileLogFilter.Warnings)) {
                this.WriteLine("[warning] " + str);
            }
        }
    }
    [Flags]
    public enum FileLogFilter : byte
    {
        All = 7,
        Errors = 1,
        Messages = 4,
        Nothing = 0,
        Warnings = 2,
        WarningsAndErrors = 3
    }

 


}
