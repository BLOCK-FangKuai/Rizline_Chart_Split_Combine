using System.Collections;

namespace Rizline_Chart
{
    public class Settings
    {
        //此处记录的是节拍
        public List<float> splitTimes = new();
        public float overlapTime = 4;
        public string baseChartName = "base";
        public List<string> files = new();
        public string resultName = "result";
        public bool autoOverWritten = false;

        public bool Check()
        {
            if (overlapTime < 0)
            {
                overlapTime = 0;
            }
            return IsFileEmpty() || splitTimes.Count + 1 == files.Count;
        }

        public bool IsFileEmpty()
        {
            return files == null || files.Count == 0;
        }
    }
}