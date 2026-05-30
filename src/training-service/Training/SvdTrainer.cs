using TrainingService.Data;
using TrainingService.Export;

namespace TrainingService.Training;

public static class SvdTrainer
{
    public static SvdWeightsExport Train(IReadOnlyList<ProcessedRecord> records, TrainingConfig cfg)
    {
        Console.WriteLine($"\n[SVD] Construindo mappings de IDs...");

        // Mappings sequenciais
        var userIds = records.Select(r => r.UserId).Distinct().OrderBy(x => x).ToList();
        var itemIds = records.Select(r => r.ItemId).Distinct().OrderBy(x => x).ToList();

        var userToInner = userIds.Select((id, idx) => (id, idx))
                                 .ToDictionary(t => t.id.ToString(), t => t.idx);
        var itemToInner = itemIds.Select((id, idx) => (id, idx))
                                 .ToDictionary(t => t.id, t => t.idx);
        var innerToItem = itemIds.Select((id, idx) => (id, idx))
                                 .ToDictionary(t => t.idx.ToString(), t => t.id);

        int nUsers = userIds.Count;
        int nItems = itemIds.Count;
        int k = cfg.NFactors;

        Console.WriteLine($"      {nUsers} usuários, {nItems} itens, {records.Count} interações.");

        // Converter para índices internos
        var interactions = records
            .Select(r => (u: userToInner[r.UserId.ToString()], i: itemToInner[r.ItemId], r: (double)r.Ranking))
            .ToArray();

        // Train/test split (últimos 20% como test)
        int testStart = (int)(interactions.Length * (1.0 - cfg.TestSplitRatio));
        var train = interactions[..testStart];
        var test = interactions[testStart..];

        Console.WriteLine($"      Train: {train.Length} | Test: {test.Length}");

        // Global mean sobre o treino
        double globalMean = train.Average(x => x.r);

        // Inicializar pesos
        var rng = new Random(cfg.RandomSeed);
        double[][] pu = InitMatrix(nUsers, k, rng);
        double[][] qi = InitMatrix(nItems, k, rng);
        double[] bu = new double[nUsers];
        double[] bi = new double[nItems];

        Console.WriteLine($"\n[SVD] Treinando por {cfg.NEpochs} épocas (lr={cfg.LrAll}, reg={cfg.RegAll})...");

        double lr = cfg.LrAll;
        double reg = cfg.RegAll;

        // SGD
        var shuffled = train.ToArray();
        for (int epoch = 0; epoch < cfg.NEpochs; epoch++)
        {
            Shuffle(shuffled, rng);

            foreach (var (u, i, r) in shuffled)
            {
                double dot = Dot(pu[u], qi[i], k);
                double err = r - (globalMean + bu[u] + bi[i] + dot);

                // Salvar pu antes de atualizar (para update de qi ser correto)
                double[] puOld = (double[])pu[u].Clone();

                for (int f = 0; f < k; f++)
                {
                    pu[u][f] += lr * (err * qi[i][f] - reg * pu[u][f]);
                    qi[i][f] += lr * (err * puOld[f] - reg * qi[i][f]);
                }

                bu[u] += lr * (err - reg * bu[u]);
                bi[i] += lr * (err - reg * bi[i]);
            }

            if ((epoch + 1) % 10 == 0)
            {
                double rmse = ComputeRmse(test, globalMean, bu, bi, pu, qi, k);
                Console.WriteLine($"      Época {epoch + 1:D2}/{cfg.NEpochs} — RMSE (test): {rmse:F4}");
            }
        }

        double finalRmse = ComputeRmse(test, globalMean, bu, bi, pu, qi, k);
        Console.WriteLine($"\n[SVD] RMSE final (test 20%): {finalRmse:F4}");

        return new SvdWeightsExport
        {
            ModelType = "funk_svd",
            Hyperparameters = new SvdHyperparametersExport
            {
                NFactors = cfg.NFactors,
                LrAll = cfg.LrAll,
                RegAll = cfg.RegAll,
                NEpochs = cfg.NEpochs,
            },
            RatingScale = [cfg.RatingMin, cfg.RatingMax],
            GlobalMean = globalMean,
            NUsers = nUsers,
            NItems = nItems,
            UserIdToInner = userToInner,
            ItemIdToInner = itemToInner,
            InnerToItemId = innerToItem,
            Bu = bu,
            Bi = bi,
            Pu = pu,
            Qi = qi,
        };
    }

    // -------------------------------------------------------------------------

    private static double[][] InitMatrix(int rows, int cols, Random rng)
    {
        var m = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            m[i] = new double[cols];
            for (int j = 0; j < cols; j++)
                m[i][j] = SampleNormal(rng, 0.0, 0.1);
        }
        return m;
    }

    private static double SampleNormal(Random rng, double mean, double std)
    {
        // Box-Muller transform
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        return mean + std * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
    }

    private static double Dot(double[] a, double[] b, int k)
    {
        double s = 0;
        for (int f = 0; f < k; f++) s += a[f] * b[f];
        return s;
    }

    private static double ComputeRmse(
        (int u, int i, double r)[] pairs,
        double mu, double[] bu, double[] bi, double[][] pu, double[][] qi, int k)
    {
        if (pairs.Length == 0) return 0;
        double sse = 0;
        foreach (var (u, i, r) in pairs)
        {
            double pred = mu + bu[u] + bi[i] + Dot(pu[u], qi[i], k);
            sse += Math.Pow(r - pred, 2);
        }
        return Math.Sqrt(sse / pairs.Length);
    }

    private static void Shuffle<T>(T[] arr, Random rng)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}
