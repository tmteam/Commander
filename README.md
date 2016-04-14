# Gin #
Simple Background Worker

Allows you to run commands and/or regular tasks under windows dispatcher, or by buit-in task dispatcher.

## Examples: ##

### in application: ###
```
divide -a 10 -b 5  //run "divide 10 by 5"

divide -a 10 -b 5 -every 20s -count 10   //run "divide 10 by 5" in a loop, with interval of 20 seconds, 10 times

writeHello -at 02:00 -every 24h  //write "hello" every day at 2 a.m.
```
### in console: ###
```
example.exe -divide -a 10 -b 5  //run "divide 10 by 5"
```

### where "divide" is: ###

```
[Command("a/b math operation")]
    public class DivideCommand: CommandBase<double>
    {
        [Setting("a", "Double, that is divided by divider (b) ")]
        public double Dividend { get; set; }
        [Setting("b", "Double, who divides divider (a)")]
        public double Divider { get; set; }

        protected override double RunAndReturn() {
            Log.WriteMessage("output from the inside of command ");
            if (Divider == 0) {
                Log.WriteError("Divider is 0. Answer will be equal to Double.NaN");
                return Double.NaN;
            }
            return Dividend / Divider;
        }
    }
  ```  
