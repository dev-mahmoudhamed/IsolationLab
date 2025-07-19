using System.IO.MemoryMappedFiles;

namespace IsolationLab
{
    public static class ReadCommittedSession
    {
        private static bool _isLocked;
        private static string filePath = "shared_bool.bin";
        private static long fileSize = 1;
        private static FileStream _logFs = new FileStream(Program.logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
        private static StreamWriter _logStreamWriter = new StreamWriter(_logFs, leaveOpen: true) { AutoFlush = true };
        public async static Task BeginReadCommittedSession()
        {
            _isLocked = GetLock();
            try
            {
                if (!_isLocked)
                {
                    SetLock(true);
                    _isLocked = GetLock();
                }
                while (true)
                {
                    Console.Write("> ");
                    var cmd = Console.ReadLine()?.Trim()!;

                    if (cmd.ToUpper().StartsWith("SELECT"))
                    {
                        while (GetLock())
                        {
                            Thread.Sleep(100);
                        }
                        var data = await MessageReader.ReadData();
                        //Console.WriteLine(data.Length != 0 ? data : "File is empty");
                        continue;
                    }

                    if (cmd.ToUpper() == "COMMIT")
                    {
                        Console.WriteLine("Commit transaction");
                        var data = await MessageReader.ReadLog();
                        await _logStreamWriter.DisposeAsync();
                        await _logFs.DisposeAsync();

                        string envNewLine = Environment.NewLine;
                        data = data.Substring(0, data.Length - envNewLine.Length);
                        await MessageWriter.COMMIT(data);
                        await MessageWriter.ClearLog();

                        SetLock(false);
                        _isLocked = GetLock();
                        continue;
                    }

                    if (cmd.ToUpper() == "ROLLBACK")
                    {
                        Console.WriteLine("Rolling back transaction");

                        await _logStreamWriter.DisposeAsync();
                        await _logFs.DisposeAsync();

                        await MessageWriter.ROLLBACK(Program.logPath, Program.dataPath, isPersist: false);
                        continue;
                    }
                    await MessageWriter.WriteData(cmd!, _logStreamWriter, persistent: false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static void SetLock(bool value)
        {
            if (!File.Exists(filePath))
            {
                using var fs = new FileStream(filePath,
                    FileMode.CreateNew,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                fs.SetLength(fileSize);
            }

            using var mmf = MemoryMappedFile.CreateFromFile(
                filePath,
                FileMode.Open,
                "MySharedBool",
                fileSize,
                MemoryMappedFileAccess.ReadWrite
            );

            using var accessor = mmf.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Write);
            accessor.Write(0, value);
        }
        public static bool GetLock()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("[GetLock] File does not exist yet.");
                return false;
            }

            using var mmf = MemoryMappedFile.CreateFromFile(
                filePath,
                FileMode.Open,
                "MySharedBool",
                fileSize,
                MemoryMappedFileAccess.Read
            );

            using var accessor = mmf.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Read);
            bool value = accessor.ReadBoolean(0);
            return value;
        }
    }
}