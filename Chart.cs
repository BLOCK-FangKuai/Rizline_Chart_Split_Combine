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
        public List<Theme> themes = new();
        public List<ChallengeTime> challengeTimes = new();
        public float bPM;
        public List<KeyPoint> bpmShifts = new();
        public List<Line> lines = new();
        public List<CanvasMove> canvasMoves = new();
        public CameraMove cameraMove = new();
        public Dictionary<string, string> __db = new();
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
        public int r;
        public int g;
        public int b;
        public int a;
    }

    public class ChallengeTime
    {
        public float checkPoint;
        public float start;
        public float end;
        public float transTime;
    }

    public class Line
    {
        public List<LinePoint> linePoints = new();
        public List<Note> notes = new();
        public List<ColorKeyPoint> judgeRingColor = new();
        public List<ColorKeyPoint> lineColor = new();
    }

    public class LinePoint
    {
        public float time;
        public float xPosition;
        public Color color = new();
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
        public Color startColor = new();
        public Color endColor = new();
        public float time;
    }

    public class CanvasMove
    {
        public int index;
        public List<KeyPoint> xPositionKeyPoints = new();
        public List<KeyPoint> speedKeyPoints = new();
    }

    public class CameraMove
    {
        public List<KeyPoint> scaleKeyPoints = new();
        public List<KeyPoint> xPositionKeyPoints = new();
    }

    public class KeyPoint
    {
        public float time;
        public float value;
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
