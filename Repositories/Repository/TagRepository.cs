using BussinessObjects.Models;
using DataAccessObjects.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _TagDAO;

        public TagRepository(TagDAO TagDAO)
        {
            _TagDAO = TagDAO;
        }
        public List<Tag> GetAllTag() => _TagDAO.GetAllTags();
        public void AddTag(Tag Tag) => _TagDAO.AddTag(Tag);
        public void UpdateTag(Tag Tag) => _TagDAO.UpdateTag(Tag);
        public void DeleteTag(Tag Tag) => _TagDAO.DeleteTag(Tag.TagId);
        public Tag GetTagByID(int id) => _TagDAO.GetTagById(id);
    }
}
