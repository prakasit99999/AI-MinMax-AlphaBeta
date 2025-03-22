using System.Collections.Generic;

public class Evaluation
{
    // ========== ค่าของตัวหมาก ==========
    private static readonly Dictionary<int, int> PieceValues = new Dictionary<int, int>
    {
        { 1, 100 },   // White Pawn
        { -1, -100 },  // Black Pawn
        { 2, 300 },   // White Knight
        { -2, -300 },  // Black Knight
        { 3, 325 },   // White Bishop (Bishop Pair ได้เปรียบ)
        { -3, -325 },  // Black Bishop
        { 4, 500 },   // White Rook
        { -4, 500 },  // Black Rook
        { 5, 900 },   // White Queen
        { -5, -900 },  // Black Queen
        { 6, 10000 }, // White King
        { -6, -10000 } // Black King
    };

    // ========== คะแนนตำแหน่งหมาก ==========
    // คะแนนตำแหน่งเบี้ย (เดิม)
    private static readonly int[,] PawnPositionScore = new int[8, 8]
    {
    { 0, 0, 0, 0, 0, 0, 0, 0 },  // แถว 0 (มี 8 ตัวเลข)
    { 5, 5, 5, 5, 5, 5, 5, 5 },  // แถว 1
    { 1, 1, 2, 3, 3, 2, 1, 1 },  // แถว 2
    { 0, 0, 0, 2, 2, 0, 0, 0 },  // แถว 3
    { 0, 0, 0, 1, 1, 0, 0, 0 },  // แถว 4
    { 1, -1, -2, 0, 0, -2, -1, 1 },  // แถว 5
    { 1, 2, 2, -2, -2, 2, 2, 1 },  // แถว 6
    { 0, 0, 0, 0, 0, 0, 0, 0 }   // แถว 7
    };
    // คะแนนตำแหน่ง Knight
    private static readonly int[,] KnightPositionScore = new int[8, 8]
    {
        { -50, -40, -30, -30, -30, -30, -40, -50 },
        { -40, -20,   0,   5,   5,   0, -20, -40 },
        { -30,   0,  10,  15,  15,  10,   0, -30 },
        { -30,   5,  15,  20,  20,  15,   5, -30 },
        { -30,   0,  15,  20,  20,  15,   0, -30 },
        { -30,   5,  10,  15,  15,  10,   5, -30 },
        { -40, -20,   0,   5,   5,   0, -20, -40 },
        { -50, -40, -30, -30, -30, -30, -40, -50 }
    };

    // คะแนนตำแหน่ง Bishop
    private static readonly int[,] BishopPositionScore = new int[8, 8]
    {
        { -20, -10, -10, -10, -10, -10, -10, -20 },
        { -10,   0,   0,   0,   0,   0,   0, -10 },
        { -10,   0,   5,  10,  10,   5,   0, -10 },
        { -10,   5,   5,  10,  10,   5,   5, -10 },
        { -10,   0,  10,  10,  10,  10,   0, -10 },
        { -10,  10,  10,  10,  10,  10,  10, -10 },
        { -10,   5,   0,   0,   0,   0,   5, -10 },
        { -20, -10, -10, -10, -10, -10, -10, -20 }
    };

    // ========== กลยุทธ์เพิ่มเติม ==========
    public static int Evaluate(ChessBoard board)
    {
        int score = 0;
        bool isEndgame = IsEndgame(board); // ตรวจสอบว่าเป็นเกม Endgame หรือไม่

        // 1. คะแนน Material (เดิม)
        score += CalculateMaterialScore(board);

        // 2. คะแนนตำแหน่งหมาก (ปรับตามประเภท)
        score += CalculatePositionalScore(board, isEndgame);

        // 3. Mobility (จำนวนการเดินที่เป็นไปได้)
        score += CalculateMobilityScore(board);

        // 4. King Safety (ความปลอดภัยของ King)
        score += CalculateKingSafety(board, isEndgame);

        // 5. Pawn Structure (โครงสร้างเบี้ย)
        score += EvaluatePawnStructure(board);

        return board.IsWhiteTurn ? score : -score;
    }

