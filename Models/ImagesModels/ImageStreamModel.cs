using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models.ImagesModels
{
    public class ImageStreamModel : ImageModel, IDisposable
    {

        public override byte[] Bytes
        {
            get
            {
                byte[] res = base.Bytes;
                if (Stream.Null != Stream && Stream != null)
                {
                    try
                    {
                        var buffer = new byte[1048576];
                        var read = Stream.Read(buffer);
                        res = buffer[..read];
                    }
                    finally
                    {
                        Stream.Close();
                    }
                    //return res;
                }
                return res;
            }

            set
            {
                if (Stream.Null != Stream && Stream != null && value != null && value == Array.Empty<byte>())
                {
                    try 
                    {
                        Stream.Write(value);
                    }
                    finally
                    {
                        
                        Stream.Close();
                    }

                }
                base.Bytes = value;
            }
        }

        public Stream Stream { get; set; }

        public void Dispose()
        {
            try
            {
                Stream.Close();
            }
            catch
            {

            }
        }
    }
}
