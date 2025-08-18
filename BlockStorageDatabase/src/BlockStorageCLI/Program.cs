using System.Text;

string path = @"C:\Users\s.huelskamp\source\repos\BlockStorageDatabase\BlockStorageDatabase\src\BlockStorageCLI\db\test.txt";


//Open the stream and read it back.
using (FileStream fs = File.OpenRead(path)) {
    byte[] b = new byte[4096];
    UTF8Encoding temp = new UTF8Encoding(true);
    int readLen;
    while ((readLen = fs.Read(b, 0, b.Length)) > 0) {
        Console.WriteLine(temp.GetString(b, 0, readLen));
    }
}


void AddText(FileStream fs, string value) {
    byte[] info = new UTF8Encoding(true).GetBytes(value);
    fs.Write(info, 0, info.Length);
}
