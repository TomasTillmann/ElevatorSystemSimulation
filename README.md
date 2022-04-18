# Elevator systems - optimalization
Imagine we want to construct a building and we want to design an elevator system for it. How can we do it, so the elevator system is the most efficient one for this specific building? We run simulations of different elevator systems, compare them and pick the best one. This is what this program is about.


## Definitions
### **Simulation**
* It is a discrete simulation, where every step of the simulation elevators can either move up, down or stay (these are all the events). It uses standard discrete simulation techniques and patterns.

#### Attributes
* Scheduler

### **Elevator**
* Elevator in a building. There might be elevators with different parameters in the same building.

#### Attributes
* speed
* capacity 
* acceleration
* average waiting time of elevator for passengers getting on/off
* current number of people

#### Actions
* up()
* down()
* stay()

### **Building**
* Building where we want our efficient elevator system.

* number of floors
* number of elevators and elevator types

### **Population distribution**
* Assigns each floor how likely a person on this floor would like to use an elevator and where probably would the person go, at a given time. 
* e.g. In an office building in the afternoon from floor 5,6,7 it is very likely a person would call an elevator and would like to get to floor one or underground (parking), because their workday is over, but in the morning it will be the other way around.

#### Attribues
* time 
    * time of day
    * can be disretized in morning, after lunch, afternoon or in hours, minutes, ...

* each floor has list of probabilities, each corresponding to what floor a person might want to get (e.g. floor 1: 2 - 0.2, 3 - 0.3 4 - 0.2 5 - 0.2 6 - 0.1)

* each floor has probability of person wanting to use the elevator (e.g. floor 1 - 0.8, floor 2 - 0.05, ... floor 6 (last) - 0)

* population size
    * how many persons can spawn in a day
    * represents average number of people using building's elevators each day

### **Situation**
* Represents where (in what floors) are all the elevators and where are all the people either waiting for elevator or already in an elevator.
* Central elevator scheduler makes decisions based on the surrent situation.
* Every time an event happens, the current situation changes to next situation. Situations are atomic.

#### Attribues
* list of elevators
* list of floors with people count

### **Central Elevator Scheduler**
* Gives instructions to elevators based on the current situation, meaning it assigns every elevator some action (event). Central Elevator Scheduler obeys some scheduling algorithm.
* Can use different scheduling algorithms at different times (e.g. would like to use different scheduling algorithm in the morning and in the afternoon).

#### Attribues
* population distribution
    * it's crucial that CES has this knowledge, because thanks to this, it can decide globally and not just locally by the current situation
* situation
* strategy - algorithms to obey at given times

### **Evaluation function**
* evaluates given strategy


## How to use? 
You can either run your own simulations based on different parameters, compare different algorithms and try to optimize it for yourself or you can use more sophisticated approach and let this program run several simulations with different algorithms and tweaked parameters to find the most optimal solution.

## Parameters
These are parameters you are able to set before running the simulation:
1. number of floors in the building
1. number of elevators 
1. population distribution
1. each elevator's parameters 
1. CES strategy



## Optimization simulations
### What to optimize?
* It is quite obvious, that the more elevators and the more efficient they are the more efficient is our elevator system going to be. However, we would like to minimize the number of elevators, because each lift schaft is economicaly just a wasted space, that could have been used for something more profitable.

* The same goes for elevator parameters (speed, capacity, ...). It's reasonable to keep them bounded below some maximum parameters, to make each elevator affordable.

* Hence, it makes sense to try to optimize our elevator system not just solely on performance but also on it's cost.

* Neverthless, how good elevator system is will ultimately determined by some Evaluation function, that can use completely different evaluating principle.

### General simulation
* in general simulation, you don't specify number of elevators and their qualities.

* general simulation tries to not only optimize strategy, but also optimize number of elevators and their parameters to satisfy some Evaluation function.

* general simulation just runs multiple concrete strategies with different elevator counts and qualities and compares those using some quality function.

* You can choose the Evaluation function
    * best bet would be quality function based on some price-quality ratio 

### Concrete simulation
* you specify concrete number of elevators and their qualities

* concrete simulation just tries to find the best possible strategy for given building and population distribution


## User Simulation 
* After some optimization simulation has found the best approach how to tackle given building and population distribution, it might be convenient to try to run a simulation with found optimal elevator system and see how it behaves for yourself. This is exactly what user simulation is for.

* During user simulation, you can play around and change some parameters, to see how adaptive optimal solution is and have feel for how it behaves:
    1. spawn persons to exact floors
    1. change CES strategy
    1. tweak population distribution

## Future
* it would be great to not just have very simple scheduling algorithms at our disposal but also some more sophisticated techniques, like genetic algorithms, machine learning etc ...

