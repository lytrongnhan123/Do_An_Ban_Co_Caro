using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_Ban_Co_Caro
{
    public class CheckWin
    {      
        public bool EndGame(Button btn)
        {
            if (endCheck_Horizontal(btn) || endCheckVertical(btn)
                || endCheckPrimary(btn) || endCheckSub(btn))
                return true;
             return false;
        }
        public bool endCheck_Horizontal(Button btn)
        {
            int col = btn.Location.X /  ChessBoardSize.CHESS_WIDTH;
            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;
            int countLeft = 0;
            int countRight = 0;
            for (int i = col; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[row, col].Ownership == ChessBoardManagement.BtnMatrix[row, i].Ownership)
                {
                    countLeft++;
                }
                else
                    break;
            }
            for (int i = col + 1; i <= ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (ChessBoardManagement.BtnMatrix[row, col].Ownership == ChessBoardManagement.BtnMatrix[row, i].Ownership)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft + countRight >= 5?true:false;
        }
        private bool endCheckVertical(Button btn)
        {
            int col = btn.Location.X / ChessBoardSize.CHESS_WIDTH;
            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;

            int countTop = 0;
            for (int i = row; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)              
                    countTop++;                
                else
                    break;
            }
            int countBottom = 0;
            for (int i = row + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)              
                    countBottom++;                
                else
                    break;
            }
            return countTop + countBottom >= 5? true:false;
        }
        private bool endCheckPrimary(Button btn)
        {
            int col = btn.Location.X / ChessBoardSize.CHESS_WIDTH;
            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;
            int countTop = 0;
            for (int i = 0; i <= col; i++)
            {
                if (row - i < 0 || col - i < 0)
                    break;
                if (ChessBoardManagement.BtnMatrix[row - i,col - i].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)                
                    countTop++;              
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= ChessBoardSize.CHESS_BOARD_WIDTH - col; i++)
            {
                if (row + i >= ChessBoardSize.CHESS_BOARD_HEIGHT || col + i >= ChessBoardSize.CHESS_BOARD_WIDTH)
                    break;
                

                if (ChessBoardManagement.BtnMatrix[row + i,col + i].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)
                    countBottom++;
                
                else
                    break;
            }

            return countTop + countBottom >= 5 ? true : false;
        }
        private bool endCheckSub(Button btn)
        {
            int col = btn.Location.X / ChessBoardSize.CHESS_WIDTH;
            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;
            int countTop = 0;
            for (int i = 0; i <= col; i++)
            {
                if (row- i < 0 || col + i > ChessBoardSize.CHESS_BOARD_WIDTH)
                    break;               

                if (ChessBoardManagement.BtnMatrix[row- i,col + i].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)               
                    countTop++;               
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= ChessBoardSize.CHESS_BOARD_WIDTH - col; i++)
            {
                if (row + i >= ChessBoardSize.CHESS_BOARD_HEIGHT || col - i < 0)
                     break;              

                if (ChessBoardManagement.BtnMatrix[row + i,col - i].Ownership == ChessBoardManagement.BtnMatrix[row, col].Ownership)            
                    countBottom++;
                
                else
                    break;
            }
            return countTop + countBottom >= 5 ? true : false;
        }
    }
}
