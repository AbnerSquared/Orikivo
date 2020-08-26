# Versioning
> This is the versioning system that Orikivo utilizes.

## Release
```
{REWRITE}.{RELEASE}.{PATCH}
```

- If the library is ever rewritten, bump the `REWRITE` number.
- If a new release is posted, bump the `RELEASE` number and reset the `PATCH` number.
- If a new patch is posted, bump the `PATCH` number.
- If a release is undergoing many iterative changes, use Release Candidate versioning instead.

## Release Candidate
```
{REWRITE}.{RELEASE}rc-{CHANGE}
```
- The `REWRITE` and `RELEASE` numbers remain the same until a new release is specified.
- For each released update, bump the `CHANGE` number.

## Alpha/Beta:
```
{RELEASE}.{BREAKING}.{CHANGE}{VERSION}
```
- If a new release is posted, bump the `RELEASE` number and reset the `BREAKING` and `CHANGE` number.
- If any breaking change or addition is made, bump the `BREAKING` number.
- If any basic patch, change, or addition is made, bump the `CHANGE` number.
- The `VERSION` character is either `b (BETA)` or `a (ALPHA)`.
