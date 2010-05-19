using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;

namespace CoverArt
{
    public class QuadDistort
    {
        const double PIOver2 = Math.PI / 2.0;

        private struct Vector
        {
            public PointF Origin;
            public float Direction;

            public Vector(PointF origin, float direction)
            {
                this.Origin = origin;
                this.Direction = direction;
            }
        }

        public static Bitmap Distort(Bitmap sourceBitmap, Point topleft, Point topright, Point bottomleft, Point bottomright)
        {
            return Distort(sourceBitmap, topleft, topright, bottomleft, bottomright, 2);
        }

        public static Bitmap Distort(Bitmap sourceBitmap, Point topleft, Point topright, Point bottomleft, Point bottomright, int interpolation)
        {
            double sourceWidth = sourceBitmap.Width;
            double sourceHeight = sourceBitmap.Height;

            //Find dimensions of new image
            Point[] pointarray = new Point[] { topleft, topright, bottomright, bottomleft };

            int width = int.MinValue;
            int height = int.MinValue;

            foreach (Point p in pointarray)
            {
                width = Math.Max(width, p.X);
                height = Math.Max(height, p.Y);
            }

            Bitmap bitmap = new Bitmap(width, height);

            //For faster image processing
            BitmapProcessing.FastBitmap newBmp = new BitmapProcessing.FastBitmap(bitmap);
            BitmapProcessing.FastBitmap sourceBmp = new BitmapProcessing.FastBitmap(sourceBitmap);

            newBmp.LockImage();
            sourceBmp.LockImage();

            //Key points
            PointF A = (PointF)topleft;
            PointF B = (PointF)topright;
            PointF C = (PointF)bottomright;
            PointF D = (PointF)bottomleft;

            // sides
            float mAB = GetAngle(A, B);
            float mCD = GetAngle(C, D);
            float mAD = GetAngle(A, D);
            float mBC = GetAngle(B, C);

            //Get corner intersections
            PointF O = GetIntersection(new Vector(B, mAB), new Vector(C, mCD));
            PointF N = GetIntersection(new Vector(A, mAD), new Vector(B, mBC));

            if (interpolation <= 0) interpolation = 1;
            int middleX = (int)(interpolation / 2.0);

            //Array of surronding pixels used for interpolation
            double[, ,] source = new double[interpolation, interpolation, 4];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //System.Windows.Forms.Application.DoEvents();

                    PointF P = new PointF(x, y);

                    float mPO = mAB; //Default value
                    float mPN = mBC;

                    if (O != PointF.Empty) //If intersection found, get coefficient
                        mPO = GetAngle(O, P);

                    if (N != PointF.Empty) //If intersection found, get coefficient
                        mPN = GetAngle(N, P);

                    //Get intersections
                    PointF L = GetIntersection(new Vector(P, mPO), new Vector(A, mAD));                    
                    if (L == PointF.Empty) L = A;

                    PointF M = GetIntersection(new Vector(P, mPO), new Vector(C, mBC));                    
                    if (M == PointF.Empty) M = C;

                    PointF J = GetIntersection(new Vector(P, mPN), new Vector(B, mAB));
                    if (J == PointF.Empty) J = B;

                    PointF K = GetIntersection(new Vector(P, mPN), new Vector(D, mCD));
                    if (K == PointF.Empty) K = D;

                    double dJP = GetDistance(J, P);
                    double dLP = GetDistance(L, P);

                    double dJK = GetDistance(J, K);
                    double dLM = GetDistance(L, M);

                    //set direction
                    if (dLM < GetDistance(M, P)) dLP = -dLP;
                    if (dJK < GetDistance(K, P)) dJP = -dJP;

                    ////interpolation

                    //find the pixels which surround the point
                    double yP0 = sourceHeight * dJP / dJK;
                    double xP0 = sourceWidth * dLP / dLM;

                    //top left coordinates of surrounding pixels
                    if (xP0 < 0) xP0--;
                    if (yP0 < 0) yP0--;

                    int left = (int)xP0;
                    int top = (int)yP0;

                    if ((left < -1 || left > sourceWidth) && (top < -1 || top > sourceHeight))
                    {
                        //if outside of source image just move on
                        continue;
                    }

                    //weights
                    double xFrac = xP0 - (double)left;
                    double xFracRec = 1.0 - xFrac;
                    double yFrac = yP0 - (double)top;
                    double yFracRec = 1.0 - yFrac;

                    //get source pixel colors, or white if out of range (to interpolate into the background color)
                    int x0;
                    int y0;
                    Color c;

                    for (int sx = 0; sx < interpolation; sx++)
                    {
                        for (int sy = 0; sy < interpolation; sy++)
                        {
                            x0 = left + sx;
                            y0 = top + sy;

                            if (x0 > 0 && y0 > 0 &&
                                x0 < sourceWidth && y0 < sourceHeight)
                            {
                                c = sourceBmp.GetPixel(x0, y0);

                                source[sx, sy, 0] = c.R;
                                source[sx, sy, 1] = c.G;
                                source[sx, sy, 2] = c.B;
                                source[sx, sy, 3] = 255.0f;
                            }
                            else
                            {
                                // set full transparency in this case
                                source[sx, sy, 0] = 0;
                                source[sx, sy, 1] = 0;
                                source[sx, sy, 2] = 0;
                                source[sx, sy, 3] = 0;
                            }
                        }
                    }

                    //interpolate on x
                    for (int sy = 0; sy < interpolation; sy++)
                    {
                        //check transparency
                        if (source[middleX, sy, 3] != 0 && source[0, sy, 3] == 0)
                        {
                            //copy colors from 1, sy
                            source[0, sy, 0] = source[1, sy, 0];
                            source[0, sy, 1] = source[1, sy, 1];
                            source[0, sy, 2] = source[1, sy, 2];
                            source[0, sy, 3] = source[1, sy, 3];
                        }
                        else
                        {
                            //compute colors by interpolation
                            source[0, sy, 0] = source[0, sy, 0] * xFracRec + source[middleX, sy, 0] * xFrac;
                            source[0, sy, 1] = source[0, sy, 1] * xFracRec + source[middleX, sy, 1] * xFrac;
                            source[0, sy, 2] = source[0, sy, 2] * xFracRec + source[middleX, sy, 2] * xFrac;
                            source[0, sy, 3] = source[0, sy, 3] * xFracRec + source[middleX, sy, 3] * xFrac;
                        }

                        //interpolate transparency
                        source[0, sy, 3] = source[0, sy, 3] * xFracRec + source[middleX, sy, 3] * xFrac;
                    }

                    //now interpolate on y

                    //check transparency
                    if (source[0, middleX, 3] != 0 && source[0, 0, 3] == 0)
                    {
                        //copy colors from 0, 1
                        source[0, 0, 0] = source[0, middleX, 0];
                        source[0, 0, 1] = source[0, middleX, 1];
                        source[0, 0, 2] = source[0, middleX, 2];
                        source[0, 0, 3] = source[0, middleX, 3];
                    }
                    else
                    {
                        source[0, 0, 0] = source[0, 0, 0] * yFracRec + source[0, middleX, 0] * yFrac;
                        source[0, 0, 1] = source[0, 0, 1] * yFracRec + source[0, middleX, 1] * yFrac;
                        source[0, 0, 2] = source[0, 0, 2] * yFracRec + source[0, middleX, 2] * yFrac;
                        source[0, 0, 3] = source[0, 0, 3] * yFracRec + source[0, middleX, 3] * yFrac;
                    }

                    //interpolate transparency
                    source[0, 0, 3] = source[0, 0, 3] * yFracRec + source[0, middleX, 3] * yFrac;

                    //store to bitmap
                    if (source[0, 0, 3] != 0) //pixel has color
                        newBmp.SetPixel(x, y, Color.FromArgb((int)source[0, 0, 3], (int)source[0, 0, 0], (int)source[0, 0, 1], (int)source[0, 0, 2]));
                }
            }

