# Elevator systems - optimalization
Imagine we want to construct a building and we want to design an elevator system for it. How can we do it, so the elevator system is the most efficient one for this specific building? We run simulations with different parameters, compare them and pick the best one. This is what this program is about.

## Run your own simulations 
You can run your own simulations based on different parameters. You'll also be able to compare different elevator scheduling algorithms against each other on the same building or different buildings.

### Set parameters
These are some of the parameters you'll be able to set.
1. number of floors in the building
1. number of lift shafts
1. total population of the building
1. population distribution
1. elevators speed, capacity, acceleration, ...
1. average waiting time of elevator for passengers getting on/off

* Number of floors and number of lift shafts defines a building. But same buildings can have different population size and distribution. Population size should reasonably represent the average number of people using elevators in the building per day.

* The population distribution assigns each floor a probability of a person wanting to use an elevator at a given time (e.g. in the morning, floor one would probably have a lot of people wanting to use elevator, because they just got in and want to go to their offices).

* It is quite obvious, that the more lift shafts we could use or the better each elevator is the more efficient would our elevator system be. However, we would like to minimize the number of lift schafts, because each lift schaft is economicaly just a wasted space, that could have been used for something more profitable.
The same goes for elevator parameters (speed, capacity, ...). It's reasonable to keep them bounded below some maximum parameters.


### Set algorithm
* For your own simulation, you'll be able to choose one of the most frequently used scheduling algorithms, such as first come first served, priority scheduling, shortest remaining time first, round robin, SCAN, etc ...

### Run
* The idea is, that you've defined your building with it's population distribution. Now you run simulations with different algorithms and different number of lift schafts, to see, what you think is optimal.
You can either just run the simulation or interact with it. Spawning people to desired floor destinations to see, how the system changes.
Based on acquired data, you can yourself decide, what simulation is the best fit for your new building or apply quality function on it.

## Let it be optimized for you
After you define your building and your population distribution, you can either run simulations with different parameters and observe how it behaves, as was described above or you can have it done for you.
Using this approach, program will run several simulations with different parameters (algorithm simulations) and try to find the best one based on some quality function.

### Algorithm simulation
* it is simple a simulation on a given building and population distribution with given parameters
* parameters, that can be tweaked:
    1. **algorithm**
        * can also change over time (change according to changes in population distribution, e.g. in the morning it'll use up

    1. **number of lift schafts**
        * of course, the more the better, but sometimes the difference can be minor (e.g. population is 100 and there is one elevator that can serve 80 people comfortably, adding new lift schaft would improve overall efficiency, but the cost of extra schaft isn't worth it)
    
    1. **population distribution**
        * even if some algorithm operates very well on average population distribution, it might be utterly inefficient on different but quite similiar distributions. Situation where people suddenly behave a little differently (different distribution) could very much happen in real life.

    1. **elevator parameters**
        * speed
        * capacity
        * waiting time
        * acceleration

### Quality function
* this function will measure how succesful is given algorithm simulation, based on:
    1. **waiting time for the elevator**
        * average, worst case, best case, median ...
    1. **time spent in the elevator**
        * average, worst case, best case, median ...



