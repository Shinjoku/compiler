using System.IO;


namespace Compiler.General
{
    class Lexical
    {
        private StreamReader _file;
        public bool ReachedEndOfFile;

        public void OpenFile(string filePath)
        {
            _file = new StreamReader(filePath);
        }

        public char GetNextChar()
        {
            try
            {
                return (char)_file.Read();
            }
            catch (EndOfStreamException)
            {
                ReachedEndOfFile = true;
                return (char)0;
            }
        }

        public void CloseFile()
        {
            _file.Close();
        }

        public void Run()
        {
            var tokenGenerator = new TokenGenerator();
            tokenGenerator.GetToken();
        }
    }
}
