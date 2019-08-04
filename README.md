[![Build status](https://ci.appveyor.com/api/projects/status/l7s7j1wencecnrtq?svg=true)](https://ci.appveyor.com/project/monkog/2sat-problem)
# :bulb: 2-SAT Problem Solver
This application solves the 2-satisfiability problem. It accepts an XML file describing the formula and if the 2-SAT problem is satisfiable, it outputs another XML file containing the solution.

## Boolean Satisfiability Problem
The boolean satisfiability problem is the problem of determining whether a boolean formula  is satisfiable (evaluates to true) or unsatisfiable (evaluates to false). It finds out whether it's possible to assign to boolean variables values, for which the result of the formula equals true (is satisfiable).  

## 2 Satisfiability Problem
The 2-SAT problem is a special case of the boolean satisfiability problem in which only two variables can occur in a single clause of the formula. The boolean satisfiability problem is a NP-complete problem, but the 2-SAT problem can be solved in polynomial time.  

Each such formula can be converted to a specific form. The formula has to be a **conjunction** (boolean `∧` operator) of specific clauses. Each clause has to be a **disjunction** (boolean `∨` operator) of two variables or their negations (boolean `¬` operator). Such form of the formula is called **Conjunctive Normal Form (CNF)**.

## Input file format
The input file should be an XML file. The root node should be named **SAT2**. The root node should have children nodes named **Condition**. The number of children determines how many clauses will the formula have. Each Condition node should define two attributes **x** and **y** specifying variables used in the clause.

## Output file format
If the given formula is satisfiable, an XML file containing the solution will be produced. Like in the input format, the root node will be named **SAT2**. It will have **Solution** children of number equal to the number of variables in the input formula. Each Solution node will have a **var** attribute determining which variable it represents and the **value** attribute stating the computed value for which the formula will evaluate to true.

## Example
For given formula  

![(a ∨ b) ∧ (b ∨ ¬ c) ∧ (¬ a ∨ ¬ c) ∧ (¬ b ∨ d)](https://latex.codecogs.com/gif.latex?(a\vee&space;b)\wedge&space;(b\vee&space;\sim&space;c)\wedge&space;(\sim&space;a\vee&space;\sim&space;c)\wedge&space;(\sim&space;b\vee&space;d))

the input file should look like this

```xml
<SAT2>
  <Condition x="a" y="b"/>
  <Condition x="b" y="-c"/>
  <Condition x="-a" y="-c"/>
  <Condition x="-b" y="d"/>
</SAT2>
```
The result file containing the solution will look as follows
```xml
<SAT2>
  <Solution var="a" value="0" />
  <Solution var="b" value="1" />
  <Solution var="c" value="0" />
  <Solution var="d" value="1" />
</SAT2>
```

## :link: Useful links and resources
:bulb: [Boolean Satisfiability Problem on Wikipedia](https://en.wikipedia.org/wiki/Boolean_satisfiability_problem)  

:art: [Background graphic](https://upload.wikimedia.org/wikipedia/commons/2/2f/Implication_graph.svg)
