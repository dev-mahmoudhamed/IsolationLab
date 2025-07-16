namespace IsolationLab
{
    public static class ReadUncommittedSession
    {
        public static async Task BeginReadUncommittedSession()
        {
            Console.Write("> ");
            var initialCommand = Console.ReadLine()?.Trim()!;

            try
            {
                await using var logFs = new FileStream(Program.logPath, FileMode.Append, FileAccess.Write);
                await using var logStreamWriter = new StreamWriter(logFs, leaveOpen: true) { AutoFlush = true };
                await MessageWriter.COMMIT(initialCommand, logStreamWriter, persistent: true);

                while (true)
                {
                    Console.Write("> ");
                    var cmd = Console.ReadLine()?.Trim()!;

                    if (cmd.ToUpper().StartsWith("SELECT"))
                    {
                        var data = await MessageReader.ReadData();
                        Console.WriteLine(data);
                    }

                    if (cmd.ToUpper() == "COMMIT")
                    {
                        Console.WriteLine("Commit transaction");
                        await logStreamWriter.DisposeAsync();
                        await logFs.DisposeAsync();

                        await File.WriteAllTextAsync(Program.logPath, string.Empty);
                        Environment.Exit(0);
                    }

                    if (cmd.ToUpper() == "ROLLBACK")
                    {
                        Console.WriteLine("Rolling back transaction");

                        await logStreamWriter.DisposeAsync();
                        await logFs.DisposeAsync();

                        await MessageWriter.ROLLBACK(Program.logPath, Program.dataPath, dataDeletion: true);
                        Environment.Exit(0);
                    }
                    await MessageWriter.COMMIT(cmd!, logStreamWriter, persistent: true);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}