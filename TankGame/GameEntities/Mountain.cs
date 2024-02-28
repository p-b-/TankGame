//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TankGame.Engine;

//namespace TankGame.GameEntities
//{
//    internal class Mountain : TwoDimensionalEntity
//    {
//        internal void SetColour(Color c)
//        {
//            this.LineColour = c;
//        }
//        internal Mountain(int mountainTypeIndex, double rotation, double scale) : base(rotation, false, scale)
//        {
//            switch (mountainTypeIndex)
//            {
//                case 0:
//                    AddOutlinePoint(new Point(-21, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(21, 0));
//                    AddOutlinePoint(new Point(-12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//                case 1:
//                    AddOutlinePoint(new Point(-21, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(21, 0));
//                    AddOutlinePoint(new Point(12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//                case 2:
//                    AddOutlinePoint(new Point(-42, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(21, 0));
//                    AddOutlinePoint(new Point(12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//                case 3:
//                    AddOutlinePoint(new Point(-42, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(21, 0));
//                    AddOutlinePoint(new Point(12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//                case 4:
//                    AddOutlinePoint(new Point(-21, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(42, 0));
//                    AddOutlinePoint(new Point(-12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//                case 5:
//                    AddOutlinePoint(new Point(-21, 0));
//                    AddOutlinePoint(new Point(0, 30));
//                    AddOutlinePoint(new Point(42, 0));
//                    AddOutlinePoint(new Point(12, 0));
//                    AddTriangle(0, 1, 3);
//                    AddTriangle(3, 1, 2);
//                    break;
//            }
//        }
//    }
//}
