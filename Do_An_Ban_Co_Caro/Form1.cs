using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
namespace Do_An_Ban_Co_Caro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManagement ChessBoard;
        AI ai = new AI();

        private Mp3Player mp3Player = new Mp3Player();

        SocketManagement socket;
        #endregion
        public Form1()
        {
            InitializeComponent();
           
            Control.CheckForIllegalCrossThreadCalls = false;

            ChessBoard = new ChessBoardManagement();
            ChessBoard.chessboard_Manager(pnlChessBoard, TxtBoxPlayerName, ptrBoxMark);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            processBarCoolDown.Step = ChessBoardSize.COOL_DOWN_STEP;
            processBarCoolDown.Maximum = ChessBoardSize.COOL_DOWN_TIME;
            processBarCoolDown.Value = 0;

            tmCoolDown.Interval = ChessBoardSize.COOL_DOWN_INTERVAL;

            socket = new SocketManagement();

            pnlChessBoard.Enabled = false;
            ChessBoard.DrawChessBoard();
            btnLAN.Enabled = false;
            newLANGameToolStripMenuItem.Enabled = false;
            btnPlayGame.Enabled = false;
            loadToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
        }
        #region Methods
        private void EndGame()
        {
            tmCoolDown.Stop();
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;

            //MessageBox.Show("Game đã kết thúc!");
        }

        private void NewGamePvP()
        {
            processBarCoolDown.Value = 0;
            tmCoolDown.Stop();
            undoToolStripMenuItem.Enabled = true;

            ChessBoard.DrawChessBoard();
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void Undo()
        {
            ChessBoard.Undo();
            processBarCoolDown.Value = 0;
        }

        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmCoolDown.Start();
            pnlChessBoard.Enabled = false;
            processBarCoolDown.Value = 0;

            socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "", e.ClickedPoint));

            undoToolStripMenuItem.Enabled = false;

            Listen();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();

            socket.Send(new SocketData((int)SocketCommand.END_GAME, "", new Point()));
        }
        
        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            processBarCoolDown.PerformStep();

            if (processBarCoolDown.Value >= processBarCoolDown.Maximum)
            {
                EndGame();

                socket.Send(new SocketData((int)SocketCommand.TIME_OUT, "", new Point()));
            }
        }

        private void playerVsPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            ChessBoard.NewGame(); 
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoardManagement.Mode = 1;
            pnlChessBoard.Enabled = true;
        }
        private void onlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            loadToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            pnlChessBoard.Enabled = false;
            onlineToolStripMenuItem.Enabled = false;
            ChessBoardManagement.Mode = 3;
            btnLAN.Enabled = true;
            
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ChessBoardManagement.Mode==2)
            {
                ChessBoard.AI_Undo();
            }
            else if(ChessBoardManagement.Mode == 1)
            {
                ChessBoard.PvP_UnDo();
            }
            else
            {
                socket.Send(new SocketData((int)SocketCommand.UNDO, "", new Point()));
                Undo();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void newLANGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onlineToolStripMenuItem.Enabled = false;
            ChessBoardManagement.Mode = 3;
            socket.Send(new SocketData((int)SocketCommand.NEW_GAME_PVP, "", new Point()));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = "";
            if (ChessBoardManagement.Mode == 1)
                file = "PvP";
            else if (ChessBoardManagement.Mode == 2)
            {
                if (AI.level == 0)
                    file = "AILevel1";
                else if (AI.level == 1)
                    file = "AILevel2";
                else if (AI.level == 2)
                    file = "AILevel3";
                else if (AI.level == 3)
                    file = "AILevel4";
                else if (AI.level == 4)
                    file = "AILevel5";
            }
            string linkFile = Application.StartupPath + "\\Resources\\SaveGame\\" + file + ".txt";
            StreamWriter sw = new StreamWriter(linkFile);

            foreach (var s in ChessBoard.stack_Save)
            {
                sw.WriteLine(s.row + " " + s.col + " " + s.Ownership);
            }
            sw.Close();

            ChessBoard.stack_Save.Clear();

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChessBoard.NewGame();
            ChessBoard.DrawChessBoard();

            string file = "";
            if (ChessBoardManagement.Mode == 1)
                file = "PvP";
            else if (ChessBoardManagement.Mode == 2)
            {
                if (AI.level == 0)
                    file = "AILevel1";
                else if (AI.level == 1)
                    file = "AILevel2";
                else if (AI.level == 2)
                    file = "AILevel3";
                else if (AI.level == 3)
                    file = "AILevel4";
                else if (AI.level == 4)
                    file = "AILevel5";
            }
            string linkFile = Application.StartupPath + "\\Resources\\SaveGame\\" + file + ".txt";
            string[] a = File.ReadAllLines(linkFile);
            int[,] b = new int[a.Length + 1, 3];
            b[0,0] = -1;

            for (int j = 0; j < a.Length; j++)
            {
                for (int i = 0; i < 3; i++)
                    b[j, i] = -1;
            }
            int u = 0;
            string temp = "";
            for (int i = 0; i < a.Length; i++)
            {
                temp = "";
                u = 0;
                for (int j = 0; j < a[i].Length; j++)
                {
                    if (a[i][j] != ' ')
                    {
                        temp += a[i][j].ToString();
                        if (j == a[i].Length - 1)
                        {
                            b[i, u++] = Convert.ToInt32(temp);
                            temp = "";
                        }
                    }
                    else
                    {
                        b[i, u++] = Convert.ToInt32(temp);
                        temp = "";
                    }
                }
            }

            if (b[0, 0] != -1)
            {
                for (int j = 0; j < a.Length; j++)
                {
                    ChessBoard.Load(b[j, 0], b[j, 1], b[j, 2]);
                }
                if (b[0, 2] == 1)
                {
                    ChessBoard.CurrentPlayer = 1;
                }
                else
                    ChessBoard.CurrentPlayer = 0;
            }
            else
                MessageBox.Show("Chưa từng lưu");
        }

        private void btnPlayGame_Click(object sender, EventArgs e)
        {
            Listen();
            onlineToolStripMenuItem.Enabled = false;
            newLANGameToolStripMenuItem.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn muốn thoát game?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.EXIT, "", new Point()));
                }
                catch
                {

                }
            }
            
        }

        private void btnFileMusic_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Mp3 Files|*.mp3";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    mp3Player.OpenFile(ofd.FileName);
                }
            }
        }

        private void btnPlayMusic_Click(object sender, EventArgs e)
        {
            mp3Player.PlayMusic();
        }

        private void btnPauseMusic_Click(object sender, EventArgs e)
        {
            mp3Player.PauseMusic();
        }

        private void btnLAN_Click(object sender, EventArgs e)
        {
            socket.IP = TxtBoxIp.Text;
            btnPlayGame.Enabled = true;
            onlineToolStripMenuItem.Enabled = false;

            if (!socket.ConnectToServer())
            {
                btnLAN.Enabled = false;
                undoToolStripMenuItem.Enabled = true;
                socket.isServer = true;
                pnlChessBoard.Enabled = false;
                socket.CreateServer();
                MessageBox.Show("Nhấn (Play game) đến khi tìm được đối thủ", "Hướng dẫn");
            }
            else
            {
                btnLAN.Enabled = false;
                undoToolStripMenuItem.Enabled = true;
                socket.isServer = false;
                pnlChessBoard.Enabled = false;
                MessageBox.Show("Đã kết nối", "Thông báo");
                socket.Send(new SocketData((int)SocketCommand.START_GAME, "Đã tìm thấy đối thủ", new Point()));
                Listen();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            TxtBoxIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(TxtBoxIp.Text))
            {
                TxtBoxIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        private void Listen()
        {
            Thread listenThread = new Thread(() =>
            {
                try
                {
                    SocketData data = (SocketData)socket.Receive();

                    ProcessData(data);
                }
                catch 
                {

                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGamePvP();
                        pnlChessBoard.Enabled = true;
                    }));
                    if (!socket.isServer)
                    {
                        ChessBoard.CurrentPlayer = 1;
                        ChessBoard.ChangePlayer();
                    }
                    break;
                case (int)SocketCommand.NEW_GAME_PVP:
                    if (MessageBox.Show("Đối thủ muốn chơi ván mới", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        socket.Send(MessageBox.Show("Đống ý chơi tiếp", "Thông báo"));
                    else
                    {
                        socket.Send(new SocketData((int)SocketCommand.NEW_GAME, "", new Point()));
                        this.Invoke((MethodInvoker)(() =>
                        {
                            NewGamePvP();
                            pnlChessBoard.Enabled = false;
                        }));
                        if (socket.isServer)
                        {
                            ChessBoard.CurrentPlayer = 1;
                            ChessBoard.ChangePlayer();
                        }
                    }
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        processBarCoolDown.Value = 0;
                        pnlChessBoard.Enabled = true;
                        tmCoolDown.Start();
                        ChessBoard.OtherPlayerMark(data.Point);


                        undoToolStripMenuItem.Enabled = true;
                    }));
                    break;
                case (int)SocketCommand.UNDO:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        Undo();
                        processBarCoolDown.Value = 0;
                    }));
                    break;
                case (int)SocketCommand.END_GAME:
                    MessageBox.Show("Đã có người chiến thắng","Thông báo");
                    break;
                case (int)SocketCommand.TIME_OUT:
                    MessageBox.Show("Đã hết thời gian","Thông báo");
                    break;
                case (int)SocketCommand.EXIT:
                    pnlChessBoard.Enabled = false;
                    onlineToolStripMenuItem.Enabled = false;
                    undoToolStripMenuItem.Enabled = false;
                    tmCoolDown.Stop();
                    MessageBox.Show("Người chơi còn lại đã thoát", "Thông báo");
                    if (socket.isServer)
                    {
                        processBarCoolDown.Value = 0;
                        socket.CloseConnect();
                    }
                    break;
                case (int)SocketCommand.START_GAME:
                    MessageBox.Show(data.Message);
                    pnlChessBoard.Enabled = true;
                    break;
                default:
                    break;
            }

            Listen();
        }

        #endregion
        #region 5 level
        private Random rd = new Random();
        private void level1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoard.CurrentPlayer = rd.Next(0, 2);
            AI.level = 0;
            ChessBoardManagement.Mode = 2;
            pnlChessBoard.Enabled = true;
            ChessBoard.NewGame();
            if (ChessBoard.CurrentPlayer == 0)
            {
                MessageBox.Show("Máy đi trước");
                Point point = new Point(ChessBoardSize.CHESS_BOARD_HEIGHT / 2, ChessBoardSize.CHESS_BOARD_WIDTH / 2);
                ChessBoard.AI_Click(point);
            }
            else
            {
                MessageBox.Show("Người chơi đi trước");
            }
        }
        private void level2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoard.CurrentPlayer = rd.Next(0, 2);
            AI.level = 1;
            ChessBoardManagement.Mode = 2;
            pnlChessBoard.Enabled = true;
            ChessBoard.NewGame();
            if (ChessBoard.CurrentPlayer==0)
            {
                MessageBox.Show("Máy đi trước");
                Point point = new Point(ChessBoardSize.CHESS_BOARD_HEIGHT / 2, ChessBoardSize.CHESS_BOARD_WIDTH / 2);
                ChessBoard.AI_Click(point);
            }
            else
            {
                MessageBox.Show("Người chơi đi trước");
            }
        }

        private void level3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoard.CurrentPlayer = rd.Next(0, 2);
            AI.level = 2;
            ChessBoardManagement.Mode = 2;
            pnlChessBoard.Enabled = true;
            ChessBoard.NewGame();
            if (ChessBoard.CurrentPlayer == 0)
            {
                MessageBox.Show("Máy đi trước");
                Point point = new Point(ChessBoardSize.CHESS_BOARD_HEIGHT / 2, ChessBoardSize.CHESS_BOARD_WIDTH / 2);
                ChessBoard.AI_Click(point);
            }
            else
            {
                MessageBox.Show("Người chơi đi trước");
            }
        }

        private void level4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoard.CurrentPlayer = rd.Next(0, 2);
            AI.level = 3;
            ChessBoardManagement.Mode = 2;
            pnlChessBoard.Enabled = true;
            ChessBoard.NewGame();
            if (ChessBoard.CurrentPlayer == 0)
            {
                MessageBox.Show("Máy đi trước");
                Point point = new Point(ChessBoardSize.CHESS_BOARD_HEIGHT / 2, ChessBoardSize.CHESS_BOARD_WIDTH / 2);
                ChessBoard.AI_Click(point);
            }
            else
            {
                MessageBox.Show("Người chơi đi trước");
            }
        }
        private void level5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGamePvP();
            saveToolStripMenuItem.Enabled = true;
            loadToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = true;
            onlineToolStripMenuItem.Enabled = true;
            ChessBoard.CurrentPlayer = rd.Next(0, 2);
            AI.level = 4;
            ChessBoardManagement.Mode = 2;
            pnlChessBoard.Enabled = true;
            ChessBoard.NewGame();
            if (ChessBoard.CurrentPlayer == 0)
            {
                MessageBox.Show("Máy đi trước");
                Point point = new Point(ChessBoardSize.CHESS_BOARD_HEIGHT / 2, ChessBoardSize.CHESS_BOARD_WIDTH / 2);
                ChessBoard.AI_Click(point);
            }
            else
            {
                MessageBox.Show("Người chơi đi trước");
            }
        }


        #endregion

        

    }
}
                    
   



