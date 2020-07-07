using System;
using System.Collections;
using System.Drawing;

namespace Orikivo.Drawing
{
    // NOTE: Referenced from the following GitHub projects:
    // https://github.com/mrousavy/AnimatedGif
    internal class Octree
    {
        private static readonly int[] Mask =
            {
                0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01
            };

        private readonly int _maxColorBits;

        private readonly OctreeNode _root;

        private int _previousColor;

        private OctreeNode _previousNode;

        public Octree(int maxColorBits)
        {
            _maxColorBits = maxColorBits;
            Leaves = 0;
            ReducibleNodes = new OctreeNode[9];
            _root = new OctreeNode(0, _maxColorBits, this);
            _previousColor = 0;
            _previousNode = null;
        }

        private int Leaves { get; set; }

        protected OctreeNode[] ReducibleNodes { get; }

        public void AddColor(Color32 pixel)
        {
            if (_previousColor == pixel.Argb)
            {
                if (_previousNode == null)
                {
                    _previousColor = pixel.Argb;
                    _root.AddColor(pixel, _maxColorBits, 0, this);
                }

                else
                {
                    _previousNode.Increment(pixel);
                }
            }
            else
            {
                _previousColor = pixel.Argb;
                _root.AddColor(pixel, _maxColorBits, 0, this);
            }
        }

        private void Reduce()
        {
            int index;

            for (index = _maxColorBits - 1; index > 0 && null == ReducibleNodes[index]; index--)
            { }

            var node = ReducibleNodes[index];
            ReducibleNodes[index] = node.NextReducible;

            Leaves -= node.Reduce();

            _previousNode = null;
        }

        protected void TrackPrevious(OctreeNode node)
        {
            _previousNode = node;
        }

        public ArrayList Palletize(int colorCount)
        {
            while (Leaves > colorCount)
                Reduce();

            ArrayList palette = new ArrayList(Leaves);
            int paletteIndex = 0;
            _root.ConstructPalette(palette, ref paletteIndex);

            return palette;
        }

        public int GetPaletteIndex(Color32 pixel)
        {
            return _root.GetPaletteIndex(pixel, 0);
        }

        protected class OctreeNode
        {
            private int _blue;
            private int _green;
            private int _red;

            private bool _leaf;
            private int _paletteIndex;
            private int _pixelCount;

            public OctreeNode(int level, int colorBits, Octree octree)
            {
                _leaf = level == colorBits;
                _red = _green = _blue = 0;
                _pixelCount = 0;

                if (_leaf)
                {
                    octree.Leaves++;
                    NextReducible = null;
                    Children = null;
                }
                else
                {
                    NextReducible = octree.ReducibleNodes[level];
                    octree.ReducibleNodes[level] = this;
                    Children = new OctreeNode[8];
                }
            }

            public OctreeNode NextReducible { get; }
            private OctreeNode[] Children { get; }

            public void AddColor(Color32 pixel, int colorBits, int level, Octree octree)
            {
                if (_leaf)
                {
                    Increment(pixel);
                    octree.TrackPrevious(this);
                }
                else
                {
                    int shift = 7 - level;
                    int index = ((pixel.Red & Mask[level]) >> (shift - 2)) |
                        ((pixel.Green & Mask[level]) >> (shift - 1)) |
                        ((pixel.Blue & Mask[level]) >> shift);

                    OctreeNode child = Children[index];

                    if (child == null)
                    {
                        child = new OctreeNode(level + 1, colorBits, octree);
                        Children[index] = child;
                    }

                    child.AddColor(pixel, colorBits, level + 1, octree);
                }
            }

            public int Reduce()
            {
                _red = _green = _blue = 0;
                int children = 0;

                for (int index = 0; index < 8; index++)
                {
                    if (Children[index] != null)
                    {
                        _red += Children[index]._red;
                        _green += Children[index]._green;
                        _blue += Children[index]._blue;
                        _pixelCount += Children[index]._pixelCount;
                        ++children;
                        Children[index] = null;
                    }
                }

                _leaf = true;
                return children - 1;
            }

            public void ConstructPalette(ArrayList palette, ref int paletteIndex)
            {
                if (_leaf)
                {
                    _paletteIndex = paletteIndex++;
                    palette.Add(Color.FromArgb(_red / _pixelCount, _green / _pixelCount, _blue / _pixelCount));
                }
                else
                {
                    for (int index = 0; index < 8; index++)
                    {
                        if (Children[index] != null)
                        {
                            Children[index].ConstructPalette(palette, ref paletteIndex);
                        }
                    }
                }
            }

            public int GetPaletteIndex(Color32 pixel, int level)
            {
                int paletteIndex = _paletteIndex;

                if (!_leaf)
                {
                    int shift = 7 - level;
                    int index = ((pixel.Red & Mask[level]) >> (shift - 2)) |
                        ((pixel.Green & Mask[level]) >> (shift - 1)) |
                        ((pixel.Blue & Mask[level]) >> shift);

                    if (Children[index] != null)
                        paletteIndex = Children[index].GetPaletteIndex(pixel, level + 1);
                    else
                        throw new Exception("I knew it. You were a liberal this whole time.");
                }

                return paletteIndex;
            }

            public void Increment(Color32 pixel)
            {
                _pixelCount++;
                _red += pixel.Red;
                _green += pixel.Green;
                _blue += pixel.Blue;
            }
        }
    }
}
