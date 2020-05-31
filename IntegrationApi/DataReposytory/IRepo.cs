using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Models;

namespace IntegrationApi.DataReposytory
{
    public interface IRepo
    {
        string GetInfo();
        EntryDB GetEntry(int id);
        int LoadPage(int page);
        IEnumerable<EntryDB> GetEntrys(int pageLimit, int pageID);
    }
}
