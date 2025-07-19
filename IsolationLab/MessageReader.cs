namespace IsolationLab
{
    public static class MessageReader
    {
        public static async Task<string> ReadData()
        {
            using FileStream fileStream = new FileStream(
                Program.dataPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            using StreamReader dataStreamReader = new StreamReader(fileStream);
            var data = await dataStreamReader.ReadToEndAsync();

            fileStream.Dispose();
            dataStreamReader.Dispose();
            return data;
        }



        public static async Task<string> ReadLog()
        {
            using FileStream logFs = new FileStream(
                Program.logPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            using StreamReader logStreamReader = new StreamReader(logFs);
            var data = await logStreamReader.ReadToEndAsync();

            logFs.Dispose();
            logStreamReader.Dispose();
            return data;
        }
    }
}
