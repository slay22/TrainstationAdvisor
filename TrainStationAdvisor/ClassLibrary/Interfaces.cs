using System;
using System.Data;

namespace TrainStationAdvisor.ClassLibrary.Interfaces
{
    public interface ICellStorage
    {
        void InsertCellId(string ACellId, string ALatitude, string ALongitude);
        DataRow GetCellByID(string ACellId);
    }
}
