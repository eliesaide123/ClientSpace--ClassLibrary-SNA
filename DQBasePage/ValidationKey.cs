using System;
using System.Collections.Generic;
using System.Text;

namespace DQBasePage
{
    public class ServiceSession
    {
        #region Fields

        private const string SVC_PREFIX = "WinServ-";

        private readonly string sessionId;

        private readonly int myRandNumber;

        #endregion

        #region Properties

        public string SessionID
        {
            get { return sessionId; }
        }

        public string ValMod
        {
            get { return myRandNumber.ToString("D"); }
        }

        public string Service
        {
            get
            {
                var txtBuffer = new StringBuilder();

                var rndNum = myRandNumber;

                if (rndNum >= 65)
                {
                    rndNum = rndNum % 55;
                }

                if (rndNum <= 5)
                {
                    rndNum = 10;
                }

                foreach (var sessionChar in SessionID.ToCharArray())
                {
                    var mod = (int)sessionChar;
                    mod = mod % rndNum;
                    txtBuffer.Append(mod.ToString("D"));
                }

                return txtBuffer.ToString();
            }
        }

        #endregion

        #region ctors

        public ServiceSession(int initialSeed)
        {
            var newSeed = GetSeed(initialSeed);

            var prng = new Random(newSeed);

            myRandNumber = prng.Next(1, 1000);

            var uniqueSessionNumber = myRandNumber;
            sessionId = string.Format("{0}{1}", SVC_PREFIX, uniqueSessionNumber);
        }

        #endregion

        #region Implementations

        private static int GetSeed(int initSeed)
        {
            return DateTime.Now.Millisecond * initSeed;
        }

        #endregion

        #region Partial Methods
        #endregion

        #region Overrides
        #endregion

        #region Interface Implementations
        #endregion

        #region Events / Handlers
        #endregion

        #region Iterators
        #endregion

        #region Nested Type Definitions {class, interface, structure, enumeration, delegate}
        #endregion
    }
}
