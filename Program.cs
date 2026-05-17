using Newtonsoft.Json;

namespace Rizline_Chart
{
    public class Program
    {
        static string directory;
        static List<string> fileNames;
        //此处记录的是节拍
        static List<float> splitTime = new();

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

            fileNames = GetAllFiles(directory);
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

        private static void AutoImport()
        {
            try
            {
                fileNames.Sort((name1, name2) =>
                {
                    return int.Parse(name1.Split(".")[0]).CompareTo(int.Parse(name2.Split(".")[0]));
                });
            }
            catch
            {
                throw;
            }
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
            Console.ReadKey();
        }
    }
}
