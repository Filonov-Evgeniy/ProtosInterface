using Microsoft.EntityFrameworkCore;

namespace OZTM.Data.Views;

[Keyless]
public partial class ComponentRelationship
{
    public int FinalProductId { get; set; }
    public int ParentProductId { get; set; }
    public int IncludedProductId { get; set; }
    public int OperationId { get; set; }
    public int OperationVariantId { get; set; }
    public int EquipmentId { get; set; }
    public int FunctionalAreaId { get; set; }
    public int TerritorialAreaId { get; set; }
    public double Amount { get; set; }
    public double TotalAmount { get; set; }
}