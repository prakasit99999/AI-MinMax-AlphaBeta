using System;

public class ChessAI
{
    public enum Difficulty { Easy, Medium, Hard }

    public Move FindBestMove(ChessBoard board, Difficulty difficulty)
    {
        SearchAlgorithm algorithm = new Minimax(); // กำหนดค่าเริ่มต้น
        int depth = 1;

        switch (difficulty)
        {
            case Difficulty.Easy:
                algorithm = new Minimax();
                depth = 1;
                break;
            case Difficulty.Medium:
                //algorithm = new AlphaBetaPruning();
                //depth = 3;
                break;
            case Difficulty.Hard:
                //algorithm = new AlphaBetaPruning();
                //depth = 5;
                break;
            default:
                throw new ArgumentException("Invalid difficulty level");
        }

        return algorithm.FindBestMove(board, depth);
    }
}