using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== หมากรุก AI ===");
            Console.WriteLine("1. Player vs AI");
            Console.WriteLine("2. AI vs AI");
            Console.WriteLine("3. ออก");
            Console.Write("เลือกโหมด: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    PlayPlayerVsAI();
                    break;
                case "2":
                    PlayAIVsAI();
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("คำสั่งไม่ถูกต้อง!");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // ======== โหมด Player vs AI ========
    static void PlayPlayerVsAI()
    {
        // เลือกระดับความยาก
        AICore.Difficulty difficulty = SelectDifficulty();

        ChessBoard board = new ChessBoard();
        AICore ai = new AICore();

        while (!board.IsGameOver())
        {
            Console.Clear();
            board.PrintBoard();

            if (board.IsWhiteTurn)
            {
                // ผู้เล่นเดิน (ขาว)
                Console.WriteLine("=== ตาของคุณ (White) ===");
                Console.Write("การเดินของคุณ (เช่น e2e4): ");
                string moveInput = Console.ReadLine().ToLower();

                if (ParseMove(moveInput, out Move playerMove))
                {
                    if (IsLegalMove(board, playerMove))
                    {
                        board.MakeMove(playerMove);
                    }
                    else
                    {
                        Console.WriteLine("การเดินไม่ถูกต้อง! กดปุ่มเพื่อลองใหม่...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("รูปแบบไม่ถูกต้อง! กดปุ่มเพื่อลองใหม่...");
                    Console.ReadKey();
                }
            }
            else
            {
                // AI เดิน (ดำ) ตามระดับความยากที่เลือก
                Console.WriteLine("=== AI กำลังคิด... ===");
                Move aiMove = ai.FindBestMove(board, difficulty);
                board.MakeMove(aiMove);
                PrintMove(aiMove, "AI");
                Console.ReadKey();
            }
        }

        Console.Clear();
        Console.WriteLine("=== จบเกม ===");
        board.PrintBoard();
        Console.WriteLine("กดปุ่มเพื่อกลับสู่เมนูหลัก...");
        Console.ReadKey();
    }

    // ======== โหมด AI vs AI ========
    static void PlayAIVsAI()
    {
        // เลือกระดับความยากสำหรับ White AI และ Black AI
        AICore.Difficulty whiteDifficulty = SelectDifficulty("White AI");
        AICore.Difficulty blackDifficulty = SelectDifficulty("Black AI");

        ChessBoard board = new ChessBoard();
        AICore whiteAI = new AICore();
        AICore blackAI = new AICore();

        while (!board.IsGameOver())
        {
            Console.Clear();
            board.PrintBoard();

            Move aiMove;
            if (board.IsWhiteTurn)
            {
                aiMove = whiteAI.FindBestMove(board, whiteDifficulty);
                PrintMove(aiMove, $"White AI (ระดับ {whiteDifficulty})");
            }
            else
            {
                aiMove = blackAI.FindBestMove(board, blackDifficulty);
                PrintMove(aiMove, $"Black AI (ระดับ {blackDifficulty})");
            }

            board.MakeMove(aiMove);
            Console.WriteLine("กดปุ่มเพื่อดำเนินการต่อ...");
            Console.ReadKey();
        }

        Console.Clear();
        Console.WriteLine("=== จบเกม ===");
        board.PrintBoard();
        Console.WriteLine("กดปุ่มเพื่อกลับสู่เมนูหลัก...");
        Console.ReadKey();
    }

    // ======== ตรวจสอบการเดินที่ถูกต้อง ========
    static bool IsLegalMove(ChessBoard board, Move move)
    {
        List<Move> legalMoves = MoveGenerator.GenerateMoves(board);
        foreach (Move legalMove in legalMoves)
        {
            if (legalMove.FromX == move.FromX && legalMove.FromY == move.FromY &&
                legalMove.ToX == move.ToX && legalMove.ToY == move.ToY)
            {
                return true;
            }
        }
        return false;
    }

    // ======== แปลงคำสั่งผู้เล่นเป็น Move Object ========
    static bool ParseMove(string input, out Move move)
    {
        move = null;
        input = input.Replace(" ", "").Replace("-", "");
        if (input.Length != 4) return false;

        try
        {
            // แปลง File (ตัวอักษร) เป็นคอลัมน์ (y)
            int fromY = input[0] - 'a'; // a=0, b=1, ..., h=7
            int toY = input[2] - 'a';

            // แปลง Rank (ตัวเลข) เป็นแถว (x)
            int fromX = 8 - (input[1] - '0'); // 8 - rank (chess) = array index
            int toX = 8 - (input[3] - '0');

            move = new Move(fromX, fromY, toX, toY);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ======== เลือกระดับความยาก ========
    static AICore.Difficulty SelectDifficulty()
    {
        Console.Clear();
        Console.WriteLine("=== เลือกระดับความยาก ===");
        Console.WriteLine("1. ง่าย (Easy)");
        Console.WriteLine("2. ปานกลาง (Medium)");
        Console.WriteLine("3. ยาก (Hard)");
        Console.Write("เลือก: ");
        string input = Console.ReadLine();

        switch (input)
        {
            case "1": return AICore.Difficulty.Easy;
            case "2": return AICore.Difficulty.Medium;
            case "3": return AICore.Difficulty.Hard;
            default:
                Console.WriteLine("ใช้ระดับยากเป็นค่าเริ่มต้น");
                return AICore.Difficulty.Easy;
        }
    }
    // ======== เลือกระดับความยากสำหรับ AI ========
    static AICore.Difficulty SelectDifficulty(string aiName)
    {
        Console.Clear();
        Console.WriteLine($"=== ระดับความยากของ {aiName} ===");
        Console.WriteLine("1. ง่าย (Easy)");
        Console.WriteLine("2. ปานกลาง (Medium)");
        Console.WriteLine("3. ยาก (Hard)");
        Console.Write("เลือก: ");
        string input = Console.ReadLine();

        switch (input)
        {
            case "1": return AICore.Difficulty.Easy;
            case "2": return AICore.Difficulty.Medium;
            case "3": return AICore.Difficulty.Hard;
            default:
                Console.WriteLine("ใช้ระดับยากเป็นค่าเริ่มต้น");
                return AICore.Difficulty.Hard;
        }
    }

    // ======== แสดงการเดิน ========
    static void PrintMove(Move move, string player)
    {
        string from = $"{(char)('a' + move.FromY)}{8 - move.FromX}";
        string to = $"{(char)('a' + move.ToY)}{8 - move.ToX}";
        Console.WriteLine($"{player} เดิน: {from} -> {to}");
    }
}