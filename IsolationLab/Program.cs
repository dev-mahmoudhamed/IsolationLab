namespace IsolationLab
{

    public class Program
    {
        public readonly static string logPath = Path.Combine(Directory.GetCurrentDirectory(), "data/log.txt");
        public readonly static string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data/data.txt");
        static async Task Main()
        {
            string directory = Path.GetDirectoryName(logPath)!;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var iso = AskUserForIsolationLevel();
            Console.WriteLine($"Begin transaction with [{((IsolationLevel)iso)}] implementation.\n");
            await SessionContext.StartSessionAsync(iso);
        }

        private static int AskUserForIsolationLevel()
        {
            Console.WriteLine("┌──────────────────────────────────────────┐");
            Console.WriteLine("│  Select SQL Server Isolation Level       │");
            Console.WriteLine("├──────────────────────────────────────────┤");
            Console.WriteLine("│  1. Read Uncommitted  (NOLOCK)           │");
            Console.WriteLine("│  2. Read Committed    (default)          │");
            Console.WriteLine("└──────────────────────────────────────────┘");

            while (true)
            {
                Console.Write("Enter 1‑2 : ");
                var choice = Console.ReadLine()?.Trim();
                int iso;
                bool isValidIso = int.TryParse(choice, out iso);
                if (isValidIso && (iso > 0 && iso < 3))
                {
                    return iso;
                }
                Console.WriteLine("Invalid choice. Please enter 1‑5.");
            }
        }

    }

    static class SessionContext
    {
        public static async Task StartSessionAsync(int iso)
        {
            switch (iso)
            {
                case 1:
                    await ReadUncommittedSession.BeginReadUncommittedSession();
                    break;
                case 2:
                    await ReadCommittedSession.BeginReadCommittedSession();
                    break;
                default:
                    break;
            }
        }
    }
}