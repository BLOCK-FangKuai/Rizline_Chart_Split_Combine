using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizline_Chart
{
    public class Chart
    {
        public int fileVersion;
        public int chartDelayMs;
        public List<Theme> themes;
        public List<ChallengeTime> challengeTimes;
        public float bPM;
        public List<KeyPoint> bpmShifts;
        public List<Line>? lines;
        public List<CanvasMove> canvasMoves;
        public List<CameraMove> cameraMoves;
        public Dictionary<string, string>? __db;
    }

    public class Theme
    {
        /// <summary>
        /// 第一个值表示背景色
        /// 第二个值表示音符颜色
        /// 第三个值表示UI颜色
        /// </summary>
        public Color[] colorsList = new Color[3];
    }

    public class Color
    {
        public int r, g, b, a;
    }

    public class ChallengeTime
    {
        public float checkPoint, start, end, transTime;
    }

    public class Line
    {
        public List<LinePoint> linePoints;
        public List<Note> notes;
        public List<ColorKeyPoint> judgeRingColor;
        public List<ColorKeyPoint> lineColor;
    }

    public class LinePoint
    {
        public float time;
        public float xPosition;
        public Color color;
        public int easeType;
        public int canvasIndex;
        public float floorPosition;
    }

    public class Note
    {
        public NoteType type;
        public float time;
        public float floorPosition;
        /// <summary>
        /// Hold音符特有的参数
        /// 第一个数表示结束的时间
        /// 第二个数表示Hold尾所在画布的index
        /// 第三个数表示Hold尾在所在画布的floorPosition
        /// </summary>
        public float[] otherInformations = new float[3];
    }

    public class ColorKeyPoint
    {
        public Color startColor;
        public Color endColor;
        public float time;
    }

    public class CanvasMove
    {
        public int index;
        public List<KeyPoint> xPositionKeyPoints;
        public List<KeyPoint> speedKeyPoints;
    }

    public class CameraMove
    {
        public List<KeyPoint> scaleKeyPoints;
        public List<KeyPoint> xPositionKeyPoints;
    }

    public class KeyPoint
    {
        public float time, value;
        public EaseType easeType;
        public float floorPosition;
    }

    public enum NoteType
    {
        tap,
        drag,
        hold
    }

    public enum EaseType
    {
        linear = 0,
        inQuad = 1,
        outQuad = 2,
        inOutQuad = 3,
        inCubic = 4,
        outCubic = 5,
        inOutCubic = 6,
        inQuart = 7,
        outQuart = 8,
        inOutQuart = 9,
        inQuint = 10,
        outQuint = 11,
        inOutQuint = 12,
        zero = 13,
        one = 14,
        inCircle = 15,
        outCircle = 16,
        sin = 17,
        cos = 18
    }
}
