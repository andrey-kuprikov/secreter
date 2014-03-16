using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Secreter.Properties;


namespace Secreter
{
    public class Manager
    {
        private string dataFileLocation;

        public Manager(string dataFileLocation)
        {
            this.dataFileLocation = dataFileLocation;
        }

        public string secretKey { private get; set; }

        public string[] GetNames()
        {
            var passwords = this.Load();

            return passwords.Select(n => n.name).ToArray();
        }

        public void Add(string name, string login, string password)
        {
            var passwords = this.Load().ToList();

            passwords.Add(new PasswordRecord {
                name = name,
                login = login,
                password = password
            });

            this.Save(passwords.ToArray());
        }

        public PasswordRecord Get(string name)
        {
            var passwords = this.Load();

            return passwords.Where(p => p.name == name).First();
        }

        private void Save(IEnumerable<PasswordRecord> passwords)
        {
            if (!File.Exists(this.dataFileLocation))
            {
                this.CreateEmptyDataFile();
            }

            byte[] IV;
            using (var fs = new FileStream(this.dataFileLocation, FileMode.Open))
            {
                var IVLength = Encryptor.GetIVLength();
                IV = new byte[IVLength];
                fs.Read(IV, 0, IVLength);
            }
            
            var serializer = this.GetSerializer();
            var ms = new MemoryStream();
            serializer.Serialize(ms, passwords);
            ms.Seek(0, SeekOrigin.Begin);

            var enc = new Encryptor(ms, IV);

            var tempKey = this.secretKey;
            if (string.IsNullOrEmpty(tempKey))
            {
                throw new InvalidKeyException();
            }

            using (var fs = new FileStream(this.dataFileLocation, FileMode.Open))
            {
                //fs.Seek(0, SeekOrigin.Begin);
                fs.Write(IV, 0, IV.Length);

                enc.Encrypt(tempKey, fs);
                tempKey = null;
            }
        }

        private PasswordRecord[] Load()
        {
            if (!File.Exists(this.dataFileLocation))
            {
                this.CreateEmptyDataFile();
            }

            using (var fs = new FileStream(this.dataFileLocation, FileMode.Open))
            {
                var IVLength = Encryptor.GetIVLength();
                var IV = new byte[IVLength];
                fs.Read(IV, 0, IVLength);

                // var dataStream = new MemoryStream();
                // fs.CopyTo(dataStream);
                // dataStream.Seek(0, SeekOrigin.Begin);

                var enc = new Encryptor(fs, IV);

                var tempKey = this.secretKey;
                if (string.IsNullOrEmpty(tempKey))
                {
                    throw new InvalidKeyException();
                }

                using (var decryptedStream = enc.Decrypt(tempKey))
                {
                    tempKey = null;

                    var serializer = this.GetSerializer();
                    //var tmp = new MemoryStream();
                    //decryptedStream.CopyTo(tmp);
                    //tmp.Seek(0, SeekOrigin.Begin);

                    var passwords = (PasswordRecord[])serializer.Deserialize(decryptedStream);

                    return passwords;
                }
            }
        }

        private XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(PasswordRecord[]), new XmlRootAttribute() { ElementName = "items" });
        }

        private void CreateEmptyDataFile()
        {
            var IV = Encryptor.CreateIV();

            using (var fs = new FileStream(this.dataFileLocation, FileMode.CreateNew))
            {
                fs.Write(IV, 0, IV.Length);
            }

            this.Save(new PasswordRecord[0]);
        }
    }
}
