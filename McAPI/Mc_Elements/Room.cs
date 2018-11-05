using Autodesk.Revit.DB;
using Mc_Util;
using MiracadAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rvt = Autodesk.Revit.DB;

namespace McAPI.Mc_Elements
{
    public class Room
    {
        // Получение кривой из BoundarySegment помещения
        private static Curve GetCurve(BoundarySegment boundarySegment)
        {
            Curve curve = boundarySegment.GetCurve().Clone();
            return curve;
        }

        /// <summary>
        /// Получаем список кривых и элементов контура помещения смещенных на расстояние в виде списка Mc_Class
        /// </summary>
        /// <param name="room">Помещение</param>
        /// <param name="offset">Смещение в мм</param>
        /// <returns>Возвращается список объектов Mc_Class</returns>
        public static List<Mc_Class> BoundarySegmentsOffset(Autodesk.Revit.DB.Architecture.Room room, double offset)
        {
            SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();
            //   opt.StoreFreeBoundaryFaces = true;
            opt.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;

            IList<IList<BoundarySegment>> loops
                      = room.GetBoundarySegments(opt);
            double shortsegment = room.Document.Application.ShortCurveTolerance;

            List<Mc_Class> lst_mcclass = new List<Mc_Class>();

            CurveLoop CloopBase = null;
            foreach (IList<BoundarySegment> loop in loops)
            {
                CurveLoop curveloop = loop.Select(x => GetCurve(x)).ToList().ToCurveloop(shortsegment);

                if (curveloop == null)
                {
                    Solid solroom = room.ClosedShell.Where(x => x is Solid).FirstOrDefault() as Solid;
                    if (solroom != null)
                        curveloop = solroom.GetFaces(SubfaceType.Bottom)[0].GetEdgesAsCurveLoops()[0]
                            .Approximation(shortsegment);
                }

                if (curveloop != null)
                {
                    if (CloopBase == null)
                    {
                        curveloop = curveloop.Offset(offset.rvt_ToFeet());
                        CloopBase = curveloop;
                    }
                    else
                    {
                        curveloop = curveloop.Offset(-offset.rvt_ToFeet());
                    }

                    if (curveloop == null)
                        throw new Exception("Не удалось создать контур!");


                    List<Curve> lst_curvesOffset = curveloop.ToCurves().Where(x => x.Length > shortsegment).ToList();
                    foreach (BoundarySegment seg in loop)
                    {
                        Curve cur = GetCurve(seg);
                        if (cur.Length >= shortsegment)
                        {
                            double angle = cur.GetAngle();
                            Line line = Line.CreateBound(
                                cur.MidPoint().PolarP2(angle, (Math.Abs(offset) + 1.0).rvt_ToFeet()),
                                cur.MidPoint().PolarP5(angle, (Math.Abs(offset) + 1.0).rvt_ToFeet()));

#if(Ver15)
                                ElementLink elink = new ElementLink(seg.Element, null);
                                Curve curve_offset = null;
                                if ((curve_offset = GetIntersectWith(room.Document, line, lst_curvesOffset)) != null)
                                {
                                    if (elink.Element != null)
                                    {
                                        Mc_Class mcclass = new Mc_Class(UniqueId());
                                        mcclass.Mc_Prop_1 = elink.ToIdLink();
                                        mcclass.Mc_Obj_1 = curve_offset;
                                        lst_mcclass.Add(mcclass);
                                    }
                                }
#else

                            if (seg.ElementId != ElementId.InvalidElementId)
                            {
                                ElementLink elink = null;
                                RevitLinkInstance rlink = room.Document.GetElement(seg.ElementId) as RevitLinkInstance;
                                if (rlink is RevitLinkInstance)
                                {
                                    Element host = rlink.GetLinkDocument().GetElement(seg.LinkElementId);
                                    elink = new ElementLink(host, rlink);
                                }
                                else
                                {
                                    elink = new ElementLink(room.Document.GetElement(seg.ElementId), null);

                                    Curve curve_offset = null;
                                    if ((curve_offset = GetIntersectWith(room.Document, line, lst_curvesOffset)) != null)
                                    {
                                        if (elink.Element != null)
                                        {

                                            Mc_Class mcclass = new Mc_Class(UniqueId());
                                            mcclass.Mc_Prop_1 = elink.ToIdLink();
                                            mcclass.Mc_Obj_1 = curve_offset;
                                            lst_mcclass.Add(mcclass);
                                        }
                                    }
                                }

                            }
#endif
                        }

                    }

                }
            }
            //   Mc_Windows.MsgBox(lst_mcclass.Select(x => x.Mc_Prop_1).ToList().JoinList());
            return lst_mcclass;
            //.Select(x => x as object).ToList();
        }

        // Создаем уникальный ID для Mc_Class
        private static string UniqueId()
        {
            Guid guid = Guid.NewGuid();
            byte[] gb = guid.ToByteArray();
            int i = Math.Abs(BitConverter.ToInt32(gb, 0));
            return i.ToString("D10");
        }
        static Curve GetIntersectWith(Document doc, Curve cur, List<Curve> lst_curves)
        {
            IntersectionResultArray resArray = new IntersectionResultArray();
            foreach (Curve cur2 in lst_curves)
            {

                cur.Intersect(cur2, out resArray);
                if (null != resArray)
                {
                    /*
                    using (Transaction trans = new Transaction(doc, "Trans"))
                    {
                        trans.Start();
                        ModelCurve mc = doc.Create.NewModelCurve(cur, doc.ActiveView.SketchPlane);
                        trans.Commit();
                    }
                     */
                    return cur2;
                }
            }
            return null;
        }

        /// <summary>
        /// Создать помещение по указанной точке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static rvt.Architecture.Room Create(XYZ point)
        {
            Autodesk.Revit.DB.Document doc = Mc_Doc.ActiveDocument;
            Level lev = doc.GetLevelByZ(point.Z);
            try
            {
                return doc.Create.NewRoom(lev, new UV(point.X, point.Y));
            }
            catch { return null; }
        }

        /// <summary>
        /// Создать помещения на указанном уровне
        /// </summary>
        /// <param name="level">Уровень, на котором необходимо создать помещения</param>
        /// <returns></returns>
        public static List<rvt.Architecture.Room> Create(Level level)
        {
            Autodesk.Revit.DB.Document doc = Mc_Doc.ActiveDocument;
            try
            {
                return doc.Create.NewRooms2(level).ToList().ToElements(doc).Cast<rvt.Architecture.Room>().ToList();
            }
            catch { return new List<rvt.Architecture.Room>(); }
        }

    }
}
