using BussinessObjects;
using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
namespace DataAccessObjects.DAO
{
    public class TagDAO
    {
        private FunewsManagementContext _context;

        public TagDAO(FunewsManagementContext context)
        {
            _context = context;
        }
        public Tag? GetTagById(int TagId)
        {
            return _context.Tags.FirstOrDefault(u => u.TagId == TagId);
        }
        public List<Tag> GetAllTags()
        {
            return _context.Tags.ToList();
        }
        public void AddTag(Tag Tag)
        {
            _context.Tags.Add(Tag);
            _context.SaveChanges();
        }
        public void UpdateTag(Tag Tag)
        {
            _context.Entry<Tag>(Tag).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
        public void DeleteTag(int TagId)
        {
            var Tag = GetTagById(TagId);
            if (Tag != null)
            {
                _context.Tags.Remove(Tag);
                _context.SaveChanges();
            }
        }
    }
}
