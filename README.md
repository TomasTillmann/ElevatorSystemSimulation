# Elevator systems - optimization
## Abstract
Imagine we want to construct a building and we want to design an elevator system for it. How can we do it, so the elevator system is the most efficient one for this specific building? We run simulations of different elevator systems and different algorithms, compare them and pick the best one. This is what this program is about.

## Problem
Imagine a building with an elevator system. How should the elevator system work to be the most efficient one? What is the best elevator system strategy for this concrete building?

Before we dive into these questions, we need to start with what an elevator system actually is. 
Elevator system consists of elevators, each having some parameters (speed, capacity, ...) and actions (move up, move down, stay, open doors, ...)
and strategy (SCAN, first comes first served, ...), that controls elevators. This strategy is what we would like to optimize. 

Thus we want to find the best way how should an elevator system behave.
Elevator system makes it's decisions based on these information:
* on what floors are requests
* what floors must each elevator visit
* where each elevator is

These are also information that every elevator system needs to have at it's disposal, otherwise it could only operate randomly.
Just having these information as input is sufficient for developing some more sophisticated strategy, but there are other information, that an elevator system can have access to:

**Current situation information**:
* how many people each elevator has
* how many people in elevator want to go in each floor
* how many people requesting for elevator is on each floor
* how many people is in the building

**Population predictions**:
* how likely a request appears at some floor (can change over time)
* how likely a person from floor A would like to go to floor B (can change over time)

If elevator system has some of these (or possibly some different) information at his disposal, his strategy can be much more sophisticated and has potential to operate much better. Note, that information, that each elevator needs to have at it's disposal are *Current situation information*.

How likely a request appears at some floor changes over time. For example in up peak period, it is very likely that requests occur in ground floor. In down peak period, most of the requests would appear in higher floors.
Similiarly, for how likely a person would like to go from floor A to a different floor B. In up peak, persons from ground floor would probably like to go to their offices, let's say floors 5 to 10 and during down peak period, they would like to go from their offices to ground floor.

What information elevator system has depends on how sophisticated it is.
Some elevator systems for example have the abilitiy to know how many people is in the building, each floor, or in each elevator, because elevator users have some sort of identification card with which they request the elevator.

Some other systems have the ability to know how many people in elevator want to go in each floor, because when user calls for the elevator, he doesn't just press a button, but also configures on a display where would he like to go.

There is many different elevator systems with different information at their disposal. It is safe to say, that elevator systems with more available information have the biggest potential to be the most optimal, but we might not always have the financial needs for the best elevator systems. Sometimes, we would like to find the most optimal strategy for some not so sophisticated elevator systems.

Consequently, there is also a handful of metrics against we can measure how some strategy is succesful. Some very reasonable metrics are average waiting time for an elevator, average waiting time in an elevator, worst-case waiting time or even some average of all of these metrics combined ...

So now we know what problem we want to solve. We want to find the best strategy for some elevator system or more elevator systems with different available information about building's population. How good a strategy is could be predefined by some metrics.

## Formalization of the problem 
### Input
* Buildig $B = (E, I)$ with elevator system $E$ that has some available information about building's population $I$
    * $I = (C, P)$
    * where $C$ are current situation information and $P$ are population predictions 
* Metrics that define how succesful strategy is $M$

### Output
* find the most optimal strategy $S$ for given building $B$ against given metrics $M$

## My approach
Let there be some efficiency function $q_M: q(S, B) \rightarrow [0,1]$, obeying given metrics $M$.
This efficiency function takes strategy $S$ and building $B$ and rates it by number from 0 to 1. The bigger the number, the more $S$ is optimal forr elevator system $E$ in $B$.

