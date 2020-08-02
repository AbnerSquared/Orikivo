# Refactoring Guidelines
> This is intended to be a general set of rules to follow when writing or refactoring code on Orikivo.

## General

### Explicitly state variables where needed
> If the value of the variable does not specify a type, it must be explicitly written.

```cs
// DO:
var v = new Class();

// DO NOT:
Class v = new Class();
```

### Limit non-bracket methods to a single depth
> This means that if there's a chain of single non-bracket methods, it should be wrapped.
```cs
// DO:
foreach(var v in enumerable)
{
    if (v == c)
        return true;
}

// DO NOT:
foreach (var v in enumerable)
    if (v == c)
        return true;
```

## Comments

### Prefix to-do comments with `TODO:`
```cs
// TODO: Do this thing on this method
public void Method()
{
  return;
}
```

### Prefix reference URLs with `REF:`
```cs
// REF: https://www.google.com
public void Method()
{
  return;
}
```

## LINQ

### Use .Count() instead of .Where().Count()
> .Count() supports the usage of predicates.
```cs
// DO:
enumerable.Count(x => x.IsTrue);

// DO NOT:
enumerable.Where(x => x.IsTrue).Count();
```