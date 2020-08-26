# Refactoring Guidelines
> This is intended to be a general set of rules to follow when writing or refactoring code on Orikivo.

## General

### Elements must appear in the correct order
> This is inherited from StyleCop [SA1201](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1201.md).

### Keep indentations at 4 spaces long
> This is used to help keep everything consistent.
```cs
// DO:
if (true)
{
    // ...
}

// DO NOT:
if (true)
{
// ...
}
```

### Isolate brackets on their own line
> This is the preferred style of brackets that Orikivo uses.
```cs
// DO:
if (true)
{
    // ...
}

// DO NOT:
if (true) {
    // ...
}
```

### Compare DateTime differences with TimeSpan when possible
> This helps ensure consistency with comparison.
```cs
// DO:
(DateTime.UtcNow - time) >= TimeSpan.Zero;

// DO NOT:
(DateTime.UtcNow - time).TotalSeconds > 0;
```

### Explicitly state variables where needed
> If the value of the variable does not specify a type, it must be explicitly written.

```cs
// DO:
var v = new Class();

// DO NOT:
Class v = new Class();
```

### Use safe casting where possible
> On top of simplifying code, this makes sure that the value is of the specified type.
```cs
// DO:
if (item is ItemType casted)
    casted.DoSomething();

// DO NOT:
if (item is ItemType)
    ((ItemType)item).DoSomething();
```

### Use bitwise operations on Enum flags
> If an enum is going to be checked for multiple values, mark the Enum as a flag instead.
```cs
// DO:
ItemTag tag = ItemTag.A | ItemTag.B;

// DO NOT:
var tags = new List<ItemTag>
{
    ItemTag.A,
    ItemTag.B
};
```

### Chain simple predicates for if statements
> Simple predicates are usually single word comparisons.
```cs
// DO:
if (item.HasValue && item.Value == 1)
{
    // ... 
}

// DO NOT:
if (item.HasValue)
{
    if (item.Value == 1)
    {
        // ...
    }
}
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

### Use Check.NotNullOrEmpty when possible
> This checks if an enumerable contains any elements. If it is marked as null or has no elements, this returns false instead.
```cs
// DO:
bool specified = Check.NotNullOrEmpty(enumerable);

// DO NOT:
bool specified = enumerable?.Any() ?? false;
```

### Use .Exists() when possible
> If the IEnumerable<T> reference is a List<T>, you can use .Exists() instead of .Any().
```cs
// DO:
bool exists = list.Exists(x => x != null);

// DO NOT:
bool exists = list.Any(x => x != null);
```

### Keep static value reference classes as internal
> Since most of these references aren't intended to be used outside of Orikivo, they should remain hidden to others.
```cs
// DO:
internal static class Stats
{
    internal static readonly string Id = "value";
}

// DO NOT:
public static class Stats
{
    public static readonly string Id = "value";
}
```

### Do not specify an Enum property for the number 0 if the Enum is marked as bitwise
> Due to the nature of bitwise operations, if an enum is specified at 0, it will return that value.

### Increment bitwise Enum properties by the power of 2
> This ensures that bitwise operations are consistent.
```cs
// DO:
[Flags]
public enum ItemTag
{
    A = 1,
    B = 2,
    C = 4,
    D = 8
}

// DO NOT:
[Flags]
public enum ItemTag
{
    A = 1,
    B = 2,
    C = 3,
    D = 4
}
```

### Increment non-bitwise Enum properties by 1
> To help keep with consistency, enumerations that tend to increase by the power of 2 are usually bitwise.
```cs
// DO:
public enum ItemTag
{
    A = 1,
    B = 2,
    C = 3,
    D = 4
}

// DO NOT:
public enum ItemTag
{
    A = 1,
    B = 2,
    C = 4,
    D = 8
}
```

## Comments

### Prefix to-do comments with `TODO:`
```cs
// TODO: Do this thing on this method
public void Method()
{
    // ...
}
```

### Prefix reference URLs with `REF:`
```cs
// REF: https://www.google.com
public void Method()
{
    // ...
}
```
### Prefix important notes with `NOTE:`
> This is used to emphasize on specific comments.
```cs
// NOTE: This is an important note I wanted you to see.
public void Method()
{
    // ...
}
```


## LINQ

### Use .Any() instead of .Count() when checking for 0
> .Any() is simply the shorthand version, which makes more sense.
```cs
// DO:
bool isEmpty = !enumerable.Any();

// DO NOT:
bool isEmpty = enumerable.Count == 0;
```

### Use .Count() instead of .Where().Count()
> .Count() supports the usage of predicates.
```cs
// DO:
enumerable.Count(x => x.IsTrue);

// DO NOT:
enumerable.Where(x => x.IsTrue).Count();
```

### Use .FirstOrDefault() where possible
> .FirstOrDefault() removes a lot of bulk that surrounds a foreach block.
```cs
// DO:
return enumerable.FirstOrDefault(x.Value == value);

// DO NOT:
foreach (object item in enumerable)
{
    if (item.Value == value)
        return item;
}
```
