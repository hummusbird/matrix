using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace matrix
{
    class Program
    {
        public static bool perfmode = false;
        public static ConsoleColor fg = ConsoleColor.DarkGreen;
        public static ConsoleColor bg = ConsoleColor.Green;
        public static Random rnd = new Random();
        public static int asc1 = 32, asc2 = 256;

        public static int len1 = 5, len2 = 25;

        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                int pos = Array.FindIndex(args, v => v == arg);

                switch (arg)
                {
                    case "-c": // colour
                        if (args.Length >= pos + 2)
                        {
                            switch (args[pos + 1].ToLower())
                            {
                                case "red":
                                    fg = ConsoleColor.Red;
                                    bg = ConsoleColor.DarkRed;
                                    break;
                                case "blue":
                                    fg = ConsoleColor.Blue;
                                    bg = ConsoleColor.DarkBlue;
                                    break;
                                case "purple":
                                    fg = ConsoleColor.Magenta;
                                    bg = ConsoleColor.DarkMagenta;
                                    break;
                                case "gray":
                                    fg = ConsoleColor.Gray;
                                    bg = ConsoleColor.DarkGray;
                                    break;
                                case "cyan":
                                    fg = ConsoleColor.Cyan;
                                    bg = ConsoleColor.DarkCyan;
                                    break;
                                case "yellow":
                                    fg = ConsoleColor.Yellow;
                                    bg = ConsoleColor.DarkYellow;
                                    break;
                            }
                        }
                        break;
                    case "-P": // performance mode (doesn't randomise chars or draw fadeout)
                        perfmode = true;
                        break;
                    case "-ascii": // ascii charset
                        if (args.Length >= pos + 2)
                        {
                            try
                            {
                                Program.asc1 = int.Parse(args[pos + 1].Split(',')[0]);
                                Program.asc2 = int.Parse(args[pos + 1].Split(',')[1]);
                            }
                            catch { }
                        }
                        break;
                    case "-l": // rain length
                        if (args.Length >= pos + 2)
                        {
                            try
                            {
                                Program.len1 = int.Parse(args[pos + 1].Split(',')[0]);
                                Program.len2 = int.Parse(args[pos + 1].Split(',')[0]);
                                Program.len2 = int.Parse(args[pos + 1].Split(',')[1]);
                            }
                            catch { }
                        }
                        break;
                }
            }

            Console.CursorVisible = false;

            List<Line> lines = new List<Line>();

            for (int i = 0; i < Console.BufferWidth -1; i++)
            {
                Line line = new Line(i);
                lines.Add(line);
            }

            while (true)
            {
               foreach (Line line in lines)
               {
                    line.DrawLine();
               }
            }
        }
    }

    public class Window
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        static readonly IntPtr ptr = Process.GetCurrentProcess().MainWindowHandle;
        static Rect WindowRect = new Rect();

        public static Rect GetPosition() // Get the console window's current position
        {
            GetWindowRect(ptr, ref WindowRect);
            return WindowRect;
        }
    }

    public class Line
    {
        static int delay = 100;
        int loop = Program.rnd.Next(Program.len1, Program.len2);
        int row;
        int column = 0 - Program.rnd.Next(delay);
        List<char> charlist = new List<char>();

        public Line(int _row)
        {
            row = _row;
            
            for (int i = 0; i < 250; i++)
            {
                charlist.Add(Convert.ToChar(Program.rnd.Next(Program.asc1,Program.asc2)));
            }
        }

        public void DrawLine()
        {
            for (int i = 0; i < (loop < column ? loop : column); i++)
            {
                if (!Program.perfmode) { charlist[Program.rnd.Next(0, 250)] = Convert.ToChar(Program.rnd.Next(Program.asc1, Program.asc2)); }

                if (column - i < (Window.GetPosition().Bottom - Window.GetPosition().Top - 39) / 16)
                {
                    bool run = !Program.perfmode;

                    ConsoleColor colour = Program.fg;
                    if (i == loop - 1) { colour = ConsoleColor.Black; run = true; }
                    else if (i == loop - 2) { colour = Program.bg; }
                    else if (i == 1) { run = true; }
                    else if (i == 0) { colour = ConsoleColor.White; run = true; }


                    if (run) 
                    {
                        Console.ForegroundColor = colour;
                        Console.SetCursorPosition(row, column - i - 1); 
                        Console.WriteLine(charlist[column - i]); 
                    }
                }
            }
            
            column++;

            if (column - loop > (Window.GetPosition().Bottom - Window.GetPosition().Top - 39) / 16)
            {
                column = 0 - Program.rnd.Next(delay);
                loop = Program.rnd.Next(Program.len1, Program.len2);
            }
        }
    }
}
