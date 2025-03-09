using System;

public class MockBoard : ChessBoard
{
    // ======== Constructor ========
    public MockBoard(int[,] customBoard, bool isWhiteTurn)
    {
        Board = customBoard;
        IsWhiteTurn = isWhiteTurn; // ตอนนี้สามารถตั้งค่าได้แล้ว
    }

    // ======== แสดงกระดานในคอนโซล ========
    public new void PrintBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Console.Write($"{Board[x, y],3}"); // จัดรูปแบบให้สวยงาม
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Turn: {(IsWhiteTurn ? "White" : "Black")}\n");
    }
}