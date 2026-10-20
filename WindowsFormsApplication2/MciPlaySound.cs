using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace DoorAccess2025
{
    class MciPlaySound
    {
        #region 変数
        private StructMCI m_MciInfo = new StructMCI();
        private StringBuilder m_sbCommand = new StringBuilder();
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        private string m_strShortName = "";
        [MarshalAs(UnmanagedType.LPTStr, SizeConst = 128)]
        private string m_strCommandResult = "";
        private long m_lReturn = 0;
        private string m_strDeviceName = "";
        #endregion
        #region 
        public enum State
        {
            // 準備中
            Ready = 0,
            // 再生中
            Playing = 1,
            // 再生完了
            Stop = 2
        };
        public struct StructMCI
        {
            public string FileName;
            public State State;
        };
        #endregion

        #region 再生部品
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            string lpszLongPath,
            string shortFile,
            int cchBuffer);

        // WAVE再生部品
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        private static extern int mciSendString(
           string lpstrCommand,
           string lpstrReturnString,
           int uReturnLength,
           IntPtr hwndCallback);
        #endregion
        #region 
        public string DeviceName
        {
            get
            {
                return m_strDeviceName;
            }
        }
        public string FileName
        {
            get
            {

                return m_MciInfo.FileName;
            }
            set
            {
                lock (m_sbCommand)
                {
                    if (!File.Exists(value))
                    {
                        m_MciInfo.FileName = "";
                        throw new Exception("文件不存在。");
                    }
                    m_MciInfo.FileName = value;
                    m_lReturn =
                        mciSendString("close all",
                            m_strCommandResult,
                            m_strCommandResult.Length,
                            IntPtr.Zero);
                    if (m_lReturn != 0)
                    {
                        throw new Exception("初期化失敗。");
                    }
                    ClearCommand();
                    m_strShortName = "";
                    m_strShortName = m_strShortName.PadLeft(260, Convert.ToChar(" "));

                    m_lReturn = GetShortPathName(m_MciInfo.FileName,
                        m_strShortName,
                        m_strShortName.Length);


                    m_sbCommand.Append("open \"");
                    m_sbCommand.Append(GetPath(m_strShortName));
                    m_sbCommand.Append("\" type WaveAudio alias micmediaplay");

                    m_lReturn =
                        mciSendString(m_sbCommand.ToString(),
                            m_strCommandResult,
                            m_strCommandResult.Length,
                            IntPtr.Zero);


                    if (m_lReturn != 0)
                    {
                        throw new Exception("初期化失敗。");
                    }

                    m_strDeviceName = "micmediaplay";

                    m_lReturn =
                        mciSendString("set micmediaplay time format milliseconds",
                            m_strCommandResult,
                            m_strCommandResult.Length,
                            IntPtr.Zero);


                    if (m_lReturn != 0)
                    {
                        throw new Exception("初期化失敗。");
                    }


                    m_MciInfo.State = State.Ready;
                }
            }
        }
        #endregion
        #region 
        public void Play()
        {
            lock (m_sbCommand)
            {

                m_strCommandResult = "";
                m_strCommandResult =
                    m_strCommandResult.PadLeft(128, Convert.ToChar(" "));


                if (String.IsNullOrEmpty(m_MciInfo.FileName))
                {
                    throw new Exception("音声文件不存在。");
                }


                if (m_MciInfo.State != State.Ready)
                {
                    return;
                }
                m_lReturn = mciSendString("play micmediaplay from 0",
                    m_strCommandResult,
                    m_strCommandResult.Length,
                    IntPtr.Zero);
                if (m_lReturn != 0)
                {
                    throw new Exception("再生失敗。");
                }
                m_MciInfo.State = State.Playing;
            }
        }
        public void StopPlay()
        {
            lock (m_sbCommand)
            {
                m_MciInfo.State = State.Stop;
            }
        }
        public void Close()
        {
            m_lReturn = mciSendString("close all",
                m_strCommandResult,
                m_strCommandResult.Length,
                IntPtr.Zero);
            if (m_lReturn != 0)
            {
                throw new Exception("釈放失敗。");
            }
        }
        public bool IsStop()
        {
            lock (m_sbCommand)
            {

                m_strCommandResult = "";
                m_strCommandResult =
                    m_strCommandResult.PadLeft(128, Convert.ToChar(" "));
                if (m_MciInfo.State == State.Stop)
                {

                    if (String.IsNullOrEmpty(m_strDeviceName))
                    {
                        m_lReturn = mciSendString("close micmediaplay",
                            m_strCommandResult,
                            m_strCommandResult.Length,
                            IntPtr.Zero);
                        if (m_lReturn != 0)
                        {
                            throw new Exception("停止失敗。");
                        }

                        m_strDeviceName = "";
                    }
                    m_lReturn = mciSendString("close all",
                        m_strCommandResult,
                        m_strCommandResult.Length,
                        IntPtr.Zero);
                    if (m_lReturn != 0)
                    {
                        throw new Exception("停止失敗。");
                    }
                    return true;
                }
                m_lReturn = mciSendString("status micmediaplay mode",
                    m_strCommandResult,
                    m_strCommandResult.Length,
                    IntPtr.Zero);


                if (m_lReturn != 0)
                {
                    throw new Exception("状態取得失敗");
                }

                if (m_strCommandResult.IndexOf("stopped") >= 0)
                {
                    m_MciInfo.State = State.Ready;
                    return true;
                }
                return false;
            }
        }
        private string GetPath(string strPath)
        {

            if (strPath.Length < 1)
            {
                return "";
            }
            strPath = strPath.Trim();
            strPath = strPath.Substring(0, strPath.Length - 1);
            return strPath;
        }
        private void ClearCommand()
        {
            if (m_sbCommand.Length != 0)
            {
                m_sbCommand.Remove(0, m_sbCommand.Length);
            }
        }
        #endregion
        public static bool PlaySound(string pszSound, uint fdwSound)
        {
            mciSendString("close iris_sound_alias", null, 0, IntPtr.Zero);
            mciSendString("open \"" + System.AppDomain.CurrentDomain.BaseDirectory + pszSound + "\" alias iris_sound_alias", null, 0, IntPtr.Zero);
            mciSendString("play iris_sound_alias", null, 0, IntPtr.Zero);
            return true;
        }
    }
}
