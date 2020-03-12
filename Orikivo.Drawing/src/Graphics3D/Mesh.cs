using System.Collections.Generic;

namespace Orikivo.Drawing.Graphics3D
{
    public struct Mesh
    {
        public static Mesh Cube
        {
            get
            {
                Vector3 zzz = new Vector3(0, 0, 0); // p1
                Vector3 zoz = new Vector3(0, 1, 0); // p2
                Vector3 ooz = new Vector3(1, 1, 0); // p3
                Vector3 ozz = new Vector3(1, 0, 0); // p4

                Vector3 zzo = new Vector3(0, 0, 1); // p5
                Vector3 zoo = new Vector3(0, 1, 1); // p6
                Vector3 ooo = new Vector3(1, 1, 1); // p7
                Vector3 ozo = new Vector3(1, 0, 1); //p8

                Vector3[] southUpper = { zzz, zoz, ooz };
                Vector3[] southLower = { zzz, ooz, ozz };

                Vector3[] eastUpper = { ozz, ooz, ooo };
                Vector3[] eastLower = { ozz, ooo, ozo };

                Vector3[] northUpper = { ozo, ooo, zoo };
                Vector3[] northLower = { ozo, zoo, zzo };

                Vector3[] westUpper = { zzo, zoo, zoz };
                Vector3[] westLower = { zzo, zoz, zzz };

                Vector3[] topUpper = { zoz, zoo, ooo };
                Vector3[] topLower = { zoz, ooo, ooz };

                Vector3[] bottomUpper = { ozo, zzo, zzz };
                Vector3[] bottomLower = { ozo, zzz, ozz };

                // 000 010 110
                // 000 110 100
                
                // 100 110 111
                // 100 111 101
                
                // 101 111 011
                // 101 011 001

                // 001 011 010
                // 001 010 000

                // 010 011 111
                // 010 111 110

                // 101 001 000
                // 101 000 100


                Triangle[] triangles = new Triangle[]
                        {
                        new Triangle(southUpper), new Triangle(southLower),
                        new Triangle(eastUpper), new Triangle(eastLower),
                        new Triangle(northUpper), new Triangle(northLower),
                        new Triangle(westUpper), new Triangle(westLower),
                        new Triangle(topUpper), new Triangle(topLower),
                        new Triangle(bottomUpper), new Triangle(bottomLower)
                        };

                return new Mesh(triangles);
            }
        }

        public Mesh(Triangle[] triangles)
        {
            Triangles = new List<Triangle>(triangles);
        }

        public List<Triangle> Triangles { get; } // Vector<T>
    }
}
