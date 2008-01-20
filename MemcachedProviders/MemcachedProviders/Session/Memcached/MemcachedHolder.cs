using System;
using System.Collections.Generic;
using System.Text;

namespace MemcachedProviders.Session.Memcached
{
    [Serializable]
    internal class MemcachedHolder
    {
        private byte[] _objContent;
        private bool _bLocked;
        private DateTime _dSetTime;
        private int _iLockId;
        private int _iActionFlag;

        public MemcachedHolder(byte[] objContent, bool bLocked,
            DateTime dSetTime, int iLockId, int iActionFlag)
        {
            this._iActionFlag = iActionFlag;
            this._dSetTime = dSetTime;
            this._bLocked = bLocked;
            this._objContent = objContent;
            this._iLockId = iLockId;
        }


        public int ActionFlag
        {
            get { return this._iActionFlag; }
            set { this._iActionFlag = value; }
        }

        public int LockId 
        {
            get { return this._iLockId; }
            set { this._iLockId = value; } 
        }

        public DateTime SetTime
        {
            get { return this._dSetTime; }
            set { this._dSetTime = value; }
        }

        public TimeSpan LockAge
        {
            get { return DateTime.Now.Subtract(_dSetTime); }
        }

        public bool Locked
        {
            get { return this._bLocked; }
            set { this._bLocked = value; }
        }

        public byte[] Content 
        {
            get { return this._objContent; }
            set { this._objContent = value; }
        }
    }
}
