namespace AWbuy.AWBuy.Res
{ 
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class AWSpy
    {
        private static int nextReconMax = 0xa4cb80;
        private static int nextReconMin = 0xa4cb80;
        private Random random = new Random();
        private Stopwatch stopWatch = new Stopwatch();
        private List<awRecon> userList = new List<awRecon>();

        private struct Target
        {
            public string name;
            public string id;

        }

        public void addRecon(int targetID, bool sabUser)
        {
            if (targetID >= 0)
            {
                awRecon item = new awRecon {
                    targetID = Convert.ToString(targetID),
                    targetUser = "err"
                };
                this.userList.Add(item);
            }
        }

        public void addRecon(string Target, bool SabUser)
        {
            awRecon item = new awRecon {
                targetUser = Target,
                targetID = "-1",
                sabUser = SabUser,
                nextSab = 0L
            };
            this.userList.Add(item);
        }

        private void checkList()
        {
            for (int i = 0; i < this.userList.Count; i++)
            {
                if (this.userList[i].nextRecon < this.stopWatch.ElapsedMilliseconds)
                {
                    this.doRecon(this.userList[i]);
                }
                if (this.userList[i].sabUser && (this.userList[i].nextSab < this.stopWatch.ElapsedMilliseconds))
                {
                    this.doSab(this.userList[i]);
                }
            }
        }

        private bool doRecon(awRecon target) => 
            true;

        private void doSab(awRecon target)
        {
        }

        private void setNextRecon(awRecon reconTarget)
        {
            reconTarget.nextRecon = (this.stopWatch.ElapsedMilliseconds + nextReconMin) + this.random.Next(nextReconMax);
            reconTarget.reconTime = DateTime.Now;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct awRecon
        {
            public long nextRecon;
            public string targetID;
            public string targetUser;
            public DateTime reconTime;
            public bool sabUser;
            public long nextSab;
        }
    }
}

