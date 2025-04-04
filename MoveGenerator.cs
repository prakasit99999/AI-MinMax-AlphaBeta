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
        // กรองการเดินที่กิน King
        return FilterLegalMoves(board, moves)
              .Where(m => Math.Abs(board.Board[m.ToX, m.ToY]) != 6)
              .ToList();
    }

    // ตรวจสอบการเดินที่ถูกต้องตามกฎ
    private static List<Move> FilterLegalMoves(ChessBoard board, List<Move> pseudoMoves)
    {
        List<Move> legalMoves = new List<Move>();
        bool isWhite = board.IsWhiteTurn;

        foreach (Move move in pseudoMoves)
        {
            ChessBoard tempBoard = board.Clone();
            tempBoard.MakeMove(move);
            if (!tempBoard.IsInCheck(isWhite))
            {
                legalMoves.Add(move);
            }
        }
        return legalMoves;
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
            int[] promotionPieces = { 5, 4, 3, 2 };
            foreach (int piece in promotionPieces)
            {
                Move promoMove = new Move(fromX, fromY, toX, toY);
                promoMove.PromotionPiece = board.IsWhiteTurn ? piece : -piece; // <-- ใช้งาน setter
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
                    Move move = new Move(x, y, newX, newY);
                    ChessBoard tempBoard = board.Clone();
                    tempBoard.MakeMove(move);
                    if (!tempBoard.IsInCheck(board.IsWhiteTurn))
                    {
                        moves.Add(move);
                    }
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

        // เดินปกติ
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
        int row = isWhite ? 7 : 0;

        // Kingside Castling
        if (CanCastleKingside(board, isWhite))
        {
            moves.Add(new Move(x, y, row, 6));
        }

        // Queenside Castling
        if (CanCastleQueenside(board, isWhite))
        {
            moves.Add(new Move(x, y, row, 2));
        }
    }
    // ========== ตรวจสอบเงื่อนไข Castling ==========
    private static bool CanCastleKingside(ChessBoard board, bool isWhite)
    {
        int row = isWhite ? 7 : 0;
        bool kingMoved = isWhite ? board.WhiteKingMoved : board.BlackKingMoved;
        bool rookMoved = isWhite ? board.WhiteRookKingSideMoved : board.BlackRookKingSideMoved;

        // ตรวจสอบว่า Rook ยังอยู่ที่ตำแหน่งเริ่มต้น และไม่ถูกกิน
        bool rookPresent = board.Board[row, 7] == (isWhite ? 4 : -4);

        return !kingMoved && !rookMoved && rookPresent &&
               board.Board[row, 5] == 0 && board.Board[row, 6] == 0 &&
               !IsSquareUnderAttack(board, new Square(row, 4)) &&
               !IsSquareUnderAttack(board, new Square(row, 5)) &&
               !IsSquareUnderAttack(board, new Square(row, 6));
    }

    private static bool CanCastleQueenside(ChessBoard board, bool isWhite)
    {
        int row = isWhite ? 7 : 0;
        bool kingMoved = isWhite ? board.WhiteKingMoved : board.BlackKingMoved;
        bool rookMoved = isWhite ? board.WhiteRookQueenSideMoved : board.BlackRookQueenSideMoved;

        // ตรวจสอบว่า Rook ยังอยู่ที่ตำแหน่งเริ่มต้น และไม่ถูกกิน
        bool rookPresent = board.Board[row, 0] == (isWhite ? 4 : -4);

        return !kingMoved && !rookMoved && rookPresent &&
               board.Board[row, 1] == 0 && board.Board[row, 2] == 0 && board.Board[row, 3] == 0 &&
               !IsSquareUnderAttack(board, new Square(row, 4)) &&
               !IsSquareUnderAttack(board, new Square(row, 3)) &&
               !IsSquareUnderAttack(board, new Square(row, 2));
    }

    // ========== ตรวจสอบความปลอดภัย ==========
    private static bool IsSquareUnderAttack(ChessBoard board, Square square)
    {
        ChessBoard tempBoard = board.CloneWithTurn(!board.IsWhiteTurn);
        return board.IsSquareUnderAttack(square, !board.IsWhiteTurn);
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