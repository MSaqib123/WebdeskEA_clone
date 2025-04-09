using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.ViewModels
{
    public class CoaCategoryHierarchyDto
    {
        public CoaCategoryDto CoaCategory { get; set; }
        public List<COADto> COADtoList { get; set; }
        public int CoaTypeId { get; set; } = 0;
    }
}
