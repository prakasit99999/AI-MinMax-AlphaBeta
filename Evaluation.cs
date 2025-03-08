public class Evaluation
{
    // ค่าของตัวหมาก (ปรับตามความสำคัญ)
// ใน Evaluation.cs
private static readonly Dictionary<int, int> PieceValues = new Dictionary<int, int>
{
    { 1, 100 },   // White Pawn
    { -1, 100 },  // Black Pawn
    { 2, 300 },   // White Knight
    { -2, 300 },  // Black Knight
    { 3, 325 },   // White Bishop
    { -3, 325 },  // Black Bishop
    { 4, 500 },   // White Rook
    { -4, 500 },  // Black Rook
    { 5, 900 },   // White Queen
    { -5, 900 },  // Black Queen
    { 6, 10000 }, // White King
    { -6, 10000 } // Black King
};

    public static int Evaluate(ChessBoard board)
    {
        int score = 0;
        // คำนวณ Material Score (เดิม)
        // เพิ่ม Positional Score
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = board.Board[x, y];
                if (Math.Abs(piece) == 1) // เบี้ย
                    score += (piece > 0) ? PawnPositionScore[x, y] : -PawnPositionScore[7 - x, y];
            }
        }
        return board.IsWhiteTurn ? score : -score;
    }

}