using System.Collections.Generic;

public static class MoveGenerator
{
    public static List<Move> GenerateMoves(ChessBoard board)
    {
        List<Move> moves = new List<Move>();
        int[,] currentBoard = board.Board;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                int piece = currentBoard[x, y];
                if (piece == 0) continue;

                bool isCurrentPlayerPiece = (board.IsWhiteTurn && piece > 0) || (!board.IsWhiteTurn && piece < 0);
                if (!isCurrentPlayerPiece) continue;

                switch (Math.Abs(piece))
                {
                    case 1: // เบี้ย
                        GeneratePawnMoves(board, x, y, moves);
                        break;
                    case 2: // ม้า
                        GenerateKnightMoves(board, x, y, moves);
                        break;
                    case 3: // บิชอป
                        GenerateBishopMoves(board, x, y, moves);
                        break;
                    case 4: // เรือ
                        GenerateRookMoves(board, x, y, moves);
                        break;
                    case 5: // ควีน
                        GenerateQueenMoves(board, x, y, moves);
                        break;
                    case 6: // ราชา
                        GenerateKingMoves(board, x, y, moves);
                        break;
                }
            }
        }
        return moves;
    }

    // ========== ฟังก์ชันสร้างการเดินของหมากแต่ละประเภท ==========
    private static void GeneratePawnMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int direction = board.IsWhiteTurn ? -1 : 1; // หมากขาวเดินขึ้น (x ลดลง)
        int startRow = board.IsWhiteTurn ? 6 : 1; // แถวเริ่มต้นของเบี้ย

        // เดินหน้า 1 ช่อง
        int newX = x + direction;
        if (newX >= 0 && newX < 8 && board.Board[newX, y] == 0)
        {
            AddPawnMove(board, x, y, newX, y, moves);
        }

        // เดินหน้า 2 ช่อง (เริ่มต้น)
        if (x == startRow && board.Board[newX, y] == 0 && board.Board[newX + direction, y] == 0)
        {
            AddPawnMove(board, x, y, newX + direction, y, moves);
        }

        // โจมตีทแยง
        int[] captureY = { y - 1, y + 1 };
        foreach (int cy in captureY)
        {
            if (cy < 0 || cy >= 8) continue;
            int targetX = x + direction;
            if (targetX < 0 || targetX >= 8) continue;

            int targetPiece = board.Board[targetX, cy];
            if (targetPiece != 0 && ((board.IsWhiteTurn && targetPiece < 0) || (!board.IsWhiteTurn && targetPiece > 0)))
            {
                AddPawnMove(board, x, y, targetX, cy, moves);
            }
        }
    }

    private static void AddPawnMove(ChessBoard board, int fromX, int fromY, int toX, int toY, List<Move> moves)
    {
        if (toX == 0 || toX == 7)
        {
            // สร้างการเดิน Promote ทั้ง 4 ตัว (Queen, Rook, Bishop, Knight)
            int[] promotionPieces = { 5, 4, 3, 2 };
            foreach (int promo in promotionPieces)
            {
                Move promoMove = new Move(fromX, fromY, toX, toY);
                promoMove.PromotionPiece = board.IsWhiteTurn ? promo : -promo;
                moves.Add(promoMove);
            }
        }
        else
        {
            moves.Add(new Move(fromX, fromY, toX, toY));
        }
    }

    private static void GenerateKnightMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] knightMoves = {
            {2, 1}, {2, -1}, {-2, 1}, {-2, -1},
            {1, 2}, {1, -2}, {-1, 2}, {-1, -2}
        };

        for (int i = 0; i < knightMoves.GetLength(0); i++)
        {
            int newX = x + knightMoves[i, 0];
            int newY = y + knightMoves[i, 1];

            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8)
            {
                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0 || (board.IsWhiteTurn ? targetPiece < 0 : targetPiece > 0))
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
            }
        }


    }

    private static void GenerateBishopMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] directions = { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
        GenerateSlidingMoves(board, x, y, directions, moves);
    }

    private static void GenerateRookMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] directions = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
        GenerateSlidingMoves(board, x, y, directions, moves);
    }

    private static void GenerateQueenMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        GenerateBishopMoves(board, x, y, moves);
        GenerateRookMoves(board, x, y, moves);
    }

    private static void GenerateKingMoves(ChessBoard board, int x, int y, List<Move> moves)
    {
        int[,] kingMoves = {
            {1, 0}, {-1, 0}, {0, 1}, {0, -1},
            {1, 1}, {1, -1}, {-1, 1}, {-1, -1}
        };

        for (int i = 0; i < kingMoves.GetLength(0); i++)
        {
            int newX = x + kingMoves[i, 0];
            int newY = y + kingMoves[i, 1];

            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8)
            {
                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0 || (board.IsWhiteTurn ? targetPiece < 0 : targetPiece > 0))
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
            }
        }

        // Castling
        int piece = board.Board[x, y];
        bool isWhite = piece > 0;
        if ((isWhite && !board.WhiteKingMoved) || (!isWhite && !board.BlackKingMoved))
        {
            // Kingside Castling
            if ((isWhite && !board.WhiteRookKingSideMoved) || (!isWhite && !board.BlackRookKingSideMoved))
            {
                int rookY = 7;
                if (board.Board[x, rookY] == (isWhite ? 4 : -4) &&
                    board.Board[x, y + 1] == 0 && board.Board[x, y + 2] == 0)
                {
                    moves.Add(new Move(x, y, x, y + 2));
                }
            }

            // Queenside Castling
            if ((isWhite && !board.WhiteRookQueenSideMoved) || (!isWhite && !board.BlackRookQueenSideMoved))
            {
                int rookY = 0;
                if (board.Board[x, rookY] == (isWhite ? 4 : -4) &&
                    board.Board[x, y - 1] == 0 && board.Board[x, y - 2] == 0 && board.Board[x, y - 3] == 0)
                {
                    moves.Add(new Move(x, y, x, y - 2));
                }
            }
        }
    }

    private static void GenerateSlidingMoves(ChessBoard board, int x, int y, int[,] directions, List<Move> moves)
    {
        for (int d = 0; d < directions.GetLength(0); d++)
        {
            int dx = directions[d, 0];
            int dy = directions[d, 1];

            for (int step = 1; step < 8; step++)
            {
                int newX = x + dx * step;
                int newY = y + dy * step;

                if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8) break;

                int targetPiece = board.Board[newX, newY];
                if (targetPiece == 0)
                {
                    moves.Add(new Move(x, y, newX, newY));
                }
                else
                {
                    if ((board.IsWhiteTurn && targetPiece < 0) || (!board.IsWhiteTurn && targetPiece > 0))
                    {
                        moves.Add(new Move(x, y, newX, newY));
                    }
                    break;
                }
            }
        }
    }
}