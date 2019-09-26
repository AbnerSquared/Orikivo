using System.Collections.Generic;
using Orikivo.Utility;

namespace Orikivo
{
    // Switch Value to an Expression class, from which that will
    // hold all of the needed values and variables.
    public class Equation
    {
        public Equation(Equation parent = null, int layer = 0, int position = 0, string value = "")
        {
            Value = value;
            Parent = parent;
            Layer = layer;
            Position = position;
            Children = new List<Equation>();
        }

        public Equation(string s)
        {
            Equation e = Build(s);
            Value = e.Value;
            Parent = e.Parent;
            Layer = e.Layer;
            Position = e.Position;
            Children = e.Children;
        }

        public Equation(Equation parent)
        {
            Parent = parent;
            Layer = Parent.Layer + 1;
            Position = Parent.Children.Count;
        }

        public string Value {get; set;}
        public Equation Parent {get; set;}
        public int Layer {get; set;}
        public int Position {get; set;}
        public List<Equation> Children {get; set;}

        public static Equation Build(string e)
        {
            e = e.Trim();
            Equation n = new Equation();
            Equation p = new Equation();
            int a = 0;
            int z = 0;
            string s = "";
            foreach (char c in e)
            {
                if (c.EqualsAny('(', '[')) a += 1;
                else if (c.EqualsAny(')', ']')) z += 1;
            }
            if (a.Equals(z))
            {
                foreach (char c in e)
                {
                    if (c.IsSolvable())
                    {
                        if (s != "")
                        {
                            if (s.EndsWith("0"))
                            {
                                if (s.Length == 1)
                                {
                                    if (c.IsDigit())
                                    {
                                        s = "" + c;
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (s[s.Length - 2].IsOperator())
                                    {
                                        if (c.IsDigit())
                                        {
                                            s = s.ReplaceEnd(c);
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (s.EndsWith("/0"))
                            {
                                if (c.IsOperator())
                                {
                                    s = "";
                                    n = n.Throw("divide_by_zero");
                                    break;
                                }
                            }
                            if (new char[] {c, s[s.Length - 1]}.AreOperators())
                            {
                                s = "";
                                n = n.Throw("invalid_operator_usage");
                                break;
                            }
                        }
                        s += c;
                        continue;
                    }
                    else if (c.EqualsAny('(', '['))
                    {
                        if (s != "")
                        {
                            if (s[s.LengthAsIndex()].IsAlphanumeric())
                                s += "*";
                            
                            if (s.EndsWith("/0"))
                            {
                                s = "";
                                n = n.Throw("divide_by_zero");
                                break;
                            }
                        }
                        s += $"[{n.Layer + 1}, {n.Children.Count}]";
                        s.Debug($"[Opening a new equation in {n.Index()}...]");
                        n.Conjoin(s);
                        p = n;
                        n = new Equation(p);
                        p.Adopt(n);
                        s.Debug($"[Set {n.Index()} in layer {p.Layer}.]");
                        s = "";
                        continue;
                    }
                    else if (c.EqualsAny(')', ']'))
                    {
                        if (s.EndsWith("/0"))
                        {
                            s = "";
                            n = n.Throw("divide_by_zero");
                            break;
                        }
                        n.Value += s;
                        if (n.HasValue())
                        {
                            Debugger.Write("[No property was written. Setting to null...]");
                            n.Assign("null");
                        }
                        n.Value.Debug($"[Closing {n.Index()}...]");
                        n = n.Parent;
                        s = "";
                        continue;
                    }
                }
                if (s != "")
                {
                    if (s.EndsWith("/0"))
                    {
                        s = "";
                        n = n.Throw("divide_by_zero");
                    }
                    else
                    {
                        n.Conjoin(s);
                        n.Value.Debug($"[Extending {n.Index()}...]");
                    }
                }
            }
            else
            {
                Debugger.Write("[Incorrect equation structure. Now cancelling...]");
                n.Throw("invalid_equation_structure");
            }
            n = n.GetRootParent();
            return n;
        }

        public bool HasParent() =>
            Parent.Exists();

        public Equation GetRootParent()
        {
            Equation x = this;
            do x = (Parent ?? x);
            while (Layer > 0);
            return x;
        }

        public Equation Throw(string s)
        {
            Equation x = this.GetRootParent();
            x.Value = $"error | {s}";
            x.AbandonAll();
            return x;
        }

        public void Adopt(Equation child) =>
            Children.Add(child);

        public void Abandon(Equation child) =>
            Children.Remove(child);

        public void AbandonAll() =>
            Children = new List<Equation>();

        public bool HasChildren()
        {
            if (Children.Exists())
            {
                return Children.Count > 0;
            }
            return false;
        }
        public string Index() =>
            HasParent() ? $"[{Layer}, {Position}]" : "root";

        public bool HasValue() =>
            Value == "" || Value == null;

        public void Assign(string s) =>
            Value = s;

        public void Conjoin(string s) =>
            Value += s;

        public List<Equation> ToList()
        {
            List<Equation> r = new List<Equation>();
            Equation n = this;
            Equation p = new Equation();
            r.Add(n);
            sort:
            if (n.HasChildren())
            {
                foreach (Equation c in n.Children)
                {
                    r.Add(c);
                    if (c.HasChildren())
                    {
                        p = n;
                        n = c;
                        goto sort;
                    }
                    else
                    {
                        if (n.Children.Count != c.Position + 1) continue;
                        p.Abandon(n);
                        n = p;
                        p = n.Parent;
                        goto sort;
                    }
                }
            }
            else
            {
                if (n.HasParent())
                {
                    n.Parent.Abandon(n);
                    p = n.Parent;
                    n = p;
                    goto sort;
                }
            }
            return r;
        }

        public override string ToString() =>
            $"{Index()} | {Value ?? "null"}";

        public List<string> ToSummary()
        {
            List<string> r = new List<string>();
            List<Equation> l = ToList();
            foreach(Equation e in l) r.Add(e.ToString());
            return r;
        }

        public string Summarize() =>
            string.Join('\n', this.ToSummary());
    }
}