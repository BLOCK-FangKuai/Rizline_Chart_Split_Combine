using System.Collections;

namespace Rizline_Chart
{
    public class Settings
    {
        //此处记录的是节拍
        public List<float> splitTimes = new();
        public float overlapTime = 4;
        public string baseChartName = "base";
        public List<string>? files;
        public string resultName = "result";
        public bool autoOverride = false;

        public bool Check()
        {
            return IsFileEmpty() || splitTimes.Count + 1 == files.Count;
        }

        public bool IsFileEmpty()
        {
            return files == null || files.Count == 0;
        }
    }
}