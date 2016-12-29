using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


//Bibliography:
//http://tomswitzer.net/2009/12/jarvis-march/ 


// DEBUG
// am scris "DEBUG" langa liniile de cod pe care le-am folosit sa afisez chestii pe ecran

//VISUAL TIP  --- in cazul in care alegem sa coloram acoperirea pe masura ce functioneaza algoritmul
//--- daca alegem doar sa-l coloram la final, muchiile acoperirii sunt in ordinea in care ele sunt asezate in vector
//am scris "VISUAL TIP" in locurile unde, dupa ochi, imi pare ca ar trebui introduse colorarile vizuale
// Legenda Culorilor : 'final color' - o culoare care sa reprezinte ca punctul a fost adaugat definitiv in acoperire
//                   : 'intermediary color' - o culoare care sa reprezinte ca punctul este potential pentru acoperire
// Culorile sunt la libera alegere, singura idee e sa fie diferite una de cealalta

namespace Geometry
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point() { }
        public Point(double x, double y) { X = x; Y = y; }

        public override string ToString()
        {
            return X + "|" + Y;
        }

        /* Compute line equation of A1 and A2, in the current point 
         |x  y  1|
         |x1 y1 1|
         |x2 y2 1|

        - sign of the returned value also gives the point position on the line :
            if returned < 0 : point is on the right of line
            if returned > 0 : point is on the left of line
            if returned ==0 : point is on the line 
        */
        public double Line_eq(Point A1, Point A2)
        {
            return X * (A1.Y - A2.Y) + Y * (A2.X - A1.X) + A1.X * A2.Y - A2.X * A1.Y;
        }

        //distance to a point by the formula
        //d(A1,A2) = rad( (x1-x2)^2 - (y1-y2)^2 )
        public double DistTo(Point A)
        {
            return Math.Sqrt(Math.Pow(this.X - A.X, 2) + Math.Pow(this.Y - A.Y, 2));
        }

    }
    public static class JarvisMarch
    {
        public static Point[] ConvexHull(Point[] poligon)
        {
            //the convex hull to be returned
            //we keep it in a list as to add points to it on the run
            // (we don't know the size of the hull at the beginning) 
            List<Point> hull = new List<Point>();

            if (poligon.Length >= 3) // we need at least three points for the algorithm    
            {
                //Traverse the poligon to find the leftmost point, which will be our starting point
                // O(n)
                int leftmost = 0;
                for (int i = 1; i < poligon.Length; ++i)
                    if (poligon[i].X < poligon[leftmost].X)
                        leftmost = i;
                // Now we have found the starting point


                int current = leftmost;  //the current point will always be considered to be the leftmost point 
                                         // the ones left to it were already added to the hull

                int next = 0; // next point to add in hull will be the rightmost to the current point 
                              //   (cel mai la dreapta fata de punctul curent )

                //Go counterclockwise until we reach the starting point again,
                do// O(h) , h - number of points in convex hull
                {
                   // Console.WriteLine("Added : " + poligon[current]); //DEBUG

                    // Add current point to hull
                    hull.Add(poligon[current]);
                    
                    // VISUAL TIP : hull[hull.Count - 1] gets connected to hull[hull.Count -2 ] - final color

                    //We choose the next point to add in hull :

                    //initially the next point in the poligon
                    next = (current + 1) % poligon.Length;    //we use modulo n to get back to 0 if "current == length - 1"

                   // Console.WriteLine("Next is : " + poligon[next]); //DEBUG

                    //traverse all the points to find the next one to add in hull
                    for (int i = 0; i < poligon.Length; ++i)
                    {

                        //VISUAL TIP: color the line poligon[current] - poligon[next] - intermediary color

                        if (i == current || i == next)  //unecessary to check for these
                            continue;

                        //If P[i] is to the right of the edge Current-Next, we update Next
                        //So if there is a point more to the right of Current than Next currently is, we update Next
                        //Thus, at each step, we will always choose the rightmost point to the current one

                        double direction = poligon[i].Line_eq(poligon[current], poligon[next]);

                        // if P[i] is to the right of line Current-Next, update next
                        if (direction < 0)
                            next = i;

                        //if P[i] is on the line Current-Next
                        //and if Next is on the segment Current--P[i], update next
                        if (direction == 0 && poligon[current].DistTo(poligon[i]) > poligon[current].DistTo(poligon[next]))
                            next = i;

                    }

                    //Console.WriteLine("Next gets : " + poligon[next]); //DEBUG

                    
                    //foreach (Point p in hull) //DEBUG
                    //    Console.Write(p + "  ");
                    //Console.Write("\n\n");

                    current = next; // we update the current point to add it in the hull

                } while (current != leftmost);  // When we reach leftmost, we reached the starting point, so we finished
            }

            return hull.ToArray();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                StreamReader fin = new StreamReader("input.txt");
                string dimLine = fin.ReadLine();
                Console.WriteLine(dimLine);
                if( int.TryParse(dimLine, out int dim) )
                {
                    Point[] poligon = new Point[dim];
                    for (int i = 0; i < dim; ++i)
                    {
                        string pointLine = fin.ReadLine();                        
                        double[] coords = pointLine.Split(' ').Select(nr => double.Parse(nr)).ToArray();
                        poligon[i] = new Point(coords[0], coords[1]);
                        Console.Write(poligon[i]+" ");
                    }
                    Console.WriteLine();
                    //aici apelam functia Deniiiiis !!!! 
                    Console.WriteLine("\nAcoperirea convexa este :");
                    Point[] hull = JarvisMarch.ConvexHull(poligon);
                    foreach (Point p in hull)
                        Console.Write(p+" ");
                    Console.WriteLine();
                }




            }
            //thrown by StreamReader
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //thrown by double.Parse
            catch (ArgumentNullException ex)    //if passed string is null
            {
                Console.WriteLine(ex.Message);
            }
            catch (OverflowException ex)   // if argument is smaller than double.MinValue or bigger than double.MaxValue
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)       // if passed string is not the format of a double
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
