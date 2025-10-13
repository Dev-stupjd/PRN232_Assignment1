using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ITagRepository
    {
        List<Tag> GetAllTag();
        void AddTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(Tag tag);
        Tag GetTagByID(int id);
    }
}
