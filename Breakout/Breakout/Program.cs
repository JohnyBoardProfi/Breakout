using System;
using System.Collections.Generic;
using System.Threading;

namespace Breakout
{
    class Program
    {
        static object lockObject = new object();
        static int firstPlayerPadSize = 10, ballPositionX = 0, ballPositionY = 0, ballDirectionX = -1, ballDirectionY = 1, firstPlayerPosition = 0, point = 0;
        static bool[,] bricks = new bool[10, 4];

        static void SetBallAtTheMiddleOfTheGameField()
        {
            ballPositionX = Console.WindowWidth / 2;
            ballPositionY = Console.WindowHeight / 2;
        }

        static void RemoveScrollBars()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }

        static void Init()
        {
            point = 0;
            bricks = new bool[10, 4];
            RemovePaddle();
            firstPlayerPosition = Console.WindowWidth / 2 - firstPlayerPadSize / 2;
            SetBallAtTheMiddleOfTheGameField();
            DrawBricks();
            DrawPaddle();
        }

        private static void MoveBall()
        {
            ballPositionX += ballDirectionX;
            ballPositionY += ballDirectionY;
            for (int i = 0; i < bricks.GetLength(0); i++)
            {
                for (int j = 0; j < bricks.GetLength(1); j++)
                {
                    if (bricks[i, j] && ballPositionX >= (i * 5) && ballPositionX <= (i * 5 + 4) && ballPositionY == addHeightToBricks(j))
                    {
                        ballDirectionY *= -1;
                        bricks[i, j] = false;
                        point++;
                        if (point == 40)
                        {
                            Init();
                            return;
                        }
                        for (int k = 0; k < 5; k++)
                        {
                            PrintAtPosition((i * 5) + k, addHeightToBricks(j), ' ');                        
                        }
                    }

                }
            }
            if (ballPositionY == 0)
            {
                ballDirectionY = 1;
            }
            if (ballPositionX > Console.WindowWidth - 2 || ballPositionX == 0)
            {
                ballDirectionX = -1 * ballDirectionX;
            }
            if (ballPositionY > Console.WindowHeight - 3)
            {
                if (firstPlayerPosition <= ballPositionX && firstPlayerPosition + firstPlayerPadSize >= ballPositionX)
                {
                    ballDirectionY = -1 * ballDirectionY;
                }
                else
                {
                    Init();
                }
            }
        }
        static void PrintAtPosition(int x, int y, char symbol)
        {
            lock (lockObject)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(symbol);
            }
        }
        static Queue<int[]> ballPath = new Queue<int[]>();

        static void RemoveBall()
        {
            int[] a;
            while (ballPath.Count > 0)
            {
                a = ballPath.Dequeue();
                PrintAtPosition(a[0], a[1], ' ');
            }
        }
        static void DrawBall()
        {
            PrintAtPosition(ballPositionX, ballPositionY, '*');
            ballPath.Enqueue(new int[] { ballPositionX, ballPositionY });
        }
        static void CursorAtTheStart() => Console.SetCursorPosition(0, 0);

        static void RemovePaddle()
        {
            for (int x = firstPlayerPosition; x < firstPlayerPosition + firstPlayerPadSize; x++)
            {
                PrintAtPosition(x, Console.WindowHeight - 1, ' ');
            }
        }

        static void DrawPaddle()
        {
            for (int x = firstPlayerPosition; x < firstPlayerPosition + firstPlayerPadSize; x++)
            {
                PrintAtPosition(x, Console.WindowHeight - 1, '_');
            }
        }
        static int addHeightToBricks(int j) => j + 5;

        static void DrawBricks()
        {
            for (int i = 0; i < bricks.GetLength(0); i++)
            {
                for (int j = 0; j < bricks.GetLength(1); j++)
                {
                    bricks[i, j] = true;
                    PrintAtPosition((i * 5), addHeightToBricks(j), '■');
                    for (int k = 1; k < 4; k++)
                    {
                        PrintAtPosition((i * 5) + k, addHeightToBricks(j), '■');
                    }
                    PrintAtPosition((i * 5) + 4, addHeightToBricks(j), '■');
                }
            }
        }

        static void paddleRight() => firstPlayerPosition += Convert.ToInt32(firstPlayerPosition < Console.WindowWidth - firstPlayerPadSize - 2);

        static void paddleLeft() => firstPlayerPosition -= Convert.ToInt32(firstPlayerPosition > 0);

        static void Main()
        {
            Console.WindowWidth = 50;
            Console.BufferWidth = 50;
            RemoveScrollBars();
            Init();
            new Thread(() =>
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        RemovePaddle();
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            paddleRight();
                        }
                        if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            paddleLeft();
                        }
                        DrawPaddle();
                    }
                }
            }).Start();
            CursorAtTheStart();
            while (true)
            {
                RemoveBall();
                MoveBall();
                DrawBall();
                Thread.Sleep(100);
            }
        }
    }
}
