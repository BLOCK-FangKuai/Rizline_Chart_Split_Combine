namespace Rizline_Chart_Split_Combine
{
    public class Settings
    {
        public List<float> splitTimes = new();
        public float overlapTime = 0;
        public bool finalCameraMoveEaseSetZero = false;
        public float cameraMoveOffset = 0.015625f;
        public string baseChartName = "base.json";
        public List<string> files = new();
        public string resultName = "result";
        public bool autoOverWritten = false;
        public bool automatic = false;

        public bool Check(out string message)
        {
            if (splitTimes.Distinct().ToList().Count < splitTimes.Count)
            {
                message = "splitTimes中有重复元素";
                return false;
            }
            for (int i = 0; i < splitTimes.Count - 1; i++)
            {
                if (splitTimes[i] >= splitTimes[i + 1])
                {
                    message = "splitTimes必须从小到大排序";
                    return false;
                }
            }
            if (overlapTime < 0)
            {
                message = "overlapTime不可小于0";
                return false;
            }
            if (!finalCameraMoveEaseSetZero && (cameraMoveOffset > 0.015625 || cameraMoveOffset <= 0))
            {
                message = "cameraMoveOffset值的范围为(0, 0.015625]";
                return false;
            }
            if (IsFileEmpty())
            {
                message = "files为空";
                return false;
            }
            message = "splitTimes的数量+1须等于files的数量";
            return splitTimes.Count + 1 == files.Count;
        }

        public bool IsFileEmpty()
        {
            return files == null || files.Count == 0;
        }
    }
}