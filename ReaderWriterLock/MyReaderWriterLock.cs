using System;
using System.Threading;

namespace ReaderWriterLock;

public class MyReaderWriterLock : IRwLock
{
    private object readerLock = new();
    private int readersCounter;
    private AutoResetEvent resetEvent = new(true);
    private ManualResetEvent manualResetEvent = new(true);

    public void ReadLocked(Action action)
    {
        manualResetEvent.WaitOne();
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
        manualResetEvent.Reset();
        resetEvent.WaitOne();
        action();
        resetEvent.Set();
        manualResetEvent.Set();
    }
}