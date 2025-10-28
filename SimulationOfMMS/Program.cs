using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== M/M/s Queue Simulation ===\n");

        // ===== Input Section =====
        Console.Write("Enter 'r' for rate or 'm' for mean (arrival): ");
        char arrivalType = char.Parse(Console.ReadLine().ToLower());

        Console.Write("Enter arrival value: ");
        double arrivalValue = double.Parse(Console.ReadLine());

        Console.Write("Enter 'r' for rate or 'm' for mean (service): ");
        char serviceType = char.Parse(Console.ReadLine().ToLower());

        Console.Write("Enter service value: ");
        double serviceValue = double.Parse(Console.ReadLine());

        Console.Write("Enter time unit (hour/min/sec): ");
        string timeUnit = Console.ReadLine().ToLower();

        Console.Write("Enter number of servers (s): ");
        int s = int.Parse(Console.ReadLine());

        // ===== Conversion Section =====
        double lambda = ConvertToRate(arrivalType, arrivalValue, timeUnit);
        double mu = ConvertToRate(serviceType, serviceValue, timeUnit);

        // ===== Calculations =====
        double rho = lambda / (s * mu);

        if (rho >= 1)
        {
            Console.WriteLine("\n⚠️ The system is unstable (λ ≥ sμ). Simulation cannot proceed.");
            return;
        }

        double P0, Lq, Ls, Wq, Ws;

        // ===== Handle M/M/1 Case Separately =====
        if (s == 1)
        {
            P0 = 1 - rho;
            Lq = Math.Pow(rho, 2) / (1 - rho);
            Ls = rho / (1 - rho);
            Wq = Lq / lambda;
            Ws = 1 / (mu - lambda);
        }
        else
        {
            // ===== Multi-server (M/M/s) Case =====
            double sum = 0;
            for (int n = 0; n < s; n++)
                sum += Math.Pow(lambda / mu, n) / Factorial(n);

            double lastTerm = Math.Pow(lambda / mu, s) / (Factorial(s) * (1 - rho));
            P0 = 1 / (sum + lastTerm);

            Lq = (P0 * Math.Pow(lambda / mu, s) * rho) /
                 (Factorial(s) * Math.Pow(1 - rho, 2));

            Ls = Lq + (lambda / mu);
            Wq = Lq / lambda;
            Ws = Wq + (1 / mu);
        }

        // ===== Output Section =====
        Console.WriteLine("\n--- Results ---");
        Console.WriteLine($"Lambda (Arrival Rate): {lambda:E6} per second");
        Console.WriteLine($"Mu (Service Rate): {mu:E6} per second");
        Console.WriteLine($"s (Servers): {s}");
        Console.WriteLine($"Rho (Traffic Intensity): {rho:F4}");
        Console.WriteLine($"P0 (Idle Probability): {P0:F6}");
        Console.WriteLine($"Lq (Avg. number in queue): {Lq:F4}");
        Console.WriteLine($"Ls (Avg. number in system): {Ls:F4}");
        Console.WriteLine($"Wq (Avg. waiting time in queue): {FormatTime(Wq)}");
        Console.WriteLine($"Ws (Avg. time in system): {FormatTime(Ws)}");

        Console.WriteLine("\n=== Simulation Complete ===");
    }

    // ===== Helper: Convert rate or mean to per-second rate =====
    static double ConvertToRate(char type, double value, string unit)
    {
        double seconds = 1.0;

        if (unit == "hour") seconds = 3600.0;
        else if (unit == "min") seconds = 60.0;

        if (type == 'r') // rate
            return value / seconds;
        else // mean
            return 1.0 / (value * seconds);
    }

    // ===== Helper: Factorial =====
    static double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    // ===== Helper: Format time in sec/min/hour =====
    static string FormatTime(double seconds)
    {
        if (seconds < 60)
            return $"{seconds:F2} sec";
        else if (seconds < 3600)
            return $"{seconds / 60.0:F2} min";
        else
            return $"{seconds / 3600.0:F2} hr";
    }
}
