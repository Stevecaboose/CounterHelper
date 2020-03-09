# Counter Helper

The Windows PerformanceCounter is a useful library that can be used to monitor and collect data from a specific computer.
There are a vast amounts of counters that can be used. If you do some googling, a very common counter used in examples is
CPU usage. If you would like to explore more about what counters are available, you can open up the Windows Performance Monitor
and take a look.

Counter Helper is a dll that utilizes the Windows PerformanceCounter library (System.Diagnostics.PerformanceCounter)

One of the core concepts of this library is to add useful functionality to the Windows PerformanceCounter.
While it is simple to create a counter, and call its current value every few seconds, it might be difficult to manage multiple counters that
you want to run simultaneously.

One way this library tries to achieve this is by taking all the parts you might need to make a PerformanceCounter object and turning it into an
object of its own. By doing this, you can easily import settings that are to be used to make multiple PerformanceCounters. 

A PerformanceCounter is using a wrapper termed as a PerformanceManager

This manager can take in either an Options object or a PerformanceCounter

The manager is then used to perform actions on its respective PerformanceCounter

To simplify things, there is a method call to start the PerformanceCounters each in their own thread so they can run independently.
While these threads are running, a new thread can be used to capture the current values of the PerformanceCounters at their current state.
This allows specific counters to be managed independently from each other. Giving you more control and manageability over the multiple
PerformanceCounters you might have.

The reason I made this is to support and R&D project I am working on that utilizes the PerformanceCounter library in order to monitor 
multiple machines. Which then record the data to a database. From there, there is a web page where you can access the data and view it
in multiple different ways and manage the Options.

I also made this library because I did not find a sufficient open source library to accomplish this. The PerformanceCounter libraries that
I saw here on GitHub were lacking in useful features and were not equipped to handle time-based PerformanceCounters. This project is still 
super early and is open to suggestions and pull requests. I hope to continue to update this library as long as I can think of other 
useful ways to improve it or add new features.

### Installation

Requires .NET 4.8 to build