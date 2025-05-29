using Microsoft.EntityFrameworkCore;
using NetworkGraph;
using NetworkGraph.models;


public class NetworkCalculator(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    private List<OperationTiming> _timings;

    public List<OperationTiming> CalculateTimings()
    {
        _timings = new List<OperationTiming>();


        var validProducts = _context.Product
            .Where(p => p.TypeId != 3)
            .Include(p => p.ProductLinkParentProducts) 
            .Include(p => p.ProductLinkIncludedProducts) 
            .ToList();

        var operations = _context.Operation
            .Include(o => o.OperationVariants)
            .Include(o => o.Product)
            .Where(o => validProducts.Select(p => p.Id).Contains(o.ProductId))
            .ToList();

        var finalProducts = validProducts.Where(p => p.TypeId == 0).ToList();
        if (finalProducts.Any())
        {
            var fakeProduct = new Product { Id = -1, TypeId = -1, Name = "фиктивное" };
            var fakeOperation = new Operation
            {
                Id = -1,
                Product = fakeProduct,
                ProductId = fakeProduct.Id,
                OperationVariants = new List<OperationVariant>
            {
                new OperationVariant { Duration = 0 }
            }
            };
            operations.Add(fakeOperation);
        }

        var dependencies = new Dictionary<int, List<int>>();

        foreach (var op in operations)
        {
            var deps = new List<int>();

            if (op.Product.TypeId == 1)
            {
                deps.AddRange(operations
                    .Where(o => o.ProductId == op.ProductId &&
                               o.Id < op.Id &&
                               o.Product.TypeId != 3)
                    .Select(o => o.Id));
            }

            deps.AddRange(
                from pl in _context.ProductLink
                where pl.ParentProductId == op.ProductId
                join o in _context.Operation on pl.IncludedProductId equals o.ProductId
                where o.Id < op.Id && o.Product.TypeId != 3
                select o.Id
            );

            if (op.Id == -1)
            {
                deps.AddRange(
                    operations
                        .Where(o => o.Product.TypeId == 0 && o.Id != -1) 
                        .Select(o => o.Id)
                );
            }

            dependencies[op.Id] = deps.Distinct().ToList();
        }

        var sortedOps = new List<Operation>();
        var visited = new HashSet<int>();

        void Visit(Operation op)
        {
            if (visited.Contains(op.Id)) return;
            visited.Add(op.Id);

            foreach (var depId in dependencies[op.Id])
            {
                var depOp = operations.FirstOrDefault(o => o.Id == depId);
                if (depOp != null) Visit(depOp);
            }

            sortedOps.Add(op);
        }

        foreach (var op in operations)
        {
            if (!visited.Contains(op.Id))
            {
                Visit(op);
            }
        }

        foreach (var op in sortedOps)
        {
            var variant = op.OperationVariants.FirstOrDefault();
            if (variant == null) continue;

            var timing = new OperationTiming
            {
                Operation = op,
                Variant = variant
            };

            if (!dependencies[op.Id].Any())
            {
                timing.EarlyStart = 0;
            }
            else
            {
                timing.EarlyStart = _timings
                    .Where(t => dependencies[op.Id].Contains(t.Operation.Id))
                    .Max(t => t.EarlyFinish); 
            }

            timing.EarlyFinish = timing.EarlyStart + variant.Duration;
            _timings.Add(timing);
        }

        if (_timings.Any())
        {
            double projectDuration = _timings.Max(t => t.EarlyFinish);
            foreach (var timing in _timings.AsEnumerable().Reverse())
            {
                var successors = operations
                    .Where(o => dependencies[o.Id].Contains(timing.Operation.Id))
                    .Select(o => o.Id)
                    .ToList();

                timing.LateFinish = successors.Any()
                    ? _timings.Where(t => successors.Contains(t.Operation.Id))
                          .Min(t => t.LateStart)
                    : projectDuration;

                timing.LateStart = timing.LateFinish - timing.Variant.Duration;
            }
        }

        return _timings;
    }
}