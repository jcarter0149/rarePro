using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Rare.Web.Data
{
    public class TagDataEntity
    {
        public int Id { get; set; }
        public string Label {  get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<PostDataEntity> Posts { get; set; }
    }
}
