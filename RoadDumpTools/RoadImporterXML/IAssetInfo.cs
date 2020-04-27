using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadImporterXML
{
    //code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
    public interface IAssetInfo
    {
        void ReadFromGame(NetInfo gameNetInfo);
        ///void WriteToGame(NetInfo gameNetInfo);  not needed for NDT
    }
}
