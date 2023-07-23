using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        double maxEval = moveEvaluater(board, moves[0], 0, 0.0, board.IsWhiteToMove);
        int maxEvalIndex = 0;
        for(int i = 1; i < moves.Length; i++){
            double eval = moveEvaluater(board, moves[i], 0, 0.0, board.IsWhiteToMove);
            if(eval > maxEval && board.IsWhiteToMove){
                maxEval = eval;
                maxEvalIndex = i;
            }
            else if(eval < maxEval && !board.IsWhiteToMove){
                maxEval = eval;
                maxEvalIndex = i;
            }
        }
        Console.WriteLine("Result: " + maxEval);
        return moves[maxEvalIndex];
    }
    private double moveEvaluater(Board board, Move move, int movesMade, double eval, bool white){
        if(movesMade > 1){
            return eval;
        }
        board.MakeMove(move);
        double pos = positionEvaluator(board, white);
        if(board.PlyCount < 4 && move.MovePieceType == PieceType.Pawn){
            if(white){
                pos += 0.3;
            }
            else{
                pos -= 0.3;
            }
        }
        else if(board.PlyCount < 10 && (move.MovePieceType == PieceType.King || move.MovePieceType == PieceType.Pawn || move.MovePieceType == PieceType.Queen)){
            if(white){
                pos += 0.3;
            }
            else{
                pos -= 0.3;
            }
        }
        if(move.IsCastles){
            if(white){
                pos += 0.8;
            }
            else{
                pos -= 0.8;
            }
        }
        /*else if(move.MovePieceType == PieceType.King && board.PlyCount < 80 && !isMyTurn(board, white)){
            pos += 1000.0;
        }*/
        if(move.IsCapture){
            if(move.CapturePieceType > move.MovePieceType){
                if(white){
                    pos += move.CapturePieceType - move.MovePieceType;
                }
                else{
                    pos -= move.CapturePieceType - move.MovePieceType;
                }
                
            }
        }
        Move[] moves = board.GetLegalMoves();
        if(board.IsInCheckmate() && !isMyTurn(board, white)){
            board.UndoMove(move);
            Console.WriteLine(move);
            return 1000000;
        }
        else if(board.IsInCheckmate()){
            board.UndoMove(move);
            return -1000000;
        }
        
        
        for(int i = 0; i < moves.Length; i++){
            double newEval = moveEvaluater(board, moves[i], movesMade+1, pos, white);
            if(!isMyTurn(board, white)){
                if(newEval > pos && white){
                    pos = newEval;
                }
                else if(newEval < pos && !white){
                    pos = newEval;
                }
            }
            else{
                if(newEval < pos && white){
                    pos = newEval;
                }
                else if(newEval > pos && !white){
                    pos = newEval;
                }
            }
            
        }
        board.UndoMove(move);
        
        return pos;
    }
    private int materialDifference(Board board){
        PieceList[] list = board.GetAllPieceLists();
        return list[0].Count + 3*(list[1].Count +list[2].Count) + 5*list[3].Count + 9*list[4].Count - (list[6].Count + 3*(list[7].Count +list[8].Count) + 5*list[9].Count + 9*list[10].Count);
    }
    private double positionEvaluator(Board board, bool white){
        Move[] moves = board.GetLegalMoves();
        double pos;
        if(white){
            pos = Math.Cbrt(moves.Length - 15.0) * board.PlyCount / 3000;
        }
        else{
            pos = Math.Cbrt(moves.Length - 15.0) * board.PlyCount / -3000;
        }
        if(true){
            pos += materialDifference(board);
        }
        else{
            pos -= materialDifference(board);
        }
        return pos;
    }
    private bool isMyTurn(Board board, bool white){
        return ((!white && board.IsWhiteToMove) || (white && !board.IsWhiteToMove));
    }
    private int getSpace(Board board){
        return 0;
    }
}