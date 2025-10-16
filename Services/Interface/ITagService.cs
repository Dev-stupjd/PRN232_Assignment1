using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITagService
    {
        // Queries
        IEnumerable<Tag> GetAll(string? keyword = null, int page = 1, int pageSize = 20);
        Tag GetById(int id);

        // Commands
        Tag Create(Tag tag);
        Tag Update(Tag tag);
        bool Delete(int id);
    }
}
