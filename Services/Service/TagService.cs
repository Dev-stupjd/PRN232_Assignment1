using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tags;

        public TagService(ITagRepository tags)
        {
            _tags = tags;
        }

        // READ ALL with simple search + paging
        public IEnumerable<Tag> GetAll(string? keyword = null, int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Invalid paging.");
            var data = _tags.GetAllTag().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                data = data.Where(t =>
                    (t.TagName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (t.Note?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return data
                .OrderByDescending(t => t.TagId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Tag GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be positive.", nameof(id));
            var tag = _tags.GetTagByID(id);
            return tag ?? throw new KeyNotFoundException("Tag not found.");
        }

        // CREATE with basic validation
        public Tag Create(Tag tag)
        {
            Validate(tag, isUpdate: false);

            // (optional) enforce unique tag name
            var exists = _tags.GetAllTag()
                              .Any(t => t.TagName!.Equals(tag.TagName, StringComparison.OrdinalIgnoreCase));
            if (exists)
                throw new InvalidOperationException("Tag name already exists.");

            _tags.AddTag(tag);
            return tag;
        }

        // UPDATE with basic validation
        public Tag Update(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            if (_tags.GetTagByID(tag.TagId) is null)
                throw new KeyNotFoundException("Tag not found.");

            Validate(tag, isUpdate: true);

            _tags.UpdateTag(tag);
            return tag;
        }

        // DELETE
        public bool Delete(int id)
        {
            var existing = _tags.GetTagByID(id);
            if (existing is null)
                return false;

            _tags.DeleteTag(existing);
            return true;
        }

        // --- helpers ---
        private static void Validate(Tag tag, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(tag.TagName))
                throw new ArgumentException("Tag name is required.");
            // add more rules as needed
        }
    }
}
