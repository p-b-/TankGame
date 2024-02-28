using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Maths
{
    internal struct Angle
    {
        internal double Value { get; private set; }

        internal double Cos
        {
            get
            {
                return Math.Cos(Value);
            }
        }
        internal double Sin
        {
            get
            {
                return Math.Sin(Value);
            }
        }

        internal Angle(double value) 
        {
            Value = value;
        }

        public static implicit operator Angle(double rhs)
        {
            return new Angle(rhs);
        }


        public static Angle operator +(Angle a, Angle b)
        {
            double newAngle = a.Value + b.Value;
            if (newAngle > Math.PI * 2)
            {
                newAngle -= Math.PI * 2;
            }
            else if (newAngle < 0)
            {
                newAngle += Math.PI * 2;
            }
            return new Angle(newAngle);
        }

        internal Angle Inverted()
        {
            return new Angle(-Value);
        }

        public override string ToString()
        {
            return $"{180*Value/Math.PI}";
        }
    }
}