            sourceBmp.UnlockImage();
            newBmp.UnlockImage();

            return bitmap;
        }

        private static double GetDistance(PointF A, PointF B)
        {
            float a = A.X - B.X;
            float b = A.Y - B.Y;
            return Math.Sqrt((double)(a * a + b * b));
        }

        private static PointF GetIntersection(Vector vector1, Vector vector2) //Vector[] pointAngularCoeff)
        {
            if (vector1.Origin.X == vector2.Origin.X && vector1.Origin.Y == vector2.Origin.Y) 
                return vector1.Origin;

            if (vector1.Direction == vector2.Direction) return PointF.Empty; //Parallel, no intersection

            float newX = float.Epsilon;
            float newY = float.Epsilon;

            if (float.IsInfinity(vector1.Direction))
            {
                newX = vector1.Origin.X;
                newY = vector2.Origin.Y + vector2.Direction * (-vector2.Origin.X + vector1.Origin.X);
            }

            if (float.IsInfinity(vector2.Direction))
            {
                newX = vector2.Origin.X;
                newY = vector1.Origin.Y + vector1.Direction * (-vector1.Origin.X + vector2.Origin.X);
            }

            if (newX == float.Epsilon)
            {
                float q1 = vector1.Origin.Y - vector1.Direction * vector1.Origin.X;
                float q2 = vector2.Origin.Y - vector2.Direction * vector2.Origin.X;
                newX = (q1 - q2) / (vector2.Direction - vector1.Direction);
                newY = vector1.Direction * newX + q1;
            }

            if (float.IsInfinity(newX) || float.IsInfinity(newY))
                return PointF.Empty; //no intersection found
            else
            {
                return new PointF(newX, newY);
            }
        }

        private static float GetAngle(PointF from, PointF to)
        {
            double angle = GetAngleRad(from, to);
            double t = angle % Math.PI;

            if (t == PIOver2)
                return float.PositiveInfinity;
            else
            {
                if (t == -PIOver2)
                    return float.NegativeInfinity;
                else
                    return (float)Math.Tan(angle);
            }
        }

        private static double GetAngleRad(PointF from, PointF to)
        {
            if (to.Y == from.Y)
            {
                if (from.X > to.X)
                    return Math.PI;
                else
                    return 0;
            }
            else if (to.X == from.X)
            {
                if (to.Y < from.Y)
                    return -PIOver2;
                else
                    return PIOver2;
            }
            else
            {
                double m = Math.Atan(((to.Y - from.Y) / (to.X - from.X)));

                if (to.X < 0)
                    if (m > 0)
                        return m + PIOver2;
                    else
                        return m - Math.PI;
                else
                    return m;
            }
        }
    }
}