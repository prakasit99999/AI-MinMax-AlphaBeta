// Alpha-Beta.cs
using System;

namespace AI_MinMax_AlphaBeta.Alpha_Beta
{
    internal class AlphaBeta : SearchAlgorithm, Minimax
    {
        public override Move FindBestMove(ChessBoard board, int depth)
        {
            List<Move> moves = MoveGenerator.GenerateMoves(board);
            Move bestMove = moves[0];
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (Move move in moves)
            {
                ChessBoard newBoard = board.Clone();
                newBoard.MakeMove(move);
                int score = AlphaBetaRecursive(newBoard, depth - 1, alpha, beta, false);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, bestScore);
            }
            return bestMove;
        }

        private int AlphaBetaRecursive(ChessBoard board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0 || board.IsGameOver())
                return base.MinimaxRecursive(board, depth, isMaximizing); // เรียกใช้ Minimax จากคลาสแม่

            List<Move> moves = MoveGenerator.GenerateMoves(board);
            int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

            foreach (Move move in moves)
            {
                ChessBoard newBoard = board.Clone();
                newBoard.MakeMove(move);
                int score = AlphaBetaRecursive(newBoard, depth - 1, alpha, beta, !isMaximizing);

                if (isMaximizing)
                {
                    bestScore = Math.Max(bestScore, score);
                    alpha = Math.Max(alpha, bestScore);
                }
                else
                {
                    bestScore = Math.Min(bestScore, score);
                    beta = Math.Min(beta, bestScore);
                }

                if (beta <= alpha)
                    break; // Alpha-Beta Pruning
            }
            return bestScore;
        }
    }
}