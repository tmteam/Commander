# Commander #
Simple Background Worker

Allows you to run regular tasks under windows dispatcher, or by buit-in task dispatcher.

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