If we have $q_M$ and some set of strategies $S_{set}$, we can very easily find $S_{optimal} \in S_{set}:$ $q_M(S_{optimal},B) = max(\{q_M(S, B) | \forall S \in S_{set}\}$ that is the most optimal.

Obtaining some $S_{set}$ isn't very difficult. It can be for example a set of some well-known scheduling algorithms, such as SCAN, First comes first served, priority scheduling, round robin scheduling, ...
Set $S_{set}$ could also potentially contain some user defined algorithms or some algorithms developed specificaly for $B$ (for example by some genetic algorithm,...).

It is much harder to obtain $q_M$, which is right now the only missing piece needed for solving the problem. 

This is how I want to define this efficiency function $q_M$. This efficiency function $q_M$ will run discrete simulation.
The simulation runs for some reasonably long time and after it ends it calculates how well $S$ did according to $M$.



## Formal model
Floors are a set $F$, where $F \subseteq \mathbb{Z}$, each element $i \in F$ represent $i-th$ floor.

Elevator  $e \in E$ is a tuple $(a, P_s, P_d)$, where $a \in A$ represent elevator's current action, where $A$ is a set of all possible actions: move up, move down, idle, board. $P_s$ is a set of elevator's static parameters: capacity, acceleration, speed.
These parameters remain the same for the whole simulation and they represent parameters that do not change dynamically. $P_d$ is a set of dynamic parameters, describing elevator's current state: current speed, current capacity, current people count. Set $E$ is a set of all possible elevators.

Elevators $L$ are $L \subseteq E$.

Building $b$ is a tuple $b = (F_b, L_b)$, $F_b \subseteq \mathbb{Z}$, representing floors and $L_b \subseteq E$ representing elevators.

Population distribution of a building $b = (F_b, L_b)$ is a tuple $p_{d_b} = (w_{b_1}, w_{b_2}) \in P_{d_b}$, where $w_{b_1}: F_b \rightarrow [0,1]$, is a probability function representing how likely a request for elevator will occur at $j-th, j \in F_b$ floor and $w_{b_2}$ is an $|F_b|$-tuple $(w_1, w_2, ..., |F_b|)$, where $w_i: F_b \rightarrow [0,1]$ are probability functions representing probability of person wanting to go from $i-th, i \in F_b$ floor to $j-th, j \in F_b$ floor. $P_{d_b}$ is a set of all possible population distributions for building $b$.

Count of people on floors of a building $b = (F_b, L_b)$ $c_{p_b} \in C_{p_b}$ is a function $c_{p_b}: F_b \rightarrow \mathbb{N}$, representing how many people/requests are on each floor.
Set $C_{p_b}$ is a set of all possible count of people on floors functions for $b$. 

Elevators position of a building $b = (F_b, L_b)$ $e_{p_b} \in E_{p_b}$ is a function $e_{p_b}: L \rightarrow F_b$, representing on which floors elevators are.
Set $E_{p_b}$ is a set of all possible elevators positions functions for $b$.

Population context $c_b$ of a building $b$ is a function $c_b: c_b(t) \rightarrow (c_{p_b}, p_{d_b})$,representing population of a building $b$ at some time $t \in \mathbb{R}$, where $c_{p_b} \in C_{p_b}, p_{d_b} \in P_{d_b}$.

Population context can simulate how a building's population and it's behavior changes over time. Population context provides all necessary inputs for our elevator system.

Strategy is a function $s(L, c_f, p_d) \rightarrow L'$, where $L \subset E$, $c_f$ is a count function and $p_d$ is a population distribution.
For every strategy function holds, that if $L = (a, P_s, P_d)$, then $L' = (a', P_s, P_d)$, $a' \in A$.
Strategy function looks at the current con

And finally, elevator system $e_b \in E_b$ for building $b$ is a tuple $(L, s)$, where $L \subset E$ is a set of elevators and $s$ is a strategy function.

Now we know what an elevator system is, so we can try to optimize it.

## Simulation
Simulation referes to discrete event simulation obeying next-event time progression paradigm (TODO: reference wiki).
Simulation will start at some initial situation $T_{0}$, for example situation, where all elevators are in first floor ($f_t \in T_{0}, f_t(l)= 0$ $\forall l \in L \in T_{0}$), there are no people yet ($g_t \in T_{0}, g_t(f) = 0$ $\forall f \in F \in T_{0}$) and hence no elevator must visit some floor ($h_t(e) = \emptyset$ $\forall e \in E$).
One step of a simulation coresponds to transition from one situation to some other situation according to elevator system strategy function. Formally defined by induction:
$T_1 = s(T_0)$ and $T_i+1 = s(T_{i})$, $i \in \mathbb{N}$.

In each step, from $T_i$ to $T_{i+1}$:

* update global time $t$ by time of step $t_{s_i}$, $t = t + t_{s_i}$
    * global time keeps track of for how long the simulation is going and dictates population distribution 
* time of step is determined by speed of currently moved elevator(s)
    * elevators can have different speeds, so some elevators move from one floor to another in one simulation step, but others are not that fast, so they are between some two floors
* update population distribution, $p_d(t) \in (s(T_i) = T_{i+1})$
* update elevators locations, $f_t \in (s(T_i) = T_{i+1})$
* spawn requests/people, update $g_t \in (s(T_i) = T_{i+1})$.
* update what floors each elevator needs to visit, update $h_t \in (s(T_i) = T_{i+1})$.

Another step of discrete event simulation is triggered by some event. If strategy function is reasonably defined, each step should corespond to simulating an event when elevator or elevators arrive to a new floor.

