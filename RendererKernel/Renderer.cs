using Cosmos.Core;
using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace _3DKernel
{
    public class Vertex {
        public Vertex(double _x, double _y) { x = _x; y = _y; }
        public Vertex() { x = 0; y = 0; }
        public double x, y;
    }

    internal class Renderer {
        public static void triangle(Canvas screen, Color color, Vertex v1, Vertex v2, Vertex v3) {
            // 28.4 fixed-point coordinates
            int Y1 = (int)Math.Round(16.0f * v1.y);
            int Y2 = (int)Math.Round(16.0f * v2.y);
            int Y3 = (int)Math.Round(16.0f * v3.y);

            int X1 = (int)Math.Round(16.0f * v1.x);
            int X2 = (int)Math.Round(16.0f * v2.x);
            int X3 = (int)Math.Round(16.0f * v3.x);

            // Deltas
            int DX12 = X1 - X2;
            int DX23 = X2 - X3;
            int DX31 = X3 - X1;

            int DY12 = Y1 - Y2;
            int DY23 = Y2 - Y3;
            int DY31 = Y3 - Y1;

            // Fixed-point deltas
            int FDX12 = DX12 << 4;
            int FDX23 = DX23 << 4;
            int FDX31 = DX31 << 4;

            int FDY12 = DY12 << 4;
            int FDY23 = DY23 << 4;
            int FDY31 = DY31 << 4;

            // Bounding rectangle
            int minx = (Math.Min(Math.Min(X1, X2), X3) + 0xF) >> 4;
            int maxx = (Math.Max(Math.Max(X1, X2), X3) + 0xF) >> 4;
            int miny = (Math.Min(Math.Min(Y1, Y2), Y3) + 0xF) >> 4;
            int maxy = (Math.Max(Math.Max(Y1, Y2), Y3) + 0xF) >> 4;

            // Half-edge constants
            int C1 = DY12 * X1 - DX12 * Y1;
            int C2 = DY23 * X2 - DX23 * Y2;
            int C3 = DY31 * X3 - DX31 * Y3;

            // Correct for fill convention
            if(DY12< 0 || (DY12 == 0 && DX12 > 0)) C1++;
            if(DY23< 0 || (DY23 == 0 && DX23 > 0)) C2++;
            if(DY31< 0 || (DY31 == 0 && DX31 > 0)) C3++;

            int CY1 = C1 + DX12 * (miny << 4) - DY12 * (minx << 4);
            int CY2 = C2 + DX23 * (miny << 4) - DY23 * (minx << 4);
            int CY3 = C3 + DX31 * (miny << 4) - DY31 * (minx << 4);

            for(int y = miny; y<maxy; y++) {
                int CX1 = CY1;
                int CX2 = CY2;
                int CX3 = CY3;
   
                for(int x = minx; x<maxx; x++) {
                    if(CX1 > 0 && CX2 > 0 && CX3 > 0) {
                        screen.DrawPoint(color,x,y);
                    }
                    CX1 -= FDY12;
                    CX2 -= FDY23;
                    CX3 -= FDY31;
                }
            CY1 += FDX12;
            CY2 += FDX23;
            CY3 += FDX31;
            }
        }

        public static void isoCube(Canvas screen, Color color, int x, int y, int size) {
            // Corners
            Vertex center = new Vertex(x,y);
            Vertex ct = new Vertex(x,y-size);
            Vertex cb = new Vertex(x,y+size);
            Vertex rb = new Vertex(x+size,y+size/2);
            Vertex rt = new Vertex(x+size,y-size/2);
            Vertex lb = new Vertex(x-size,y+size/2);
            Vertex lt = new Vertex(x-size,y-size/2);

            // Colors
            Color ori_col = color;
            Color da_col = Color.FromArgb(color.ToArgb() - 0x101010);
            Color sh_col = Color.FromArgb(color.ToArgb() - 0x303030);

            // Draw inner half
            triangle(screen, ori_col, center, cb, rt);
            triangle(screen, da_col, center, rt, lt);
            triangle(screen, sh_col, center, lt, cb);

            // Draw outer part
            triangle(screen, ori_col, cb, rb, rt);
            triangle(screen, da_col, rt, ct, lt);
            triangle(screen, sh_col, lt, lb, cb);
        }
    }

    public class Point3D {
        public double x, y, z;
        public double u, v;

        public Point3D(int _x, int _y, int _z) { x = _x; y = _y; z = _z; toScreen(); }

        private void toScreen() { u = (x / z); v = (y / z); }
    }

    public class Renderer3D {
        double zFar = 100.0, zNear = 0.01;
        private int[] per_proj_mat = {
            2/1280, 0     , 0, -1,
            0     , 2/-768, 0, -1,
            0     , 0     , 
        };
        public static void cube(Canvas screen, int x, int y, int z, int size) {
            // Setup Points; pb = point bottom, pt = point top
            Point3D pb1 = new Point3D(x       , y       , z + size);
            Point3D pb2 = new Point3D(x + size, y       , z + size);
            Point3D pb3 = new Point3D(x       , y + size, z + size);
            Point3D pb4 = new Point3D(x + size, y + size, z + size);
            Point3D pt1 = new Point3D(x       , y       , z);
            Point3D pt2 = new Point3D(x + size, y       , z);
            Point3D pt3 = new Point3D(x       , y + size, z);
            Point3D pt4 = new Point3D(x + size, y + size, z);

            // Draw the Points
            screen.DrawCircle(Color.White, (int)pb1.u, (int)pb1.v, 10);
            screen.DrawCircle(Color.White, (int)pb2.u, (int)pb2.v, 10);
            screen.DrawCircle(Color.White, (int)pb3.u, (int)pb3.v, 10);
            screen.DrawCircle(Color.White, (int)pb4.u, (int)pb4.v, 10);
            // Draw the Lines
        }
    }
}
