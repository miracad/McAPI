using Mc_Util;
using MiracadAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McAPI.McSystem.Drawing
{
    public class Bitmap
    {
        private static int _rnd_int;


        /// <summary>
        /// Возвращает список Mc_Class, с заполненным списком Mc_Lst - строка из пикселей
        /// каждый объект Mc_Class содержит в свойстве 
        /// Mc_Prop1 = цвет в виде строки (Color)111111
        /// Mc_Prop_2 = Name;
        ///Mc_Prop_3 = RGB (152;123;0)
        ///Mc_Prop_4 = A (Alfa-channel)
        ///Mc_Prop_5 = ToArgb (int)
        /// </summary>
        /// <param name="path_file">Укажите путь к файлу картинки</param>
        /// <returns>List_Mc_Class</returns>
        public static List<Mc_Class> GetPixelsToMcLstClass_ID_Name_RGB_A_ToArgb(string path_file)
        {
            System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage(new Uri(path_file));

            System.Drawing.Bitmap img = (System.Drawing.Bitmap)Image.FromFile(path_file);
            //new System.Drawing.Bitmap(path_file);
            List<Mc_Class> lst_color = new List<Mc_Class>();
            _rnd_int = Mc_Convert.Random();
            for (int i = 0; i < img.Width; i++)
            {
                Mc_Class mc_class = new Mc_Class(_rnd_int.ToString());
                _rnd_int++;

                for (int j = 0; j < img.Height; j++)
                {
                    Mc_Class mc_classY = new Mc_Class(_rnd_int.ToString());
                    _rnd_int++;
                    Color pixel = img.GetPixel(i, j);
                    //ColorTranslator.
                    mc_classY.Mc_Prop_1 = pixel.ToIdColor();
                    mc_classY.Mc_Prop_2 = pixel.Name;
                    mc_classY.Mc_Prop_3 = Convert.ToInt16(pixel.R).ToString() + ";" + Convert.ToInt16(pixel.G).ToString()
                        + ";" + Convert.ToInt16(pixel.B).ToString();
                    mc_classY.Mc_Prop_4 = pixel.A.ToString();
                    mc_classY.Mc_Prop_5 = pixel.ToArgb().ToString();
                    mc_classY.Mc_Obj_1 = pixel;
                    mc_class.Mc_List.Add(mc_classY);
                }
                lst_color.Add(mc_class);
            }
            //  Mc_Windows.MsgBox(lst_color.Count.ToString());
            return lst_color;
        }

        public static string GetPixelsSize(string path_file)
        {
            System.Drawing.Bitmap img = (System.Drawing.Bitmap)Image.FromFile(path_file);
            Size size = img.Size;
            if (size.IsEmpty)
                return size.Width.ToString() + ":" + size.Height.ToString();
            else
                return "0:0";
        }

        /// <summary>
        /// Возвращает список Mc_Class, 
        /// каждый объект Mc_Class содержит в свойстве 
        /// Mc_Prop1 = Позиция цвета пикселя (12:15)
        /// Mc_Prop2 = цвет в виде строки (Color)111111
        /// Mc_Prop_3 = Name;
        ///Mc_Prop_4 = RGB (152;123;0)
        ///Mc_Prop_5 = A (Alfa-channel)
        ///Mc_Prop_6 = ToArgb (int)
        /// </summary>
        /// <param name="path_file">Укажите путь к файлу картинки</param>
        /// <returns>List_Mc_Class</returns>
        public static List<Mc_Class> GetPixelsToMcClass_Range_ID_Name_RGB_A_ToArgb(string path_file)
        {
            System.Drawing.Bitmap img = (System.Drawing.Bitmap)Image.FromFile(path_file);
            //  System.Drawing.Bitmap img = new System.Drawing.Bitmap(path_file);
            List<Mc_Class> lst_color = new List<Mc_Class>();
            _rnd_int = Mc_Convert.Random();
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Mc_Class mc_class = new Mc_Class(_rnd_int.ToString());
                    _rnd_int++;
                    Color pixel = img.GetPixel(i, j);
                    mc_class.Mc_Prop_1 = i.ToString() + ":" + j.ToString();
                    mc_class.Mc_Prop_2 = pixel.ToIdColor();
                    mc_class.Mc_Prop_3 = pixel.Name;
                    mc_class.Mc_Prop_4 = pixel.R.ToString() + ";" + pixel.G.ToString() + ";" + pixel.B.ToString();
                    mc_class.Mc_Prop_5 = pixel.A.ToString();
                    mc_class.Mc_Prop_6 = pixel.ToArgb().ToString();
                    mc_class.Mc_Obj_1 = pixel;
                    lst_color.Add(mc_class);
                }

            }
            //  Mc_Windows.MsgBox(lst_color.Count.ToString());
            return lst_color;
        }

        /// <summary>
        /// Возвращает список Mc_Class в заданном диапазоне, указав начальное значение по x и y и указав кол-во, 
        /// каждый объект Mc_Class содержит в свойстве 
        /// Mc_Prop1 = Позиция цвета пикселя (12:15)
        /// Mc_Prop2 = цвет в виде строки (Color)111111
        /// Mc_Prop_3 = Name;
        /// Mc_Prop_4 = RGB (152;123;0)
        /// Mc_Prop_5 = A (Alfa-channel)
        /// Mc_Prop_6 = ToArgb (int)
        /// </summary>
        /// <param name="path_file">Путь к файлу картинки</param>
        /// <param name="x_start">Начальный диапозон считывания пикселей по X</param>
        /// <param name="y_start">Начальный диапозон считывания пикселей по Y</param>
        /// <param name="x_length">Конечный диапозон считывания пикселей по X</param>
        /// <param name="y_length">Конечный диапозон считывания пикселей по Y</param>
        /// <returns></returns>
        public static List<Mc_Class> GetPixelsRangeToMcClass_Range_ID_Name_RGB_A_ToArgb
            (string path_file, int x_start, int y_start, int x_length, int y_length)
        {
            System.Drawing.Bitmap img = (System.Drawing.Bitmap)Image.FromFile(path_file);
            //  System.Drawing.Bitmap img = new System.Drawing.Bitmap(path_file);
            List<Mc_Class> lst_color = new List<Mc_Class>();
            _rnd_int = Mc_Convert.Random();
            int x_end = x_start + x_length;
            int y_end = y_start + y_length;
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    if (x_start >= i && i < x_end && y_start >= j && j < y_end)
                    {
                        Mc_Class mc_class = new Mc_Class(_rnd_int.ToString());
                        _rnd_int++;
                        Color pixel = img.GetPixel(i, j);
                        mc_class.Mc_Prop_1 = i.ToString() + ":" + j.ToString();
                        mc_class.Mc_Prop_2 = pixel.ToIdColor();
                        mc_class.Mc_Prop_3 = pixel.Name;
                        mc_class.Mc_Prop_4 = pixel.R.ToString() + ";" + pixel.G.ToString() + ";" + pixel.B.ToString();
                        mc_class.Mc_Prop_5 = pixel.A.ToString();
                        mc_class.Mc_Prop_6 = pixel.ToArgb().ToString();
                        mc_class.Mc_Obj_1 = pixel;
                        lst_color.Add(mc_class);
                    }
                }
            }
            //  Mc_Windows.MsgBox(lst_color.Count.ToString());
            return lst_color;
        }
    }
}
