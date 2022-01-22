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
        public static void ThreadLine()
        {
            Line line = new Line(Program.rnd.Next(0, Console.BufferWidth - 1));
            while (true)
            {
                line.DrawLine();
                Thread.Sleep(50);
            }
        }

        public static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            List<Line> lines = new List<Line>();

            for (int i = 0; i < Console.BufferWidth -1; i++)
            {
                //Thread t = new Thread(new ThreadStart(ThreadLine));
                //t.Start();
                Line line = new Line(i); //rnd.Next(0, Console.BufferWidth - 1));
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
        int loop = Program.rnd.Next(5, 25);
        int row;
        int column = 0 - Program.rnd.Next(delay);
        List<char> charlist = new List<char>();

        public Line(int _row)
        {
            row = _row;
            
            for (int i = 0; i < 250; i++)
            {
                charlist.Add(Convert.ToChar(Program.rnd.Next(32,256)));
            }
        }

        public void DrawLine()
        {
            for (int i = 0; i < (loop < column ? loop : column); i++)
            {
                charlist[Program.rnd.Next(0, 250)] = Convert.ToChar(Program.rnd.Next(32,256));

                if (column - i < (Window.GetPosition().Bottom - Window.GetPosition().Top - 39) / 16)
                {
                    Console.SetCursorPosition(row, column - i - 1);

                    bool run = true;

                    ConsoleColor colour = ConsoleColor.DarkGreen;
                    if (i == loop - 1) { colour = ConsoleColor.Black; run = true; }
                    else if (i == loop - 2) { colour = ConsoleColor.Green; run = true; }
                    else if (i == 1) { run = true; }
                    else if (i == 0) { colour = ConsoleColor.White; run = true; }

                    Console.ForegroundColor = colour;
                    if (run) { Console.WriteLine(charlist[column - i]); }
                }
            }
            
            column++;

            if (column - loop > (Window.GetPosition().Bottom - Window.GetPosition().Top - 39) / 16)
            {
                column = 0 - Program.rnd.Next(delay);
                loop = Program.rnd.Next(5, 25);
            }
        }
    }
}
