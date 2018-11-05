using Mc_Util;
using MiracadAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McAPI.McSystem.IO
{
    /// <summary>
    /// Получаем иерархические данные
    /// </summary>
    public class HierarchicalData
    {
        private static string _ImageExtension;
        private static string _ToolTipExtension;
        private static string _FileExtension;
        private static int _rnd_int;


        /// <summary>
        /// Получим иерархическое дерево папок и файлов, с картинками и описанием.
        /// </summary>
        /// <param name="rootDir">Путь директории</param>
        /// <param name="FileExtension">Искомое расширение у файлов</param>
        /// <param name="ImageExtension">Искомое расширение картинки файлов</param>
        /// <param name="ToolTipExtension">Искомое расширение для описания файлов</param>
        /// <returns>Mc_Class</returns>
        public static Mc_Class DirectoryToMcClass(string rootDir, string FileExtension, string ImageExtension, string ToolTipExtension)
        {
            // Mc_Windows.MsgBox("TYT1");
            //  Расширение файла для искомых картинок (jpg)
            _ImageExtension = "." + ImageExtension;
            // Расширение файла для искомого описания (txt)
            _ToolTipExtension = "." + ToolTipExtension;
            // Расширение искомый файлов (rfa)
            _FileExtension = "." + FileExtension;

            // Создаем список элементов-Mc_Class для записи иерархических данных
            //  List<Mc_Class> TreeView_1 = new List<Mc_Class>();
            _rnd_int = Mc_Convert.Random();
            Mc_Class TreeView_1 = new Mc_Class(_rnd_int.ToString());
            _rnd_int++;
            // Генерируем случайным числом идентификатор элемента Mc_Class.
            // Всем последующим будем прибавлять к этому значения +1
            //  Mc_Windows.MsgBox("TYT2");

            // Определяем для имени пути директории его экземпляр класса DirectoryInfo
            var rootDirectoryInfo = new DirectoryInfo(rootDir);
            // Здесь формируем иерархическое дерево
            TreeView_1 = CreateDirectoryNode(rootDirectoryInfo);
            //  Mc_Windows.MsgBox("TYT3");
            // Преобразуем список элементов класса Mc_Class в список элементов object (любого класса)
            // Для правильной идентификации программой Miracad.
            // Mc_Windows.MsgBox(TreeView_1.Mc_List[0].Mc_List.Count.ToString());
            return TreeView_1;
            //.Select(x => x as object).ToList();
        }



        static Mc_Class CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            // Получаем все папки, вложенные в текущую папку directoryInfo
            DirectoryInfo[] dirinf = directoryInfo.GetDirectories();

            // Для текущей папки создаем элемент Mc_Class
            // rnd_int - это уникальный идентификатор элемента, генерируемый случайным образом в виде числа
            Mc_Class directoryNode = new Mc_Class(_rnd_int.ToString());
            _rnd_int++;
            // В параметр Mc_Prop_1 записываем имя папки
            directoryNode.Mc_Prop_1 = directoryInfo.Name;
            // В параметр Mc_Prop_2 записываем путь к картинке
            if (File.Exists(directoryInfo.FullName + _ImageExtension))
                directoryNode.Mc_Prop_2 = directoryInfo.FullName + _ImageExtension;
            // В параметр Mc_Prop_2 записываем путь к описанию
            string path = directoryInfo.FullName + _ToolTipExtension;

            // Если по пути обнаружен файл, то находим его содержимое и записываем в параметр Mc_Prop_3
            string txt_file = "";
            if (File.Exists(path))
            {
                txt_file = System.IO.File.ReadAllText(path);// McU.TxtUtils.GetText(path);
                directoryNode.Mc_Prop_3 = txt_file;
            }
            else
                txt_file = "";

            // Полный путь к файлу записываем в параметр Mc_Prop_4
            directoryNode.Mc_Prop_4 = directoryInfo.FullName;


            foreach (var directory in dirinf)
            // Ищем в каждой вложенной папке другие папки и файлы рекурсией
            {

                directoryNode.Mc_List.Add(CreateDirectoryNode(directory));
            }

            foreach (var file in directoryInfo.GetFiles())
            // Если в папке есть файлы
            {
                if (file.Extension == _FileExtension)
                // С необходимым расширением, то создаем элемент Mc_Class и 
                // записываем в его параметры необходимые данные (по аналогии с данными директории).
                {
                    path = file.FullName.Replace(file.Extension, _ToolTipExtension);
                    if (File.Exists(path))
                        txt_file = System.IO.File.ReadAllText(path);
                    else
                        txt_file = "";
                    //McU.TxtUtils.GetText(path);
                    _rnd_int++;
                    directoryNode.Mc_List.Add(new Mc_Class(_rnd_int.ToString())
                    {
                        Mc_Prop_1 = file.Name.Replace(file.Extension, ""),

                        Mc_Prop_2 = file.FullName.Replace(file.Extension, _ImageExtension),

                        Mc_Prop_3 = txt_file,

                        Mc_Prop_4 = file.FullName

                    });

                }
            }

            return directoryNode;

        }


    }
}
