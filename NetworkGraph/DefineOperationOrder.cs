using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetworkGraph;
using NetworkGraph.models;
using Google.OrTools.Sat;

namespace NetworkGraph
{
    public class DefineOperationOrder(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public void DefineOperationOrderMethod(List<OperationTiming> timings)
        {
            // 1. Получаем словарь {EquipmentId: List<OperationId>}
            var sql = @"SELECT 
           ovc.Equipment_Id AS EquipmentId,
           ov.Operation_Id AS OperationId
        FROM Operation_Variant_Component ovc
        JOIN Operation_Variant ov ON ovc.Operation_Variant_Id = ov.Id
        WHERE ovc.Equipment_Id IS NOT NULL";

            var equipmentOperations = _context.Database.SqlQueryRaw<EquipmentOperation>(sql)
                .AsEnumerable()
                .GroupBy(x => x.EquipmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.OperationId).Distinct().ToList()
                );

            // 2. Создаем хранилище для бинарных массивов
            Dictionary<int, int[]> equipmentBinaryMasks = new Dictionary<int, int[]>();

            // 3. Перебираем все станки и создаем для каждого бинарный массив
            foreach (var equipment in equipmentOperations)
            {
                int equipmentId = equipment.Key;
                int operationCount = equipment.Value.Count; // Количество операций для этого станка

                // Создаем бинарный массив нужного размера (инициализирован нулями)
                int[] binaryMask = new int[operationCount];

                // Добавляем в хранилище
                equipmentBinaryMasks.Add(equipmentId, binaryMask);
            }

            Console.WriteLine("A");
        }

        // Вспомогательный класс для результата SQL-запроса
        public class EquipmentOperation
        {
            public int EquipmentId { get; set; }
            public int OperationId { get; set; }
        }

        public Dictionary<int, List<OperationSchedule>> OptimizeSchedule(
    Dictionary<int, List<OperationTiming>> operationsByEquipment,
    int targetOperationId = -1)
        {
            var result = new Dictionary<int, List<OperationSchedule>>();

            foreach (var equipment in operationsByEquipment)
            {
                int equipmentId = equipment.Key;
                var operations = equipment.Value
                    .Select((t, index) => new OperationSchedule
                    {
                        OperationId = t.Operation.Id,
                        LocalIndex = index + 1,
                        EarlyStart = t.EarlyStart,
                        Duration = t.Variant.Duration,
                        LateFinish = t.LateFinish
                    }).ToList();

                // Создаем модель
                CpModel model = new CpModel();

                // 1. Переменные решения
                var starts = operations.Select(op =>
                    model.NewIntVar((int)op.EarlyStart, (int)op.LateFinish, $"start_{op.LocalIndex}")).ToArray();
                var ends = operations.Select(op =>
                    model.NewIntVar((int)op.EarlyStart, (int)op.LateFinish, $"end_{op.LocalIndex}")).ToArray();

                // 2. Добавляем ограничения
                for (int i = 0; i < operations.Count; i++)
                {
                    // Длительность операции
                    model.Add(ends[i] == starts[i] + (int)operations[i].Duration);

                    // Временные ограничения
                    model.Add(starts[i] >= (int)operations[i].EarlyStart);
                    model.Add(ends[i] <= (int)operations[i].LateFinish);
                }

                // 3. Ограничение на непересечение операций
                for (int i = 0; i < operations.Count; i++)
                {
                    for (int j = i + 1; j < operations.Count; j++)
                    {
                        // Используем булевы переменные для определения порядка
                        var precedes = model.NewBoolVar($"precedes_{i}_{j}");
                        model.Add(starts[i] + (int)operations[i].Duration <= starts[j]).OnlyEnforceIf(precedes);
                        model.Add(starts[j] + (int)operations[j].Duration <= starts[i]).OnlyEnforceIf(precedes.Not());
                    }
                }

                // 4. Целевая функция (минимизация времени окончания)
                if (targetOperationId >= 0)
                {
                    var targetOp = operations.FirstOrDefault(op => op.OperationId == targetOperationId);
                    if (targetOp != null)
                    {
                        model.Minimize(ends[operations.IndexOf(targetOp)]);
                    }
                }
                else
                {
                    // Минимизация максимального времени окончания
                    var maxEnd = model.NewIntVar(0, (int)operations.Max(op => op.LateFinish), "max_end");
                    model.AddMaxEquality(maxEnd, ends);
                    model.Minimize(maxEnd);
                }

                // 5. Решаем
                CpSolver solver = new CpSolver();
                CpSolverStatus status = solver.Solve(model);

                if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
                {
                    // Заполняем результаты
                    for (int i = 0; i < operations.Count; i++)
                    {
                        operations[i].ActualStart = solver.Value(starts[i]);
                        operations[i].ActualFinish = solver.Value(ends[i]);
                    }

                    result.Add(equipmentId, operations);
                }
            }

            return result;
        }   
    }
}
