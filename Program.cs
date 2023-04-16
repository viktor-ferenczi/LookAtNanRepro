using System;
using VRageMath;

namespace LookAtNanRepro
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var turretController = new DecoupledTurretController();
            var total = 0;
            var failed = 0;
            try
            {
                // Vary the shoot direction
                for (var a = 0; a < 10; a++)
                {
                    var shootDirection = Vector3D.Rotate(Vector3D.Up, MatrixD.CreateFromAxisAngle(Vector3D.Right, 0.01 * a));
                    turretController.ShootDirection = shootDirection;
                    
                    // Run the LookAt calculation
                    var target = new Vector3D(0, 10, 0);
                    var result = turretController.LookAt(target);
                    total++;

                    // Verify whether the resulting vector has any invalid (NaN or Infinity) components
                    var good = result.IsValid();
                    if (!good) {
                        failed++;
                    }
                    
                    var verdict = good ? "OK" : "FAIL";
                    Console.WriteLine($"{verdict}: ShootDirection=({shootDirection}) => {result}");
                    // Console.WriteLine($"{result}: ShootDirection=({shootDirection}), target=({target}) => {vector3}");
                }
            }
            finally
            {
                Console.WriteLine($"Total: {total}");
                Console.WriteLine($"Failed: {failed}");
                Console.WriteLine($"LowestAzimuthorAbsDotProduct = {turretController.LowestAzimuthorAbsDotProduct}");
                Console.WriteLine($"HighestAzimuthorAbsDotProduct = {turretController.HighestAzimuthorAbsDotProduct}");
                Console.WriteLine($"LowestElevatorAbsDotProduct = {turretController.LowestElevatorAbsDotProduct}");
                Console.WriteLine($"HighestElevatorAbsDotProduct = {turretController.HighestElevatorAbsDotProduct}");
            }
        }
    }
}