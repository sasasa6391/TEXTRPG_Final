using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{


    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)] public char Char;
        [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    public class Renderer
    {

        const int STD_OUTPUT_HANDLE = -11;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public Coord dwFontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [Flags]
        public enum ForegroundColor : short
        {
            Black = 0x0000,
            Blue = 0x0001,
            Green = 0x0002,
            Cyan = Blue | Green,
            Red = 0x0004,
            Magenta = Blue | Red,
            Yellow = Green | Red,
            White = Blue | Green | Red,
            IntenseBlack = 0x0008,
            IntenseBlue = Blue | IntenseBlack,
            IntenseGreen = Green | IntenseBlack,
            IntenseCyan = Cyan | IntenseBlack,
            IntenseRed = Red | IntenseBlack,
            IntenseMagenta = Magenta | IntenseBlack,
            IntenseYellow = Yellow | IntenseBlack,
            IntenseWhite = White | IntenseBlack
        }

        private static CharInfo[] _buffer;

        private static short _width;
        private static short _height;

        public static short Width => _width;
        public static short Height => _height;


        public static int CurrentLogY;
        public static int LogStart = 22;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private const int SW_MAXIMIZE = 3;


        public Renderer()
        {
            IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
            CONSOLE_FONT_INFO_EX cfi = new CONSOLE_FONT_INFO_EX();
            cfi.cbSize = (uint)Marshal.SizeOf(cfi);
            cfi.nFont = 0;
            cfi.dwFontSize = new Coord() { X = 0, Y = 22 }; // 폰트 크기 지정
            cfi.FontFamily = TMPF_TRUETYPE;
            cfi.FontWeight = 400;
            cfi.FaceName = "Cursive"; // 폰트 이름 지정

            bool setFontResult = SetCurrentConsoleFontEx(hnd, false, ref cfi);


            // 콘솔 창 최대화
            IntPtr consoleWindow = GetConsoleWindow();
            //ShowWindow(consoleWindow, SW_MAXIMIZE);

            // 최대 콘솔 크기를 설정
            _width = (short)(120);
            _height = (short)(30);

            //Console.SetBufferSize(_width, _height);
            Console.SetWindowSize(_width, _height);

            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            _buffer = new CharInfo[_width * _height];

            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i].Char = ' '; // Set character
                _buffer[i].Attributes = (short)ForegroundColor.White;
            }

            // 메인 쓰레드
            // 렌더링 쓰레드
            // 게임 돌아가는건 따로 있고
            // 찍어주는건 

            new Thread(() =>
            {
                while (true)
                {
                    if (IsRender)
                    {
                        lock (_buffer)
                        {
                            Render();
                        }
                    }
                    Thread.Sleep(Managers.Game.GetGameSleepTime(10));
                }
            }).Start();

        }

        private static bool IsRender = true;
        public static void StopRenderThread()
        {
            Render();
            IsRender = false;
        }

        public static void RestartRenderThread()
        {
            IsRender = true;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        static int frame = 0;
        static int color = 0;
        public static void Render()
        {
            IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            frame++;
            if (frame == 2)
            {
                frame = 0;
                color++;
                color %= 5;
            }
            Coord bufferSize = new Coord { X = _width, Y = _height };
            Coord bufferCoord = new Coord { X = 0, Y = 0 };
            SmallRect writeRegion = new SmallRect { Left = 0, Top = 0, Right = (short)(_width - 1), Bottom = (short)(_height - 1) };

            bool success = WriteConsoleOutput(consoleHandle, _buffer, bufferSize, bufferCoord, ref writeRegion);
            if (!success)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }



        public static void DrawBorder(string title = "", int titleStartOffsetX = Game.lobbyOffsetX)
        {

            lock (_buffer)
            {
                ConsoleClear();
                if (!string.IsNullOrEmpty(title))
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Draw(0, 2, new string(' ', Width));
                    int correctLength = GetPrintingLength(title);
                    int horizontalStart = (Width - correctLength) / 2;
                    if (horizontalStart < 0) horizontalStart = 3;
                    Draw(horizontalStart + titleStartOffsetX - 1, 2, 'y' + title + 'w');
                }

                for (int i = 0; i < Width; i++)
                {
                    _buffer[i] = new CharInfo { Char = ' ', Attributes = 0x0080 };
                    _buffer[i + (_height - 1) * _width] = new CharInfo { Char = ' ', Attributes = 0x0080 };
                }

                for (int i = 1; i < _height - 1; i++)
                {
                    _buffer[i * Width] = new CharInfo { Char = ' ', Attributes = 0x0080 };
                    _buffer[Width - 1 + i * _width] = new CharInfo { Char = ' ', Attributes = 0x0080 };
                }
                if (Managers.Scene.CurrentScene is MainScene || Managers.Scene.CurrentScene is TitleScene || Managers.Scene.CurrentScene is CasinoScene)
                {
                }
                else
                {
                    Renderer.DrawKeyGuide("[ESC : 뒤로가기]");
                }
            }
        }


        public static void DrawCenter(int line, string content, int startOffsetX = 0)
        {
            int pad = _width - 3 - GetPrintingLength(content);
            Draw(1 + startOffsetX + pad / 2, line, content /*+ "".PadRight(pad - 1)*/);
        }


        public static void Draw(int startX, int startY, string s, bool isEngOK = false)
        {
            lock (_buffer)
            {
                var cInfo = GetCharInfo(s, isEngOK);


                int endX = Math.Min(startX + cInfo.Length - 1, _width - 1);

                for (int i = startX; i <= endX; i++)
                {
                    _buffer[i + startY * _width] = cInfo[i - startX];
                }
            }
        }

        public static void Draw(int startX, int startY, CharInfo c)
        {
            lock (_buffer)
            {
                _buffer[startX + startY * _width] = c;
            }
        }


        public static void DrawOptions(int startX, int line, List<ActionOption> options, int selectionLine = 0)
        {
            var strs = new List<string>();
            foreach (var e in options)
            {
                strs.Add(e.Description);
            }
            DrawOptions(startX, line, strs, selectionLine);
        }


        public static void DrawOptions(int startX, int line, List<string> options, int selectionLine = 0)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var str = options[i];

                if (selectionLine == i)
                {
                    str = 'g' + str + 'w';
                }
                Draw(startX, i * 2 + line, str);
            }


            for (int i = 0; i < options.Count; i++)
            {
                var cnt = GetPrintingLength(options[i]);

                for (int j = 0; j < 3; j++)
                {
                    Draw(startX - 2, i * 2 + line + j - 1, "  ");
                    Draw(startX + cnt, i * 2 + line + j - 1, "  ");
                }
                Draw(startX, i * 2 + line - 1, $"g{new string(' ', cnt)}w");
                Draw(startX, i * 2 + line + 1, $"g{new string(' ', cnt)}w");
            }

            for (int i = 0; i < options.Count; i++)
            {
                var str = options[i];

                Draw(startX, i * 2 + line, str);

                var cnt = GetPrintingLength(str);

                if (selectionLine == i)
                {
                    Draw(startX - 2, i * 2 + line, "g│ w");
                    Draw(startX + cnt, i * 2 + line, "g│ w");
                    Draw(startX - 2, i * 2 + line - 1, "g┌ w");
                    Draw(startX + cnt, i * 2 + line - 1, "g┐ w");
                    Draw(startX - 2, i * 2 + line + 1, "g└ w");
                    Draw(startX + cnt, i * 2 + line + 1, "g┘ w");
                    Draw(startX, i * 2 + line - 1, $"g{new string('━', cnt)}w");
                    Draw(startX, i * 2 + line + 1, $"g{new string('━', cnt)}w");
                }
            }
        }
        public static void DrawOptionsCenter(int line, List<ActionOption> options, int selectionLine = 0, int offsetX = 0)
        {

            for (int i = 0; i < options.Count; i++)
            {
                var cnt = GetPrintingLength(options[i].Description);

                for (int j = 0; j < 3; j++)
                {
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line + j - 1, "  ");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line + j - 1, "  ");
                }
                Draw(offsetX + _width / 2 - cnt / 2 - 2, i * 2 + line - 1, $"g{new string(' ', cnt + 1)}w");
                Draw(offsetX + _width / 2 - (cnt - 1) / 2 - 2, i * 2 + line + 1, $"g{new string(' ', cnt + (cnt % 2 == 0 ? 0 : 1))}w");
            }

            for (int i = 0; i < options.Count; i++)
            {
                ActionOption option = options[i];

                var str = option.Description;

                DrawCenter(i * 2 + line, str, offsetX);

                var cnt = GetPrintingLength(str);

                if (selectionLine == i)
                {
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line, "g│ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line, "g│ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line - 1, "g┌ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line - 1, "g┐ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line + 1, "g└ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line + 1, "g┘ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 2, i * 2 + line - 1, $"g{new string('─', cnt + 1)}w");
                    Draw(offsetX + _width / 2 - (cnt - 1) / 2 - 2, i * 2 + line + 1, $"g{new string('─', cnt + (cnt % 2 == 0 ? 0 : 1))}w");
                }
            }
        }

        public static void DrawOptionsCenter(int line, List<string> options, int selectionLine = 0, int offsetX = 0)
        {

            for (int i = 0; i < options.Count; i++)
            {
                var cnt = GetPrintingLength(options[i]);

                for (int j = 0; j < 3; j++)
                {
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line + j - 1, "  ");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line + j - 1, "  ");
                }
                Draw(offsetX + _width / 2 - cnt / 2 - 2, i * 2 + line - 1, $"g{new string(' ', cnt + 1)}w");
                Draw(offsetX + _width / 2 - (cnt - 1) / 2 - 2, i * 2 + line + 1, $"g{new string(' ', cnt + (cnt % 2 == 0 ? 0 : 1))}w");
            }

            for (int i = 0; i < options.Count; i++)
            {
                var str = options[i];

                DrawCenter(i * 2 + line, str, offsetX);

                var cnt = GetPrintingLength(str);

                if (selectionLine == i)
                {
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line, "g│ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line, "g│ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line - 1, "g┌ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line - 1, "g┐ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 3, i * 2 + line + 1, "g└ w");
                    Draw(offsetX + _width / 2 + (cnt - 1) / 2, i * 2 + line + 1, "g┘ w");
                    Draw(offsetX + _width / 2 - cnt / 2 - 2, i * 2 + line - 1, $"g{new string('─', cnt + 1)}w");
                    Draw(offsetX + _width / 2 - (cnt - 1) / 2 - 2, i * 2 + line + 1, $"g{new string('─', cnt + (cnt % 2 == 0 ? 0 : 1))}w");
                }
            }
        }

        public static void ClearLine(int startY, int endY)
        {
            lock (_buffer)
            {
                for (int i = startY; i <= endY; i++)
                {
                    for (int j = 0; j < _width; j++)
                    {
                        _buffer[i * _width + j] = new CharInfo { Char = ' ', Attributes = (short)ForegroundColor.White };
                    }
                }
            }
        }
        public static void ClearLine(int startX, int endX, int startY, int endY)
        {
            lock (_buffer)
            {
                for (int i = startY; i <= endY; i++)
                {
                    for (int j = startX; j < endX; j++)
                    {
                        _buffer[i * _width + j] = new CharInfo { Char = ' ', Attributes = (short)ForegroundColor.White };
                    }
                }
            }
        }


        public static CharInfo[] GetCharInfo(string s, bool isEngOK = false)
        {
            var cInfo = new List<CharInfo>();

            var currentColor = (short)ForegroundColor.White;

            for (int i = 0; i < s.Length; i++)
            {
                if (isEngOK == true)
                {
                    cInfo.Add(new CharInfo { Char = s[i], Attributes = currentColor });

                    // 한국어면 1칸 추가
                    if (IsKorean(s[i]))
                    {
                        cInfo.Add(new CharInfo { Char = ' ', Attributes = currentColor });
                    }
                }
                else
                {
                    switch (s[i])
                    {
                        case 'w':
                            currentColor = (short)ForegroundColor.White;
                            break;
                        case 'r':
                            currentColor = (short)ForegroundColor.Red;
                            break;
                        case 'y':
                            currentColor = (short)ForegroundColor.Yellow;
                            break;
                        case 'd':
                            currentColor = (short)ForegroundColor.IntenseBlack;
                            break;
                        case 'g':
                            currentColor = (short)ForegroundColor.Green;
                            break;
                        case 'b':
                            currentColor = (short)ForegroundColor.Blue;
                            break;
                        /*
                        case 'B':
                            currentColor = (short)ForegroundColor.Black;
                            break;
                        */
                        case 'z':
                            currentColor = (short)0x0010;
                            break;
                        case 'n':
                            currentColor = (short)0x0020;
                            break;
                        case 'x':
                            currentColor = (short)0x0040;
                            break;
                        case 'v':
                            currentColor = (short)0x0080;
                            break;
                        case 'V':
                            currentColor = (short)0x0060;
                            break;

                        default:
                            cInfo.Add(new CharInfo { Char = s[i], Attributes = currentColor });

                            // 한국어면 1칸 추가
                            if (IsKorean(s[i]))
                            {
                                cInfo.Add(new CharInfo { Char = ' ', Attributes = currentColor });
                            }
                            // 
                            //if (s[i] > 127)
                            //{
                            //    cInfo.Add(new CharInfo { Char = ' ', Attributes = currentColor });
                            //}
                            break;

                    }
                }

            }

            return cInfo.ToArray();
        }


        public static CharInfo[] GetCharInfoEng(string s)
        {
            var cInfo = new List<CharInfo>();

            var currentColor = (short)ForegroundColor.White;

            for (int i = 0; i < s.Length; i++)
            {
                cInfo.Add(new CharInfo { Char = s[i], Attributes = currentColor });

                // 한국어면 1칸 추가
                if (IsKorean(s[i]))
                {
                    cInfo.Add(new CharInfo { Char = ' ', Attributes = currentColor });
                }
            }

            return cInfo.ToArray();
        }

        public static float Lerp(float a, float b, float t)
        {
            return a * (1.0f - t) + t * b;
        }
        public static void ShowText(int x, int y, string s, int tickSleep = 10, bool clearLine = true)
        {
            var currentColor = (short)ForegroundColor.White;

            int cnt = 0;

            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case 'w':
                        currentColor = (short)ForegroundColor.White;
                        break;
                    case 'r':
                        currentColor = (short)ForegroundColor.Red;
                        break;
                    case 'y':
                        currentColor = (short)ForegroundColor.Yellow;
                        break;
                    case 'd':
                        currentColor = (short)ForegroundColor.IntenseBlack;
                        break;
                    case 'g':
                        currentColor = (short)ForegroundColor.Green;
                        break;
                    case 'b':
                        currentColor = (short)ForegroundColor.Blue;
                        break;
                    /*
                    case 'B':
                        currentColor = (short)ForegroundColor.Black;
                        break;
                    */
                    default:
                        Draw(x + (cnt++), y, new CharInfo { Char = s[i], Attributes = currentColor });
                        // 한국어면 1칸 추가
                        if (IsKorean(s[i]))
                        {
                            Draw(x + (cnt++), y, new CharInfo { Char = ' ', Attributes = currentColor });
                        }
                        break;

                }
                Thread.Sleep(Managers.Game.GetGameSleepTime(tickSleep));
            }
        }


        public static void AddLogLine(string s, int tickSleep = 10)
        {
            ShowText(0, CurrentLogY, s, tickSleep);
            CurrentLogY += 2;
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void AddLog(string s, int x)
        {
            ShowText(x, CurrentLogY, s, 10, false);
        }

        public static void ClearLogLine()
        {
            ClearLine(LogStart, CurrentLogY);
            CurrentLogY = LogStart;

        }

        public static void ConsoleClear()
        {
            ClearLine(0, Renderer.Height - 1);
        }

        public static void DrawKeyGuide(string keyGuide, bool noColor = true)
        {
            ClearLine(2, _width - 1, _height - 2, _height - 2);
            Draw(2, _height - 2, keyGuide, noColor);
        }

        public static int GetPrintingLength(string line)
        {
            int ret = 0;
            foreach (var e in line)
            {
                if (e == 'w' || e == 'r' || e == 'y' || e == 'd' || e == 'g' || e == 'b' /*|| e == 'B' */|| e == 'z' || e == 'n' || e == 'x' || e == 'v' || e == 'V')
                {
                    continue;
                }
                else if (IsKorean(e))
                {
                    ret += 2;
                }
                else
                {
                    ret += 1;
                }
            }
            return ret;
        }
        private static bool IsKorean(char c) => '가' <= c && c <= '힣';

        public static string GetBarString(int value, int maxValue, char color, int barCount = 25)
        {
            //string hp = "HP:x" + new string(' ', 3) + 'w' + new string(' ', 19 - idx) + 'w';

            bool isRated = false;

            float rate = (float)value / (float)maxValue;

            string ret = $"{color}";

            for (int i = 0; i < barCount; i++)
            {
                if ((float)i * (1.0f / (float)barCount) >= rate && isRated == false)
                {
                    isRated = true;
                    ret += 'v';
                }
                ret += ' ';
            }
            ret += 'w';
            return ret;
        }

        public static string GetColorString(Bitmap bit, string str, int pixelY)
        {
            var cStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                Color pixelColor = bit.GetPixel(i * 3, pixelY * 3);

                var c = FindNearestColor(pixelColor);

                switch (c)
                {
                    /*
                    case ForegroundColor.Black:
                        cStr += 'B';
                        break;
                    */
                    case ForegroundColor.Blue:
                        cStr += 'b';
                        break;
                    case ForegroundColor.White:
                        cStr += 'w';
                        break;
                    case ForegroundColor.IntenseBlack:
                        cStr += 'd';
                        break;
                    case ForegroundColor.Green:
                        cStr += 'g';
                        break;
                    case ForegroundColor.Red:
                        cStr += 'r';
                        break;
                    case ForegroundColor.Yellow:
                        cStr += 'y';
                        break;
                    default:
                        cStr += 'y';
                        break;
                }
                cStr += str[i];
            }
            cStr += 'w';

            return cStr;
        }


        // RGB 값 매핑
        private static readonly Dictionary<ForegroundColor, Color> ColorMap = new Dictionary<ForegroundColor, Color>
    {
        { ForegroundColor.Black, Color.FromArgb(0, 0, 0) },
        { ForegroundColor.Blue, Color.FromArgb(0, 0, 255) },
        { ForegroundColor.Green, Color.FromArgb(0, 255, 0) },
        { ForegroundColor.Cyan, Color.FromArgb(0, 255, 255) },
        { ForegroundColor.Red, Color.FromArgb(255, 0, 0) },
        { ForegroundColor.Magenta, Color.FromArgb(255, 0, 255) },
        { ForegroundColor.Yellow, Color.FromArgb(255, 255, 0) },
        { ForegroundColor.White, Color.FromArgb(255, 255, 255) },
        { ForegroundColor.IntenseBlack, Color.FromArgb(128, 128, 128) },
        { ForegroundColor.IntenseBlue, Color.FromArgb(0, 0, 128) },
        { ForegroundColor.IntenseGreen, Color.FromArgb(0, 128, 0) },
        { ForegroundColor.IntenseCyan, Color.FromArgb(0, 128, 128) },
        { ForegroundColor.IntenseRed, Color.FromArgb(128, 0, 0) },
        { ForegroundColor.IntenseMagenta, Color.FromArgb(128, 0, 128) },
        { ForegroundColor.IntenseYellow, Color.FromArgb(128, 128, 0) },
        { ForegroundColor.IntenseWhite, Color.FromArgb(192, 192, 192) }
    };


        // 가장 가까운 색상 찾기
        private static ForegroundColor FindNearestColor(Color color)
        {
            return ColorMap
                .OrderBy(kvp => ColorDistance(color, kvp.Value))
                .First()
                .Key;
        }

        // 색상 간의 유클리드 거리 계산
        private static double ColorDistance(Color c1, Color c2)
        {
            return Math.Sqrt(
                Math.Pow(c1.R - c2.R, 2) +
                Math.Pow(c1.G - c2.G, 2) +
                Math.Pow(c1.B - c2.B, 2));
        }

        public static void Down(string str = "", bool IgnoreSleep = false)
        {

            for (int i = 0; i < Height; i++)
            {
                lock (_buffer)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        _buffer[i * Width + j] = new CharInfo { Char = ' ', Attributes = 0 };
                    }
                    Thread.Sleep(Managers.Game.GetGameSleepTime(2));
                }
            }
            Renderer.DrawCenter(Renderer.Height / 2, str);
            if (IgnoreSleep == false)
            {
                Thread.Sleep(Managers.Game.GetGameSleepTime(500));
            }
            Renderer.ClearLine(Renderer.Height / 2, Renderer.Height / 2);
        }

        public static void DeleteVertical(int sx, int ex, int sy, int ey)
        {

            for (int i = ey; i >= sy; i--)
            {
                lock (_buffer)
                {
                    for (int j = sx; j <= ex; j++)
                    {
                        _buffer[i * Width + j] = _buffer[(i + 1) * Width + j];
                        _buffer[(i + 1) * Width + j] = new CharInfo { Char = ' ', Attributes = 0 };
                    }
                    Thread.Sleep(Managers.Game.GetGameSleepTime(100));
                }
            }
        }

        public static void DeleteHorizontal(int sx, int ex, int sy, int ey)
        {

            for (int i = sx; i <= ex; i++)
            {
                lock (_buffer)
                {
                    for (int j = sy; j <= ey; j++)
                    {
                        _buffer[j * Width + i] = new CharInfo { Char = ' ', Attributes = 0 };
                    }
                    Thread.Sleep(Managers.Game.GetGameSleepTime(10));
                }
            }
        }

        public static void ShowCharacterInfo(int diffAtk = 0, int diffDef = 0, int diffSpeed = 0)
        {
            // ==== 캐릭터 정보 표시 ====

            const int startOffset = 4;
            const int startOffsetX = 4;

            var atkString = $"공격력 : y{Game.Player.DefaultDamage}w";
            var defString = $"방어력 : y{Game.Player.DefaultDefense}w";
            var speedString = $"순발력 : y{Game.Player.DefaultSpeed}w";

            if (Math.Abs(diffAtk) > 0)
            {
                atkString += $" {(diffAtk > 0 ? "g+" : "r-")} {Math.Abs(diffAtk)}w";
            }
            if (Math.Abs(diffDef) > 0)
            {
                defString += $" {(diffDef > 0 ? "g+" : "r-")} {Math.Abs(diffDef)}w";
            }
            if (Math.Abs(diffSpeed) > 0)
            {
                speedString += $" {(diffSpeed > 0 ? "g+" : "r-")} {Math.Abs(diffSpeed)}w";
            }

            Draw(startOffsetX + 6, startOffset, $"캐릭터 정보");
            Draw(startOffsetX, startOffset + 2, $"레벨 : y{Game.Player.Level}w");
            Draw(startOffsetX, startOffset + 4, $"이름 : {Game.Player.Name} ( y{Game.Player.Job.String()}w )");
            Draw(startOffsetX, startOffset + 6, $"체  력 : y{Game.Player.Hp} / {Game.Player.DefaultHpMax}w");
            Draw(startOffsetX, startOffset + 8, $"마  나 : y{Game.Player.Mp} / {Game.Player.DefaultMpMax}w");
            Draw(startOffsetX, startOffset + 10, atkString);
            Draw(startOffsetX, startOffset + 12, defString);
            Draw(startOffsetX, startOffset + 14, speedString);
            Draw(startOffsetX, startOffset + 16, $"경험치 : y{Game.Player.totalExp} / {Game.Player.nextLevelExp}w");
            Draw(startOffsetX, startOffset + 18, $"보유 골드 : y{Game.Player.Inventory.Gold} Gw");

            for (int i = 0; i < Height; i++)
            {
                Draw(startOffsetX + 35, i, new CharInfo { Char = ' ', Attributes = 0x0080 });
            }

        }
        public static void ShakeHorizontal(int sx, int ex, int sy, int ey)
        {
            var buffer = new CharInfo[(ex - sx) * (ey - sy)];

            // 왼쪽 오른쪽 반복
            for (int w = 0; w < 3; w++)
            {
                int cnt = 3;
                if (w == 1) cnt = 6;

                for (int k = 0; k < cnt; k++)
                {
                    if (w % 2 == 0)
                    {
                        lock (_buffer)
                        {
                            sx--;
                            for (int i = sx; i < ex; i++)
                            {
                                for (int j = sy; j <= ey; j++)
                                {

                                    _buffer[j * Width + i] = _buffer[j * Width + i + 1];
                                    _buffer[j * Width + i + 1] = new CharInfo { Char = ' ', Attributes = 0 };
                                }
                            }
                            ex--;
                        }
                    }
                    else
                    {
                        lock (_buffer)
                        {
                            ex++;
                            for (int i = ex; i > sx; i--)
                            {
                                for (int j = sy; j <= ey; j++)
                                {

                                    _buffer[j * Width + i] = _buffer[j * Width + i - 1];
                                    _buffer[j * Width + i - 1] = new CharInfo { Char = ' ', Attributes = 0 };
                                }
                            }
                            sx++;
                        }
                    }
                    Thread.Sleep(Managers.Game.GetGameSleepTime(20));
                }
            }
        }
    }
}
