using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do_An_Ban_Co_Caro
{
    public class ButtonChess
    {
        public int Ownership
        { get; set; }    
       
        public int row
        { get; set; }
        public int col
        { get; set; }
        public ButtonChess(int row, int col, int Ownership)
        {
            this.row = row;
            this.col = col;
            this.Ownership = Ownership;
        }
        public ButtonChess(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        public ButtonChess()
        { }
    }
}
