using System;

class Logger
{
    // Singleton instance
    private static Logger instance;
    private static readonly object lockObj = new object();

    // Private constructor to prevent external instantiation
    private Logger()
    {
        Console.WriteLine("======Logger instance created=====.");
    }

    // Public method to get the single instance
    public static Logger GetInstance()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
            }
        }
        return instance;
    }

    // A method to simulate logging
    public void Log(string message)
    {
        Console.WriteLine("LOG: " + message);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Logger logger1 = Logger.GetInstance();
        logger1.Log("=====This is the first log message.====");

        Logger logger2 = Logger.GetInstance();
        logger2.Log("====This is the second log message====.");

        // Verify both loggers are the same instance
        if (ReferenceEquals(logger1, logger2))
        {
            Console.WriteLine("\n=====Same logger instance used.=======");
        }
        else
        {
            Console.WriteLine("\n======Different logger instances created!=======");
        }
    }
}