In strategy function definition, there are no constraints on what situations can some situation be mapped. But in order to model a real world scenario, strategy function should be able to map situation only on situations, where elevators don't change their parameters, elevators positions are only one floor away from each other and so on ...

TODO: dodefinuj strategy function definition, aby byli dosazitelne jenom nejake sitauci - tak by potom situace odpovidali nejakemu stavovemu prostoru (graf kde hrany reprezentuji dosazitelnost)


## Efficiency function
We will measure efficiency by some efficiency function $q_b \in Q_b: E_b \rightarrow \mathbb{R}$, where $Q_b$ is set of all efficiency functions for $b$.
If for some two elevator systems $e_{b1}, e_{b2} \in E_b$ $q_b \in Q_b: q_b(e_{b1}) > q_b(e_{b2})$, we say that $e_{b1}$ is more efficient than $e_{b2}$ according to $q_b$.
Depending by what metrics we consider elevator system efficient, we choose appropriate efficiency function.

Some reasonable metrics are:

* average/worst-case/median/... waiting time for elevator
* average/worst-case/median/... waiting time in elevator
* how it behaves under little bit different population distribution
* TODO others, might be a good idea to reference to current knowledge section

We can define waiting time of a person for elevator as number of situations between first button press (request) of a person on some floor and first elevator on the same floor with action board, such that the person can actually board the elevator (e.g. maximum capacity isn't surpassed).

Defining waiting time of a person in an elevator is very similiar. It is number of situations between person's boarding and getting off the elevator.

Efficiency function evaluates elevator systems by running simulations.

## My approach
We take some elevator systems and evaluate them through efficiency function.
Efficiency function runs several simulations on this elevator system. 
We choose reasonable set of efficiency functions beforehand. What efficiency functions we want to use depends on what metrics are important for us.
After all elevator systems have been evaluated, we pick the best elevator system based on requirements and collected data (e.g. pick elevator system that performs best on average for every efficiency function).

This approach has several advantages. Firstly, we can easily see how specific elevator system behaves and what decisions does it make in each situation.
Secondly, we can also easily tweak input parameters and see by how much different elevator systems differ. Thridly, we are very flexible in what efficiency functions to choose and by what metrics evaluate elevator systems. And last but not least, we can very easily add on new strategies in the future and test them against already collected data.

The only disadvantage I see is that each simulation might take some nontrivial amount of time, but I don't think it should be an issue (definitely not on simple strategies, like some scheduling algorithms).


# Program implementation of formal model
TODO: 
* make this more software oriented, math definitions above
* delete some and update

### **Simulation**
* every step of the simulation elevators can either move up, down, stay or board people (these are all the events).

#### Attributes
* scheduler
* global time
* current situation

### **Elevator**
* Controlled by strategy
* Elevator in a building. There might be elevators with different parameters in the same building, hence each of a different type.
* Elevator doesn't need to have all attributes set. Some elevators aren't sophisticated enough to know how many people is on board and knows just the current weight. Some others might not even know the current weight.

#### Attributes
* speed
* capacity
* acceleration
* average waiting time of elevator for passengers getting on/off
* current number of people
* current weight

#### Actions
* up()
* down()
* stay()
* board()

### **Building**
* Building where we want our efficient elevator system.

* number of floors
* population distribution 

### **Population distribution**
#### Attribues
* each floor has assigned probability - coresponds to $w_b \in p_b$ 

* each floor has list of probabilities to what floors person would likely want to go - corresponds to $w_f \in p_b$, each entry coresponds to $w_i \in w_f$ 

* population size - coresponds to $s \in p_b$

#### Actions
* Distribute(time)
    * assigns each floor requests/persons according to distribution

### **Situation**
* Some attributes might be set or might not. It depends how sophisticated you want your elevator system to be. For example, if elevator system users have some sort of ID card, than each person can call an elevator by the id card and therefore the CES could be certain about the number of people in a given floor. In this scenario, situation should carry this information. 
But in a different scenario, where users don't have an identification, CES couldn't know how many people is actually waiting on each floor. It's only information is how many times a button is pressed (and one person can press the button how many times he likes), so in this scenario it might not make sense to remember people count.

#### Attributes
* list of elevators with their positions
* list of floors with people count
* list of floors with indication whether there is a request for elevator or not 
* list of floors to visit for each elevator

### **Elevator system**
#### Attribues
* elevators
* strategy

### **Evaluation function**
* evaluates given strategy

#### Action
* Evaluate(simulation)

TODO:
* too early to specify user guide
* general and user simulations arent very clear
* disclaimer. this section is outdated
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

