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
            return data;
        }
    }
}
