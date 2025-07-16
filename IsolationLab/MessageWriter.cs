namespace IsolationLab
{
    public static class MessageWriter
    {
        public static async Task COMMIT(string message, StreamWriter streamWriter, bool persistent = false)
        {
            await streamWriter.WriteLineAsync(message);

            if (persistent)
            {
                try
                {
                    await using var dataFs = new FileStream(Program.dataPath, FileMode.Append, FileAccess.Write);
                    await using var dataStreamWriter = new StreamWriter(dataFs);
                    await dataStreamWriter.WriteLineAsync(message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        public async static Task ROLLBACK(string logPath, string dataPath, bool dataDeletion = false)
        {

            await using var logStream = new FileStream(logPath, FileMode.Open, FileAccess.ReadWrite);
            if (logStream.Length == 0) return;
            var bytesToRemove = logStream.Length + Environment.NewLine.Length;
            logStream.SetLength(0);
            await logStream.DisposeAsync();

            if (dataDeletion)
            {
                using var dataStream = new FileStream(dataPath, FileMode.Open, FileAccess.ReadWrite);
                if (dataStream.Length == 0) return;

                long newDataLength = Math.Max(0, dataStream.Length - bytesToRemove) + Environment.NewLine.Length;
                dataStream.SetLength(newDataLength);
                await dataStream.DisposeAsync();
            }
        }

    }
}