    // ========== 1. คำนวณ Material Score ==========
    private static int CalculateMaterialScore(ChessBoard board)
    {
        int material = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = board.Board[x, y];
                material += PieceValues.TryGetValue(piece, out int value) ? value : 0;
            }
        }
        return material;
    }

    // ========== 2. คะแนนตำแหน่งหมาก ==========
    private static int CalculatePositionalScore(ChessBoard board, bool isEndgame)
    {
        int positionalScore = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = board.Board[x, y];
                int absPiece = Math.Abs(piece);
                int sign = piece > 0 ? 1 : -1;

                // ปรับตำแหน่งตามสี (White = ด้านล่าง, Black = ด้านบน)
                int evalX = piece > 0 ? x : 7 - x;

                switch (absPiece)
                {
                    case 1: // เบี้ย
                        positionalScore += sign * PawnPositionScore[evalX, y];
                        break;
                    case 2: // ม้า
                        positionalScore += sign * KnightPositionScore[evalX, y];
                        break;
                    case 3: // บิชอป
                        positionalScore += sign * BishopPositionScore[evalX, y];
                        break;
                    case 6: // ราชา (Endgame ให้เดิน King เข้าสู่ศูนย์กลาง)
                        if (isEndgame)
                            positionalScore += sign * (Math.Abs(3 - x) + Math.Abs(3 - y)) * (-10); // ยิ่งใกล้ศูนย์กลางยิ่งดี
                        break;
                }
            }
        }
        return positionalScore;
    }

    // ========== 3. คำนวณ Mobility (การเคลื่อนไหวของหมาก) ==========
    private static int CalculateMobilityScore(ChessBoard board)
    {
        int mobility = 0;
        List<Move> moves = MoveGenerator.GenerateMoves(board);
        foreach (Move move in moves)
        {
            int piece = Math.Abs(board.Board[move.FromX, move.FromY]);
            // ให้คะแนนตามประเภทหมาก (Queen สูงสุด, Pawn ต่ำสุด)
            mobility += piece switch
            {
                5 => 3,   // Queen
                4 => 2,   // Rook
                2 or 3 => 1, // Knight/Bishop
                _ => 0     // หมากอื่นไม่นับ
            };
        }
        return mobility;
    }

    // ========== 4. ตรวจสอบความปลอดภัยของ King ==========
    private static int CalculateKingSafety(ChessBoard board, bool isEndgame)
    {
        int safetyScore = 0;
        int whiteKingX = -1, whiteKingY = -1;
        int blackKingX = -1, blackKingY = -1;

        // หาตำแหน่ง King
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board.Board[x, y] == 6)
                {
                    whiteKingX = x;
                    whiteKingY = y;
                }
                else if (board.Board[x, y] == -6)
                {
                    blackKingX = x;
                    blackKingY = y;
                }
            }
        }

        // ตรวจสอบ Castling และ Pawn Shield (เฉพาะไม่ใช่ Endgame)
        if (!isEndgame)
        {
            // White King
            if (whiteKingX == 0 && whiteKingY == 4) // ยังไม่ Castling
                safetyScore -= 50;
            else if (whiteKingX == 0 && whiteKingY == 6) // Kingside Castling
                safetyScore += 30;

            // Black King
            if (blackKingX == 7 && blackKingY == 4)
                safetyScore += 50;
            else if (blackKingX == 7 && blackKingY == 6)
                safetyScore -= 30;
        }

        return safetyScore;
    }

    // ========== 5. ตรวจสอบโครงสร้างเบี้ย ==========
    private static int EvaluatePawnStructure(ChessBoard board)
    {
        int pawnScore = 0;
        // ตรวจสอบ Isolated Pawns (เบี้ยโดดเดี่ยว)
        for (int y = 0; y < 8; y++)
        {
            bool whiteIsolated = true, blackIsolated = true;
            for (int x = 0; x < 8; x++)
            {
                if (board.Board[x, y] == 1)
                {
                    // ตรวจสอบเบี้ยขาว: มีเบี้ยเพื่อนบ้านในแถวข้างเคียงหรือไม่
                    if ((y > 0 && HasPawnInFile(board, y - 1, 1)) ||
                        (y < 7 && HasPawnInFile(board, y + 1, 1)))
                        whiteIsolated = false;
                }
                else if (board.Board[x, y] == -1)
                {
                    // ตรวจสอบเบี้ยดำ
                    if ((y > 0 && HasPawnInFile(board, y - 1, -1)) ||
                        (y < 7 && HasPawnInFile(board, y + 1, -1)))
                        blackIsolated = false;
                }
            }
            if (whiteIsolated) pawnScore -= 20;
            if (blackIsolated) pawnScore += 20;
        }
        return pawnScore;
    }

    private static bool HasPawnInFile(ChessBoard board, int file, int pawnType)
    {
        for (int x = 0; x < 8; x++)
        {
            if (board.Board[x, file] == pawnType)
                return true;
        }
        return false;
    }

    // ========== ตรวจสอบว่าเป็นเกม Endgame ==========
    private static bool IsEndgame(ChessBoard board)
    {
        int totalMaterial = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = Math.Abs(board.Board[x, y]);
                if (piece == 0) continue; // ข้ามช่องว่าง
                if (piece == 5 || piece == 4) // Queen หรือ Rook ยังอยู่
                    return false;
                totalMaterial += PieceValues[piece];
            }
        }
        return totalMaterial < 2000;
    }
}