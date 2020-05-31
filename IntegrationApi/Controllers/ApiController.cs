using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatabaseConnection;
using Models;
using IntegrationApi.DataReposytory;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IntegrationApi.Controllers
{

    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly InterfaceImplementation _repository = new InterfaceImplementation();


        [Route("/info")]
        [HttpGet]
        public ActionResult<string> GetInfo()
        {
            var SingleEntry = _repository.GetInfo();
            return Ok(SingleEntry);
        }
        [Route("/page")]
        [HttpPost]
        public ActionResult LoadPage(Strona pageNumber)
        {
            int loadOk = _repository.LoadPage(pageNumber.pageNumber);
            if (loadOk == 0)
                return NotFound();
            return NoContent();
        }

        [Route("/entries")]
        [HttpGet]
        public ActionResult<IEnumerable<EntryDB>> GetAllEntrys(int pageLimit, int pageID)
        {
            
            var ListOfEntrys = _repository.GetEntrys(pageLimit, pageID);
            return Ok(ListOfEntrys);
        }

        [Route("/entries/{id}")]
        [HttpGet("{id}")]
        public ActionResult GetEntry(int id)
        {
            EntryDB entry = _repository.GetEntry(id);
            return Ok(entry);
        }
    }
}