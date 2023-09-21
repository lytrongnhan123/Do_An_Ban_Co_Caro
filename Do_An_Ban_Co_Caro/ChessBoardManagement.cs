using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_Ban_Co_Caro
{
    public class ChessBoardManagement
    {
        public static int Mode = 0;
        public Stack<ButtonChess> stack_Save = new Stack<ButtonChess>();
        CheckWin checkwin = new CheckWin();
        public static Stack<Button> stack_Btn = new Stack<Button>();
        public static  ButtonChess[,] BtnMatrix = new ButtonChess[ChessBoardSize.CHESS_BOARD_HEIGHT, ChessBoardSize.CHESS_BOARD_WIDTH];
        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard 
        { 
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        private List<Player> player;
        public List<Player> Player 
        {
            get { return player; }
            set { player = value; }
        }

        public int CurrentPlayer
        { get; set; }

        private TextBox playerName;
        public TextBox PlayerName 
        {
            get { return playerName; }
            set { playerName = value; }
        }

        private PictureBox mark;
        public PictureBox PlayerMark 
        {
            get { return mark; }
            set { mark = value; }
        }

        private List<List<Button>> matrix;
        public List<List<Button>> Matrix 
        {
            get { return matrix; }
            set { matrix = value; }
        }

        private event EventHandler<ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private Stack<PlayInfo> playTimeLine;
        public Stack<PlayInfo> PlayTimeLine 
        {
            get { return playTimeLine; }
            set { playTimeLine = value; }
        }
        #endregion

        #region Initialize
        public void chessboard_Manager(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;

            this.Player = new List<Player>()
            {
                new Player("Player 1 turn", Image.FromFile(Application.StartupPath + "\\Resources\\X_icon.png")),
                new Player("Player 2 turn", Image.FromFile(Application.StartupPath + "\\Resources\\O_icon.png"))
            };
        }
        #endregion

        #region Methods;
        public void DrawChessBoard()
        {
            chessBoard.Controls.Clear();
            PlayTimeLine = new Stack<PlayInfo>();

            CurrentPlayer = 0;

            ChangePlayer();

            Matrix = new List<List<Button>>();

            Button firstBtn = new Button()
            { 
                Width = 0,  
                Location = new Point(0, 0) 
            };
            for (int i = 0; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                Matrix.Add(new List<Button>());

                for (int j = 0; j < ChessBoardSize.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = ChessBoardSize.CHESS_WIDTH,
                        Height = ChessBoardSize.CHESS_HEIGHT,
                        Location = new Point(firstBtn.Location.X + firstBtn.Width, firstBtn.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString() 
                    };
                    BtnMatrix[i, j] = new ButtonChess(i, j, 0);
                    btn.Click += btn_Click;
                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    firstBtn = btn;
                }
                firstBtn.Location = new Point(0, firstBtn.Location.Y + ChessBoardSize.CHESS_HEIGHT);
                firstBtn.Width = 0;
                firstBtn.Height = 0;
            }
        }
        public void NewGame()
        {
            
            if (stack_Btn.Count == 0)
                return;

            foreach(var Btn in stack_Btn)
            {
                Btn.BackgroundImage = null;
            }

            ChessBoardArray();
            stack_Save.Clear();
        }
        public void ChessBoardArray()
        {
            for (int i = 0; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)           
                for (int j = 0; j < ChessBoardSize.CHESS_BOARD_WIDTH; j++)
                    BtnMatrix[i, j] = new ButtonChess(i, j, 0);
        }
        

        public void Load(int row, int col, int Ownership)
        {
            Button btn = Matrix[row][col];

            ButtonChess Btn = new ButtonChess(row,col,Ownership);

            stack_Save.Push(Btn);

            if (Ownership == 1)
            {
                btn.BackgroundImage = Player[0].Mark;
                stack_Btn.Push(btn);
                BtnMatrix[row, col].Ownership = 1;
            }
            else
            {
                btn.BackgroundImage = Player[1].Mark;
                stack_Btn.Push(btn);
                BtnMatrix[row, col].Ownership = 2;
            }
        }
        public void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;           
            if (btn.BackgroundImage != null)
                return;
            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;
            int col = btn.Location.X / ChessBoardSize.CHESS_WIDTH;


            stack_Btn.Push(btn);

            if (CurrentPlayer == 0)
                BtnMatrix[row, col].Ownership = 1;
            else
                BtnMatrix[row, col].Ownership = 2;

            ButtonChess _btn = new ButtonChess(row, col, BtnMatrix[row, col].Ownership);
            stack_Save.Push(_btn);

            Mark(btn);

            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            ChangePlayer();

            if (Mode == 3)
            {
                if (playerMarked != null)
                {
                    playerMarked(this, new ButtonClickEvent(GetChessPoint(btn)));
                }

                if (endGameCheck(btn))
                {
                    EndGame();
                }
            }
            else if(Mode == 2)
            {
                AI Ai = new AI();
                if (checkwin.EndGame(btn))
                {
                    if (CurrentPlayer == 1)
                        MessageBox.Show("X win");
                    else
                        MessageBox.Show("O Win");
                    NewGame();
                }
                else
                {
                    Point point = Ai.Start();
                    AI_Click(point);
                }
            }
            else
            {
                if (checkwin.EndGame(btn))
                {
                    if (CurrentPlayer == 1)
                        MessageBox.Show("X win");
                    else
                        MessageBox.Show("O Win");
                    NewGame();
                }
            }

        }
        public void AI_Click(Point point)
        {               
            Button btn = Matrix[point.Y][point.X];
            stack_Btn.Push(btn);

            int row = btn.Location.Y / ChessBoardSize.CHESS_HEIGHT;
            int col = btn.Location.X / ChessBoardSize.CHESS_WIDTH;
            if (CurrentPlayer == 0)
                BtnMatrix[point.Y, point.X].Ownership = 1;
            else
                BtnMatrix[point.Y, point.X].Ownership = 2;
            ButtonChess Btn = new ButtonChess(row,col, BtnMatrix[point.Y, point.X].Ownership);
            stack_Save.Push(Btn);
            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();
            if (checkwin.EndGame(btn))
            {
                if (CurrentPlayer == 1)
                    MessageBox.Show("X win");
                else
                    MessageBox.Show("O Win");
                NewGame();
            }
        }

        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.Y][point.X];

            if (btn.BackgroundImage != null)
                return;

            Mark(btn);

            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            ChangePlayer();

            if (endGameCheck(btn))
            {
                EndGame();
            }
        }
        public void EndGame()
        {
            if (endedGame != null)
            {
                endedGame(this, new EventArgs());
            }
        }

        public bool Undo()
        {
            if (PlayTimeLine.Count <= 0)
                return false;

            bool undo1 = UndoAStep();
            bool undo2 = UndoAStep();

            PlayInfo lastPoint = PlayTimeLine.Peek();
            CurrentPlayer = lastPoint.CurrentPlayer == 1 ? 0 : 1;
            return undo1 && undo2;
        }
        #region UNDO
        public void PvP_UnDo()
        {
            if (stack_Btn.Count == 0)
                return;
            Button Btn = stack_Btn.Pop();
            BtnMatrix[Btn.Location.Y / ChessBoardSize.CHESS_HEIGHT, Btn.Location.X / ChessBoardSize.CHESS_WIDTH].Ownership = 0;
            Btn.BackgroundImage = null;

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        }
        public void AI_Undo()
        {
            if (stack_Btn.Count == 0)
                return;
            else if (stack_Btn.Count == 1 && CurrentPlayer == 1)
                return;
            else
            {
                Button Btn = stack_Btn.Pop();
                BtnMatrix[Btn.Location.Y / ChessBoardSize.CHESS_HEIGHT, Btn.Location.X / ChessBoardSize.CHESS_WIDTH].Ownership = 0;
                Btn.BackgroundImage = null;

                Btn = stack_Btn.Pop();
                BtnMatrix[Btn.Location.Y / ChessBoardSize.CHESS_HEIGHT, Btn.Location.X / ChessBoardSize.CHESS_WIDTH].Ownership = 0;
                Btn.BackgroundImage = null;
            }
            
        }

        private bool UndoAStep()
        {
            if (PlayTimeLine.Count <= 0)
                return false;
            PlayInfo lastPoint = PlayTimeLine.Pop();
            Button btn = Matrix[lastPoint.Point.Y][lastPoint.Point.X];

            btn.BackgroundImage = null;

            if (PlayTimeLine.Count <= 0)
            {
                CurrentPlayer = 0;
            }
            else
            {
                lastPoint = PlayTimeLine.Peek();
            }

            ChangePlayer();

            return true;
        }
        #endregion
        private bool endGameCheck(Button btn)
        {
            return endCheckHorizontal(btn) || endCheckVertical(btn) || endCheckPrimary(btn) || endCheckSub(btn);
        }

        private Point GetChessPoint(Button btn)
        {
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);

            Point point = new Point(horizontal, vertical);

            return point;
        }

        private bool endCheckHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }

            int countRight = 0;
            for (int i = point.X + 1; i < ChessBoardSize.CHESS_BOARD_WIDTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft + countRight >= 5;
        }

        private bool endCheckVertical(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = point.Y + 1; i < ChessBoardSize.CHESS_BOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom >= 5;
        }

        private bool endCheckPrimary(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X - i < 0)
                {
                    break; 
                }

                if (Matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= ChessBoardSize.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= ChessBoardSize.CHESS_BOARD_HEIGHT || point.X + i >= ChessBoardSize.CHESS_BOARD_WIDTH)
                {
                    break;
                }

                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom >= 5;
        }

        private bool endCheckSub(Button btn)
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X + i > ChessBoardSize.CHESS_BOARD_WIDTH)
                {
                    break;
                }

                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= ChessBoardSize.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= ChessBoardSize.CHESS_BOARD_HEIGHT || point.X - i < 0)
                {
                    break;
                }

                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom >= 5;
        }

        public void Mark(Button btn)
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark;
        }

        public void ChangePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;

            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        #endregion
    }
    public class ButtonClickEvent : EventArgs
    {
        private Point clickedPoint;
        public Point ClickedPoint { get => clickedPoint; set => clickedPoint = value; }

        public ButtonClickEvent(Point point)
        {
            this.ClickedPoint = point;
        }
    }
}
