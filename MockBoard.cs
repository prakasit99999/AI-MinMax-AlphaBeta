public class MockBoard : ChessBoard
{
    public MockBoard(int[,] customBoard, bool isWhiteTurn)
    {
        Board = customBoard;
        IsWhiteTurn = isWhiteTurn;
    }
    // ฟังก์ชันแสดงกระดานในคอนโซลเพื่อตรวจสอบ
// ========= ฟังก์ชันหลัก =========
    public static int Evaluate(ChessBoard board)
    {
        int materialScore = 0;
        int positionalScore = 0;

        // 1. คำนวณ Material Score
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = board.Board[x, y];
                materialScore += PieceValues.GetValueOrDefault(piece, 0);
            }
        }

        // 2. คำนวณ Positional Score
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = board.Board[x, y];
                int absPiece = Math.Abs(piece);

                // เบี้ย
                if (absPiece == 1)
                {
                    positionalScore += (piece > 0)
                        ? PawnPositionScore[x, y]   // White
                        : -PawnPositionScore[7 - x, y]; // Black (mirror)
                }

                // ม้า
                else if (absPiece == 2)
                {
                    positionalScore += (piece > 0)
                        ? KnightPositionScore[x, y]
                        : -KnightPositionScore[7 - x, y];
                }
            }
        }

        // 3. ปัจจัยอื่นๆ (เช่น King Safety, Mobility)
        

        // คืนค่าคะแนนตามผู้เล่นปัจจุบัน
        return board.IsWhiteTurn 
            ? (materialScore + positionalScore) 
            : -(materialScore + positionalScore);
    }
}