namespace ValueBuffer.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mem = new ValueBufferMemoryStream();
            var buffer = new byte[10];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;
            }
            mem.Write(buffer);
            Console.WriteLine(mem.Position);
            mem.Position = 0;
            var b = new byte[10];
            mem.Read(b,5,7);
        }
    }
}