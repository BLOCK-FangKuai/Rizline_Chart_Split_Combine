using Newtonsoft.Json;
using System.Xml.Linq;

namespace Rizline_Chart
{
    public class Program
    {
        static string directory;
        static List<string> filePaths;
        //此处记录的是节拍
        static List<float> splitTimes = new();

        public static void Main(string[] args)
        {
            while (true)
            {
                directory = PyInput("请输入json文件目录：", "charts");
                if (!Directory.Exists(directory))
                {
                    directory = Path.Combine(Directory.GetCurrentDirectory(), directory);
                    if (Directory.Exists(directory))
                    {
                        break;
                    }
                    Pause($"目录{directory}不存在，请重新输入");
                }
                else
                {
                    break;
                }
            }
            
            PyPrint($"选择目录：{Path.GetFullPath(directory)}");

            PyPrint("是否使用自动导入？（Y/N）");

            while (true)
            {
                var choose = Console.ReadKey(true);
                if ($"{choose.KeyChar}".ToUpper().Equals("Y"))
                {
                    AutoImport();
                    break;
                }
                else if ($"{choose.KeyChar}".ToUpper().Equals("N"))
                {
                    ManualImport();
                    break;
                }
            }

            
            //string json = File.ReadAllText("D:\\Codes\\Rizline_Chart\\Test\\Research_Json.json");
            //Chart? chart = JsonConvert.DeserializeObject<Chart>(json);
            Pause();
        }

        private static void AutoImport()
        {
            filePaths = GetAllFiles(directory);
            List<string> chartFiles = GetChartFiles();
            SortChartFiles(chartFiles);
        }

        private static void GetSettings(List<string> filePaths)
        {
            if (filePaths.Contains(Path.Combine(directory, "settings.json")))
            {

            }
        }

        private static List<string> GetAllFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                return [.. files];
            }
            else
            {
                throw new DirectoryNotFoundException(path);
            }
        }

        private static List<string> GetChartFiles()
        {
            List<string> chartFiles = new();
            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                //文件名带有数字编号且后缀为.json的计为谱面文件
                bool isChartFile = int.TryParse(fileName.Split(".")[0], out _) && Path.GetExtension(filePath).ToLower().Equals("json");
                if (isChartFile)
                {
                    chartFiles.Add(fileName);
                }
            }
            return chartFiles;
        }

        private static void SortChartFiles(in List<string> chartFiles)
        {
            chartFiles.Sort((name1, name2) =>
            {
                //根据编号大小排序
                int numSort = int.Parse(name1.Split(".", 1)[0]).CompareTo(int.Parse(name2.Split(".", 1)[0]));
                //如果编号大小相同，则按照编号后面的名称排序
                if (numSort == 0)
                {
                    int nameSort = name1.Split(".", 1)[1].CompareTo(name2.Split(".", 1)[1]);
                    return nameSort;
                }
                return numSort;
            });
        }

        private static void ManualImport()
        {

        }

        private static string PyInput(string message = "", string @default = "")
        {
            Console.WriteLine(message);
            string? read = Console.ReadLine();
            string result = string.IsNullOrWhiteSpace(read) ? @default : read;
            return result;
        }

        private static void PyPrint(string message = "") {  Console.WriteLine(message); }

        private static void Pause(string message = "")
        {
            Console.WriteLine(message);
            Console.WriteLine("按任意键继续……");
            Console.ReadKey(true);
        }
    }
}
