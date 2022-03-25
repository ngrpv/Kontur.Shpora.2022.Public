using System;
using System.Threading;

namespace ReaderWriterLock;

public class MyReaderWriterLock : IRwLock
{
    private object readerLock = new();
    private int readersCounter;
    private AutoResetEvent resetEvent = new(true);

    public void ReadLocked(Action action)
    {
       // resetEvent.WaitOne();
        lock (readerLock)
        {
            readersCounter++;
            if (readersCounter == 1)
            {
               resetEvent.WaitOne();
            }
        }

        action();
        lock (readerLock)
        {
            readersCounter--;
            if (readersCounter == 0)
            {
                resetEvent.Set();
            }
        }
    }

    public void WriteLocked(Action action)
    {
      //  resetEvent.WaitOne();
        resetEvent.Reset();
        action();
        resetEvent.Set();
    }
}