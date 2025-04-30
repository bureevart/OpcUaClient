using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Interfaces.Services
{
    public interface ITagCacheService
    {
        public Tag? GetTag(string displayName);

        public void SetTag(string displayName, Tag tag);

        public void DeleteTag(string displayName);
    }
}
