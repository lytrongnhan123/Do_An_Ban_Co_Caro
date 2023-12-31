﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Do_An_Ban_Co_Caro
{
    class Mp3Player
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwdCallBack);

        public void OpenFile(string File)
        {
            string Format = @"open ""{0}"" type MPEGVideo alias MediaFile";
            string command = string.Format(Format, File);
            mciSendString(command, null, 0, 0);
        }

        public void PlayMusic()
        {
            string command = "play MediaFile";
            mciSendString(command, null, 0, 0);
        }
        public void PauseMusic()
        {
            string command = "stop MediaFile";
            mciSendString(command, null, 0, 0);
        }
    }
}

