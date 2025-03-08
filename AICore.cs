using System;

public class ChessAI
{
    public enum Difficulty { Easy, Medium, Hard }

    // ======== ค้นหาการเดินที่ดีที่สุดตามระดับความยาก ========
    public Move FindBestMove(ChessBoard board, Difficulty difficulty)
    {
        SearchAlgorithm algorithm;
        int depth;

        switch (difficulty)
        {
            case Difficulty.Easy:
                algorithm = new Minimax();
                depth = 1; // ระดับง่าย: ความลึก 1
                break;
            case Difficulty.Medium:
                algorithm = new AlphaBetaPruning();
                depth = 3; // ระดับกลาง: ความลึก 3
                break;
            case Difficulty.Hard:
                algorithm = new AlphaBetaPruning();
                depth = 5; // ระดับยาก: ความลึก 5
                break;
            default:
                throw new ArgumentException("Invalid difficulty level");
        }

        return algorithm.FindBestMove(board, depth);
    }
}