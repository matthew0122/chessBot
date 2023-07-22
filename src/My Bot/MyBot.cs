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
            if(eval > maxEval){
                maxEval = eval;
                maxEvalIndex = i;
            }
        }
        Console.WriteLine("Result: " + maxEval);
        return moves[maxEvalIndex];
    }
    private double moveEvaluater(Board board, Move move, int movesMade, double eval, bool white){
        if(movesMade > 3){
            return eval;
        }
        
        board.MakeMove(move);
        double pos = positionEvaluator(board, white);
        if(move.IsCastles){
            pos += 1.5;
        }
        if(move.MovePieceType == PieceType.King && board.PlyCount < 60 && ((!white && board.IsWhiteToMove) || (white && !board.IsWhiteToMove))){
            pos -= 100.0;
        }
        Move[] moves = board.GetLegalMoves();
        /*if(board.IsInCheckmate() && ((!white && board.IsWhiteToMove) || (white && !board.IsWhiteToMove))){
            board.UndoMove(move);
            Console.WriteLine(move);
            return 10000;
        }
        else if(board.IsInCheckmate()){
            board.UndoMove(move);
            return -10000;
        }*/
        
        
        int index= 0;
        for(int i = 0; i < moves.Length; i++){
            double newEval = moveEvaluater(board, moves[i], movesMade+1, pos, white);
            if((!white && board.IsWhiteToMove) || (white && !board.IsWhiteToMove)){
                if(newEval > pos){
                    pos = newEval;
                    index = i;
                }
            }
            else{
                if(newEval < pos){
                    pos = newEval;
                    index = i;
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
        double pos = Math.Cbrt(moves.Length - 15.0) * board.PlyCount / -3000;
        if(white){
            pos += materialDifference(board);
        }
        else{
            pos -= materialDifference(board);
        }
        return pos;
    }
}