# Unity.ProceduralPlant

Procedural Plant Implementation for Unity Based on "[The Algorithm Beauty of Plants](http://algorithmicbotany.org/papers/abop/abop.pdf)"

## Quick Start

1. Add the package from the following git URL:

```
git@github.com:9087/Unity.ProceduralPlant.git
```

2. Create a `Plant Species` asset from the `Create` menu.

3. Set available materials on the asset (e.g. `Default-Material`).

4. Create an empty GameObject. Add the `Plant` component to the empty GameObject.

5. Drag the `Plant Species` asset into the `Plant` component's `Species` field and adjust properties on the `Plant Species` to create various types of plants!

## Examples

**Plant-like structure**

![Figure 1.24 n=3 delta=22.5](Documentation/figure.1.24.c.n=3.delta=22.5f.png)

```
n = 3, δ = 22.5
F;
F->FF-[-F+F+F]+[+F-F-F];
```

**Bush-like structure**

![Figure 1.25 n=4 delta=22.5](Documentation/figure.1.25.n=4.delta=22.5f.png)

```
n = 4, δ = 22.5
A;
A->[&FL!A]/////'[&FL!A]///////'[&FL!A];
F->S/////F;
S->FL;
```

![Figure 1.25 n=7 delta=22.5](Documentation/figure.1.25.n=7.delta=22.5f.color.png)

```
n = 7, δ = 22.5
A;
A;A->[&FL!A]/////'[&FL!A]///////'[&FL!A];
F->S/////F;
S->FL;
L->['''^^{-f+f+f-|-f+f+f}];
```

**Flower field with stochastic application of productions**

![Figure 1.28 Flower field](Documentation/figure.1.28.flowerfield.png)

```
n = 5, δ = 18
"plant";
"plant"     ->    "internode"+["plant"+"flower"]--//[--"leaf"]"internode"[++"leaf"]-["plant""flower"]++"plant""flower";
"internode" ->    F"seg"[//&&"leaf"][//^^"leaf"]F"seg";
"seg"       -0.1> "seg"[//&&"leaf"][//^^leaf]F"seg";
"seg"       -0.7> "seg"F"seg";
"seg"       -0.2> "seg";
"leaf"      ->    ['{+f-ff-f+|+f-ff-f}];
"flower"    ->    [&&&"pedicel"'/"wedge"////"wedge"////"wedge"////"wedge"////"wedge"];
"pedicel"   ->    FF;
"wedge"     ->    ['^F][{&&&&-f+f|-f+f}];

```

## TODO

- [x] Generate leaf mesh
- [ ] Optimize vertex and triangle count
- [ ] Optimize generation efficiency
- [x] Use different materials on different parts
- [x] Stochastic application of productions
- [ ] Timed Lindenmayer systems