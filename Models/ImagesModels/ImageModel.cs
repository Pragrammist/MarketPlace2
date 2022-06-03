using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models.ImagesModels
{
    public class ImageModel
    {

        public static ImageModel Null { get; private set; } = new ImageModel();
        public int Id { get; set; }
        public virtual byte[] Bytes { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int Size { get; set; }
    }
}
