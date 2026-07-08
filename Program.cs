using Newtonsoft.Json;
using System.Xml.Linq;

namespace Rizline_Chart
{
    public class Program
    {
        static string directory = "";
        static List<string> filePaths = new();
        static Settings settings = new();
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

                    if (YNChoose("是否使用自动导入？（Y/N）"))
                    {
                        AutoImport();
                        //PyPrint(JsonConvert.SerializeObject(resultChart, serializerSettings));
                    }
                    else
                    {
                        ManualImport();
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
            settings.files = chartFiles;
            if (!settings.Check())
            {
                PyPrint("文件夹内谱面文件数目m与配置文件的时间切割点的数量n不对应：m≠n+1");
                for (int i = 0; i < settings.files.Count; i++)
                {
                    PyPrint($"[{i + 1}] {settings.files[i]}");
                }
                throw new Exception();
            }
            CombineCharts();
            OutputChart();
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
                    bool isChartFile = int.TryParse(fileName.Split(".")[0], out _) && Path.GetExtension(filePath).ToLower().Equals(".json");
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
                var g = name2.Split(".", 2);
                int numSort = int.Parse(name1.Split(".", 2)[0]).CompareTo(int.Parse(name2.Split(".", 2)[0]));
                //如果编号大小相同，则按照编号后面的名称排序
                if (numSort == 0)
                {
                    int nameSort = name1.Split(".", 1)[1].CompareTo(name2.Split(".", 1)[1]);
                    return nameSort;
                }
                return numSort;
            });
        }

        private static void CombineCharts()
        {
            for (int i = 0; i < settings.files.Count; i++)
            {
                Chart chart;
                string chartPath = Path.Combine(directory, settings.files[i]);
                string json = File.ReadAllText(chartPath);
                try
                {
                    chart = JsonConvert.DeserializeObject<Chart>(json);
                }
                catch
                {
                    throw new Exception($"谱面文件格式不正确：{chartPath}");
                }
                Chart splitedChart = SplitChart(chart, i, settings.splitTimes.Count);
                resultChart.lines.AddRange(splitedChart.lines);
                resultChart.canvasMoves.AddRange(splitedChart.canvasMoves);
                resultChart.cameraMoves.AddRange(splitedChart.cameraMoves);
            }
        }

        private static Chart SplitChart(Chart chart, int index, int count)
        {
            float start = index == 0 ? 0 : settings.splitTimes[index - 1];
            float end = index == count ? float.MaxValue : settings.splitTimes[index];
            float overlap = settings.overlapTime;

            for (int i = 0; i < chart.lines.Count; i++)
            {
                chart.lines[i] = CutLine(chart.lines[i], chart.canvasMoves, start, end, overlap);
            }
            for (int i = 0; i < chart.canvasMoves.Count; i++)
            {
                chart.canvasMoves[i] = CutCanvasMove(chart.canvasMoves[i], start, end, overlap);
            }
            for (int i = 0; i < chart.cameraMoves.Count; i++)
            {
                chart.cameraMoves[i] = CutCameraMove(chart.cameraMoves[i], start, end);
            }

            return chart;
        }

        private static Line CutLine(Line line, List<CanvasMove> cameraMoves, float start, float end, float overlap)
        {
            float lineStart = start - overlap;
            float lineEnd = end + overlap;

            //截取在指定时间内的节点，将最近的超界的节点的时间设置为边界值
            List<LinePoint> inLinePoints = line.linePoints.FindAll(point => point.time > lineStart && point.time < lineEnd);
            int firstPointIndex = inLinePoints.Count > 0 ? line.linePoints.IndexOf(inLinePoints[0]) : 0;
            int finalPointIndex = inLinePoints.Count > 0 ? line.linePoints.IndexOf(inLinePoints[^1]) : line.linePoints.Count - 1;
            line.linePoints[firstPointIndex].time = line.linePoints[firstPointIndex].time < lineStart ?
                lineStart : line.linePoints[firstPointIndex].time;
            line.linePoints[finalPointIndex].time = line.linePoints[finalPointIndex].time > lineEnd ?
                lineEnd : line.linePoints[finalPointIndex].time;
            inLinePoints.Insert(0, line.linePoints[firstPointIndex]);
            inLinePoints.Add(line.linePoints[finalPointIndex]);

            //增加画布的编号
            foreach (var point in line.linePoints)
            {
                point.canvasIndex += resultChart.canvasMoves.Count;
            }

            //截取在指定时间内的note
            line.notes = line.notes.FindAll(note => note.time >= start && note.time < end);

            //修正hold尾部的画布编号
            List<Note> holds = line.notes.FindAll(note => note.type == NoteType.hold);
            for (int i = 0; i < holds.Count; i++)
            {
                holds[i].otherInformations[1] += resultChart.canvasMoves.Count;
            }

            //将最后一个hold的结束时间设置为分割结束时间
            if (holds.Count > 0)
            {
                Note finalHold = holds[^1];
                if (finalHold != null)
                {
                    if (finalHold.otherInformations[0] > end)
                    {
                        finalHold.otherInformations[0] = end;
                        float originalCanvasIndex = finalHold.otherInformations[1] - resultChart.canvasMoves.Count;
                        CanvasMove canvasMove = cameraMoves.Find(canvasMove => canvasMove.index == originalCanvasIndex);
                        finalHold.otherInformations[2] = CalculateFloorPosition(canvasMove, finalHold.otherInformations[0]);
                    }
                }
            }
            return line;
        }

        private static CanvasMove CutCanvasMove(CanvasMove canvasMove, float start, float end, float overlap)
        {
            start -= overlap;
            end += overlap;

            canvasMove.index += resultChart.canvasMoves.Count;

            //截取在指定时间内的关键帧节点，将最近的超界的关键帧节点的时间设置为边界值
            List<KeyPoint> inKeyPoints = canvasMove.xPositionKeyPoints.FindAll(point => point.time > start && point.time < end);
            int firstPointIndex = inKeyPoints.Count > 0 ? canvasMove.xPositionKeyPoints.IndexOf(inKeyPoints[0]) : 0;
            int finalPointIndex = inKeyPoints.Count > 0 ? canvasMove.xPositionKeyPoints.IndexOf(inKeyPoints[^1]) : canvasMove.xPositionKeyPoints.Count - 1;
            canvasMove.xPositionKeyPoints[firstPointIndex].time =
                canvasMove.xPositionKeyPoints[firstPointIndex].time < start ?
                start : canvasMove.xPositionKeyPoints[firstPointIndex].time;
            canvasMove.xPositionKeyPoints[finalPointIndex].time =
                canvasMove.xPositionKeyPoints[finalPointIndex].time > end ?
                end : canvasMove.xPositionKeyPoints[finalPointIndex].time;
            inKeyPoints.Insert(0, canvasMove.xPositionKeyPoints[firstPointIndex]);
            inKeyPoints.Add(canvasMove.xPositionKeyPoints[finalPointIndex]);

            canvasMove.xPositionKeyPoints = inKeyPoints;

            inKeyPoints = canvasMove.speedKeyPoints.FindAll(point => point.time > start && point.time <= end);
            firstPointIndex = inKeyPoints.Count > 0 ? canvasMove.speedKeyPoints.IndexOf(inKeyPoints[0]) : 0;
            canvasMove.speedKeyPoints[firstPointIndex].time = start;
            inKeyPoints.Insert(0, canvasMove.speedKeyPoints[firstPointIndex]);

            canvasMove.speedKeyPoints = inKeyPoints;

            return canvasMove;
        }

        private static CameraMove CutCameraMove(CameraMove cameraMove, float start, float end)
        {
            //截取在指定时间内的关键帧节点，将最开始的超界的关键帧节点的时间设置为边界值
            List<KeyPoint> inKeyPoints = cameraMove.xPositionKeyPoints.FindAll(point => point.time > start && point.time <= end);
            int firstPointIndex = inKeyPoints.Count > 0 ? cameraMove.xPositionKeyPoints.IndexOf(inKeyPoints[0]) : 0;
            cameraMove.xPositionKeyPoints[firstPointIndex].time =
                cameraMove.xPositionKeyPoints[firstPointIndex].time < start ?
                start : cameraMove.xPositionKeyPoints[firstPointIndex].time;
            inKeyPoints.Insert(0, cameraMove.xPositionKeyPoints[firstPointIndex]);

            cameraMove.xPositionKeyPoints = inKeyPoints;

            inKeyPoints = cameraMove.scaleKeyPoints.FindAll(point => point.time > start && point.time <= end);
            firstPointIndex = inKeyPoints.Count > 0 ? cameraMove.scaleKeyPoints.IndexOf(inKeyPoints[0]) : 0;
            cameraMove.scaleKeyPoints[firstPointIndex].time = start;
            inKeyPoints.Insert(0, cameraMove.scaleKeyPoints[firstPointIndex]);

            cameraMove.scaleKeyPoints = inKeyPoints;

            return cameraMove;
        }

        private static void OutputChart()
        {
            string outputPath = Path.Combine(directory, $"{settings.resultName}.json");
            string json = JsonConvert.SerializeObject(resultChart, serializerSettings);
            if (settings.autoOverWritten)
            {
                if (!TryWrite(outputPath, json))
                {
                    while (true)
                    {
                        string resultName = PyInput("请输入要输出的谱面文件的文件名：");
                        outputPath = Path.Combine(directory, $"{resultName}.json");
                        if (TryWrite(outputPath, json))
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (File.Exists(outputPath))
                {
                    PyPrint(outputPath);
                    if (YNChoose("文件已存在，是否覆盖文件？(Y/N)"))
                    {
                        File.WriteAllText(outputPath, json);
                    }
                    else
                    {
                        while (true)
                        {
                            string resultName = PyInput("请输入要输出的谱面文件的文件名：");
                            outputPath = Path.Combine(directory, $"{resultName}.json");
                            if (!File.Exists(outputPath))
                            {
                                if (TryWrite(outputPath, json))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                PyPrint(outputPath);
                                if (YNChoose("文件已存在，是否覆盖文件？(Y/N)"))
                                {
                                    File.WriteAllText(outputPath, json);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!TryWrite(outputPath, json))
                    {
                        while (true)
                        {
                            string resultName = PyInput("请输入要输出的谱面文件的文件名：");
                            outputPath = Path.Combine(directory, $"{resultName}.json");
                            if (TryWrite(outputPath, json))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            PyPrint($"已将谱面文件输出到{outputPath}");
            resultChart = new();
        }

        private static void ManualImport()
        {

        }

        private static float CalculateFloorPosition(CanvasMove canvasMove, float time)
        {
            float floorPosition = 0;
            List<KeyPoint> speeds = canvasMove.speedKeyPoints;
            if (speeds.Count == 1)
            {
                floorPosition = speeds[0].value * time;
            }
            else
            {
                for (int i = 0; i < speeds.Count - 1; i++)
                {
                    if (speeds[i + 1].time < time)
                    {
                        floorPosition += speeds[i].value * (speeds[i + 1].time - speeds[i].time);
                    }
                    else
                    {
                        floorPosition += speeds[i].value * (time - speeds[i].time);
                        break;
                    }
                }
            }
            return floorPosition;
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

        private static bool YNChoose(string message)
        {
            PyPrint(message);
            while (true)
            {
                var choose = Console.ReadKey(true);
                if ($"{choose.KeyChar}".ToUpper().Equals("Y"))
                {
                    return true;
                }
                else if ($"{choose.KeyChar}".ToUpper().Equals("N"))
                {
                    return false;
                }
            }
        }

        private static bool TryWrite(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception e)
            {
                PyPrint(e.Message);
                return false;
            }
        }
    }
}
