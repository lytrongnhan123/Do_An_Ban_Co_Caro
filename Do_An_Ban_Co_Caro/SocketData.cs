using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do_An_Ban_Co_Caro
{
    [Serializable]
    public class SocketData
    {
        private int command;
        public int Command { get => command; set => command = value; }

        private Point point;
        public Point Point { get => point; set => point = value; }

        private string message;
        public string Message { get => message; set => message = value; }

        public SocketData(int command, string message, Point point)
        {
            this.Command = command;
            this.Message = message;
            this.Point = point;
        }
    }
    public enum SocketCommand
    {
        SEND_POINT,
        NOTIFY,
        NEW_GAME,
        NEW_GAME_PVP,
        UNDO,
        END_GAME,
        TIME_OUT,
        START_GAME,
        EXIT
    }
}
