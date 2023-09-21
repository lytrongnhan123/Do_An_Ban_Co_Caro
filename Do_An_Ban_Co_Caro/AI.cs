using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_Ban_Co_Caro
{
    public class AI
    {
        public static int level;
        #region Điểm theo level
        //{0,3,24,192,1536,12288,98304};
        // 0,1,9,81,729,6561,59049;
        // 0, 9, 54, 126, 1458, 13112, 118008
        //0, 3, 27, 99, 729, 6561, 59049
        private int[,] MangDiemTC = new int[5, 7]
        {
            { 0,3,24,192,1536,12288,98304},
            { 0,3,24,192,1536,12288,98304},
            { 0,3,24,192,1536,12288,98304},
            { 0, 3, 27, 99, 729, 6561, 59049},
            { 0, 3, 27, 99, 729, 6561, 59049}
        };
        private int[,] MangDiemPT = new int[5, 7]
        {
            { 0,1,9,81,729,6561,590494},
            { 0,1,9,81,729,6561,59049},
            { 0,1,9,81,729,6561,59049},
            { 0, 9, 54, 126, 1458, 13112, 118008},
            { 0, 9, 54, 126, 1458, 13112, 118008}
        };

        int[] DiemTC_3chan1 = new int[5] { 0, 0, 0, 200, 300 };
        int[] DiemTC_3chan0 = new int[5] {0,0,500,2000,5000 };
        int[] DiemTC_2chan0 = new int[5] { 0,0,0,50,100};
        int DiemTC_4quan = 50000;
        int[] DiemPT_3chan1 = new int[5] { 0, 0, 0, 400, 500 };
        int[] DiemPT_3chan0 = new int[5] { 0, 0, 500, 2000, 7000 };
        int DiemPT_4quan = 50000;
        #endregion
        ChessBoardManagement ChBM = new ChessBoardManagement();
        #region Cắt tỉa Alpha betal
        bool catTia(int row, int col)
        {
            if (catTiaNgang(row,col) && catTiaDoc(row, col) && catTiaCheoPhai(row, col) && catTiaCheoTrai(row, col))
                return true;
            return false;
        }

        bool catTiaNgang(int row, int col)
        {
            if (col <= ChessBoardSize.CHESS_BOARD_WIDTH - 5)
                if (ChessBoardManagement.BtnMatrix[row, col + 1].Ownership != 0)
                    return false;

            if (col >= 4)
                if (ChessBoardManagement.BtnMatrix[row, col - 1].Ownership != 0)
                    return false;

            return true;
        }
        bool catTiaDoc(int row, int col)
        {
            if (row <= ChessBoardSize.CHESS_BOARD_HEIGHT - 5)
                if (ChessBoardManagement.BtnMatrix[row + 1, col].Ownership != 0)
                    return false;
            if (row >= 4)
                if (ChessBoardManagement.BtnMatrix[row - 1, col].Ownership != 0)
                    return false;
            return true;
        }
        bool catTiaCheoPhai(int row, int col)
        {
            if (row <= ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col >= 4)
                if (ChessBoardManagement.BtnMatrix[row + 1, col - 1].Ownership != 0)
                    return false;
            if (col <= ChessBoardSize.CHESS_BOARD_WIDTH - 5 && row >= 4)
                if (ChessBoardManagement.BtnMatrix[row - 1, col + 1].Ownership != 0)
                    return false;
            return true;
        }
        bool catTiaCheoTrai(int row, int col)
        {
            if (row <= ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col <= ChessBoardSize.CHESS_BOARD_WIDTH - 5)
                if (ChessBoardManagement.BtnMatrix[row + 1, col + 1].Ownership != 0)
                    return false;
            if (col >= 4 && row >= 4)
                if (ChessBoardManagement.BtnMatrix[row - 1, col - 1].Ownership != 0)
                    return false;
            return true;
        }

        #endregion
        public Point Start()
        {           
            int DiemMax = 0;
            int DiemPT = 0;
            int DiemTC = 0;
            ButtonChess btn = new ButtonChess();
            for (int i = 0; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < ChessBoardSize.CHESS_BOARD_WIDTH; j++)
                {
                    if (ChessBoardManagement.BtnMatrix[i,j].Ownership == 0 && !catTia(i,j))
                    {
                        int DiemTam;

                        DiemTC = duyetTC_Ngang(i, j) + duyetTC_Doc(i, j) + duyetTC_CheoXuoi(i, j) + duyetTC_CheoNguoc(i, j)+ Attack_TheCoHaiDuong(i, j);
                        DiemPT = duyetPT_Ngang(i, j) + duyetPT_Doc(i, j) + duyetPT_CheoXuoi(i, j) + duyetPT_CheoNguoc(i, j)+ Defense_TheCoHaiDuong(i, j);


                        if (DiemPT > DiemTC)
                        {
                            DiemTam = DiemPT;
                        }
                        else
                        {
                            DiemTam = DiemTC;
                        }

                        if (DiemMax < DiemTam)
                        {
                            DiemMax = DiemTam;
                            btn = new ButtonChess(i,j);
                        }
                    }
                }
            }
            Point point = new Point(btn.col,btn.row);
            return point;
        }

        public int Attack_TheCoHaiDuong(int row, int col)
        {
            int Count = 0;
            int[] MangDiemHaiDuongTC = new int[4];
            MangDiemHaiDuongTC[0] = TheCoHaiDuong_duyetTC_Ngang(row, col);
            MangDiemHaiDuongTC[1] = TheCoHaiDuong_duyetTC_Doc(row, col);
            MangDiemHaiDuongTC[2] = TheCoHaiDuong_duyetTC_CheoXuoi(row, col);
            MangDiemHaiDuongTC[3] = TheCoHaiDuong_duyetTC_CheoNguoc(row, col);
            for (int i = 0; i < 4; i++)
                if (MangDiemHaiDuongTC[i] == 0)
                    Count++;

            return Count >= 2 ? 5000 : 0;
        }
        public int Defense_TheCoHaiDuong(int row, int col)
        {
            int Count = 0, _Count = 0;
            int[] MangDiemHaiDuongPT = new int[4];
            MangDiemHaiDuongPT[0] = TheCoHaiDuong_duyetTC_Ngang(row, col);
            MangDiemHaiDuongPT[1] = TheCoHaiDuong_duyetTC_Doc(row, col);
            MangDiemHaiDuongPT[2] = TheCoHaiDuong_duyetTC_CheoXuoi(row, col);
            MangDiemHaiDuongPT[3] = TheCoHaiDuong_duyetTC_CheoNguoc(row, col);
            for (int i = 0; i < 4; i++)
                if (MangDiemHaiDuongPT[i] == 0)
                    Count++;
            for (int i = 0;i < 4; i++)
                if (MangDiemHaiDuongPT[i] == 2)
                    _Count++;
            if (_Count >= 2)
                return 20000;
            else if (_Count + Count >= 2)
                return 17000;

            return Count >= 2 ? 10000 : 0;
        }
        #region tấn công thế cờ hai hướng
        public int TheCoHaiDuong_duyetTC_Ngang(int row, int col)
        {
            int soquanta = 0;
            int soquandich = 0; ;
            int khoangtrong = 0;
            for (int i = col + 1; i < ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                    soquanta++;
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                    soquanta++;
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }


            if (soquandich == 0 && soquanta >= 2)
                return 0;
            else if (soquanta == 3)
                return 0;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetTC_Doc(int row, int col)
        {
            int soquanta = 0;
            int soquandich = 0;
            int khoangtrong = 0;
            for (int i = row + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                    soquanta++;
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                {
                    soquandich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                    soquanta++;
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                {
                    soquandich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }

            if (soquandich == 0 && soquanta >= 2)
                return 0;
            else if (soquanta == 3)
                return 0;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetTC_CheoXuoi(int row, int col)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int khoangtrong = 0;
            for (int dem = 1; dem <= 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 1)
                    SoQuanTa++;

                else if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int dem = 1; dem <= 4 && row > 4 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 1)
                {
                    SoQuanTa++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            if (SoQuanDich == 0 && SoQuanTa >= 2)
                return 0;
            else if (SoQuanTa == 3)
                return 0;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetTC_CheoNguoc(int row, int col)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int khoangtrong = 0;
            for (int dem = 1; dem <= 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5 && row > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 1)
                    SoQuanTa++;


                else if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int dem = 1; dem <= 4 && col > 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 1)
                {
                    SoQuanTa++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            if (SoQuanDich == 0 && SoQuanTa >= 2)
                return 0;
            else if (SoQuanTa == 3)
                return 0;
            else
                return 1;
        }
        #endregion
        #region phòng thủ thế cờ hai hướng
        public int TheCoHaiDuong_duyetPT_Ngang(int row, int col)
        {
            int soquanta = 0;
            int soquandich = 0;
            int khoangtrong = 0;
            for (int i = col + 1; i < ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquanta++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                    soquandich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            khoangtrong = 0;
            for (int i = col - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquanta++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                    soquandich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }

            if (soquanta == 0 && soquandich >= 2)
                return 0;
            if (soquandich == 3)
                return 2;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetPT_Doc(int row, int col)
        {
            int soquanta = 0;
            int soquandich = 0;
            int khoangtrong = 0;
            for (int i = row + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquanta++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                    soquandich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquanta++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                    soquandich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            if (soquanta == 0 && soquandich >= 2)
                return 0;
            if (soquandich == 3)
                return 2;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetPT_CheoXuoi(int row, int col)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int khoangtrong = 0;
            for (int dem = 1; dem <= 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 2)
                    SoQuanDich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int dem = 1; dem <= 4 && row > 4 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 2)
                    SoQuanDich++;
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            if (SoQuanTa == 0 && SoQuanDich >= 2)
                return 0;
            if (SoQuanDich == 3)
                return 2;
            else
                return 1;
        }
        public int TheCoHaiDuong_duyetPT_CheoNguoc(int row, int col)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int khoangtrong = 0;
            for (int dem = 1; dem <= 4 && row > 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5; dem++)
            {

                if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 2)
                    SoQuanDich++;
                else if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 1)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            for (int dem = 1; dem <= 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 2)
                    SoQuanDich++;
                else if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 1)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    khoangtrong++;
                    if (khoangtrong == 2)
                        break;
                }
            }
            if (SoQuanTa == 0 && SoQuanDich >= 2)
                return 0;
            if (SoQuanDich == 3)
                return 2;
            else
                return 1;
        }
        #endregion

        #region Duyệt điểm

        public int duyetTC_Ngang(int row, int col)
        {
            int DiemTC = 0;
            int soquantaTrai = 0;
            int soquantaPhai = 0;
            int soquandichTrai = 0;
            int soquandichPhai = 0;
            // ben phai
            for (int i = col + 1; i < ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquantaPhai++;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandichPhai++;
                    break;
                }
                else
                    break;
            }
            if (soquantaPhai == 3 && soquandichPhai > 0)
                DiemTC += DiemTC_3chan1[level];
            else if (soquantaPhai >= 3 && soquandichPhai == 0)
                DiemTC += DiemTC_3chan0[level];
            else if (soquantaPhai == 2 && soquandichPhai == 0)
                DiemTC += DiemTC_2chan0[level];
            else if (soquantaPhai == 4)
                DiemTC += DiemTC_4quan;
            // ben trai
            for (int i = col - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquantaTrai++;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandichTrai++;
                    break;
                }
                else
                    break;
            }
            if (soquantaTrai == 3 && soquandichTrai > 0)
                DiemTC += DiemTC_3chan1[level];
            else if (soquantaTrai >= 3 && soquandichTrai == 0 || soquantaPhai + soquantaTrai == 3 && soquandichTrai + soquandichPhai == 0)
                DiemTC += DiemTC_3chan0[level];
            else if (soquantaTrai == 2 && soquandichTrai == 0)
                DiemTC += DiemTC_2chan0[level];
            else if (soquantaTrai == 4 || soquantaPhai + soquantaTrai == 4)
                DiemTC += DiemTC_4quan;

            DiemTC += MangDiemTC[level,soquantaTrai + soquantaPhai] - MangDiemPT[level,soquandichTrai + soquandichPhai];

            return DiemTC;
        }
        public int duyetTC_Doc(int row, int col)
        {

            int DiemTC = 0;
            int soquantaTren = 0;
            int soquantaDuoi = 0;
            int soquandichTren = 0;
            int soquandichDuoi = 0;
            for (int i = row + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquantaDuoi++;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandichDuoi++;
                    break;
                }
                else
                    break;
            }
            if (soquantaDuoi == 3 && soquandichDuoi > 0)
                DiemTC += DiemTC_3chan1[level];
            else if (soquantaDuoi >= 3 && soquandichDuoi == 0)
                DiemTC += DiemTC_3chan0[level];
            else if (soquantaDuoi == 2 && soquandichDuoi == 0)
                DiemTC += DiemTC_2chan0[level];
            else if (soquantaDuoi == 4)
                DiemTC += DiemTC_4quan;
            for (int i = row - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquantaTren++;
                }
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                {
                    soquandichTren++;
                    break;
                }
                else
                    break;
            }
            if (soquantaTren == 3 && soquandichTren > 0)
                DiemTC += DiemTC_3chan1[level];
            else if (soquantaTren >= 3 && soquandichTren == 0 || (soquantaTren + soquantaDuoi == 3 && soquandichTren + soquandichDuoi == 0))
                DiemTC += DiemTC_3chan0[level];
            else if (soquantaTren == 2 && soquandichTren == 0)
                DiemTC += DiemTC_2chan0[level];
            else if (soquantaTren == 4 || soquantaTren + soquantaDuoi == 4)
                DiemTC += DiemTC_4quan;

            DiemTC += MangDiemTC[level,soquantaTren + soquantaDuoi] - MangDiemPT[level,soquandichTren + soquandichDuoi];

            return DiemTC;
        }
        public int duyetTC_CheoXuoi(int row, int col)
        {
            int DiemTC = 0;
            int SoQuanTaTren = 0;
            int SoQuanTaDuoi = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;
            //bên chéo xuôi xuống
            for (int dem = 1; dem <= 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5 && row < ChessBoardSize.CHESS_BOARD_HEIGHT- 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 1)
                {
                    SoQuanTaDuoi++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 2)
                {
                    SoQuanDichCheoDuoi++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanTaDuoi >= 3 && SoQuanDichCheoDuoi > 0)
                DiemTC += DiemTC_3chan1[level];
            if (SoQuanTaDuoi >= 3 && SoQuanDichCheoDuoi == 0)
                DiemTC += DiemTC_3chan0[level];
            if (SoQuanTaDuoi == 2 && SoQuanDichCheoDuoi == 0)
                DiemTC += DiemTC_2chan0[level];
            if (SoQuanTaDuoi == 4)
                DiemTC += DiemTC_4quan;
            //chéo xuôi lên
            for (int dem = 1; dem <= 4 && row > 4 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 1)
                {
                    SoQuanTaTren++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 2)
                {
                    SoQuanDichCheoDuoi++;
                    break;
                }
                else break;
            }
            if (SoQuanTaTren >= 3 && SoQuanDichCheoTren > 0)
                DiemTC += DiemTC_3chan1[level];
            if (SoQuanTaTren >= 3 && SoQuanDichCheoTren == 0 || (SoQuanTaTren + SoQuanTaDuoi == 3 && SoQuanDichCheoTren + SoQuanDichCheoDuoi == 0))
                DiemTC += DiemTC_3chan0[level];
            if (SoQuanTaTren == 2 && SoQuanDichCheoTren == 0)
                DiemTC += DiemTC_2chan0[level];
            if (SoQuanTaTren == 4 || SoQuanTaTren + SoQuanTaDuoi == 4)
                DiemTC += DiemTC_4quan;

            DiemTC -= MangDiemPT[level, SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTC += MangDiemTC[level, SoQuanTaTren  + SoQuanTaDuoi];
            return DiemTC;
        }
        public int duyetTC_CheoNguoc(int row, int col)
        {
            int DiemTC = 0;
            int SoQuanTaTren = 0;
            int SoQuanTaDuoi = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;

            //chéo ngược lên
            for (int dem = 1; dem <= 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5 && row > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 1)
                {
                    SoQuanTaTren++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 2)
                {
                    SoQuanDichCheoTren++;
                    break;
                }
                else break;
            }
            if (SoQuanTaTren >= 3 && SoQuanDichCheoTren > 0)
                DiemTC += DiemTC_3chan1[level];
            if (SoQuanTaTren >= 3 && SoQuanDichCheoTren == 0)
                DiemTC += DiemTC_3chan0[level];
            if (SoQuanTaTren == 2 && SoQuanDichCheoTren == 0)
                DiemTC += DiemTC_2chan0[level];
            if (SoQuanTaTren == 4)
                DiemTC += DiemTC_4quan;
            for (int dem = 1; dem <= 4 && col > 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 1)
                {
                    SoQuanTaDuoi++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 2)
                {
                    SoQuanDichCheoDuoi++;
                    break;
                }
                else break;
            }
            if (SoQuanTaDuoi >= 3 && SoQuanDichCheoDuoi > 0)
                DiemTC += DiemTC_3chan1[level];
            if (SoQuanTaDuoi >= 3 && SoQuanDichCheoDuoi == 0 || (SoQuanTaTren + SoQuanTaDuoi == 3 && SoQuanDichCheoTren + SoQuanDichCheoDuoi == 0))
                DiemTC += DiemTC_3chan0[level];
            if (SoQuanTaDuoi == 2 && SoQuanDichCheoDuoi == 0)
                DiemTC += DiemTC_2chan0[level];
            if (SoQuanTaDuoi == 4 || SoQuanTaTren + SoQuanTaDuoi == 4)
                DiemTC += DiemTC_4quan;

            DiemTC -= MangDiemPT[level, SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTC += MangDiemTC[level, SoQuanTaTren  + SoQuanTaDuoi];
            return DiemTC;
        }
        public int duyetPT_Ngang(int row, int col)
        {
            int DiemPT = 0;
            int soquantaPhai = 0;
            int soquantaTrai = 0;
            int soquandichTrai = 0;
            int soquandichPhai = 0;
            // ben phai
            for (int i = col + 1; i < ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquantaPhai++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandichPhai++;
                }
                else
                    break;
            }
            if (soquandichPhai >= 3 && soquantaPhai == 0)
                DiemPT += DiemPT_3chan0[level];
            if (soquandichPhai == 4 && soquantaPhai > 0)
                DiemPT += DiemPT_4quan;
            if (soquandichPhai == 3 && soquantaPhai > 0)
                DiemPT += DiemPT_3chan1[level];
            // ben trai
            for (int i = col - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 1)
                {
                    soquantaTrai++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[row, i].Ownership == 2)
                {
                    soquandichTrai++;
                }
                else
                    break;
            }

            if (soquandichTrai >= 3 && soquantaTrai == 0 || (soquandichTrai + soquandichPhai == 3 && soquantaPhai + soquantaTrai == 0))
                DiemPT += DiemPT_3chan0[level];
            if (soquandichTrai == 4 && soquantaTrai > 0 || soquandichPhai + soquandichTrai >= 4)
                DiemPT += DiemPT_4quan;
            if (soquandichTrai == 3 && soquantaTrai > 0)
                DiemPT += DiemPT_3chan1[level];

            DiemPT += MangDiemPT[level, soquandichTrai  + soquandichPhai] - MangDiemTC[level, soquantaTrai  + soquantaPhai];

            return DiemPT;
        }
        public int duyetPT_Doc(int row, int col)
        {

            int DiemPT = 0;
            int soquantaTren = 0;
            int soquantaDuoi = 0;
            int soquandichTren = 0;
            int soquandichDuoi = 0;
            for (int i = row + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquantaDuoi++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                {
                    soquandichDuoi++;
                }
                else
                    break;
            }
            if (soquandichDuoi == 3 && soquantaDuoi == 0)
                DiemPT += DiemPT_3chan0[level];
            if (soquandichDuoi == 4 && soquantaDuoi > 0)
                DiemPT += DiemTC_4quan;
            if (soquandichDuoi == 3 && soquantaDuoi > 0)
                DiemPT += DiemPT_3chan1[level];
            for (int i = row - 1; i >= 0; i--)
            {
                if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 1)
                {
                    soquantaTren++;
                    break;
                }
                else if (ChessBoardManagement.BtnMatrix[i, col].Ownership == 2)
                {
                    soquandichTren++;
                }
                else
                    break;
            }
            if (soquandichTren >= 3 && soquantaTren == 0 || (soquandichTren + soquandichDuoi == 3 && soquantaTren + soquantaDuoi == 0))
                DiemPT += DiemPT_3chan0[level];
            if (soquandichTren == 4 && soquantaTren > 0 || soquandichTren + soquandichDuoi >= 4)
                DiemPT += DiemTC_4quan;
            if (soquandichTren == 3 && soquantaTren > 0 )
                DiemPT += DiemPT_3chan1[level];

            DiemPT += MangDiemPT[level, soquandichTren + soquandichDuoi] - MangDiemTC[level, soquantaTren + soquantaDuoi];

            return DiemPT;
        }
        public int duyetPT_CheoXuoi(int row, int col)
        {
            int DiemPT = 0;
            int SoQuanTaTren = 0;
            int SoQuanTaDuoi = 0;
            int SoQuanDichTren = 0;
            int SoQuanDichDuoi = 0;

            //lên
            for (int dem = 1; dem <= 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 2)
                {
                    SoQuanDichDuoi++;
                }
                else
                    if (ChessBoardManagement.BtnMatrix[row + dem, col + dem].Ownership == 1)
                {
                    SoQuanTaDuoi++;
                    break;
                }
                else
                    break;
            }

            if (SoQuanDichDuoi >= 3 && SoQuanTaDuoi == 0)
                DiemPT += DiemPT_3chan0[level];
            if (SoQuanDichDuoi == 4 && SoQuanTaDuoi > 0)
                DiemPT += DiemPT_4quan;
            if (SoQuanDichDuoi == 3 && SoQuanTaDuoi > 0)
                DiemPT += DiemPT_3chan1[level];
            //xuống
            for (int dem = 1; dem <= 4 && row > 4 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 2)
                {
                    SoQuanDichTren++;
                }

                else if (ChessBoardManagement.BtnMatrix[row - dem, col - dem].Ownership == 1)
                {
                    SoQuanTaTren++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanDichTren >= 3 && SoQuanTaTren == 0 || SoQuanDichTren + SoQuanDichDuoi == 3 && SoQuanTaTren + SoQuanTaDuoi == 0)
                DiemPT += DiemPT_3chan0[level];
            if (SoQuanDichTren == 4 && SoQuanTaTren > 0 || SoQuanDichTren + SoQuanDichDuoi >= 4)
                DiemPT += DiemPT_4quan;
            if (SoQuanDichTren == 3 && SoQuanTaTren > 0)
                DiemPT += DiemPT_3chan1[level];
            // trường hơp x[]xx or xx[]xx
            DiemPT -= MangDiemTC[level, SoQuanTaDuoi  + SoQuanTaTren];
            DiemPT += MangDiemPT[level, SoQuanDichTren  + SoQuanTaDuoi];

            return DiemPT;
        }
        public int duyetPT_CheoNguoc(int row, int col)
        {
            int DiemPT = 0;
            int SoQuanTaTren = 0;
            int SoQuanTaDuoi = 0;
            int SoQuanDichTren = 0;
            int SoQuanDichDuoi = 0;
            //lên
            for (int dem = 1; dem <= 4 && row > 4 && col < ChessBoardSize.CHESS_BOARD_WIDTH - 5; dem++)
            {

                if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 2)
                {
                    SoQuanDichTren++;
                }

                else if (ChessBoardManagement.BtnMatrix[row - dem, col + dem].Ownership == 1)
                {
                    SoQuanTaTren++;
                    break;
                }
                else
                    break;
            }

            if (SoQuanDichTren >= 3 && SoQuanTaTren == 0)
                DiemPT += DiemPT_3chan0[level];
            if (SoQuanDichTren == 4 && SoQuanTaTren > 0)
                DiemPT += DiemPT_4quan;
            if (SoQuanDichTren == 3 && SoQuanTaTren > 0)
                DiemPT += DiemTC_3chan1[level];
            //xuống
            for (int dem = 1; dem <= 4 && row < ChessBoardSize.CHESS_BOARD_HEIGHT - 5 && col > 4; dem++)
            {
                if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 2)
                {
                    SoQuanDichDuoi++;
                }

                else if (ChessBoardManagement.BtnMatrix[row + dem, col - dem].Ownership == 1)
                {
                    SoQuanTaDuoi++;
                    break;
                }
                else
                    break;
            }

            if (SoQuanDichDuoi >= 3 && SoQuanTaDuoi == 0 || (SoQuanDichTren + SoQuanDichDuoi == 3 && SoQuanTaTren + SoQuanTaDuoi == 0))
                DiemPT += DiemPT_3chan0[level];
            if (SoQuanDichDuoi == 4 && SoQuanTaDuoi > 0 || SoQuanDichTren + SoQuanDichDuoi >= 4)
                DiemPT += DiemPT_4quan;
            if (SoQuanDichDuoi == 3 && SoQuanTaDuoi > 0)
                DiemPT += DiemTC_3chan1[level];
            // trường hơp x[]xx or xx[]xx

            DiemPT -= MangDiemTC[level, SoQuanTaTren  + SoQuanTaDuoi];
            DiemPT += MangDiemPT[level, SoQuanDichTren + SoQuanTaDuoi];

            return DiemPT;
        }
        #endregion
    }
}
