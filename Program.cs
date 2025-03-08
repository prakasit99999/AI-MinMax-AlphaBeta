using System;

class Program
{
    static void Main()
    {
        // สร้างกระดานเริ่มต้น
        ChessBoard board = new ChessBoard();
        ChessAI ai = new ChessAI();

        // ตัวอย่าง: AI ระดับง่าย (Easy)
        Console.WriteLine("=== ระดับง่าย (Easy) ===");
        Move easyMove = ai.FindBestMove(board, ChessAI.Difficulty.Easy);
        PrintMove(easyMove);
        board.MakeMove(easyMove);
        PrintBoard(board);

        // ตัวอย่าง: AI ระดับกลาง (Medium)
        Console.WriteLine("\n=== ระดับกลาง (Medium) ===");
        Move mediumMove = ai.FindBestMove(board, ChessAI.Difficulty.Medium);
        PrintMove(mediumMove);
        board.MakeMove(mediumMove);
        PrintBoard(board);

        // ตัวอย่าง: AI ระดับยาก (Hard) - ใช้กระดานเทียม
        Console.WriteLine("\n=== ระดับยาก (Hard) ===");
        int[,] customBoard = new int[8, 8]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 5, 0, 0, 0, 0, 0 }, // WhiteQueen ที่ (5,2)
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { -6, 0, 0, 0, 0, 0, 0, 0 }  // BlackKing ที่ (7,0)
        };
        MockBoard mockBoard = new MockBoard(customBoard, isWhiteTurn: true);
        Move hardMove = ai.FindBestMove(mockBoard, ChessAI.Difficulty.Hard);
        PrintMove(hardMove);
        mockBoard.MakeMove(hardMove);
        PrintBoard(mockBoard);
    }

    static void PrintMove(Move move)
    {
        Console.WriteLine($"การเดินที่ดีที่สุด: ({move.FromX}, {move.FromY}) -> ({move.ToX}, {move.ToY})");
    }

    static void PrintBoard(ChessBoard board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Console.Write(board.Board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
}
