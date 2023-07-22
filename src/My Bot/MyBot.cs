using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        double maxEval = positionEvaluater(board, moves[0], 0);
        int maxEvalIndex = 0;
        for(int i = 1; i < moves.Length; i++){
            double eval = positionEvaluater(board, moves[i], 0);
            if(eval > maxEval){
                maxEval = eval;
                maxEvalIndex = i;
            }
        }
        Console.WriteLine(maxEval);
        return moves[maxEvalIndex];
    }
    private double positionEvaluater(Board board, Move move, int movesMade){
        if(movesMade > 2){
            return -10000;
        }
        double pos = 0.0;
        board.MakeMove(move);
        Move[] moves = board.GetLegalMoves();
        pos -= Math.Cbrt(moves.Length - 15.0) * board.PlyCount / 3000;
        if(!board.IsWhiteToMove){
            pos += materialDifference(board);
        }
        else{
            pos -= materialDifference(board);
        }
        if(move.IsCastles){
            pos += 0.3;
        }
        /*
        int index;
        for(int i = 0; i < moves.Length; i++){
            double eval = positionEvaluater(board, moves[i], movesMade+1);
            if(eval > pos){
                pos = eval;
                index = i;
            }
        }*/
        board.UndoMove(move);
        return pos;
    }
    private int materialDifference(Board board){
        PieceList[] list = board.GetAllPieceLists();
        return list[0].Count + 3*(list[1].Count +list[2].Count) + 5*list[3].Count + 9*list[4].Count - (list[6].Count + 3*(list[7].Count +list[8].Count) + 5*list[9].Count + 9*list[10].Count);
    }
}