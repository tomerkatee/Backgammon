using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20
{
    public class Cubes
    {
        Cube cube1;
        Cube cube2;
        public Cubes(float centerX, float centerY, Color squareColor, Color numberColor)
        {
            cube1 = new Cube(6, centerX - 300, centerY, 120, 120, Color.Red, Color.White);
            cube2 = new Cube(6, centerX + 300, centerY, 120, 120, Color.Red, Color.White);

        }
        public void DrawCubes(Canvas canvas)
        {

            cube1.DrawCube(canvas);
            cube2.DrawCube(canvas);
        }
        public void SetNumbers((int, int) nums)
        {
            cube1.CurrentNumber = nums.Item1;
            cube2.CurrentNumber = nums.Item2;
        }
        public void ThrowCubes()
        {
            int seed = DateTime.Now.Millisecond;
            cube1.Throw(seed);
            cube2.Throw((int)Math.Sqrt(seed));
        }
        public List<int> GetMovesAvailable()
        {
            int n1 = cube1.CurrentNumber;
            int n2 = cube2.CurrentNumber;
            if (n1 == n2)
            {
                return new List<int>{ n1, n1, n1, n1 };
            }
            return new List<int> { n1, n2 };
        }
        public (int,int) CurrentNumbers
        {
            get
            {
                return (cube1.CurrentNumber, cube2.CurrentNumber);
            }
        }
        public bool TouchedCubes(float x, float y)
        {
            return cube1.Touching(x, y) || cube2.Touching(x, y);
        }
    }
}