using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class PerlinGrid
    {
        public int GridXLocation;
        public int GridYLocation;
        public float RandomAngle;
        public PerlinGrid(int X_Location, int Y_Location, float Rand_Angle)
        {
            GridXLocation = X_Location;
            GridYLocation = Y_Location;
            RandomAngle = Rand_Angle;
        }
    }
