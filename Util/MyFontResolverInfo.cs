using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadyahua.Util
{
    class MyFontResolverInfo
      : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            faceName = Directory.GetCurrentDirectory() + "/" + faceName;
            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("STSONG_Bold", StringComparison.CurrentCultureIgnoreCase))
            {

                return new FontResolverInfo("STSONG_Bold.ttf");
            }
            else if (familyName.Equals("YaHei.Consolas.1.12", StringComparison.CurrentCultureIgnoreCase))
            {

                return new FontResolverInfo("YaHei.Consolas.1.12.ttf");
            }
            return new FontResolverInfo("YaHei.Consolas.1.12.ttf");
        }
    }
}
