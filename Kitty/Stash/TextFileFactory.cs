using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kitty.Stash
{
    internal sealed class TextFileFactory : ITextFactory
    {
        public event EventHandler ReadyToUpload;

        private const int MAX_FILE_LENGTH = 255;
        private readonly string rootDir;

        public TextFileFactory(string rootDir)
        {
            this.rootDir = rootDir;
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);
        }

        public ITextStash GenerateNew()
        {
            var textFile = new TextFile();
            textFile.Disposed += TextStash_Disposed;
            return textFile;
        }

        private void TextStash_Disposed(object sender, EventArgs e)
        {
            TextFile textFile = sender as TextFile;
            textFile.Disposed -= TextStash_Disposed;
            string srcFile = textFile.FilePath;
            string desFile = Path.Combine(rootDir, Path.GetFileName(srcFile));
            File.Move(srcFile, desFile);
            ReadyToUpload?.Invoke(this, e);
#if DEBUG
            Debug.WriteLine(File.ReadAllText(desFile));
#endif
        }

        public Task<BaseStash[]> GetStashsAsync()
        {
            return Task.Run(() =>
            {
                var files = Directory.GetFiles(rootDir);
                List<BaseStash> stashs = new List<BaseStash>(files.Length);
                foreach (string file in files)
                {
                    try
                    {
                        string raw = File.ReadAllText(file);
                        long createdTime = long.Parse(Path.GetFileName(file));
                        BaseStash stash = new TextStash()
                        {
                            Tag = file,
                            Content = Encoding.UTF8.GetBytes(raw),
                            Created = DateTime.FromBinary(createdTime)
                        };
                        stashs.Add(stash);
                    }
                    catch { }
                }
                return stashs.ToArray();
            });
        }

        public void DeleteStash(BaseStash stash)
        {
            string filePath = stash.Tag as string;
            if (!string.IsNullOrEmpty(filePath))
            {
                File.Delete(filePath);
#if DEBUG
                Debug.WriteLine("Deleted: " + filePath);
#endif
            }
        }

        private class TextFile : ITextStash
        {
            public event EventHandler Disposed;
            private StreamWriter writer;
            private int length = 0;

            public string FilePath { get; private set; }

            public bool IsEmpty { get; private set; } = true;

            public bool IsFull { get; private set; } = false;

            public void Dispose()
            {
                writer?.Dispose();
                writer = null;
                Disposed?.Invoke(this, EventArgs.Empty);
            }

            public void Write(string st)
            {
                writer.Write(st);
                length += st.Length;
                if (length >= MAX_FILE_LENGTH)
                    IsFull = true;
                IsEmpty = false;
            }

            public TextFile()
            {
                long createdTime = DateTime.Now.ToBinary();
                FilePath = Path.Combine(Path.GetTempPath(), createdTime.ToString());
                writer = new StreamWriter(new FileStream(FilePath, FileMode.Create, FileAccess.Write));
            }
        }
    }
}
