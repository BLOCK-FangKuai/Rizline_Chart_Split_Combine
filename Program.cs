using Newtonsoft.Json;
using System.Xml.Linq;

namespace Rizline_Chart
{
    public class Program
    {
        static string directory;
        static List<string> filePaths;
        static Settings settings;
        static Chart resultChart = new();

        static JsonSerializerSettings serializerSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void Main(string[] args)
        {
            while (true)
            {
                try
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
                            //PyPrint(JsonConvert.SerializeObject(resultChart, serializerSettings));
                            break;
                        }
                        else if ($"{choose.KeyChar}".ToUpper().Equals("N"))
                        {
                            ManualImport();
                            break;
                        }
                    }

                    // D:\Codes\Rizline_Chart\Test\Pastel Lines

                    //string json = File.ReadAllText("D:\\Codes\\Rizline_Chart\\Test\\Research_Json.json");
                    //Chart? chart = JsonConvert.DeserializeObject<Chart>(json);
                    Pause();
                }
                catch (Exception e)
                {
                    PyPrint(e.Message);
                    Pause();
                    Console.Clear();
                }
            }
        }

        private static void AutoImport()
        {
            filePaths = GetAllFiles(directory);
            GetSettings(filePaths);
            GetBaseChart();
            List<string> chartFiles = GetChartFiles();
            SortChartFiles(chartFiles);
        }

        private static void GetSettings(List<string> filePaths)
        {
            string settingPath = Path.Combine(directory, "settings.json");
            if (filePaths.Contains(settingPath))
            {
                try
                {
                    string json = File.ReadAllText(settingPath);
                    settings = JsonConvert.DeserializeObject<Settings>(json);
                    if (!settings.Check())
                    {
                        throw new Exception($"配置文件参数不正确：{settingPath}");
                    }
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new FileNotFoundException($"未找到配置文件：{settingPath}");
            }
        }

        private static void GetBaseChart()
        {
            string baseChartPath = Path.Combine(directory, $"{settings.baseChartName}.json");
            if (filePaths.Contains(baseChartPath))
            {
                try
                {
                    string json = File.ReadAllText(baseChartPath);
                    Chart baseChart = JsonConvert.DeserializeObject<Chart>(json);

                    resultChart.fileVersion = baseChart.fileVersion;
                    resultChart.chartDelayMs = baseChart.chartDelayMs;
                    resultChart.themes = baseChart.themes;
                    resultChart.challengeTimes = baseChart.challengeTimes;
                    resultChart.bPM = baseChart.bPM;
                    resultChart.bpmShifts = baseChart.bpmShifts;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new FileNotFoundException($"未找到基础谱面文件：{baseChartPath}");
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
            if (settings.IsFileEmpty())
            {
                //没有指定哪些是谱面文件时
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
            }
            else
            {
                //指定哪些是谱面文件时
                foreach (var file in settings.files)
                {
                    string filePath = Path.Combine(directory, file);
                    if (filePaths.Contains(filePath))
                    {
                        string fileName = Path.GetFileName(filePath);
                        chartFiles.Add(fileName);
                    }
                    else
                    {
                        throw new FileNotFoundException($"未找到谱面文件：{filePath}");
                    }
                }
            }
            return chartFiles;
        }

        private static void SortChartFiles(in List<string> chartFiles)
        {
            //指定谱面文件后不再进行排序
            if (!settings.IsFileEmpty())
            {
                return;
            }
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
